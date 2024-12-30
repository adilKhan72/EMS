using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFBO.Reports;
using DemoEFBL.Login;
using DemoEFDAL.Reports;
using DemoEFBO.Tasks;
using DemoEFBL.CommonLists;
using DemoEFBO.Projects;
using System.Globalization;
using System.IO;
using OfficeOpenXml;
using System.Data;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Web;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using iTextSharp.text.pdf;
using System.Runtime.InteropServices.ComTypes;
using Org.BouncyCastle.Utilities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Data.SqlClient;
using DemoEFDAL;
namespace DemoEFBL.Reports
{
    public struct Range
    {
        public DateTime Start { get; private set; }

        public DateTime End { get { return Start.AddDays(6); } }

        public Range(DateTime start)
        {
            Start = start;
        }
    }

    public class ReportsBL
    {
        #region Other function used in main Functions

        #region GetAssignments call Code Functions list

        private IEnumerable<Tuple<string, int>> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return Tuple.Create(
                    dateTimeFormat.GetMonthName(iterator.Month),
                    iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }

        private IEnumerable<Tuple<string, int>> GetMonths(DateTime startDate, DateTime endDate)
        {
            startDate = Convert.ToDateTime(startDate.ToString("MM/dd/yyyy"));
            endDate = Convert.ToDateTime(endDate.ToString("MM/dd/yyyy"));
            var months = MonthsBetween(startDate, endDate).Reverse();
            return months;
        }

        private IEnumerable<Range> GetWeeks(int year, int month)
        {
            DateTime start = new DateTime(year, month, 1).AddDays(-6);
            DateTime end = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    yield return new Range(date);
                }
            }
        }

        //public List<AssignmentsModelForExcelBO> GetAllAssignmentResult(AssignmentDTOBO assignment)
        //{
        //    try
        //    {
        //        var assignments = new List<AssignmentsModelForExcelBO>();
        //        ReportDAL objReportDAL = new ReportDAL();
        //        var result = objReportDAL.GetAllAssignments(assignment);
        //        if (result != null && result.Count > 0)
        //        {
        //            List<TasksBO> TaskList = null;
        //            for (int i = 0; i < result.Count; i++)
        //            {
        //                try
        //                {
        //                    var taskName = result[i].TaskName;
        //                    var durationInDecimal = float.Parse(result[i].ActualDuration.ToString()).ToString("0.00");
        //                    var daysInDecimal = (Convert.ToDecimal(result[i].ActualDuration) / 8).ToString("0.00");
        //                    var subTask = result[i].SubTaskName;
        //                    if (string.IsNullOrEmpty(subTask))
        //                    {
        //                        subTask = "No Sub Task";
        //                    }
        //                    assignments.Add(new AssignmentsModelForExcelBO
        //                    {
        //                        ID = result[i].ID,
        //                        Project = result[i].ProjectName,
        //                        ProjectID = result[i].ProjectId,
        //                        Task = taskName,
        //                        SubTask = subTask,
        //                        TaskOwner = result[i].TaskOwnerName,
        //                        Date = Convert.ToDateTime(result[i].AssignmentDateTime).ToString("dd/MM/yyyy"),
        //                        Duration = float.Parse(durationInDecimal),
        //                        Days = Convert.ToDecimal(daysInDecimal),
        //                        TaskTypePrefix = result[i].TaskTypePrefix,
        //                        TaskType = result[i].TaskTypeName,
        //                        Phase = result[i].Phase.ToString(),
        //                        IsApproved = (bool)result[i].IsApproved,
        //                        MainTaskID = result[i].MainTaskId,
        //                        MainTaskName = result[i].MainTaskName
        //                    });

        //                    var templist = assignments.Where(x => x.Task == taskName).ToList();
        //                    decimal remainingHours = 0;

        //                    if (templist.Count > 0)
        //                    {
        //                        var sumOfAcutalHours = templist.Sum(r => r.Duration);
        //                        if (TaskList == null)
        //                        {
        //                            TaskList = TaskListBL.GetAllTasks();
        //                        }
        //                        var task = TaskList.FirstOrDefault(t => t.TaskName == taskName);
        //                        remainingHours = task.EstimatedDuration - Convert.ToDecimal(sumOfAcutalHours);
        //                    }
        //                    assignments.Last().RemainingHours = remainingHours;
        //                }
        //                catch (Exception ex) { }
        //            }
        //            if (assignment.IsApproved == null)
        //            {
        //                return assignments.OrderBy(a => a.Project).ThenByDescending(m => DateTime.ParseExact(m.Date, "dd/MM/yyyy", null)).ToList();
        //            }
        //            else
        //            {
        //                return assignments.Where(i => i.IsApproved == assignment.IsApproved).OrderBy(a => a.Project).ThenByDescending(m => DateTime.ParseExact(m.Date, "dd/MM/yyyy", null)).ToList();
        //            }
        //        }
        //        return assignments;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public List<AssignmentsModelForExcelBO> GetTotalHoursOfMonth(List<AssignmentsModelForExcelBO> assignments)
        {
            try
            {
                //Initialize index & filtered assignments 
                var filteredAssignments = new List<AssignmentsModelForExcelBO>();
                var listOfTotalHoursOfWeek = new List<AssignmentsModelForExcelBO>();
                var listOfTotalHoursOfMonth = new List<AssignmentsModelForExcelBO>();
                var listOfWeeksBetweenMonthEnd = new List<AssignmentsModelForExcelBO>();

                //Get the projects 
                var projects = ProjectsListBL.GetAllProjects();

                //Get assignments filtered on the base of project
                foreach (var project in projects)
                {
                    filteredAssignments = assignments.Where(a => a.Project == project.Name).ToList();
                    if (filteredAssignments.Count > 0)
                    {
                        var startDate = DateTime.ParseExact(filteredAssignments.FirstOrDefault().Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var endDate = DateTime.ParseExact(filteredAssignments.LastOrDefault().Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        var months = this.GetMonths(startDate, endDate);

                        foreach (var i_month in months)
                        {
                            int weekNo = 0;
                            var month = "01/" + DateTime.ParseExact(i_month.Item1, "MMMM", CultureInfo.CurrentCulture).Month.ToString("00") + "/" + i_month.Item2;
                            var selectedMonth = DateTime.ParseExact(month, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            var weeksInSelectedMonth = this.GetWeeks(selectedMonth.Year, selectedMonth.Month).Reverse();
                            weekNo = weeksInSelectedMonth.Count();
                            foreach (var week in weeksInSelectedMonth)
                            {
                                var startDateOfWeek = week.Start.Date;
                                var endDateOfWeek = week.End.Date;

                                var endDateOfStartingMonthOfWeek = DateTime.Now;

                                var startDateOfEndingMonthOfWeek = DateTime.Now;
                                var endDayOfStartingMonthOfWeek = DateTime.DaysInMonth(startDateOfWeek.Year, startDateOfWeek.Month);

                                if (!startDateOfWeek.Month.Equals(endDateOfWeek.Month))
                                {
                                    startDateOfEndingMonthOfWeek = Convert.ToDateTime(endDateOfWeek.Month
                                        + "/01/" + endDateOfWeek.Year);
                                    endDateOfStartingMonthOfWeek = Convert.ToDateTime(startDateOfWeek.Month +
                                        "/" + endDayOfStartingMonthOfWeek + "/"
                                        + startDateOfWeek.Year);

                                    var assignmentsInSelectedWeek = filteredAssignments.Where(w =>
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= startDateOfWeek &&
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= endDateOfStartingMonthOfWeek).ToList();

                                    if (assignmentsInSelectedWeek.Count > 0)
                                    {
                                        var lastAssignmentInSelectedWeek = assignmentsInSelectedWeek.LastOrDefault();
                                        var weeklyDaysInTotal = (assignmentsInSelectedWeek.Sum(w => w.Duration) / 8).ToString("0.00");
                                        var weeklyHoursInTotal = assignmentsInSelectedWeek.Sum(w => w.Duration);
                                        var weeklyLastAssignmentId = assignments.LastOrDefault(w => w.ID == lastAssignmentInSelectedWeek.ID).ID;
                                        var indexOfSelectedLastAssignmentOfWeek = assignments.FindIndex(i => i.ID == weeklyLastAssignmentId);

                                        var monthOfWeeklyLastAssignment = DateTime.ParseExact(lastAssignmentInSelectedWeek.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMMM");

                                        var weeklyAssignmentTotal = new AssignmentsModelForExcelBO
                                        {
                                            Days = Convert.ToDecimal(weeklyDaysInTotal),
                                            ID = weeklyLastAssignmentId + 1,
                                            Date = /*DateTime.Now.ToString("dd/MM/yyyy")*/lastAssignmentInSelectedWeek.Date,
                                            Duration = weeklyHoursInTotal,
                                            Project = "TOTAL",
                                            Task = "Week #" + weekNo + ", " + i_month.Item1,
                                            TaskOwner = "HOURS",
                                            ProjectNameForWeek = lastAssignmentInSelectedWeek.Project
                                        };

                                        if (monthOfWeeklyLastAssignment == i_month.Item1)
                                        {
                                            listOfTotalHoursOfWeek.Add(weeklyAssignmentTotal);
                                        }
                                    }

                                    assignmentsInSelectedWeek = filteredAssignments.Where(w =>
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= startDateOfEndingMonthOfWeek &&
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= endDateOfWeek).ToList();

                                    if (assignmentsInSelectedWeek.Count > 0)
                                    {
                                        var lastAssignmentInSelectedWeek = assignmentsInSelectedWeek.LastOrDefault();
                                        var weeklyDaysInTotal = (assignmentsInSelectedWeek.Sum(w => w.Duration) / 8).ToString("0.00");
                                        var weeklyHoursInTotal = assignmentsInSelectedWeek.Sum(w => w.Duration);
                                        var weeklyLastAssignmentId = assignments.LastOrDefault(w => w.ID == lastAssignmentInSelectedWeek.ID).ID;
                                        var indexOfSelectedLastAssignmentOfWeek = assignments.FindIndex(i => i.ID == weeklyLastAssignmentId);

                                        var monthOfWeeklyLastAssignment = DateTime.ParseExact(lastAssignmentInSelectedWeek.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMMM");

                                        var weeklyAssignmentTotal = new AssignmentsModelForExcelBO
                                        {
                                            Days = Convert.ToDecimal(weeklyDaysInTotal),
                                            ID = weeklyLastAssignmentId + 1,
                                            Date = /*DateTime.Now.ToString("dd/MM/yyyy")*/lastAssignmentInSelectedWeek.Date,
                                            Duration = weeklyHoursInTotal,
                                            Project = "TOTAL",
                                            Task = "Week #" + weekNo + ", " + i_month.Item1,
                                            TaskOwner = "HOURS",
                                            ProjectNameForWeek = lastAssignmentInSelectedWeek.Project
                                        };

                                        if (monthOfWeeklyLastAssignment == i_month.Item1)
                                        {
                                            listOfTotalHoursOfWeek.Add(weeklyAssignmentTotal);
                                        }
                                    }
                                }
                                else
                                {
                                    var assignmentsInSelectedWeek = filteredAssignments.Where(w =>
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) >= startDateOfWeek &&
                                    DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture) <= endDateOfWeek).ToList();

                                    if (assignmentsInSelectedWeek.Count > 0)
                                    {
                                        var lastAssignmentInSelectedWeek = assignmentsInSelectedWeek.LastOrDefault();
                                        var weeklyDaysInTotal = (assignmentsInSelectedWeek.Sum(w => w.Duration) / 8).ToString("0.00");
                                        var weeklyHoursInTotal = assignmentsInSelectedWeek.Sum(w => w.Duration);
                                        var weeklyLastAssignmentId = assignments.LastOrDefault(w => w.ID == lastAssignmentInSelectedWeek.ID).ID;
                                        var indexOfSelectedLastAssignmentOfWeek = assignments.FindIndex(i => i.ID == weeklyLastAssignmentId);

                                        var monthOfWeeklyLastAssignment = DateTime.ParseExact(lastAssignmentInSelectedWeek.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MMMM");

                                        var weeklyAssignmentTotal = new AssignmentsModelForExcelBO
                                        {
                                            Days = Convert.ToDecimal(weeklyDaysInTotal),
                                            ID = weeklyLastAssignmentId + 1,
                                            Date = /*DateTime.Now.ToString("dd/MM/yyyy")*/lastAssignmentInSelectedWeek.Date,
                                            Duration = weeklyHoursInTotal,
                                            Project = "TOTAL",
                                            Task = "Week #" + weekNo + ", " + i_month.Item1,
                                            TaskOwner = "HOURS",
                                            ProjectNameForWeek = lastAssignmentInSelectedWeek.Project
                                        };

                                        listOfTotalHoursOfWeek.Add(weeklyAssignmentTotal);
                                    }
                                }
                                weekNo--;
                            }
                            var assignmentsInSelectedMonth = filteredAssignments.Where(a => DateTime.ParseExact(a.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).Month == selectedMonth.Month);
                            if (assignmentsInSelectedMonth.Count() > 0)
                            {
                                var monthlyLastAssignmentId = assignments.Where(w => DateTime.ParseExact(w.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).Month == selectedMonth.Month && w.Project == project.Name).LastOrDefault().ID;

                                if (monthlyLastAssignmentId > 0)
                                {
                                    var monthylDaysInTotal = Convert.ToDecimal(assignmentsInSelectedMonth.Sum(w => w.Duration) / 8).ToString("0.00");
                                    var monthlyHoursInTotal = assignmentsInSelectedMonth.Sum(w => w.Duration);

                                    int indexOfSelectedLastAssignmentOfMonth = assignments.FindIndex(i => i.ID == monthlyLastAssignmentId);

                                    var monthlyAssignmentTotal = new AssignmentsModelForExcelBO
                                    {
                                        Days = Convert.ToDecimal(monthylDaysInTotal),
                                        ID = monthlyLastAssignmentId + 1,
                                        Date = /*DateTime.Now.ToString("dd/MM/yyyy"),*/assignments[indexOfSelectedLastAssignmentOfMonth].Date,
                                        Duration = monthlyHoursInTotal,
                                        Task = "Month - " + i_month.Item1,
                                        Project = "TOTAL",
                                        TaskOwner = "HOURS",
                                        ProjectNameForWeek = assignmentsInSelectedMonth.LastOrDefault().Project
                                    };
                                    listOfTotalHoursOfMonth.Add(monthlyAssignmentTotal);
                                    //assignments.Insert(indexOfSelectedLastAssignmentOfMonth + 1, monthlyAssignmentTotal);
                                }
                            }
                        }
                    }
                }

                //Remove duplicate entries from the list of total hours of the week
                int currentIndexOfListOfTotalHoursOfWeek = 0;
                while (currentIndexOfListOfTotalHoursOfWeek < listOfTotalHoursOfWeek.Count - 1)
                {
                    if (listOfTotalHoursOfWeek[currentIndexOfListOfTotalHoursOfWeek].ID == listOfTotalHoursOfWeek[currentIndexOfListOfTotalHoursOfWeek + 1].ID)
                    {
                        listOfTotalHoursOfWeek.RemoveAt(currentIndexOfListOfTotalHoursOfWeek + 1);
                    }
                    //Handling out of range ex
                    if (currentIndexOfListOfTotalHoursOfWeek + 1 < listOfTotalHoursOfWeek.Count)
                    {
                        if (listOfTotalHoursOfWeek[currentIndexOfListOfTotalHoursOfWeek].Task == listOfTotalHoursOfWeek[currentIndexOfListOfTotalHoursOfWeek + 1].Task)
                        {
                            var currentIndex = currentIndexOfListOfTotalHoursOfWeek - 1;
                            if (currentIndex > -1)
                            {
                                listOfTotalHoursOfWeek.RemoveAt(currentIndex);
                            }
                            else
                            {
                                currentIndexOfListOfTotalHoursOfWeek++;
                            }
                        }
                        else
                        {
                            currentIndexOfListOfTotalHoursOfWeek++;
                        }
                    }
                }

                //After getting the distinct list insert the rows at the respective indexes
                foreach (var selectedTotal in listOfTotalHoursOfMonth)
                {

                    int indexOfSelectedTotalHourOfMonth = assignments.FindIndex(i => (i.ID == selectedTotal.ID)
                    && (i.Date == DateTime.Now.ToString("dd/MM/yyyy"))
                    && (i.Project == selectedTotal.ProjectNameForWeek));
                    if (indexOfSelectedTotalHourOfMonth >= 0)
                    {
                        assignments.Insert(indexOfSelectedTotalHourOfMonth, selectedTotal);
                    }
                    else
                    {
                        int newIndexOfSelectedTotalHourOfMonth = assignments.FindIndex(i => (i.ID == selectedTotal.ID - 1)
                        && (i.Project == selectedTotal.ProjectNameForWeek));
                        assignments.Insert(newIndexOfSelectedTotalHourOfMonth + 1, selectedTotal);
                    }
                }

                //After getting the distinct list insert the rows at the respective indexes
                foreach (var selectedTotal in listOfTotalHoursOfWeek)
                {

                    int indexOfSelectedTotalHourOfWeek = assignments.FindIndex(i => (i.ID == selectedTotal.ID)
                    && (i.Date == selectedTotal.Date)
                    && (i.Project == selectedTotal.ProjectNameForWeek));
                    if (indexOfSelectedTotalHourOfWeek >= 0)
                    {
                        assignments.Insert(indexOfSelectedTotalHourOfWeek, selectedTotal);
                    }
                    else
                    {
                        int newIndexOfSelectedTotalHourOfWeek = assignments.FindIndex(i => (i.ID == selectedTotal.ID - 1) && (i.Project == selectedTotal.ProjectNameForWeek));
                        assignments.Insert(newIndexOfSelectedTotalHourOfWeek + 1, selectedTotal);
                    }
                }

                return assignments;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        #endregion

        #region GetTotalHoursForTasks call code functions list

        private List<ProjectTotalHoursBO> GetProjectTotalHours(AssignmentDTOBO assignment)
        {
            try
            {
                var totalHours = new List<ProjectTotalHoursBO>();
                TotalHoursOfProjectParamBO objData = new TotalHoursOfProjectParamBO();
                objData.ProjectId = assignment.ProjectId;
                objData.TaskName = assignment.TaskName;
                objData.TaskOwnerId = assignment.TaskOwnerId;
                objData.FromDate = assignment.FromDate;
                objData.ToDate = assignment.ToDate;
                objData.MainTaskID = assignment.MainTaskID;
                if (assignment.IsApproved == null)
                {
                    objData.Approved = "All";
                }
                else
                {
                    objData.Approved = assignment.IsApproved.ToString(); // need to check this
                }
                if (assignment.taskTypePrefixes != null)
                {
                    if (assignment.taskTypePrefixes.Count() == 3)
                    {
                        objData.TaskType = 0;
                    }
                    if (assignment.taskTypePrefixes.Count() == 1)
                    {
                        if (assignment.taskTypePrefixes[0].Equals("A"))
                        {
                            objData.TaskType = 1;
                        }
                        if (assignment.taskTypePrefixes[0].Equals("R"))
                        {
                            objData.TaskType = 2;
                        }
                        if (assignment.taskTypePrefixes[0].Equals("N"))
                        {
                            objData.TaskType = 3;
                        }
                    }
                    if (assignment.taskTypePrefixes.Count() == 2)
                    {
                        if (assignment.taskTypePrefixes.Contains("A") && assignment.taskTypePrefixes.Contains("R"))
                        {
                            objData.TaskType = 4;
                        }
                        if (assignment.taskTypePrefixes.Contains("A") && assignment.taskTypePrefixes.Contains("N"))
                        {
                            objData.TaskType = 5;
                        }
                        if (assignment.taskTypePrefixes.Contains("N") && assignment.taskTypePrefixes.Contains("R"))
                        {
                            objData.TaskType = 6;
                        }
                    }
                }
                else
                {
                    objData.TaskType = 0;
                }
                ReportDAL objDal = new ReportDAL();
                var result = objDal.TotalHoursOfProjects(objData);
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        totalHours.Add(new ProjectTotalHoursBO
                        {
                            ProjectId = result[i].ProjectID,
                            ProjectName = result[i].ProjectName,
                            RemainingHours = result[i].RemainingHours,
                            TotalActualDuration = result[i].TotalActualDuration
                        });
                    }
                    return totalHours;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion



        #endregion



        #region Main functions 
        public Tuple<string, bool> GenerateExcelTSWeeklyClientWiseActualHrs(ExportAssignmentBO assignmentWeekly, string filePath, string workSheetName, string getProject_Name)
        {
            try
            {
                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                var breakProjectIDs = assignmentWeekly.projectwise.Split(',');
                var breakgetProject_Name = getProject_Name.Split(',');
                for (int p = 0; p < breakProjectIDs.Length; p++)
                {
                    var WorkSheetName = string.Empty;
                    if (breakgetProject_Name[p].Length > 27)
                    {
                        WorkSheetName = breakgetProject_Name[p].Substring(0, 27) + "...";
                    }
                    else
                    {
                        WorkSheetName = breakgetProject_Name[p];
                    }
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(WorkSheetName);
                    int rowStart = 1;
                    DataTable DT = new DataTable();
                    DataTable BillableDT = new DataTable();
                    ReportDAL objReportDAL = new ReportDAL();
                    assignmentWeekly.ProjectId = Convert.ToInt32(breakProjectIDs[p]);
                    if (assignmentWeekly.IsDifferencehourse == true)
                    {
                        DT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                    }
                    else
                    {
                        DT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                    }
                    //BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly,  ref BillableDT);
                    DT = MakeFormateReport(DT, Convert.ToDateTime(assignmentWeekly.FromDate));
                    BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));

                    if (DT == null || DT.Rows.Count == 0 || DT.Columns.Count == 1)
                    {
                        if (DT.Columns.Count == 1)
                        {
                            return new Tuple<string, bool>(DT.Rows[0][0].ToString(), false);
                        }
                        else
                        {
                            return new Tuple<string, bool>(null, false);
                        }

                    }
                    else
                    {


                        //ws.Cells["A:E"].AutoFitColumns();
                        //ws.Column(5).Width = 15;

                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            if (assignmentWeekly.ShowSubTask == true)
                            {
                                if (DT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    //Main Heading

                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Merge = true;
                                    ws.Row(rowStart).Height = 30;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 18;
                                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                                }
                                else if (DT.Rows[i][1].ToString() == "Client:Period")
                                {

                                    //Project Name And Time
                                    ++rowStart;

                                    ws.Row(rowStart).Height = 25;
                                    System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 13;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"G{rowStart}"].Value = $"Period: {DateRange}";

                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (DT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    //Total Hours And Days
                                    ++rowStart;
                                    ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                    ws.Row(rowStart).Height = 25;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 13;
                                    Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;

                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"G{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (DT.Rows[i][1].ToString() == "Header")
                                {
                                    //Sub Heading
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 24;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 12;
                                    System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                    ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Date";
                                    ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                    ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"C{rowStart}"].Value = "Actual Hrs";
                                    ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Value = "Main Task";
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Value = "Sub Task";
                                    ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"G{rowStart}"].Value = "Description";
                                }
                                else if (DT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    //DateTime of Project
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"G{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"G{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                                }
                                else if (DT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    //Range and Days of Project 
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = " Montserrat";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"G{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"G{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                                }
                                else if (DT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(DT.Rows[i][4]);
                                        decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0 || skip1 > 0)
                                        {
                                            //Task
                                            ++rowStart;
                                            ws.Row(rowStart).Height = 20;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat Light";
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 11;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            //if (i == 33)
                                            //{
                                            //    var asd = DT.Rows[i][2];
                                            //}

                                            ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                            ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();

                                            ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            var asd = Convert.ToDecimal(DT.Rows[i][4]);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                            ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                            ws.Cells[string.Format("G{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                            ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                            ws.Row(rowStart).CustomHeight = false;

                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                            //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                            //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                        }
                                    }
                                    else
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                        ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();



                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                        ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("G{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                    }


                                }
                            }
                            else
                            {
                                if (DT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    //Main Heading

                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Row(rowStart).Height = 30;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 18;
                                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                                }
                                else if (DT.Rows[i][1].ToString() == "Client:Period")
                                {

                                    //Project Name And Time
                                    ++rowStart;

                                    ws.Row(rowStart).Height = 25;
                                    System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Value = $"Period: {DateRange}";

                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (DT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    //Total Hours And Days
                                    ++rowStart;
                                    ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                    ws.Row(rowStart).Height = 25;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                    Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;

                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (DT.Rows[i][1].ToString() == "Header")
                                {
                                    //Sub Heading
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 24;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 12;
                                    System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                    ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Date";
                                    ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                    ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"C{rowStart}"].Value = "Actual Hrs";
                                    ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Value = "Main Task";
                                    //ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.DrawinF.Color.White);
                                    //ws.Cells[$"F{rowStart}"].Value = "Sub Task";
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Value = "Description";
                                }
                                else if (DT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    //DateTime of Project
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"F{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                                }
                                else if (DT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    //Range and Days of Project 
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = " Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = DT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"E{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                                }
                                else if (DT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(DT.Rows[i][4]);
                                        decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0 || skip1 > 0)
                                        {
                                            //Task
                                            ++rowStart;
                                            ws.Row(rowStart).Height = 20;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            //if (i == 33)
                                            //{
                                            //    var asd = DT.Rows[i][2];
                                            //}

                                            ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                            ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();

                                            ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            var asd = Convert.ToDecimal(DT.Rows[i][4]);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                                                                                                        // ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                            ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                            ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                            //ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                            ws.Row(rowStart).CustomHeight = false;

                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                            //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                            //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                        }
                                    }
                                    else
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                        ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();



                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                        ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                        //ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        //  ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                    }


                                }
                            }


                        }

                        DT.Clear();
                        BillableDT.Clear();


                    }
                    if (assignmentWeekly.ShowSubTask == true)
                    {
                        ws.Column(7).Width = 80;
                        ws.Column(6).Width = 15;
                        ws.Column(5).Width = 15;
                        ws.Column(4).Width = 14;
                        ws.Column(3).Width = 12;
                        ws.Column(2).Width = 20;
                        ws.Column(1).Width = 15;
                    }
                    else
                    {
                        //      ws.Column(7).Width = 80;
                        ws.Column(6).Width = 80;
                        ws.Column(5).Width = 15;
                        ws.Column(4).Width = 14;
                        ws.Column(3).Width = 12;
                        ws.Column(2).Width = 20;
                        ws.Column(1).Width = 15;
                    }


                }
                pck.Save();

                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "TimeSheetWeekly.xlsx",//assignmentWeekly.fileName,
                    Inline = true,
                };

                HttpResponse Response = HttpContext.Current.Response;
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception e)
            {
                throw e;

            }
        }
        public Tuple<string, bool> GenerateExcelTSWeeklyActualHrs(ExportAssignmentBO assignmentWeekly, string filePath, string workSheetName)
        {
            try
            {
                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);
                int rowStart = 1;
                DataTable DT = new DataTable();
                DataTable BillableDT = new DataTable();
                ReportDAL objReportDAL = new ReportDAL();
                if (assignmentWeekly.IsDifferencehourse == true)
                {
                    DT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                }
                else
                {
                    DT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                }

                //BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly,  ref BillableDT);
                DT = MakeFormateReport(DT, Convert.ToDateTime(assignmentWeekly.FromDate));
                BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));

                if (DT == null || DT.Rows.Count == 0 || DT.Columns.Count == 1)
                {
                    if (DT.Columns.Count == 1)
                    {
                        return new Tuple<string, bool>(DT.Rows[0][0].ToString(), false);
                    }
                    else
                    {
                        return new Tuple<string, bool>(null, false);
                    }

                }
                else
                {


                    //ws.Cells["A:E"].AutoFitColumns();
                    //ws.Column(5).Width = 15;

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        if (assignmentWeekly.ShowSubTask == true)
                        {
                            if (DT.Rows[i][1].ToString() == "Mainheading")
                            {
                                //Main Heading

                                ws.Cells[$"A{rowStart}:G{rowStart}"].Merge = true;
                                ws.Row(rowStart).Height = 30;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 18;
                                System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                            }
                            else if (DT.Rows[i][1].ToString() == "Client:Period")
                            {

                                //Project Name And Time
                                ++rowStart;

                                ws.Row(rowStart).Height = 25;
                                System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 13;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"G{rowStart}"].Value = $"Period: {DateRange}";

                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (DT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                //Total Hours And Days
                                ++rowStart;
                                ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                ws.Row(rowStart).Height = 25;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 13;
                                Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;

                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"G{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (DT.Rows[i][1].ToString() == "Header")
                            {
                                //Sub Heading
                                ++rowStart;
                                ws.Row(rowStart).Height = 24;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 12;
                                System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Date";
                                ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"C{rowStart}"].Value = "Actual Hrs";
                                ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Value = "Main Task";
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Value = "Sub Task";
                                ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"G{rowStart}"].Value = "Description";
                            }
                            else if (DT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                //DateTime of Project
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"G{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"G{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                            }
                            else if (DT.Rows[i][1].ToString() == "Range:Days")
                            {
                                //Range and Days of Project 
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = " Montserrat";
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"G{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"G{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                            }
                            else if (DT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(DT.Rows[i][4]);
                                    decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0 || skip1 > 0)
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        //if (i == 33)
                                        //{
                                        //    var asd = DT.Rows[i][2];
                                        //}

                                        ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();

                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        var asd = Convert.ToDecimal(DT.Rows[i][4]);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("G{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                        //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                    }
                                }
                                else
                                {
                                    //Task
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 20;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Name = "Montserrat Light";
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Font.Size = 11;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:G{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                    ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();



                                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                    ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                    ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                    ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                    ws.Cells[string.Format("G{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                    ws.Row(rowStart).CustomHeight = false;

                                    //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                    //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                }


                            }
                        }
                        else
                        {
                            if (DT.Rows[i][1].ToString() == "Mainheading")
                            {
                                //Main Heading

                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                ws.Row(rowStart).Height = 30;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 18;
                                System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                            }
                            else if (DT.Rows[i][1].ToString() == "Client:Period")
                            {

                                //Project Name And Time
                                ++rowStart;

                                ws.Row(rowStart).Height = 25;
                                System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Value = $"Period: {DateRange}";

                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (DT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                //Total Hours And Days
                                ++rowStart;
                                ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                ws.Row(rowStart).Height = 25;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;

                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (DT.Rows[i][1].ToString() == "Header")
                            {
                                //Sub Heading
                                ++rowStart;
                                ws.Row(rowStart).Height = 24;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 12;
                                System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Date";
                                ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"C{rowStart}"].Value = "Actual Hrs";
                                ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Value = "Main Task";
                                //ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"F{rowStart}"].Value = "Sub Task";
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Value = "Description";
                            }
                            else if (DT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                //DateTime of Project
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"F{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                            }
                            else if (DT.Rows[i][1].ToString() == "Range:Days")
                            {
                                //Range and Days of Project 
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = " Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = DT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"F{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                            }
                            else if (DT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(DT.Rows[i][4]);
                                    decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0 || skip1 > 0)
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        //if (i == 33)
                                        //{
                                        //    var asd = DT.Rows[i][2];
                                        //}

                                        ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();

                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        var asd = Convert.ToDecimal(DT.Rows[i][4]);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                        //ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        //                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                    }
                                }
                                else
                                {
                                    //Task
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 20;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[string.Format("A{0}", rowStart)].Value = DT.Rows[i][2];
                                    ws.Cells[string.Format("B{0}", rowStart)].Value = DT.Rows[i][3].ToString();



                                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                    ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                    ws.Cells[string.Format("E{0}", rowStart)].Value = DT.Rows[i][5].ToString(); //item.Task; 
                                    //ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][6].ToString(); //item.SubTask;
                                    ws.Cells[string.Format("F{0}", rowStart)].Value = DT.Rows[i][7].ToString(); //item.Description;

                                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                    //                                    ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;


                                    ws.Row(rowStart).CustomHeight = false;

                                }


                            }
                        }

                    }

                    DT.Clear();
                    BillableDT.Clear();


                }
                if (assignmentWeekly.ShowSubTask == true)
                {
                    ws.Column(7).Width = 80;
                    ws.Column(6).Width = 20;
                    ws.Column(5).Width = 15;
                    ws.Column(4).Width = 14;
                    ws.Column(3).Width = 12;
                    ws.Column(2).Width = 20;
                    ws.Column(1).Width = 15;
                }
                else
                {
                    //ws.Column(7).Width = 80;
                    ws.Column(6).Width = 80;
                    ws.Column(5).Width = 15;
                    ws.Column(4).Width = 14;
                    ws.Column(3).Width = 12;
                    ws.Column(2).Width = 20;
                    ws.Column(1).Width = 15;
                }



                pck.Save();

                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "TimeSheetWeekly.xlsx",//assignmentWeekly.fileName,
                    Inline = true,
                };

                HttpResponse Response = HttpContext.Current.Response;
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception e)
            {
                throw e;

            }
        }




        public Tuple<string, bool> ExportToWBSReport(int ProjectID, string FileName, string filePath, string WorkSheetName)
        {
            try
            {
                decimal projectmanagementPercentge = 0;
                DataTable DT = new DataTable();
                ReportDAL objReportDAL = new ReportDAL();
                int rowStart = 1;
                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(WorkSheetName);
                projectmanagementPercentge = objReportDAL.GetProjectManagementPercentage();
                DT = objReportDAL.WBSExcelDataTable(ProjectID);
                if (DT == null || DT.Rows.Count == 0 || DT.Columns.Count == 1)
                {
                    if (DT.Columns.Count == 1)
                    {
                        return new Tuple<string, bool>(DT.Rows[0][0].ToString(), false);
                    }
                    else
                    {
                        return new Tuple<string, bool>(null, false);
                    }

                }
                else
                {
                    int GroupCount = 0;
                    var rowgroupBy = from rowgroup in DT.AsEnumerable()
                               .GroupBy(g => new { MainTaskId = g.Field<int>("MainTaskId"), MainTaskName = g.Field<string>("MainTaskName") })
                                     select rowgroup;
                    #region Header
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                    ws.Row(rowStart).Height = 30;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 18;
                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[$"A{rowStart}"].Value = "WBS' Details";




                    //Project Name And Time
                    ++rowStart;
                    System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                    ws.Row(rowStart).Height = 25;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = false;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 13;
                    System.Drawing.Color colFromH = System.Drawing.ColorTranslator.FromHtml("#123049");
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    ws.Cells[$"A{rowStart}"].Value = $"Project: {DT.Rows[0][10]}";

                    #endregion

                    #region SubHeading
                    ++rowStart;
                    ws.Row(rowStart).Height = 24;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    //ws.Cells["A5:E5"].Style.Font.Bold = true;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 12;
                    System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                    ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                    ws.Cells[$"A{rowStart}"].Value = "Description";
                    ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                    ws.Cells[$"B{rowStart}"].Value = "Hours";
                    ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                    ws.Cells[$"C{rowStart}"].Value = "Days";
                    ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                    ws.Cells[$"D{rowStart}"].Value = "Comments";
                    #endregion
                    int PhaseCount = 0;
                    decimal TotalCount = 0;
                    decimal GrandTotal = 0;
                    string AllCellName = string.Empty;
                    foreach (var item in rowgroupBy)
                    {
                        GroupCount++;
                        PhaseCount++;
                        ++rowStart;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat";
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 10;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                        ws.Cells[$"A{rowStart}"].Value = $"Phase{PhaseCount + ":" + item.Key.MainTaskName}";

                        int MainTaskID = Convert.ToInt32(item.Key.MainTaskId);
                        var list = from row in DT.AsEnumerable()
                                   where row.Field<int>("MainTaskId") == MainTaskID
                                   select row;
                        string CellName = string.Empty;
                        foreach (var listitem in list)
                        {
                            ++rowStart;
                            if (!string.IsNullOrEmpty(CellName))
                            {
                                CellName += "+B" + rowStart;
                            }
                            if (string.IsNullOrEmpty(CellName))
                            {
                                CellName += "B" + rowStart;
                            }
                            ws.Row(rowStart).Height = 20;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat Light";
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 11;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                            ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            ws.Cells[string.Format("A{0}", rowStart)].Value = listitem.Field<string>("TaskName");
                            ws.Cells[string.Format("A{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            ws.Cells[string.Format("B{0}", rowStart)].Style.Numberformat.Format = "0.00";
                            ws.Cells[string.Format("B{0}", rowStart)].Value = listitem.Field<decimal>("EstimatedDuration");
                            decimal getdays = listitem.Field<decimal>("EstimatedDuration") / 8;
                            ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                            //var asd = Convert.ToDecimal(DT.Rows[i][4]);
                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(getdays);  //.ToString("0.00")
                            ws.Cells[string.Format("D{0}", rowStart)].Value = listitem.Field<string>("Comments");
                            ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            ws.Cells[string.Format("A{0}", rowStart)].Style.WrapText = true;
                            ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                            ws.Cells[string.Format("C{0}", rowStart)].Style.WrapText = true;
                            ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;
                            ws.Row(rowStart).CustomHeight = false;
                        }
                        ++rowStart;
                        ws.Row(rowStart).Height = 20;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat Light";
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 11;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                        //ws.Cells[$"A{rowStart}:C{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                        //var getsum = list.Sum(s => s.Field<decimal>("EstimatedDuration"));
                        //TotalCount += getsum;
                        ws.Cells[string.Format("A{0}", rowStart)].Value = $"Phase{PhaseCount}";
                        ws.Cells[string.Format("A{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Numberformat.Format = "0.00";
                        ws.Cells[string.Format("B{0}", rowStart)].Formula = CellName;
                        //ws.Cells[string.Format("B{0}", rowStart)].Value = Convert.ToDecimal(getsum); 
                        //decimal getsumdays = getsum / 8;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                        //var asd = Convert.ToDecimal(DT.Rows[i][4]);
                        ws.Cells[string.Format("C{0}", rowStart)].Formula = "(" + CellName + ")/8";
                        //ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(getsumdays);  //.ToString("0.00")
                        ws.Cells[string.Format("A{0}", rowStart)].Style.WrapText = true;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.WrapText = true;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;
                        ws.Row(rowStart).CustomHeight = false;
                        AllCellName += GroupCount == rowgroupBy.Count() ? CellName : CellName + "+";
                    }
                    string TotalProjectCount = string.Empty;

                    #region Total
                    ++rowStart;
                    ++rowStart;
                    TotalProjectCount += "B" + rowStart;
                    ws.Row(rowStart).Height = 20;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat Light";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 11;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //ws.Cells[$"A{rowStart}:C{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //var getsum = list.Sum(s => s.Field<decimal>("EstimatedDuration"));
                    ws.Cells[string.Format("A{0}", rowStart)].Value = $"Total";
                    ws.Cells[string.Format("A{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    ws.Cells[string.Format("B{0}", rowStart)].Formula = AllCellName;
                    //ws.Cells[string.Format("B{0}", rowStart)].Value = Convert.ToDecimal(TotalCount);
                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    //var asd = Convert.ToDecimal(DT.Rows[i][4]);
                    ws.Cells[string.Format("C{0}", rowStart)].Formula = "(" + AllCellName + ")/8";
                    /*ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(TotalCount/8); */ //.ToString("0.00")
                    ws.Cells[string.Format("A{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;


                    ws.Row(rowStart).CustomHeight = false;
                    #endregion



                    #region ProjectManagement

                    ++rowStart;
                    TotalProjectCount += "+B" + rowStart;
                    ws.Row(rowStart).Height = 20;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat Light";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 11;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //ws.Cells[$"A{rowStart}:C{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //var getsum = list.Sum(s => s.Field<decimal>("EstimatedDuration"));
                    ws.Cells[string.Format("A{0}", rowStart)].Value = $"Project Management";
                    ws.Cells[string.Format("A{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    //decimal GetPercentage =  projectmanagementPercentge * TotalCount;
                    //ws.Cells[string.Format("B{0}", rowStart)].Value = Convert.ToDecimal(GetPercentage);
                    ws.Cells[string.Format("B{0}", rowStart)].Formula = "(" + AllCellName + ")*" + projectmanagementPercentge;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    //var asd = Convert.ToDecimal(DT.Rows[i][4]);
                    ws.Cells[string.Format("C{0}", rowStart)].Formula = "((" + AllCellName + ")*" + projectmanagementPercentge + ")/8";
                    //ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(GetPercentage/8);  //.ToString("0.00")
                    ws.Cells[string.Format("A{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;


                    ws.Row(rowStart).CustomHeight = false;
                    #endregion



                    #region GrandTotal

                    ++rowStart;

                    ws.Row(rowStart).Height = 20;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Bold = true;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Name = "Montserrat Light";
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Font.Size = 11;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //ws.Cells[$"A{rowStart}:C{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                    //var getsum = list.Sum(s => s.Field<decimal>("EstimatedDuration"));
                    ws.Cells[string.Format("A{0}", rowStart)].Value = $"Grand Total";
                    ws.Cells[string.Format("A{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    //GrandTotal = 0 + TotalCount;
                    ws.Cells[string.Format("B{0}", rowStart)].Formula = TotalProjectCount;
                    //ws.Cells[string.Format("B{0}", rowStart)].Value = Convert.ToDecimal(GrandTotal);
                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                    //var asd = Convert.ToDecimal(DT.Rows[i][4]);"(" + TotalProjectCount + ")/8";
                    ws.Cells[string.Format("C{0}", rowStart)].Formula = "(" + TotalProjectCount + ")/8";
                    //ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(GrandTotal / 8);  //.ToString("0.00")
                    ws.Cells[string.Format("A{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("C{0}", rowStart)].Style.WrapText = true;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;


                    ws.Row(rowStart).CustomHeight = false;
                    #endregion
                    //ws.Column(7).Width = 80;
                    //ws.Column(6).Width = 15;
                    ws.Column(4).Width = 50;
                    ws.Column(3).Width = 12;
                    ws.Column(2).Width = 12;
                    //ws.Column(2).Width = 20;
                    ws.Column(1).Width = 50;
                }
                pck.Save();

                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "TimeSheetWeekly.xlsx",//assignmentWeekly.fileName,
                    Inline = true,
                };

                HttpResponse Response = HttpContext.Current.Response;
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Tuple<string, bool> GeneratepdfBreakDownClientWise(ExportAssignmentBO assignmentWeekly, string filePath, string getProject_Name)
        {
            try
            {
                string fontpath = System.Web.Hosting.HostingEnvironment.MapPath("~/fonts/");
                Font fontMainHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                Font fontSubHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 12.0f, Font.BOLD, BaseColor.BLACK);
                Font fontHeader = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 11.0f, Font.BOLD, BaseColor.WHITE);
                Font fontWeekHours = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 9.0f, Font.BOLD, BaseColor.WHITE);
                Font fontTaskEntry = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.BOLD, BaseColor.DARK_GRAY);
                Font fontTaskEntry1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);

                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }
                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }




                int PdfCount = 0;
                Document document = new Document();
                System.IO.FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                document.SetPageSize(iTextSharp.text.PageSize.LEGAL);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);

                writer.PageEvent = new Footer();
                document.Open();

                PdfPTable table = new PdfPTable(5);
                var breakProjectIDs = assignmentWeekly.projectwise.Split(',');
                var breakgetProject_Name = getProject_Name.Split(',');
                for (int p = 0; p < breakProjectIDs.Length; p++)
                {
                    if (p > 0)
                    {
                        document.NewPage();

                    }
                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;

                    table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });

                    ReportDAL objReportDAL = new ReportDAL();
                    DataTable BillableDT = new DataTable();
                    assignmentWeekly.ProjectId = Convert.ToInt32(breakProjectIDs[p]);
                    BillableDT = objReportDAL.GetBreakDownSheetDataTable(assignmentWeekly);
                    if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                    {
                        if (BillableDT.Columns.Count == 1)
                        {
                            return new Tuple<string, bool>(BillableDT.Rows[0][0].ToString(), false);
                        }
                        else
                        {
                            return new Tuple<string, bool>(null, false);
                        }

                    }
                    else
                    {
                        table.AddCell(new PdfPCell(new Phrase("Main Task Breakdown", fontMainHeading))
                        {
                            Colspan = 6,
                            Border = 1,
                            FixedHeight = 28,
                            BorderColor = BaseColor.LIGHT_GRAY,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            BackgroundColor = new BaseColor(18, 48, 73),
                            PaddingBottom = 8

                        });
                        if (PdfCount > 0)
                        {
                            table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                            table.AddCell(new PdfPCell(new Phrase(""))
                            {
                                Colspan = 6,
                                FixedHeight = 20,
                                Border = PdfPCell.NO_BORDER,
                                //BorderColor = BaseColor.WHIT,
                                //BackgroundColor = new BaseColor(255, 255, 255),
                                PaddingTop = 20

                            });

                        }
                        for (int i = 0; i < BillableDT.Rows.Count; i++)
                        {
                            if (BillableDT.Rows[i][1].ToString() == "ProjectName")
                            {
                                var data = BillableDT.Rows[i][2].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase(data[0], fontMainHeading))
                                {
                                    Colspan = 6,
                                    Border = 1,
                                    FixedHeight = 28,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(18, 48, 73),
                                    PaddingBottom = 8

                                });
                            }

                            if (BillableDT.Rows[i][1].ToString() == "ClientName")
                            {
                                var data = BillableDT.Rows[i][2].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Client: {data[0]}", fontSubHeading))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    FixedHeight = 28,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }
                                table.AddCell(new PdfPCell(new Phrase($"Period: {DateRange}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                var data = BillableDT.Rows[i][2].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Total Hours: {data[0]}", fontSubHeading))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Total Days: {data[1]}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                table.AddCell(new PdfPCell(new Phrase("Main Task", fontHeader))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    FixedHeight = 21,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(18, 48, 73),
                                    PaddingBottom = 5
                                });
                                table.AddCell(new PdfPCell(new Phrase("Hours", fontHeader))
                                {
                                    Colspan = 1,
                                    Border = 15,
                                    FixedHeight = 21,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(18, 48, 73),
                                    PaddingBottom = 5
                                });
                            }
                            else
                            {
                                decimal Hhours = Convert.ToDecimal(BillableDT.Rows[i][3]);
                                decimal Billhours = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                if (Hhours > 0 && Billhours > 0)
                                {
                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                    {
                                        Colspan = 4,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_LEFT,
                                        PaddingBottom = 5
                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                    });
                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][4].ToString(), fontTaskEntry1))
                                    {
                                        Colspan = 1,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        PaddingBottom = 5
                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                    });
                                }

                            }
                        }
                        document.Add(table);
                        PdfCount++;
                    }
                }

                document.Close();
                fs.Close();
                var ImgDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/");
                var ImgPath = ImgDirectory + "WaterMark-rezaid.png";
                if (!System.IO.Directory.Exists(ImgDirectory))
                {
                    System.IO.Directory.CreateDirectory(ImgDirectory);
                }
                PdfStampInExistingFile(ImgPath, filePath);
                document.Close();
                fs.Close();
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception ex)
            {
                return new Tuple<string, bool>(null, false);
            }
        }
        public Tuple<string, bool> GeneratepdfBreakDown(ExportAssignmentBO assignmentWeekly, string filePath)
        {
            try
            {
                string fontpath = System.Web.Hosting.HostingEnvironment.MapPath("~/fonts/");
                Font fontMainHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                Font fontSubHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 12.0f, Font.BOLD, BaseColor.BLACK);
                Font fontHeader = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 11.0f, Font.BOLD, BaseColor.WHITE);
                Font fontWeekHours = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 9.0f, Font.BOLD, BaseColor.WHITE);
                Font fontTaskEntry = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.BOLD, BaseColor.DARK_GRAY);
                Font fontTaskEntry1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);

                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                int PdfCount = 0;
                ReportDAL objReportDAL = new ReportDAL();
                DataTable BillableDT = new DataTable();

                System.IO.FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                Document document = new Document();
                document.SetPageSize(iTextSharp.text.PageSize.LEGAL);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);

                writer.PageEvent = new Footer();
                document.Open();

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                assignmentWeekly.AllArrays = assignmentWeekly.AllArrays.Where(x => x.ProjectIdAll > 0).ToList();

                //assignmentWeekly.ProjectId = item.ProjectIdAll;
                BillableDT = objReportDAL.GetBreakDownSheetDataTable(assignmentWeekly);
                if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                {
                    if (BillableDT.Columns.Count == 1)
                    {
                        return new Tuple<string, bool>(BillableDT.Rows[0][0].ToString(), false);
                    }
                    else
                    {
                        return new Tuple<string, bool>(null, false);
                    }

                }
                else
                {
                    table.AddCell(new PdfPCell(new Phrase("Main Task Breakdown", fontMainHeading))
                    {
                        Colspan = 6,
                        Border = 1,
                        FixedHeight = 28,
                        BorderColor = BaseColor.LIGHT_GRAY,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        BackgroundColor = new BaseColor(18, 48, 73),
                        PaddingBottom = 8

                    });
                    if (PdfCount > 0)
                    {
                        table = new PdfPTable(5);
                        table.WidthPercentage = 100;
                        table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                        table.AddCell(new PdfPCell(new Phrase(""))
                        {
                            Colspan = 6,
                            FixedHeight = 20,
                            Border = PdfPCell.NO_BORDER,
                            //BorderColor = BaseColor.WHIT,
                            //BackgroundColor = new BaseColor(255, 255, 255),
                            PaddingTop = 20

                        });

                    }
                    for (int i = 0; i < BillableDT.Rows.Count; i++)
                    {
                        if (BillableDT.Rows[i][1].ToString() == "ProjectName")
                        {
                            var data = BillableDT.Rows[i][2].ToString().Split(':');
                            table.AddCell(new PdfPCell(new Phrase(data[0], fontMainHeading))
                            {
                                Colspan = 6,
                                Border = 1,
                                FixedHeight = 28,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(18, 48, 73),
                                PaddingBottom = 8

                            });
                        }

                        if (BillableDT.Rows[i][1].ToString() == "ClientName")
                        {
                            var data = BillableDT.Rows[i][2].ToString().Split(':');
                            table.AddCell(new PdfPCell(new Phrase($"Client: {data[0]}", fontSubHeading))
                            {
                                Colspan = 4,
                                Border = 15,
                                FixedHeight = 28,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(230, 234, 235)
                            });
                            string DateRange = string.Empty;
                            if (data.Length == 1)
                            {
                                var dataDate = BillableDT.Rows[0][2].ToString().Split(':');
                                var split = dataDate[1].Split('-');
                                DateTime dateStart = Convert.ToDateTime(split[0]);
                                DateTime dateEnd = Convert.ToDateTime(split[1]);
                                DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                            }
                            if (data.Length == 2)
                            {
                                var split = data[1].Split('-');
                                DateTime dateStart = Convert.ToDateTime(split[0]);
                                DateTime dateEnd = Convert.ToDateTime(split[1]);
                                DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                            }
                            table.AddCell(new PdfPCell(new Phrase($"Period: {DateRange}", fontSubHeading))
                            {
                                Colspan = 2,
                                Border = 15,
                                FixedHeight = 23,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(230, 234, 235)
                            });
                        }
                        else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                        {
                            var data = BillableDT.Rows[i][2].ToString().Split(':');
                            table.AddCell(new PdfPCell(new Phrase($"Total Hours: {data[0]}", fontSubHeading))
                            {
                                Colspan = 4,
                                Border = 15,
                                FixedHeight = 23,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(230, 234, 235)
                            });
                            table.AddCell(new PdfPCell(new Phrase($"Total Days: {data[1]}", fontSubHeading))
                            {
                                Colspan = 2,
                                Border = 15,
                                FixedHeight = 23,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(230, 234, 235)
                            });
                            table.AddCell(new PdfPCell(new Phrase("Main Task", fontHeader))
                            {
                                Colspan = 4,
                                Border = 15,
                                FixedHeight = 21,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(18, 48, 73),
                                PaddingBottom = 5
                            });
                            table.AddCell(new PdfPCell(new Phrase("Hours", fontHeader))
                            {
                                Colspan = 1,
                                Border = 15,
                                FixedHeight = 21,
                                BorderColor = BaseColor.LIGHT_GRAY,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                BackgroundColor = new BaseColor(18, 48, 73),
                                PaddingBottom = 5
                            });
                        }
                        else
                        {
                            decimal Hhours = Convert.ToDecimal(BillableDT.Rows[i][3]);
                            decimal Billhours = Convert.ToDecimal(BillableDT.Rows[i][4]);
                            if (Hhours > 0 && Billhours > 0)
                            {
                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_LEFT,
                                    PaddingBottom = 5
                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                });
                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][4].ToString(), fontTaskEntry1))
                                {
                                    Colspan = 1,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    PaddingBottom = 5
                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                });
                            }

                        }
                    }
                    document.Add(table);
                    PdfCount++;
                }


                document.Close();
                fs.Close();
                var ImgDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/");
                var ImgPath = ImgDirectory + "WaterMark-rezaid.png";
                if (!System.IO.Directory.Exists(ImgDirectory))
                {
                    System.IO.Directory.CreateDirectory(ImgDirectory);
                }
                PdfStampInExistingFile(ImgPath, filePath);
                document.Close();
                fs.Close();
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception ex)
            {
                return new Tuple<string, bool>(null, false);
            }
        }
        public Tuple<string, bool> GenerateExcelTSWeeklyClientWise(ExportAssignmentBO assignmentWeekly, string filePath, string workSheetName, string getProject_Name)
        {
            try
            {
                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                var breakProjectIDs = assignmentWeekly.projectwise.Split(',');
                var breakgetProject_Name = getProject_Name.Split(',');
                for (int p = 0; p < breakProjectIDs.Length; p++)
                {
                    var WorkSheetName = string.Empty;
                    if (breakgetProject_Name[p].Length > 27)
                    {
                        WorkSheetName = breakgetProject_Name[p].Substring(0, 27) + "...";
                    }
                    else
                    {
                        WorkSheetName = breakgetProject_Name[p];
                    }
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(WorkSheetName);
                    int rowStart = 1;
                    DataTable DT = new DataTable();
                    DataTable BillableDT = new DataTable();
                    ReportDAL objReportDAL = new ReportDAL();
                    assignmentWeekly.ProjectId = Convert.ToInt32(breakProjectIDs[p]);
                    if (assignmentWeekly.IsDifferencehourse == true)
                    {
                        DT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                    }
                    else
                    {
                        DT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                    }
                    //BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly,  ref BillableDT);
                    DT = MakeFormateReport(DT, Convert.ToDateTime(assignmentWeekly.FromDate));
                    BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));

                    if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                    {
                        if (DT.Columns.Count == 1)
                        {
                            return new Tuple<string, bool>(DT.Rows[0][0].ToString(), false);
                        }
                        else
                        {
                            return new Tuple<string, bool>(null, false);
                        }

                    }
                    else
                    {


                        //ws.Cells["A:E"].AutoFitColumns();
                        //ws.Column(5).Width = 15;

                        for (int i = 0; i < BillableDT.Rows.Count; i++)
                        {
                            if (assignmentWeekly.ShowSubTask == true)
                            {
                                if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    //Main Heading

                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Row(rowStart).Height = 30;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 18;
                                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                                {

                                    //Project Name And Time
                                    ++rowStart;

                                    ws.Row(rowStart).Height = 25;
                                    System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Value = $"Period: {DateRange}";

                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    //Total Hours And Days
                                    ++rowStart;
                                    ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                    ws.Row(rowStart).Height = 25;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                    Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;

                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Header")
                                {
                                    //Sub Heading
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 24;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 12;
                                    System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                    ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Date";
                                    ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                    ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"C{rowStart}"].Value = "Hours";
                                    //ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                    ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"D{rowStart}"].Value = "Main Task";
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Value = "Sub Task";
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Value = "Description";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    //DateTime of Project
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"F{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    //Range and Days of Project 
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = " Montserrat";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"F{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                                }
                                else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0 || skip1 > 0)
                                        {
                                            //Task
                                            ++rowStart;
                                            ws.Row(rowStart).Height = 20;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            //if (i == 33)
                                            //{
                                            //    var asd = DT.Rows[i][2];
                                            //}

                                            ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                            ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                            ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            var asd = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                            ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                            ws.Cells[string.Format("F{0}", rowStart)].Value = BillableDT.Rows[i][7].ToString(); //item.Description;

                                            ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                            ws.Row(rowStart).CustomHeight = false;

                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                            //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                            //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                        }
                                    }
                                    else
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                        ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();



                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][8]);
                                        }
                                        else
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        }

                                        //ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                        ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                        ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                        if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                        {
                                            ws.Cells[string.Format("F{0}", rowStart)].IsRichText = true;
                                            ExcelRichText ert2 = ws.Cells[string.Format("F{0}", rowStart)].RichText.Add("(THIS WILL NOT BE CHARGED).");
                                            ert2.Color = System.Drawing.Color.Red;
                                            ExcelRichText ert3 = ws.Cells[string.Format("F{0}", rowStart)].RichText.Add(BillableDT.Rows[i][7].ToString());
                                            ert3.Color = System.Drawing.Color.Black;
                                        }
                                        else
                                        {
                                            ws.Cells[string.Format("F{0}", rowStart)].Value += BillableDT.Rows[i][7].ToString(); //item.Description;
                                        }


                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                    }


                                }
                            }
                            else
                            {
                                if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    //Main Heading

                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                    ws.Row(rowStart).Height = 30;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 18;
                                    System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                                {

                                    //Project Name And Time
                                    ++rowStart;

                                    ws.Row(rowStart).Height = 25;
                                    System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 13;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"E{rowStart}"].Value = $"Period: {DateRange}";

                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    //Total Hours And Days
                                    ++rowStart;
                                    ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                    ws.Row(rowStart).Height = 25;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 13;
                                    Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                    //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;

                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"E{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                    ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Header")
                                {
                                    //Sub Heading
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 24;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 12;
                                    System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                    ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}"].Value = "Date";
                                    ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                    ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"C{rowStart}"].Value = "Hours";
                                    //ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                    ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"D{rowStart}"].Value = "Main Task";
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Value = "Sub Task";
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Value = "Description";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    //DateTime of Project
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"E{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    //Range and Days of Project 
                                    ++rowStart;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = " Montserrat";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 10;
                                    ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                    ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                    //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                    ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                    ws.Cells[$"E{rowStart}"].Style.Numberformat.Format = "0.0";
                                    ws.Cells[$"E{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                                }
                                else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0 || skip1 > 0)
                                        {
                                            //Task
                                            ++rowStart;
                                            ws.Row(rowStart).Height = 20;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat Light";
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 11;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                            ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                            //if (i == 33)
                                            //{
                                            //    var asd = DT.Rows[i][2];
                                            //}

                                            ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                            ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                            ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            var asd = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                            //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                            ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                            //ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                            ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][7].ToString(); //item.Description;

                                            ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                            // ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                            //ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                            ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                            ws.Row(rowStart).CustomHeight = false;

                                            //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                            //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                            //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                        }
                                    }
                                    else
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                                        ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();



                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][8]);
                                        }
                                        else
                                        {
                                            ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        }

                                        //ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(DT.Rows[i][4]);  //.ToString("0.00")

                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                        ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                                                                                                            //    ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                        if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                        {
                                            ws.Cells[string.Format("E{0}", rowStart)].IsRichText = true;
                                            ExcelRichText ert2 = ws.Cells[string.Format("E{0}", rowStart)].RichText.Add("(THIS WILL NOT BE CHARGED).");
                                            ert2.Color = System.Drawing.Color.Red;
                                            ExcelRichText ert3 = ws.Cells[string.Format("E{0}", rowStart)].RichText.Add(BillableDT.Rows[i][7].ToString());
                                            ert3.Color = System.Drawing.Color.Black;
                                        }
                                        else
                                        {
                                            ws.Cells[string.Format("E{0}", rowStart)].Value += BillableDT.Rows[i][7].ToString(); //item.Description;
                                        }


                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        //ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                        ws.Row(rowStart).CustomHeight = false;

                                    }


                                }
                            }

                        }

                        DT.Clear();
                        BillableDT.Clear();


                    }
                    if (assignmentWeekly.ShowSubTask == true)
                    {
                        ws.Column(7).Width = 80;
                        ws.Column(6).Width = 80;
                        ws.Column(5).Width = 15;
                        ws.Column(4).Width = 14;
                        ws.Column(3).Width = 12;
                        ws.Column(2).Width = 20;
                        ws.Column(1).Width = 15;
                    }
                    else
                    {
                        //ws.Column(7).Width = 80;
                        // ws.Column(6).Width = 80;
                        ws.Column(5).Width = 80;
                        ws.Column(4).Width = 14;
                        ws.Column(3).Width = 12;
                        ws.Column(2).Width = 20;
                        ws.Column(1).Width = 15;
                    }


                }
                pck.Save();

                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "TimeSheetWeekly.xlsx",//assignmentWeekly.fileName,
                    Inline = true,
                };

                HttpResponse Response = HttpContext.Current.Response;
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception e)
            {
                throw e;

            }
        }
        public Tuple<string, bool> GenerateExcelTSWeekly(ExportAssignmentBO assignmentWeekly, string filePath, string workSheetName)
        {
            try
            {
                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                FileInfo newFile = new FileInfo(filePath);
                if (newFile.Exists)
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);
                int rowStart = 1;
                DataTable DT = new DataTable();
                DataTable BillableDT = new DataTable();
                ReportDAL objReportDAL = new ReportDAL();
                if (assignmentWeekly.IsDifferencehourse == true)
                {
                    DT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                }
                else
                {
                    DT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                }
                //BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly,  ref BillableDT);
                DT = MakeFormateReport(DT, Convert.ToDateTime(assignmentWeekly.FromDate));
                BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));

                if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                {
                    if (BillableDT.Columns.Count == 1)
                    {
                        return new Tuple<string, bool>(DT.Rows[0][0].ToString(), false);
                    }
                    else
                    {
                        return new Tuple<string, bool>(null, false);
                    }

                }
                else
                {


                    //ws.Cells["A:E"].AutoFitColumns();
                    //ws.Column(5).Width = 15;

                    for (int i = 0; i < BillableDT.Rows.Count; i++)
                    {
                        if (assignmentWeekly.ShowSubTask == true)
                        {
                            if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                            {
                                //Main Heading

                                ws.Cells[$"A{rowStart}:F{rowStart}"].Merge = true;
                                ws.Row(rowStart).Height = 30;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 18;
                                System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                            {

                                //Project Name And Time
                                ++rowStart;

                                ws.Row(rowStart).Height = 25;
                                System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Value = $"Period: {DateRange}";

                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                //Total Hours And Days
                                ++rowStart;
                                ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                ws.Row(rowStart).Height = 25;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 13;
                                Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;

                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Header")
                            {
                                //Sub Heading
                                ++rowStart;
                                ws.Row(rowStart).Height = 24;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 12;
                                System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Date";
                                ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"C{rowStart}"].Value = "Hours";
                                //ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"D{rowStart}"].Value = "Main Task";
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Value = "Sub Task";
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Value = "Description";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                //DateTime of Project
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"G{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"F{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                            {
                                //Range and Days of Project 
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = " Montserrat";
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"F{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"F{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                            }
                            else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0 || skip1 > 0)
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        //if (i == 33)
                                        //{
                                        //    var asd = DT.Rows[i][2];
                                        //}

                                        ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        var asd = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                        ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("F{0}", rowStart)].Value = BillableDT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                        //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                    }
                                }
                                else
                                {
                                    //Task
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 20;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Name = "Montserrat Light";
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Font.Size = 11;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:F{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                    ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][8]);
                                    }
                                    else
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    }
                                    //.ToString("0.00")

                                    //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                    ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                    ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                    if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                    {
                                        ws.Cells[string.Format("F{0}", rowStart)].IsRichText = true;
                                        ExcelRichText ert2 = ws.Cells[string.Format("F{0}", rowStart)].RichText.Add("(THIS WILL NOT BE CHARGED).");
                                        ert2.Color = System.Drawing.Color.Red;
                                        ExcelRichText ert3 = ws.Cells[string.Format("F{0}", rowStart)].RichText.Add(BillableDT.Rows[i][7].ToString());
                                        ert3.Color = System.Drawing.Color.Black;
                                    }
                                    else
                                    {
                                        ws.Cells[string.Format("F{0}", rowStart)].Value += BillableDT.Rows[i][7].ToString(); //item.Description;
                                    }

                                    //ws.Cells[string.Format("F{0}", rowStart)].Value = "(THIS WILL NOT BE CHARGED).";
                                    //ws.Cells[string.Format("F{0}", rowStart)].Value +=  BillableDT.Rows[i][7].ToString(); //item.Description;

                                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                    ws.Row(rowStart).CustomHeight = false;

                                    //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                    //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                }


                            }
                        }
                        else
                        {
                            if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                            {
                                //Main Heading

                                ws.Cells[$"A{rowStart}:E{rowStart}"].Merge = true;
                                ws.Row(rowStart).Height = 30;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 18;
                                System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Tasks' Details";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                            {

                                //Project Name And Time
                                ++rowStart;

                                ws.Row(rowStart).Height = 25;
                                System.Drawing.Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");//#d3d3d3
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 13;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Client: {data[0]}";
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"E{rowStart}"].Value = $"Period: {DateRange}";

                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                //Total Hours And Days
                                ++rowStart;
                                ws.Row(rowStart).Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Right.Color.SetColor(System.Drawing.Color.White);
                                ws.Row(rowStart).Height = 25;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 13;
                                Color LightGrayColor = System.Drawing.ColorTranslator.FromHtml("#e6eaeb");
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(LightGrayColor);
                                //ws.Cells["A2:E2"].Style.Font.Bold = false;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;

                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"Total Hours: {Convert.ToDecimal(data[0])}";

                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"E{rowStart}"].Value = $"Total Days: {Convert.ToDecimal(data[1])}";
                                ws.Row(rowStart).Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                ws.Row(rowStart).Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Header")
                            {
                                //Sub Heading
                                ++rowStart;
                                ws.Row(rowStart).Height = 24;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                //ws.Cells["A5:E5"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 12;
                                System.Drawing.Color colFromHexSmallHeader = System.Drawing.ColorTranslator.FromHtml("#123049");
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexSmallHeader);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);

                                ws.Cells[$"A{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}"].Value = "Date";
                                ws.Cells[$"B{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"B{rowStart}"].Value = "Task Owner";
                                ws.Cells[$"C{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"C{rowStart}"].Value = "Hours";
                                //ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"D{rowStart}"].Value = "Billable Hrs";
                                ws.Cells[$"D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"D{rowStart}"].Value = "Main Task";
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Value = "Sub Task";
                                //ws.Cells[$"F{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Value = "Description";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                //DateTime of Project
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"E{rowStart}"].Value = $"Hours: {Convert.ToDecimal(data[1])}";
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                            {
                                //Range and Days of Project 
                                ++rowStart;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                System.Drawing.Color colFromHexDateHeader = System.Drawing.ColorTranslator.FromHtml("#236091");
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Fill.BackgroundColor.SetColor(colFromHexDateHeader);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Color.SetColor(System.Drawing.Color.White);
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = " Montserrat";
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Bold = true;
                                ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 10;
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Merge = true;
                                ws.Cells[$"A{rowStart}:D{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                ws.Cells[$"A{rowStart}"].Value = $"{data[0]}";
                                //ws.Cells[$"E{rowStart}:F{rowStart}"].Merge = true;
                                ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                //ws.Cells[$"E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.White);
                                ws.Cells[$"E{rowStart}"].Style.Numberformat.Format = "0.0";
                                ws.Cells[$"E{rowStart}"].Value = $"Days: {Convert.ToDecimal(data[1])}";


                            }
                            else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    //decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0 )
                                    {
                                        //Task
                                        ++rowStart;
                                        ws.Row(rowStart).Height = 20;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat Light";
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 11;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                        ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                        //if (i == 33)
                                        //{
                                        //    var asd = DT.Rows[i][2];
                                        //}

                                        ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                        ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                        ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        var asd = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                        //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")

                                        ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                        //ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                        ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][7].ToString(); //item.Description;

                                        ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                        //ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                        ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;


                                        ws.Row(rowStart).CustomHeight = false;

                                        //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                        //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                        //ws.Cells[string.Format("F{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;}

                                    }
                                }
                                else
                                {
                                    //Task
                                    ++rowStart;
                                    ws.Row(rowStart).Height = 20;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Name = "Montserrat Light";
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Font.Size = 11;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Left.Color.SetColor(System.Drawing.Color.LightGray);

                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.Border.Right.Color.SetColor(System.Drawing.Color.LightGray);
                                    ws.Cells[$"A{rowStart}:E{rowStart}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                    ws.Cells[string.Format("A{0}", rowStart)].Value = BillableDT.Rows[i][2];
                                    ws.Cells[string.Format("B{0}", rowStart)].Value = BillableDT.Rows[i][3].ToString();

                                    ws.Cells[string.Format("C{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    ws.Cells[string.Format("C{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Style.Font.Color.SetColor(System.Drawing.Color.Red);
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][8]);
                                    }
                                    else
                                    {
                                        ws.Cells[string.Format("C{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    }
                                    //.ToString("0.00")

                                    //ws.Cells[string.Format("D{0}", rowStart)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    //ws.Cells[string.Format("D{0}", rowStart)].Style.Numberformat.Format = "0.00";
                                    //ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(BillableDT.Rows[i][4]);  //.ToString("0.00")



                                    ws.Cells[string.Format("D{0}", rowStart)].Value = BillableDT.Rows[i][5].ToString(); //item.Task; 
                                    //ws.Cells[string.Format("E{0}", rowStart)].Value = BillableDT.Rows[i][6].ToString(); //item.SubTask;
                                    if (Convert.ToDecimal(BillableDT.Rows[i][4]) == Convert.ToDecimal(0))
                                    {
                                        ws.Cells[string.Format("E{0}", rowStart)].IsRichText = true;
                                        ExcelRichText ert2 = ws.Cells[string.Format("E{0}", rowStart)].RichText.Add("(THIS WILL NOT BE CHARGED).");
                                        ert2.Color = System.Drawing.Color.Red;
                                        ExcelRichText ert3 = ws.Cells[string.Format("E{0}", rowStart)].RichText.Add(BillableDT.Rows[i][7].ToString());
                                        ert3.Color = System.Drawing.Color.Black;
                                    }
                                    else
                                    {
                                        ws.Cells[string.Format("E{0}", rowStart)].Value += BillableDT.Rows[i][7].ToString(); //item.Description;
                                    }

                                    //ws.Cells[string.Format("F{0}", rowStart)].Value = "(THIS WILL NOT BE CHARGED).";
                                    //ws.Cells[string.Format("F{0}", rowStart)].Value +=  BillableDT.Rows[i][7].ToString(); //item.Description;

                                    ws.Cells[string.Format("B{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;
                                    //ws.Cells[string.Format("F{0}", rowStart)].Style.WrapText = true;
                                    //ws.Cells[string.Format("G{0}", rowStart)].Style.WrapText = true;
                                    ws.Cells[string.Format("D{0}", rowStart)].Style.WrapText = true;

                                    ws.Row(rowStart).CustomHeight = false;

                                    //ws.Cells[string.Format("D{0}", rowStart)].Value = DT.Rows[i][8].ToString(); //item.SubTask;
                                    //ws.Cells[string.Format("E{0}", rowStart)].Style.WrapText = true;

                                }


                            }
                        }

                    }

                    DT.Clear();
                    BillableDT.Clear();


                }
                if (assignmentWeekly.ShowSubTask == true)
                {
                    ws.Column(6).Width = 80;
                    ws.Column(5).Width = 15;
                    ws.Column(4).Width = 15;
                    ws.Column(3).Width = 12;
                    ws.Column(2).Width = 20;
                    ws.Column(1).Width = 15;
                }
                else
                {
                    // ws.Column(6).Width = 80;
                    ws.Column(5).Width = 80;
                    ws.Column(4).Width = 15;
                    ws.Column(3).Width = 12;
                    ws.Column(2).Width = 20;
                    ws.Column(1).Width = 15;
                }
                // ws.Column(7).Width = 80;



                pck.Save();

                byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                string contentType = MimeMapping.GetMimeMapping(filePath);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = "TimeSheetWeekly.xlsx",//assignmentWeekly.fileName,
                    Inline = true,
                };

                HttpResponse Response = HttpContext.Current.Response;
                Response.AppendHeader("Content-Disposition", cd.ToString());
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception e)
            {
                throw e;

            }
        }
        public DataTable MakeFormateReport(DataTable GetData, DateTime FromDate)
        {
            DataTable custTable = new DataTable();
            DataColumn dtColumn;
            DataRow myDataRow;
            List<AssignmentsBO> model = new List<AssignmentsBO>();
            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(Int32);
            dtColumn.ColumnName = "ID";
            dtColumn.Caption = "ID";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "RowDescription";
            dtColumn.Caption = "RowDescription";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "Date";
            dtColumn.Caption = "Date";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "TaskOwner";
            dtColumn.Caption = "TaskOwner";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "Hours";
            dtColumn.Caption = "Hours";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "Main Task";
            dtColumn.Caption = "Main Task";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "Sub Task";
            dtColumn.Caption = "Sub Task";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "Description";
            dtColumn.Caption = "Description";
            custTable.Columns.Add(dtColumn);

            dtColumn = new DataColumn();
            dtColumn.DataType = typeof(string);
            dtColumn.ColumnName = "ActualDuration";
            dtColumn.Caption = "ActualDuration";
            custTable.Columns.Add(dtColumn);

            var mainheading = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Mainheading").Select(s => s.Field<string>("Description")).ToArray();
            if (mainheading.Length > 0)
            {
                for (var i = 0; i < mainheading.Length; i++)
                {
                    myDataRow = custTable.NewRow();
                    myDataRow["RowDescription"] = "Mainheading";
                    myDataRow["Description"] = mainheading[i];
                    custTable.Rows.Add(myDataRow);
                }
            }

            var Client_Period = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Client:Period").Select(s => s.Field<string>("Description")).ToArray();
            if (Client_Period.Length > 0)
            {
                for (var i = 0; i < Client_Period.Length; i++)
                {
                    myDataRow = custTable.NewRow();
                    myDataRow["RowDescription"] = "Client:Period";
                    myDataRow["Description"] = Client_Period[i];
                    custTable.Rows.Add(myDataRow);
                }
            }

            var TotalHours_TotalDays = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Total Hours:Total Days").Select(s => s.Field<string>("Description")).ToArray();
            if (TotalHours_TotalDays.Length > 0)
            {
                for (var i = 0; i < TotalHours_TotalDays.Length; i++)
                {
                    myDataRow = custTable.NewRow();
                    myDataRow["RowDescription"] = "Total Hours:Total Days";
                    myDataRow["Description"] = TotalHours_TotalDays[i];
                    custTable.Rows.Add(myDataRow);
                }
            }

            var Header = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Header").Select(s => s.Field<string>("Description")).ToArray();
            if (Header.Length > 0)
            {
                for (var i = 0; i < Header.Length; i++)
                {
                    myDataRow = custTable.NewRow();
                    myDataRow["RowDescription"] = "Header";
                    myDataRow["Description"] = Header[i];
                    custTable.Rows.Add(myDataRow);
                }
            }

            var DateRange = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Range:Days").Select(s => s.Field<string>("Description")).ToArray();
            string[] Weeks = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Week:Hours").Select(s => s.Field<string>("Description")).ToArray();
            var EntryList = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "TaskEntry").Select(s => new AssignmentsBO
            {
                RowDescription = "TaskEntry",
                AssignmentDT = Convert.ToDateTime(s.Field<string>("Date").ToString()),
                UserFullName = s.Field<string>("TaskOwner").ToString(),
                hours = s.Field<string>("Hours").ToString(),
                ActualDuration = s.Field<string>("ActualDuration").ToString(),
                CommentText = s.Field<string>("Description").ToString(),
                MainTaskName = s.Field<string>("Main Task").ToString(),
                TaskName = s.Field<string>("Sub Task").ToString(),
            }).OrderBy(f => f.AssignmentDT).ThenBy(d => d.UserFullName).ToList();
            string EndDate = string.Empty;
            for (var row = 0; row < DateRange.Length; row++)
            {
                var DateSplit = DateRange[row].ToString().Split(':');
                if (DateSplit.Length > 0)
                {
                    var getfromtodate = DateSplit[0].Split('-');
                    if (getfromtodate.Length > 0)
                    {
                        myDataRow = custTable.NewRow();
                        myDataRow["RowDescription"] = "Range:Days";
                        myDataRow["Description"] = DateRange[row];
                        custTable.Rows.Add(myDataRow);

                        myDataRow = custTable.NewRow();
                        myDataRow["RowDescription"] = "Week:Hours";
                        myDataRow["Description"] = Weeks[row];
                        custTable.Rows.Add(myDataRow);

                        DateTime startdate = Convert.ToDateTime(getfromtodate[0]);
                        DateTime enddate = Convert.ToDateTime(getfromtodate[1]);
                        //var DateDifference = startdate - FromDate;
                        //if (DateDifference.Days > 0)
                        //{

                        //    startdate = startdate.AddDays(-DateDifference.Days);
                        //}
                        if (enddate.DayOfWeek == DayOfWeek.Friday)
                        {
                            enddate = enddate.AddDays(2);
                        }
                        var firstdateformat = startdate.ToString("yyyy-MM-dd");
                        var seconddateformat = enddate.ToString("yyyy-MM-dd");
                        var getentrylist = EntryList.Where(x => x.AssignmentDT >= startdate.Date && x.AssignmentDT <= enddate.Date).Select(s => new AssignmentsBO
                        {
                            RowDescription = s.RowDescription,
                            AssignmentDateTime = s.AssignmentDT.ToString("dd-MM-yyyy"),
                            UserFullName = s.UserFullName.ToString(),
                            hours = s.hours.ToString(),
                            ActualDuration = s.ActualDuration.ToString(),
                            CommentText = s.CommentText.ToString(),
                            MainTaskName = s.MainTaskName,
                            TaskName = s.TaskName,
                        }).ToList();
                        foreach (var item in getentrylist)
                        {
                            myDataRow = custTable.NewRow();
                            myDataRow["RowDescription"] = item.RowDescription;
                            myDataRow["Date"] = item.AssignmentDateTime;
                            myDataRow["TaskOwner"] = item.UserFullName;
                            myDataRow["Hours"] = item.hours;
                            myDataRow["ActualDuration"] = item.ActualDuration;
                            myDataRow["Description"] = item.CommentText;
                            myDataRow["Main Task"] = item.MainTaskName;
                            myDataRow["Sub Task"] = item.TaskName;
                            custTable.Rows.Add(myDataRow);
                        }
                        // model.AddRange(getentrylist);
                    }
                }
            }


            return custTable;
        }
        public Tuple<string, bool> GeneratePdfTSWeeklyClientWise(ExportAssignmentBO assignmentWeekly, string filePath, string workSheetName, string getProject_Name)
        {
            try
            {
                string fontpath = System.Web.Hosting.HostingEnvironment.MapPath("~/fonts/");
                Font fontMainHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                Font fontSubHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 12.0f, Font.BOLD, BaseColor.BLACK);
                Font fontHeader = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 11.0f, Font.BOLD, BaseColor.WHITE);
                Font fontWeekHours = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 9.0f, Font.BOLD, BaseColor.WHITE);
                Font fontTaskEntry = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);

                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                Document document = new Document();
                System.IO.FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                document.SetPageSize(iTextSharp.text.PageSize.LEGAL);
                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);

                writer.PageEvent = new Footer();
                document.Open();

                PdfPTable table = assignmentWeekly.ShowSubTask == true ? new PdfPTable(6) : new PdfPTable(5);
                var breakProjectIDs = assignmentWeekly.projectwise.Split(',');
                var breakgetProject_Name = getProject_Name.Split(',');
                for (int p = 0; p < breakProjectIDs.Length; p++)
                {
                    if (p > 0)
                    {
                        document.NewPage();

                    }
                    table = assignmentWeekly.ShowSubTask == true ? new PdfPTable(6) : new PdfPTable(5);
                    table.WidthPercentage = 100;
                    if (assignmentWeekly.ShowSubTask)
                    {
                        table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                    }
                    else
                    {
                        table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                    }


                    ReportDAL objReportDAL = new ReportDAL();
                    DataTable BillableDT = new DataTable();
                    assignmentWeekly.ProjectId = Convert.ToInt32(breakProjectIDs[p]);
                    if (assignmentWeekly.IsDifferencehourse == true)
                    {
                        BillableDT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                    }
                    else
                    {
                        BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                    }
                    

                    //DT = MakeFormateReport(DT);
                    BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));
                    if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                    {
                        if (BillableDT.Columns.Count == 1)
                        {
                            return new Tuple<string, bool>(BillableDT.Rows[0][0].ToString(), false);
                        }
                        else
                        {
                            return new Tuple<string, bool>(null, false);
                        }

                    }
                    else
                    {
                        for (int i = 0; i < BillableDT.Rows.Count; i++)
                        {
                            if (assignmentWeekly.ShowSubTask)
                            {
                                if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    if (string.IsNullOrEmpty(assignmentWeekly.pdftextboxvalue))
                                    {
                                        table.AddCell(new PdfPCell(new Phrase("Tasks' Details", fontMainHeading))
                                        {
                                            Colspan = 7,
                                            Border = 1,
                                            FixedHeight = 28,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                    }
                                    else
                                    {
                                        //Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                                        //Chunk redText = new Chunk("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle);
                                        //Chunk remaingtext = new Chunk("Tasks' Details", fontMainHeading);
                                        //var phrase = new Phrase(remaingtext);
                                        //phrase.Add(redText);
                                        table.AddCell(new PdfPCell(new Phrase("Tasks' Details (Ref#" + assignmentWeekly.pdftextboxvalue + ")", fontMainHeading))
                                        {
                                            Colspan = 7,
                                            Border = 1,
                                            FixedHeight = 28,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                        //table.AddCell(new PdfPCell(new Phrase("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle))
                                        //{
                                        //    Colspan = 2,
                                        //    Border = 1,
                                        //    FixedHeight = 28,
                                        //    BorderColor = BaseColor.LIGHT_GRAY,
                                        //    VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //    //HorizontalAlignment = Element.ALIGN_RIGHT,
                                        //    BackgroundColor = new BaseColor(18, 48, 73)
                                        //});
                                    }
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"Client : {data[0]}", fontSubHeading))
                                    {
                                        Colspan = 5,
                                        Border = 15,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }
                                    table.AddCell(new PdfPCell(new Phrase($"Period : {DateRange}", fontSubHeading))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"Total Hours : {data[0]}", fontSubHeading))
                                    {
                                        Colspan = 5,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Total Days : {data[1]}", fontSubHeading))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Header")
                                {
                                    for (int J = 2; J < BillableDT.Columns.Count - 1; J++)
                                    {
                                        if (J == 6)
                                        {
                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[6].ColumnName.ToString(), fontHeader))
                                            {
                                                Colspan = 1,
                                                Border = 15,
                                                FixedHeight = 21,
                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                BackgroundColor = new BaseColor(18, 48, 73)
                                            });
                                        }

                                        //else if (J == 7)
                                        //{

                                        //    table.AddCell(new PdfPCell(new Phrase("Status", fontHeader))
                                        //    {
                                        //        Colspan = 1,
                                        //        Border = 15,
                                        //        FixedHeight = 21,
                                        //        BorderColor = BaseColor.LIGHT_GRAY,
                                        //        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //        HorizontalAlignment = Element.ALIGN_CENTER,
                                        //        BackgroundColor = new BaseColor(18, 48, 73)
                                        //    });

                                        //}
                                        else
                                        {

                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase("Task Owner", fontHeader))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    FixedHeight = 21,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    BackgroundColor = new BaseColor(18, 48, 73)
                                                });
                                            }
                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        FixedHeight = 21,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        BackgroundColor = new BaseColor(18, 48, 73)
                                                    });
                                                }
                                                if (J == 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        FixedHeight = 21,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        BackgroundColor = new BaseColor(18, 48, 73)
                                                    });
                                                }

                                            }
                                        }
                                    }
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                    {
                                        Colspan = 5,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Hours : {data[1]}", fontWeekHours))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                    {
                                        Colspan = 5,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Days : {data[1]}", fontWeekHours))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        //decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0)
                                        {
                                            Font fontH1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                if (J == 6)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][6].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                    if (J == 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Font fontH1 = null;
                                        double gethours = Convert.ToDouble(BillableDT.Rows[i][4]);
                                        if (gethours == 0)
                                        {
                                            fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.BLACK);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 6)
                                                {
                                                    Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                    Chunk redText = new Chunk("(THIS WILL NOT BE CHARGED). ", CellStyle);
                                                    Chunk remaingtext = new Chunk(BillableDT.Rows[i][6].ToString(), fontTaskEntry);
                                                    var phrase = new Phrase(redText);
                                                    phrase.Add(remaingtext);
                                                    table.AddCell(new PdfPCell(new Phrase(phrase))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        if (J == 4)
                                                        {
                                                            Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][8].ToString(), CellStyle))
                                                            {
                                                                Colspan = 1,
                                                                Border = 15,
                                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                                //  BackgroundColor = new BaseColor(255, 255, 255),

                                                            });
                                                        }
                                                        else
                                                        {
                                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                            {
                                                                Colspan = 1,
                                                                Border = 15,
                                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                                //  BackgroundColor = new BaseColor(255, 255, 255),

                                                            });
                                                        }

                                                    }
                                                    if (J == 7)
                                                    {
                                                        Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                        Chunk redText = new Chunk("(THIS WILL NOT BE CHARGED). ", CellStyle);
                                                        Chunk remaingtext = new Chunk(BillableDT.Rows[i][7].ToString(), fontTaskEntry);
                                                        var phrase = new Phrase(redText);
                                                        phrase.Add(remaingtext);
                                                        table.AddCell(new PdfPCell(new Phrase(phrase))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            // BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 6)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][6].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                    if (J == 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            // BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    table.AddCell(new PdfPCell(new Phrase(""))
                                    {
                                        Colspan = 7,
                                        FixedHeight = 20,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        BackgroundColor = new BaseColor(255, 255, 255)
                                    });


                                }
                            }
                            else
                            {
                                if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                                {
                                    if (string.IsNullOrEmpty(assignmentWeekly.pdftextboxvalue))
                                    {
                                        table.AddCell(new PdfPCell(new Phrase("Tasks' Details", fontMainHeading))
                                        {
                                            Colspan = 6,
                                            Border = 1,
                                            FixedHeight = 28,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                    }
                                    else
                                    {
                                        //Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                                        //Chunk redText = new Chunk("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle);
                                        //Chunk remaingtext = new Chunk("Tasks' Details", fontMainHeading);
                                        //var phrase = new Phrase(remaingtext);
                                        //phrase.Add(redText);
                                        table.AddCell(new PdfPCell(new Phrase("Tasks' Details (Ref#" + assignmentWeekly.pdftextboxvalue + ")", fontMainHeading))
                                        {
                                            Colspan = 5,
                                            Border = 1,
                                            FixedHeight = 28,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                        //table.AddCell(new PdfPCell(new Phrase("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle))
                                        //{
                                        //    Colspan = 2,
                                        //    Border = 1,
                                        //    FixedHeight = 28,
                                        //    BorderColor = BaseColor.LIGHT_GRAY,
                                        //    VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //    //HorizontalAlignment = Element.ALIGN_RIGHT,
                                        //    BackgroundColor = new BaseColor(18, 48, 73)
                                        //});
                                    }
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"Client : {data[0]}", fontSubHeading))
                                    {
                                        Colspan = 4,
                                        Border = 15,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                    string DateRange = string.Empty;
                                    if (data.Length == 2)
                                    {
                                        var split = data[1].Split('-');
                                        DateTime dateStart = Convert.ToDateTime(split[0]);
                                        DateTime dateEnd = Convert.ToDateTime(split[1]);
                                        DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                    }
                                    table.AddCell(new PdfPCell(new Phrase($"Period : {DateRange}", fontSubHeading))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"Total Hours : {data[0]}", fontSubHeading))
                                    {
                                        Colspan = 4,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Total Days : {data[1]}", fontSubHeading))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        FixedHeight = 23,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(230, 234, 235)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Header")
                                {
                                    for (int J = 2; J < BillableDT.Columns.Count - 1; J++)
                                    {
                                        if (J == 6)
                                        {
                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[7].ColumnName.ToString(), fontHeader))
                                            {
                                                Colspan = 1,
                                                Border = 15,
                                                FixedHeight = 21,
                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                BackgroundColor = new BaseColor(18, 48, 73)
                                            });
                                        }

                                        //else if (J == 7)
                                        //{

                                        //    table.AddCell(new PdfPCell(new Phrase("Status", fontHeader))
                                        //    {
                                        //        Colspan = 1,
                                        //        Border = 15,
                                        //        FixedHeight = 21,
                                        //        BorderColor = BaseColor.LIGHT_GRAY,
                                        //        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //        HorizontalAlignment = Element.ALIGN_CENTER,
                                        //        BackgroundColor = new BaseColor(18, 48, 73)
                                        //    });

                                        //}
                                        else
                                        {

                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase("Task Owner", fontHeader))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    FixedHeight = 21,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    BackgroundColor = new BaseColor(18, 48, 73)
                                                });
                                            }
                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        FixedHeight = 21,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        BackgroundColor = new BaseColor(18, 48, 73)
                                                    });
                                                }

                                            }
                                        }
                                    }
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                    {
                                        Colspan = 4,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Hours : {data[1]}", fontWeekHours))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                                {
                                    var data = BillableDT.Rows[i][7].ToString().Split(':');
                                    table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                    {
                                        Colspan = 4,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                    table.AddCell(new PdfPCell(new Phrase($"Days : {data[1]}", fontWeekHours))
                                    {
                                        Colspan = 2,
                                        Border = 15,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(35, 96, 145)
                                    });
                                }
                                else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                                {
                                    if (!assignmentWeekly.IsZeroRowShow)
                                    {
                                        decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        //decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                        if (skip > 0)
                                        {
                                            Font fontH1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                if (J == 6)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Font fontH1 = null;
                                        double gethours = Convert.ToDouble(BillableDT.Rows[i][4]);
                                        if (gethours == 0)
                                        {
                                            fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.BLACK);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 6)
                                                {
                                                    Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                    Chunk redText = new Chunk("(THIS WILL NOT BE CHARGED). ", CellStyle);
                                                    Chunk remaingtext = new Chunk(BillableDT.Rows[i][7].ToString(), fontTaskEntry);
                                                    var phrase = new Phrase(redText);
                                                    phrase.Add(remaingtext);
                                                    table.AddCell(new PdfPCell(new Phrase(phrase))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        if (J == 4)
                                                        {
                                                            Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][8].ToString(), CellStyle))
                                                            {
                                                                Colspan = 1,
                                                                Border = 15,
                                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                                //  BackgroundColor = new BaseColor(255, 255, 255),

                                                            });
                                                        }
                                                        else
                                                        {
                                                            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                            {
                                                                Colspan = 1,
                                                                Border = 15,
                                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                                //  BackgroundColor = new BaseColor(255, 255, 255),

                                                            });
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                            for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                            {
                                                if (J == 3)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 6)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                                else
                                                {
                                                    if (J != 7)
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                }
                                            }
                                        }

                                        //Font fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                        //for (int J = 3; J < BillableDT.Columns.Count; J++)
                                        //{
                                        //    if (J == 3)
                                        //    {
                                        //        table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(BillableDT.Rows[i][2]).ToString("dd-MM-yyyy"), fontTaskEntry))
                                        //        {
                                        //            Colspan = 1,
                                        //            Border = 15,
                                        //            BorderColor = BaseColor.LIGHT_GRAY,
                                        //            VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //            HorizontalAlignment = Element.ALIGN_CENTER,
                                        //            // BackgroundColor = new BaseColor(255, 255, 255),

                                        //        });
                                        //    }
                                        //    if (J == 6)
                                        //    {
                                        //        var asd = BillableDT.Rows[i][J].ToString();
                                        //        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                        //        {
                                        //            Colspan = 1,
                                        //            Border = 15,
                                        //            BorderColor = BaseColor.LIGHT_GRAY,
                                        //            VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //            HorizontalAlignment = Element.ALIGN_CENTER,
                                        //            // BackgroundColor = new BaseColor(255, 255, 255),

                                        //        });
                                        //    }

                                        //    else
                                        //    {
                                        //        var assd = BillableDT.Rows[i][J].ToString();
                                        //        if (J != 7)
                                        //        {
                                        //            var asd = BillableDT.Rows[i][J].ToString();
                                        //            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                        //            {
                                        //                Colspan = 1,
                                        //                Border = 15,
                                        //                BorderColor = BaseColor.LIGHT_GRAY,
                                        //                VerticalAlignment = Element.ALIGN_MIDDLE,
                                        //                HorizontalAlignment = Element.ALIGN_CENTER,
                                        //                //  BackgroundColor = new BaseColor(255, 255, 255),

                                        //            });
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                                else
                                {
                                    table.AddCell(new PdfPCell(new Phrase(""))
                                    {
                                        Colspan = 6,
                                        FixedHeight = 20,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        BackgroundColor = new BaseColor(255, 255, 255)
                                    });


                                }
                            }

                        }
                        document.Add(table);



                    }
                }
                document.Close();
                fs.Close();

                var ImgDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/");
                var ImgPath = ImgDirectory + "WaterMark-rezaid.png";
                if (!System.IO.Directory.Exists(ImgDirectory))
                {
                    System.IO.Directory.CreateDirectory(ImgDirectory);
                }
                PdfStampInExistingFile(ImgPath, filePath);
                document.Close();
                fs.Close();
                return new Tuple<string, bool>(null, true);
            }
            catch (Exception e)
            {

                return new Tuple<string, bool>(null, false);
            }
        }
        public Tuple<string, bool> GeneratePdfTSWeekly(ExportAssignmentBO assignmentWeekly, string filePath)
        {
            try
            {
                string fontpath = System.Web.Hosting.HostingEnvironment.MapPath("~/fonts/");
                Font fontMainHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                Font fontSubHeading = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 12.0f, Font.BOLD, BaseColor.BLACK);
                Font fontHeader = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 11.0f, Font.BOLD, BaseColor.WHITE);
                Font fontWeekHours = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 9.0f, Font.BOLD, BaseColor.WHITE);
                Font fontTaskEntry = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);

                if (assignmentWeekly.ProjectId == 0)
                {
                    assignmentWeekly.ProjectId = -1;
                }
                if (assignmentWeekly.TaskOwnerId == null)
                {
                    assignmentWeekly.TaskOwnerId = null;
                }
                if (assignmentWeekly.TaskId == 0)
                {
                    assignmentWeekly.TaskId = -1;
                }
                if (assignmentWeekly.SubTaskId == 0)
                {
                    assignmentWeekly.SubTaskId = -1;
                }
                if (assignmentWeekly.commasepartedTaskOwnerids == null)
                {
                    string check = "";
                    if (assignmentWeekly.TaskOwnerId == null)
                    {
                        check = "-1";
                    }
                    else
                    {
                        check = string.Join(", ", assignmentWeekly.TaskOwnerId);
                    }


                    assignmentWeekly.commasepartedTaskOwnerids = check;
                }
                ReportDAL objReportDAL = new ReportDAL();
                DataTable BillableDT = new DataTable();
                if (assignmentWeekly.IsDifferencehourse == true)
                {
                    BillableDT = objReportDAL.DifferenceHourse(assignmentWeekly, ref BillableDT);
                }
                else
                {
                    BillableDT = objReportDAL.GetTimeSheetDataTable(assignmentWeekly, ref BillableDT);
                }
                //DT = MakeFormateReport(DT);
                BillableDT = MakeFormateReport(BillableDT, Convert.ToDateTime(assignmentWeekly.FromDate));
                if (BillableDT == null || BillableDT.Rows.Count == 0 || BillableDT.Columns.Count == 1)
                {
                    if (BillableDT.Columns.Count == 1)
                    {
                        return new Tuple<string, bool>(BillableDT.Rows[0][0].ToString(), false);
                    }
                    else
                    {
                        return new Tuple<string, bool>(null, false);
                    }

                }
                else
                {
                    System.IO.FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    Document document = new Document();
                    document.SetPageSize(iTextSharp.text.PageSize.LEGAL);
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, fs);

                    writer.PageEvent = new Footer();
                    document.Open();

                    PdfPTable table = assignmentWeekly.ShowSubTask == true ? new PdfPTable(6) : new PdfPTable(5);
                    table.WidthPercentage = 100;

                    if (assignmentWeekly.ShowSubTask == true)
                    {
                        table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                             (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                    }
                    else
                    {
                        table.SetTotalWidth(new float[] {

                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 4,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25) / 3,
                            (iTextSharp.text.PageSize.A4.Rotate().Width - 25)

                        });
                    }

                    for (int i = 0; i < BillableDT.Rows.Count; i++)
                    {
                        if (assignmentWeekly.ShowSubTask == true)
                        {
                            if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                            {
                                if (string.IsNullOrEmpty(assignmentWeekly.pdftextboxvalue))
                                {
                                    table.AddCell(new PdfPCell(new Phrase("Tasks' Details", fontMainHeading))
                                    {
                                        Colspan = 7,
                                        Border = 1,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(18, 48, 73)
                                    });
                                }
                                else
                                {
                                    table.AddCell(new PdfPCell(new Phrase("Tasks' Details (Ref#" + assignmentWeekly.pdftextboxvalue + ")", fontMainHeading))
                                    {
                                        Colspan = 6,
                                        Border = 1,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(18, 48, 73)
                                    });
                                }
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Client : {data[0]}", fontSubHeading))
                                {
                                    Colspan = 5,
                                    Border = 15,
                                    FixedHeight = 28,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }
                                table.AddCell(new PdfPCell(new Phrase($"Period : {DateRange}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Total Hours : {data[0]}", fontSubHeading))
                                {
                                    Colspan = 5,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Total Days : {data[1]}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Header")
                            {
                                for (int J = 2; J < BillableDT.Columns.Count - 1; J++)
                                {
                                    if (J == 6)
                                    {
                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[6].ColumnName.ToString(), fontHeader))
                                        {
                                            Colspan = 1,
                                            Border = 15,
                                            FixedHeight = 21,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                    }

                                    else
                                    {

                                        if (J == 3)
                                        {
                                            table.AddCell(new PdfPCell(new Phrase("Task Owner", fontHeader))
                                            {
                                                Colspan = 1,
                                                Border = 15,
                                                FixedHeight = 21,
                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                BackgroundColor = new BaseColor(18, 48, 73)
                                            });
                                        }
                                        else
                                        {
                                            if (J != 7)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    FixedHeight = 21,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    BackgroundColor = new BaseColor(18, 48, 73)
                                                });
                                            }
                                            if (J == 7)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    FixedHeight = 21,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    BackgroundColor = new BaseColor(18, 48, 73)
                                                });
                                            }

                                        }
                                    }
                                }
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                {
                                    Colspan = 5,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Hours : {data[1]}", fontWeekHours))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                {
                                    Colspan = 5,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Days : {data[1]}", fontWeekHours))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    //decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0)
                                    {
                                        Font fontH1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            if (J == 6)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][6].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Font fontH1 = null;
                                    double gethours = Convert.ToDouble(BillableDT.Rows[i][4]);
                                    if (gethours == 0)
                                    {
                                        fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.BLACK);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }
                                            if (J == 6)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    //  BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    if (J == 4)
                                                    {
                                                        Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][8].ToString(), CellStyle))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                    else
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }

                                                }
                                                if (J == 7)
                                                {
                                                    Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                    Chunk redText = new Chunk("(THIS WILL NOT BE CHARGED). ", CellStyle);
                                                    Chunk remaingtext = new Chunk(BillableDT.Rows[i][7].ToString(), fontTaskEntry);
                                                    var phrase = new Phrase(redText);
                                                    phrase.Add(remaingtext);
                                                    table.AddCell(new PdfPCell(new Phrase(phrase))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        // BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }
                                            if (J == 6)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][6].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                                if (J == 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                table.AddCell(new PdfPCell(new Phrase(""))
                                {
                                    Colspan = 6,
                                    FixedHeight = 20,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    BackgroundColor = new BaseColor(255, 255, 255)
                                });


                            }
                        }
                        else
                        {
                            if (BillableDT.Rows[i][1].ToString() == "Mainheading")
                            {
                                if (string.IsNullOrEmpty(assignmentWeekly.pdftextboxvalue))
                                {
                                    table.AddCell(new PdfPCell(new Phrase("Tasks' Details", fontMainHeading))
                                    {
                                        Colspan = 6,
                                        Border = 1,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(18, 48, 73)
                                    });
                                }
                                else
                                {
                                    //Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 18.0f, Font.BOLD, BaseColor.WHITE);
                                    //Chunk redText = new Chunk("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle);
                                    //Chunk remaingtext = new Chunk("Tasks' Details", fontMainHeading);
                                    //var phrase = new Phrase(remaingtext);
                                    //phrase.Add(redText);
                                    table.AddCell(new PdfPCell(new Phrase("Tasks' Details (Ref#" + assignmentWeekly.pdftextboxvalue + ")", fontMainHeading))
                                    {
                                        Colspan = 5,
                                        Border = 1,
                                        FixedHeight = 28,
                                        BorderColor = BaseColor.LIGHT_GRAY,
                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                        BackgroundColor = new BaseColor(18, 48, 73)
                                    });
                                    //table.AddCell(new PdfPCell(new Phrase("Ref#: " + assignmentWeekly.pdftextboxvalue, CellStyle))
                                    //{
                                    //    Colspan = 2,
                                    //    Border = 1,
                                    //    FixedHeight = 28,
                                    //    BorderColor = BaseColor.LIGHT_GRAY,
                                    //    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    //    //HorizontalAlignment = Element.ALIGN_RIGHT,
                                    //    BackgroundColor = new BaseColor(18, 48, 73)
                                    //});
                                }
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Client:Period")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Client : {data[0]}", fontSubHeading))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    FixedHeight = 28,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                string DateRange = string.Empty;
                                if (data.Length == 2)
                                {
                                    var split = data[1].Split('-');
                                    DateTime dateStart = Convert.ToDateTime(split[0]);
                                    DateTime dateEnd = Convert.ToDateTime(split[1]);
                                    DateRange = dateStart.ToString("dd/MM/yyyy") + " - " + dateEnd.ToString("dd/MM/yyyy");
                                }
                                table.AddCell(new PdfPCell(new Phrase($"Period : {DateRange}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Total Hours:Total Days")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"Total Hours : {data[0]}", fontSubHeading))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Total Days : {data[1]}", fontSubHeading))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    FixedHeight = 23,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(230, 234, 235)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Header")
                            {
                                for (int J = 2; J < BillableDT.Columns.Count - 1; J++)
                                {
                                    if (J == 6)
                                    {
                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[7].ColumnName.ToString(), fontHeader))
                                        {
                                            Colspan = 1,
                                            Border = 15,
                                            FixedHeight = 21,
                                            BorderColor = BaseColor.LIGHT_GRAY,
                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                            BackgroundColor = new BaseColor(18, 48, 73)
                                        });
                                    }

                                    //else if (J == 7)
                                    //{

                                    //    table.AddCell(new PdfPCell(new Phrase("Status", fontHeader))
                                    //    {
                                    //        Colspan = 1,
                                    //        Border = 15,
                                    //        FixedHeight = 21,
                                    //        BorderColor = BaseColor.LIGHT_GRAY,
                                    //        VerticalAlignment = Element.ALIGN_MIDDLE,
                                    //        HorizontalAlignment = Element.ALIGN_CENTER,
                                    //        BackgroundColor = new BaseColor(18, 48, 73)
                                    //    });

                                    //}
                                    else
                                    {

                                        if (J == 3)
                                        {
                                            table.AddCell(new PdfPCell(new Phrase("Task Owner", fontHeader))
                                            {
                                                Colspan = 1,
                                                Border = 15,
                                                FixedHeight = 21,
                                                BorderColor = BaseColor.LIGHT_GRAY,
                                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                                HorizontalAlignment = Element.ALIGN_CENTER,
                                                BackgroundColor = new BaseColor(18, 48, 73)
                                            });
                                        }
                                        else
                                        {
                                            if (J != 7)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Columns[J].ColumnName.ToString(), fontHeader))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    FixedHeight = 21,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    BackgroundColor = new BaseColor(18, 48, 73)
                                                });
                                            }

                                        }
                                    }
                                }
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Week:Hours")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Hours : {data[1]}", fontWeekHours))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "Range:Days")
                            {
                                var data = BillableDT.Rows[i][7].ToString().Split(':');
                                table.AddCell(new PdfPCell(new Phrase($"{data[0]}", fontWeekHours))
                                {
                                    Colspan = 4,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                                table.AddCell(new PdfPCell(new Phrase($"Days : {data[1]}", fontWeekHours))
                                {
                                    Colspan = 2,
                                    Border = 15,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    BackgroundColor = new BaseColor(35, 96, 145)
                                });
                            }
                            else if (BillableDT.Rows[i][1].ToString() == "TaskEntry")
                            {
                                if (!assignmentWeekly.IsZeroRowShow)
                                {
                                    decimal skip = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    //decimal skip1 = Convert.ToDecimal(BillableDT.Rows[i][4]);
                                    if (skip > 0)
                                    {
                                        Font fontH1 = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.DARK_GRAY);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            if (J == 6)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Font fontH1 = null;
                                    double gethours = Convert.ToDouble(BillableDT.Rows[i][4]);
                                    if (gethours == 0)
                                    {
                                        fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.BLACK);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }
                                            if (J == 6)
                                            {
                                                Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                Chunk redText = new Chunk("(THIS WILL NOT BE CHARGED). ", CellStyle);
                                                Chunk remaingtext = new Chunk(BillableDT.Rows[i][7].ToString(), fontTaskEntry);
                                                var phrase = new Phrase(redText);
                                                phrase.Add(remaingtext);
                                                table.AddCell(new PdfPCell(new Phrase(phrase))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    if (J == 4)
                                                    {
                                                        Font CellStyle = FontFactory.GetFont(fontpath + "Montserrat-Light.ttf", 9.0f, Font.NORMAL, BaseColor.RED);
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][8].ToString(), CellStyle))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }
                                                    else
                                                    {
                                                        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                        {
                                                            Colspan = 1,
                                                            Border = 15,
                                                            BorderColor = BaseColor.LIGHT_GRAY,
                                                            VerticalAlignment = Element.ALIGN_MIDDLE,
                                                            HorizontalAlignment = Element.ALIGN_CENTER,
                                                            //  BackgroundColor = new BaseColor(255, 255, 255),

                                                        });
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                        for (int J = 3; J < BillableDT.Columns.Count - 1; J++)
                                        {
                                            if (J == 3)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][2].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }
                                            if (J == 6)
                                            {
                                                table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                                {
                                                    Colspan = 1,
                                                    Border = 15,
                                                    BorderColor = BaseColor.LIGHT_GRAY,
                                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                                    // BackgroundColor = new BaseColor(255, 255, 255),

                                                });
                                            }

                                            else
                                            {
                                                if (J != 7)
                                                {
                                                    table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                                    {
                                                        Colspan = 1,
                                                        Border = 15,
                                                        BorderColor = BaseColor.LIGHT_GRAY,
                                                        VerticalAlignment = Element.ALIGN_MIDDLE,
                                                        HorizontalAlignment = Element.ALIGN_CENTER,
                                                        //  BackgroundColor = new BaseColor(255, 255, 255),

                                                    });
                                                }
                                            }
                                        }
                                    }

                                    //Font fontH1 = FontFactory.GetFont("Montserrat Light", 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                    //for (int J = 3; J < BillableDT.Columns.Count; J++)
                                    //{
                                    //    if (J == 3)
                                    //    {
                                    //        table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(BillableDT.Rows[i][2]).ToString("dd-MM-yyyy"), fontTaskEntry))
                                    //        {
                                    //            Colspan = 1,
                                    //            Border = 15,
                                    //            BorderColor = BaseColor.LIGHT_GRAY,
                                    //            VerticalAlignment = Element.ALIGN_MIDDLE,
                                    //            HorizontalAlignment = Element.ALIGN_CENTER,
                                    //            // BackgroundColor = new BaseColor(255, 255, 255),

                                    //        });
                                    //    }
                                    //    if (J == 6)
                                    //    {
                                    //        var asd = BillableDT.Rows[i][J].ToString();
                                    //        table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][7].ToString(), fontTaskEntry))
                                    //        {
                                    //            Colspan = 1,
                                    //            Border = 15,
                                    //            BorderColor = BaseColor.LIGHT_GRAY,
                                    //            VerticalAlignment = Element.ALIGN_MIDDLE,
                                    //            HorizontalAlignment = Element.ALIGN_CENTER,
                                    //            // BackgroundColor = new BaseColor(255, 255, 255),

                                    //        });
                                    //    }

                                    //    else
                                    //    {
                                    //        var assd = BillableDT.Rows[i][J].ToString();
                                    //        if (J != 7)
                                    //        {
                                    //            var asd = BillableDT.Rows[i][J].ToString();
                                    //            table.AddCell(new PdfPCell(new Phrase(BillableDT.Rows[i][J].ToString(), fontTaskEntry))
                                    //            {
                                    //                Colspan = 1,
                                    //                Border = 15,
                                    //                BorderColor = BaseColor.LIGHT_GRAY,
                                    //                VerticalAlignment = Element.ALIGN_MIDDLE,
                                    //                HorizontalAlignment = Element.ALIGN_CENTER,
                                    //                //  BackgroundColor = new BaseColor(255, 255, 255),

                                    //            });
                                    //        }
                                    //    }
                                    //}
                                }
                            }
                            else
                            {
                                table.AddCell(new PdfPCell(new Phrase(""))
                                {
                                    Colspan = 6,
                                    FixedHeight = 20,
                                    BorderColor = BaseColor.LIGHT_GRAY,
                                    BackgroundColor = new BaseColor(255, 255, 255)
                                });


                            }
                        }

                    }
                    document.Add(table);

                    document.Close();
                    fs.Close();

                    var ImgDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/");
                    var ImgPath = ImgDirectory + "WaterMark-rezaid.png";
                    if (!System.IO.Directory.Exists(ImgDirectory))
                    {
                        System.IO.Directory.CreateDirectory(ImgDirectory);
                    }
                    PdfStampInExistingFile(ImgPath, filePath);
                    document.Close();
                    fs.Close();
                    return new Tuple<string, bool>(null, true);

                }
            }
            catch (Exception e)
            {

                return new Tuple<string, bool>(null, false);
            }
        }
        public void PdfStampInExistingFile(string watermarkImagePath, string sourceFilePath)
        {
            try
            {
                byte[] bytes = System.IO.File.ReadAllBytes(sourceFilePath);
                var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);

                img.SetAbsolutePosition((PageSize.LEGAL.Width - img.ScaledWidth) / 2, (PageSize.LEGAL.Height - img.ScaledHeight) / 2);

                PdfContentByte waterMark;
                PdfGState state = new PdfGState();
                state.FillOpacity = 0.25f;

                using (MemoryStream stream = new MemoryStream())
                {
                    PdfReader reader = new PdfReader(bytes);
                    using (PdfStamper stamper = new PdfStamper(reader, stream))
                    {
                        int pages = reader.NumberOfPages;
                        for (int i = 1; i <= pages; i++)
                        {
                            waterMark = stamper.GetUnderContent(i);
                            waterMark.SetGState(state);
                            waterMark.AddImage(img);
                        }
                    }
                    bytes = stream.ToArray();
                }
                System.IO.File.WriteAllBytes(sourceFilePath, bytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public List<AssignmentsModelForExcelBO> GetAssignments(AssignmentDTOBO assignment)
        //{
        //    try
        //    {
        //        var assignments = new List<AssignmentsModelForExcelBO>();
        //        if (assignment.ToDate == null && assignment.FromDate == null)
        //        {
        //            assignment.ToDate = DateTime.Now;
        //            DateTime mondayOfLastWeek = DateTime.Now;
        //            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        //            {
        //                mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 13);
        //            }
        //            else
        //            {
        //                mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);
        //            }
        //            assignment.FromDate = mondayOfLastWeek.Date;
        //        }

        //        var assignmentsWithMonthlyHours = new List<AssignmentsModelForExcelBO>();
        //        LoginBL objLoginBL = new LoginBL();
        //        var loggedUser = objLoginBL.GetLoggedUserInfo(assignment.LoggedInUserId);
        //        if(loggedUser != null)
        //        {
        //            if(assignment != null)
        //            {
        //                if(!loggedUser.Role.Equals("Admin") && !loggedUser.Role.Equals("SuperAdmin"))
        //                {
        //                    assignments = this.GetAllAssignmentResult(assignment).Where(x => x.TaskOwner == loggedUser.FullName && x.IsApproved == false).ToList();
        //                    assignmentsWithMonthlyHours = this.GetTotalHoursOfMonth(assignments).ToList();
        //                }
        //                else
        //                {
        //                    if (assignment.taskTypePrefixes != null && assignment.taskTypePrefixes.Count() > 0)
        //                    {
        //                        assignments = this.GetAllAssignmentResult(assignment).Where(e =>
        //                        assignment.taskTypePrefixes.Contains(e.TaskTypePrefix)).ToList();
        //                    }
        //                    else
        //                    {
        //                        assignments = this.GetAllAssignmentResult(assignment).ToList();
        //                    }
        //                    assignmentsWithMonthlyHours = this.GetTotalHoursOfMonth(assignments);
        //                }
        //                return assignmentsWithMonthlyHours;
        //            }
        //            return null;
        //        }
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}

        public List<ProjectTotalHoursBO> GetTotalHoursForTasks(AssignmentDTOBO assignment)
        {
            try
            {
                if (assignment.ToDate == null && assignment.FromDate == null)
                {

                    assignment.ToDate = DateTime.Now;
                    DateTime mondayOfLastWeek = DateTime.Now;
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 13);
                    }
                    else
                    {
                        mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);
                    }
                    //DateTime mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);
                    assignment.FromDate = mondayOfLastWeek.Date;
                }

                return GetProjectTotalHours(assignment);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //public bool SaveAssignemntToDb(AssignmentsBO assignment)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(assignment.Phase))
        //        {
        //            assignment.Phase = "0";
        //        }
        //        if (string.IsNullOrEmpty(assignment.TaskType))
        //        {
        //            assignment.TaskType = "None";
        //        }

        //        ReportDAL objDal = new ReportDAL();
        //        return objDal.SaveAssignemntToDb(assignment);
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}
        public bool SaveAssignemntToDb(AddRecordBO assignment)
        {
            try
            {
                ReportDAL objDal = new ReportDAL();
                return objDal.SaveAssignemntToDb(assignment);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool DeleteAssignment(int ID)
        {
            try
            {
                if (ID > 0)
                {
                    ReportDAL objDal = new ReportDAL();
                    return objDal.DeleteAssignment(ID);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ApproveAssignment(int projectId)
        {
            try
            {
                if (projectId > 0)
                {
                    ReportDAL objDal = new ReportDAL();
                    return objDal.ApproveAssignment(projectId);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // missed due to error
        public List<ActualVsEstimatedComparisonReportModelBO> GetActualVsEstimatedComparedTasks(AssignmentDTOBO assignment)
        {
            try
            {
                if (assignment != null)
                {
                    return null;
                }
                else
                {
                    return null;
                }

                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    connection.Open();
                //    var comparedTasks = new List<ActualVsEstimatedComparisonReportViewModel>();
                //    SqlCommand cmd = new SqlCommand("sp_ActualVsEstimatedHoursComparisonReport", connection);
                //    cmd.Parameters.Add("@ProjectId", SqlDbType.Int).Value = assignment.ProjectId;
                //    cmd.Parameters.Add("@TaskName", SqlDbType.NVarChar).Value = assignment.TaskName;
                //    cmd.Parameters.Add("@TaskOwnerId", SqlDbType.Int).Value = assignment.TaskOwnerId;
                //    cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = assignment.FromDate;
                //    cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = assignment.ToDate;
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    using (SqlDataReader reader = cmd.ExecuteReader())
                //    {
                //        while (reader.Read())
                //        {
                //            if (Convert.ToInt32(reader["ProjectId"]) > 0)
                //            {
                //                var actualHours = float.Parse(reader["ActualHours"].ToString()).ToString("0.00");
                //                var estimatedDuration = float.Parse(reader["EstimatedDuration"].ToString()).ToString("0.00");
                //                var remainingHours = float.Parse(reader["RemainingHours"].ToString()).ToString("0.00");

                //                comparedTasks.Add(new ActualVsEstimatedComparisonReportViewModel
                //                {
                //                    ProjectId = Convert.ToInt32(reader["ProjectId"]),
                //                    ProjectName = reader["ProjectName"].ToString(),
                //                    TaskName = reader["TaskName"].ToString(),
                //                    ActualHours = float.Parse(actualHours),
                //                    EstimatedDuration = float.Parse(estimatedDuration),
                //                    RemainingHours = float.Parse(remainingHours),
                //                    OverBudget = reader["OverBudget"].ToString()
                //                });
                //            }
                //        }
                //        connection.Close();
                //        return comparedTasks.ToList();
                //    }
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<AssignmentsBO> GetAssignments(GetAssignmentBO obj)
        {
            try
            {
                if (obj.MainTaskID == 0)
                {
                    obj.MainTaskID = -1;
                }
                if (obj.SubTaskID == 0)
                {
                    obj.SubTaskID = -1;
                }
                if (obj.ProjectId == 0)
                {
                    obj.SubTaskID = -1;
                }
                if (obj.ToDate == null)
                {
                    obj.ToDate = DateTime.Now.Date;
                }
                List<AssignmentsBO> GetAssignment = new List<AssignmentsBO>();
                ReportDAL objDal = new ReportDAL();
                DateTime Fromdate = Convert.ToDateTime(obj.FromDate);
                DateTime ToDate = Convert.ToDateTime(obj.ToDate);
                obj.FromDate = Fromdate.Date;
                obj.ToDate = ToDate.Date;
                var result = objDal.GetAssignmentsDAL(obj);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        GetAssignment.Add(new AssignmentsBO
                        {
                            AssignmentID = result[i].ID,
                            UserID = result[i].UserProfileTableID,
                            UserFullName = result[i].FullName,
                            ProjectID = result[i].ProjectID,
                            ProjectName = result[i].Name,
                            ProjectDescription = result[i].ProjectDescription,
                            MainTaskID = result[i].MainTaskID,
                            MainTaskName = result[i].MainTaskName,
                            TaskID = result[i].TaskID,
                            TaskName = result[i].TaskName,
                            CommentText = result[i].CommentTxt,
                            AssignmentDateTime = Convert.ToDateTime(result[i].AssignmentDateTime).ToString("dd/MM/yyyy"),
                            DaysDuration = result[i].Days.ToString(),
                            ActualDuration = result[i].ActualDuration.ToString(),
                            BillableHours = result[i].BillableHours.ToString(),
                            UserIDBillable = result[i].UserProfileTableIDBillable,
                            UserNameBillable = result[i].UserNameBillable,
                            IsBillableApproved = result[i].IsBillableApproved,
                            BillableDateTime = result[i].BillableApprovedDateTime,
                            UserIDActual = result[i].UserProfileTableIDActual,
                            UserNameActual = result[i].UserNameActual,
                            IsActualApproved = result[i].IsActualApproved,
                            ActualDateTime = result[i].ActualApprovedDateTime,
                            ClientID = Convert.ToInt32(result[i].ClientID)
                        });

                    }
                    return GetAssignment;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<ProjectTotalHoursViewModel> GetReportshours(AssignmentDTO assignment)
        {
            try
            {
                if (assignment.ToDate == null && assignment.FromDate == null)
                {

                    assignment.ToDate = DateTime.Now.Date;
                    DateTime mondayOfLastWeek = DateTime.Now;
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 13);
                    }
                    else
                    {
                        mondayOfLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 5);
                    }
                    assignment.FromDate = mondayOfLastWeek.Date;
                }
                List<ProjectTotalHoursViewModel> models = new List<ProjectTotalHoursViewModel>();
                ReportDAL objDal = new ReportDAL();
                DateTime FromDate = Convert.ToDateTime(assignment.FromDate);
                DateTime ToDate = Convert.ToDateTime(assignment.ToDate);
                assignment.FromDate = FromDate.Date;
                assignment.ToDate = ToDate.Date;
                var result = objDal.GetReportHoursDAL(assignment);
                if (result.Count() > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        models.Add(new ProjectTotalHoursViewModel
                        {
                            ProjectId = Convert.ToInt32(result[i].ProjectID),
                            ProjectName = result[i].ProjectName,
                            BillableHours = result[i].BillableHours.ToString(),
                            ClientID = Convert.ToInt32(result[i].ClientID),
                            TotalActualDuration = result[i].TotalActualDuration.ToString(),
                            RemainingHours = result[i].RemainingHours.ToString(),
                            ActualD = Convert.ToDecimal(result[i].ActualD)

                        });
                    }
                    return models;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool CheckMaintaskSubTaskToDb(AddRecordBO assignment)
        {
            try
            {
                ReportDAL objDal = new ReportDAL();
                return objDal.CheckMainTaskSubtask(assignment);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<AssignmentsBO> GetAssignmentsList(GetAssignmentBO obj)
        {
            List<AssignmentsBO> model = new List<AssignmentsBO>();

            if (obj.MainTaskID == 0)
            {
                obj.MainTaskID = -1;
            }
            if (obj.SubTaskID == 0)
            {
                obj.SubTaskID = -1;
            }
            if (obj.ProjectId == 0)
            {
                obj.SubTaskID = -1;
            }
            if (obj.ToDate == null)
            {
                obj.ToDate = DateTime.Now.Date;
            }
            List<AssignmentsBO> GetAssignment = new List<AssignmentsBO>();
            ReportDAL objDal = new ReportDAL();
            DateTime Fromdate = Convert.ToDateTime(obj.FromDate);
            DateTime ToDate = Convert.ToDateTime(obj.ToDate);
            obj.FromDate = Fromdate.Date;
            obj.ToDate = ToDate.Date;

            var result = objDal.GetAssignmentsDAL(obj);
            if (result != null)
            {
                //result = result.OrderByDescending(g => g.AssignmentDateTime).ToList();
                if (obj.ClientID > 0)
                {
                    result = result.Where(x => x.ClientID == obj.ClientID).ToList();
                }
                var getprojectlist = result.GroupBy(g => g.ProjectID);
                foreach (var projectID in getprojectlist)
                {
                    GetAssignment.Add(new AssignmentsBO
                    {
                        ProjectID = projectID.Key
                    });
                }
            }
            if (obj.ToDate == null)
            {
                obj.ToDate = DateTime.Now;
            }
            foreach (var item in GetAssignment)
            {
                string check = "";
                if (obj.LoggedInUserId == null)
                {
                    check = "-1";
                }
                else
                {
                    check = string.Join(", ", obj.LoggedInUserId);
                }
                ExportAssignmentBO export = new ExportAssignmentBO();
                export.Billable = false;
                export.FromDate = obj.FromDate;
                export.ToDate = obj.ToDate;
                export.ProjectId = (int)item.ProjectID;
                export.commasepartedTaskOwnerids = check;
                export.TaskId = obj.MainTaskID;
                export.SubTaskId = obj.SubTaskID;
                export.ClinetID = obj.ClientID;
                export.DepartmentID = obj.DepartmentID;
                var GetData = objDal.GetTimeSheetDataTableList(export);
                if (GetData.Rows.Count > 0)
                {
                    //int BillableCount = 0;
                    //int RangeDaysCount = 0;
                    //int WeeksHoursCount = 0;
                    //string[] BillableHoursDays = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "BillableTotal Hours:BillableTotal Days").Select(s => s.Field<string>("Description")).ToArray();
                    //for (int i = 0; i < GetData.Rows.Count; i++)
                    //{
                    //    var billablehoursDays = BillableHoursDays.Length> BillableCount? BillableHoursDays[BillableCount].ToString().Split(':'):null;
                    //    if (GetData.Rows[i][1].ToString() == "Range:Days")
                    //    {
                    //        var data = GetData.Rows[i][7].ToString().Split(':');
                    //        AssignmentsBO Range_Days = new AssignmentsBO();
                    //        Range_Days.Range = data[0];
                    //        Range_Days.Days = data[1];
                    //        Range_Days.Billable_Days = billablehoursDays[1];
                    //        Range_Days.Rowentry = false;
                    //        Range_Days.ClientID = obj.ClientID;
                    //        model.Add(Range_Days);
                    //        RangeDaysCount++;
                    //    }
                    //    else if (GetData.Rows[i][1].ToString() == "Week:Hours")
                    //    {
                    //        var hours = GetData.Rows[i][7].ToString().Split(':');
                    //        AssignmentsBO weeks_Hours = new AssignmentsBO();
                    //        weeks_Hours.week = hours[0];
                    //        weeks_Hours.hours = hours[1];
                    //        weeks_Hours.BillableHours = billablehoursDays[0];
                    //        weeks_Hours.Rowentry = false;
                    //        weeks_Hours.ClientID = obj.ClientID;
                    //        model.Add(weeks_Hours);
                    //        WeeksHoursCount++;
                    //    }
                    //    else if (GetData.Rows[i][1].ToString() == "TaskEntry")
                    //    {
                    //        AssignmentsBO TaskEntry = new AssignmentsBO();
                    //        TaskEntry.Rowentry = true;
                    //        if (!string.IsNullOrEmpty(GetData.Rows[i][2].ToString()))
                    //        {
                    //            DateTime date = Convert.ToDateTime(GetData.Rows[i][2]);
                    //            TaskEntry.AssignmentDateTime = date.ToString("dd/MM/yyyy");
                    //        }
                    //        TaskEntry.UserFullName = GetData.Rows[i][3].ToString();
                    //        TaskEntry.ActualDuration =  GetData.Rows[i][4].ToString();
                    //        TaskEntry.TaskName = GetData.Rows[i][5].ToString();
                    //        TaskEntry.CommentText = GetData.Rows[i][7].ToString();
                    //        TaskEntry.AssignmentID = Convert.ToInt32(GetData.Rows[i][8]);
                    //        TaskEntry.ProjectID = Convert.ToInt32(GetData.Rows[i][9]);
                    //        TaskEntry.ProjectName = GetData.Rows[i][10].ToString();
                    //        TaskEntry.DaysDuration = GetData.Rows[i][11].ToString();
                    //        TaskEntry.CheckIsApproved = Convert.ToBoolean(GetData.Rows[i][12]); 
                    //        TaskEntry.MainTaskID = Convert.ToInt32(GetData.Rows[i][13]);
                    //        TaskEntry.MainTaskName = GetData.Rows[i][14].ToString();
                    //        TaskEntry.TaskID = Convert.ToInt32(GetData.Rows[i][15]);
                    //        TaskEntry.UserID = Convert.ToInt32(GetData.Rows[i][16]);
                    //        TaskEntry.BillableHours = GetData.Rows[i][17].ToString();
                    //        TaskEntry.IsBillableApproved = Convert.ToInt32(GetData.Rows[i][18]);
                    //        if (!string.IsNullOrEmpty(GetData.Rows[i][19].ToString()))
                    //        {
                    //            TaskEntry.BillableDateTime = Convert.ToDateTime(GetData.Rows[i][19]);
                    //        }
                    //        TaskEntry.IsActualApproved = Convert.ToInt32(GetData.Rows[i][20]);
                    //        if (!string.IsNullOrEmpty(GetData.Rows[i][21].ToString()))
                    //        {
                    //            TaskEntry.ActualDateTime = Convert.ToDateTime(GetData.Rows[i][21]);
                    //        }
                    //        TaskEntry.ClientID = Convert.ToInt32(GetData.Rows[i][22]);
                    //        model.Add(TaskEntry);
                    //        if (RangeDaysCount == WeeksHoursCount)
                    //        {
                    //            if(BillableCount == RangeDaysCount - 1 && BillableCount == WeeksHoursCount - 1)
                    //            {
                    //                BillableCount++;
                    //            }
                    //        }
                    //    }
                    //}
                    var list = OrderbyDESCReportGrid(GetData, export.ClinetID, Convert.ToDateTime(export.FromDate));
                    model.AddRange(list);
                }
            }
            return model;
        }

        public List<AssignmentsBO> OrderbyDESCReportGrid(DataTable GetData, int ClientID, DateTime FromDate)

        {
            List<AssignmentsBO> model = new List<AssignmentsBO>();
            var DateRange = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Range:Days").Select(s => s.Field<string>("Description")).ToArray();
            string[] Weeks = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "Week:Hours").Select(s => s.Field<string>("Description")).ToArray();
            string[] BillableHoursDays = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "BillableTotal Hours:BillableTotal Days").Select(s => s.Field<string>("Description")).ToArray();
            var EntryList = GetData.AsEnumerable().Where(s => s.Field<string>("RowDescription") == "TaskEntry").Select(s => new AssignmentsBO
            {
                Rowentry = true,
                AssignmentDT = Convert.ToDateTime(s.Field<string>("Date").ToString()),
                UserFullName = s.Field<string>("TaskOwner").ToString(),
                ActualDuration = s.Field<string>("Hours").ToString(),
                CommentText = s.Field<string>("Description").ToString(),
                AssignmentID = s.Field<int>("AssignmentID"),
                ProjectID = s.Field<int>("ProjectID"),
                ProjectName = s.Field<string>("Project_Name").ToString(),
                DaysDuration = s.Field<decimal>("Actual_Days").ToString(),
                CheckIsApproved = s.Field<bool>("IsApproved"),
                MainTaskID = s.Field<int>("MainTaskID"),
                MainTaskName = s.Field<string>("MainTaskName").ToString(),
                TaskID = s.Field<int>("TaskID"),
                TaskName = s.Field<string>("Sub Task").ToString(),
                UserID = s.Field<int>("UserProfileTableID"),
                BillableHours = s.Field<string>("BillableHours").ToString(),
                IsBillableApproved = s.Field<int>("IsBillableApproved"),
                BillableDateTime = s.Field<DateTime>("BillableApprovedDateTime"),
                IsActualApproved = s.Field<int>("IsActualApproved"),
                ActualDateTime = s.Field<DateTime>("ActualApprovedDateTime"),
                ClientID = s.Field<int>("ClientID"),
            }).OrderBy(f => f.AssignmentDT).ThenBy(d => d.UserFullName).ToList();
            string EndDate = string.Empty;
            int Countdate = 0;
            for (var row = 0; row < DateRange.Length; row++)
            {
                var DateSplit = DateRange[row].ToString().Split(':');
                if (DateSplit.Length > 0)
                {

                    var getfromtodate = DateSplit[0].Split('-');
                    if (getfromtodate.Length > 0)
                    {
                        var billablehours = BillableHoursDays[row].ToString().Split(':');
                        var WeeksSplit = Weeks[row].ToString().Split(':');
                        AssignmentsBO weeks_Hours = new AssignmentsBO();
                        weeks_Hours.week = WeeksSplit[0];
                        weeks_Hours.hours = WeeksSplit[1];
                        weeks_Hours.Billable_Hours = billablehours[0];
                        weeks_Hours.Rowentry = false;
                        weeks_Hours.ClientID = ClientID;
                        model.Add(weeks_Hours);

                        AssignmentsBO Range_Days = new AssignmentsBO();
                        Range_Days.Range = DateSplit[0];
                        Range_Days.Days = DateSplit[1];
                        Range_Days.Billable_Days = billablehours[1];
                        Range_Days.Rowentry = false;
                        Range_Days.ClientID = ClientID;
                        model.Add(Range_Days);
                        DateTime startdate = Convert.ToDateTime(getfromtodate[0]);
                        DateTime enddate = Convert.ToDateTime(getfromtodate[1]);
                        //if (Countdate == 0)
                        //{
                        //    var DateDifference = startdate - FromDate;
                        //    if (DateDifference.Days > 0)
                        //    {
                        //        startdate = startdate.AddDays(-DateDifference.Days);
                        //    }
                        //}
                        if (enddate.DayOfWeek == DayOfWeek.Friday)
                        {
                            enddate = enddate.AddDays(2);
                        }
                        var firstdateformat = startdate.ToString("yyyy-MM-dd");
                        var seconddateformat = enddate.ToString("yyyy-MM-dd");
                        var getentrylist = EntryList.Where(x => x.AssignmentDT >= startdate.Date && x.AssignmentDT <= enddate.Date).Select(s => new AssignmentsBO
                        {
                            Rowentry = s.Rowentry,
                            AssignmentDateTime = s.AssignmentDT.ToString("dd/MM/yyyy"),
                            UserFullName = s.UserFullName.ToString(),
                            ActualDuration = s.ActualDuration.ToString(),
                            CommentText = s.CommentText.ToString(),
                            AssignmentID = s.AssignmentID,
                            ProjectID = s.ProjectID,
                            ProjectName = s.ProjectName,
                            DaysDuration = s.DaysDuration,
                            CheckIsApproved = s.CheckIsApproved,
                            MainTaskID = s.MainTaskID,
                            MainTaskName = s.MainTaskName,
                            TaskID = s.TaskID,
                            TaskName = s.TaskName,
                            UserID = s.UserID,
                            BillableHours = s.BillableHours,
                            IsBillableApproved = s.IsBillableApproved,
                            BillableDateTime = s.BillableDateTime,
                            IsActualApproved = s.IsActualApproved,
                            ActualDateTime = s.ActualDateTime,
                            ClientID = s.ClientID,
                        }).ToList();
                        model.AddRange(getentrylist);
                        Countdate++;
                    }
                }
            }
            //for (int i = 0; i < GetData.Rows.Count; i++)
            //{
            //    if (GetData.Rows[i][1].ToString() == "Week:Hours")
            //    {
            //        var data = GetData.Rows[i][7].ToString().Split(':');
            //        AssignmentsBO assignments = new AssignmentsBO();
            //        //assignments.ProjectName = "Week:Hours";
            //        assignments.week = data[0];
            //        assignments.hours = data[1];
            //        assignments.Rowentry = false;
            //        model.Add(assignments);
            //    }
            //    else if (GetData.Rows[i][1].ToString() == "Range:Days")
            //    {
            //        DataRow[] results = GetData.Select("RowDescription = 'Week:Hours'");
            //        var data = GetData.Rows[i][7].ToString().Split(':');
            //        AssignmentsBO assignments = new AssignmentsBO();
            //        // assignments.ProjectName = "Range:Days";
            //        assignments.Range = data[0];
            //        assignments.Days = data[1];
            //        var getdate = assignments.Range.Split('-');
            //        DateTime lastdate = Convert.ToDateTime(getdate[1]);
            //        assignments.AssignmentDateTime = lastdate.ToString("dd/MM/yyyy");
            //        assignments.Rowentry = false;
            //        model.Add(assignments);
            //    }
            //    else if (GetData.Rows[i][1].ToString() == "TaskEntry")
            //    {
            //        AssignmentsBO assignments = new AssignmentsBO();
            //        assignments.Rowentry = true;
            //        if (!string.IsNullOrEmpty(GetData.Rows[i][2].ToString()))
            //        {
            //            DateTime date = Convert.ToDateTime(GetData.Rows[i][2]);
            //            assignments.AssignmentDateTime = date.ToString("dd/MM/yyyy");
            //        }
            //        assignments.UserFullName = GetData.Rows[i][3].ToString();
            //        assignments.ActualDuration = GetData.Rows[i][4].ToString();
            //        assignments.CommentText = GetData.Rows[i][7].ToString();
            //        assignments.AssignmentID = Convert.ToInt32(GetData.Rows[i][8]);
            //        assignments.ProjectID = Convert.ToInt32(GetData.Rows[i][9]);
            //        assignments.ProjectName = GetData.Rows[i][10].ToString();
            //        assignments.DaysDuration = GetData.Rows[i][11].ToString();
            //        assignments.IsApproved = Convert.ToInt32(GetData.Rows[i][12]);
            //        assignments.MainTaskID = Convert.ToInt32(GetData.Rows[i][13]);
            //        assignments.MainTaskName = GetData.Rows[i][14].ToString();
            //        assignments.TaskID = Convert.ToInt32(GetData.Rows[i][15]);
            //        assignments.TaskName = GetData.Rows[i][6].ToString();
            //        assignments.UserID = Convert.ToInt32(GetData.Rows[i][16]);
            //        assignments.BillableHours = GetData.Rows[i][17].ToString();
            //        assignments.IsBillableApproved = Convert.ToInt32(GetData.Rows[i][18]);
            //        if (!string.IsNullOrEmpty(GetData.Rows[i][19].ToString()))
            //        {
            //            assignments.BillableDateTime = Convert.ToDateTime(GetData.Rows[i][19]);
            //        }
            //        assignments.IsActualApproved = Convert.ToInt32(GetData.Rows[i][20]);
            //        if (!string.IsNullOrEmpty(GetData.Rows[i][21].ToString()))
            //        {
            //            assignments.ActualDateTime = Convert.ToDateTime(GetData.Rows[i][21]);
            //        }
            //        if (!string.IsNullOrEmpty(GetData.Rows[i][21].ToString()))
            //        {
            //            assignments.ClientID = Convert.ToInt32(GetData.Rows[i][22]);
            //        }
            //        model.Add(assignments);
            //    }
            //}
            return model;
        }

        #endregion


    }
    class Footer : PdfPageEventHelper
    {
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            try
            {

                PdfPTable tbfooter = new PdfPTable(3);
                //At the end of the page, we insert a extended cell
                PdfPCell cellExpand = new PdfPCell();
                tbfooter.WidthPercentage = 100;
                cellExpand.Colspan = 3;
                cellExpand.FixedHeight = 15;
                cellExpand.Border = 0;
                cellExpand.PaddingBottom = 5;
                tbfooter.SpacingAfter = 10;
                tbfooter.SpacingBefore = 10;


                tbfooter.AddCell(cellExpand);

                tbfooter.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                tbfooter.DefaultCell.Border = 0;


                string fontpath = HttpContext.Current.Server.MapPath("~/fonts/");
                Font footer = FontFactory.GetFont(fontpath + "Montserrat-Regular.ttf", 13.0f, Font.BOLD, BaseColor.BLACK);

                var LeftFoot = new PdfPCell(new Paragraph("CONFIDENTIAL", footer));
                LeftFoot.FixedHeight = 25;
                LeftFoot.HorizontalAlignment = Element.ALIGN_LEFT;
                LeftFoot.VerticalAlignment = Element.ALIGN_CENTER;
                LeftFoot.Border = 0;
                tbfooter.AddCell(LeftFoot);

                tbfooter.AddCell(new Paragraph());

                var ImgDirectory = HttpContext.Current.Server.MapPath("~/Images/");
                var ImgPath = ImgDirectory + "Logo-rezaid.png";
                if (!System.IO.Directory.Exists(ImgDirectory))
                {
                    System.IO.Directory.CreateDirectory(ImgDirectory);
                }
                var image = iTextSharp.text.Image.GetInstance(ImgPath);

                var RightFoot = new PdfPCell(image);
                RightFoot.FixedHeight = 15;
                RightFoot.HorizontalAlignment = Element.ALIGN_RIGHT;
                RightFoot.VerticalAlignment = Element.ALIGN_CENTER;

                RightFoot.Border = 0;

                tbfooter.AddCell(RightFoot);

                float[] widths1 = new float[] { 20f, 20f, 60f };
                tbfooter.SetWidths(widths1);
                tbfooter.WriteSelectedRows(0, -1, document.LeftMargin, writer.PageSize.GetBottom(document.BottomMargin) + 15, writer.DirectContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
