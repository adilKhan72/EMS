using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoEFCommon.FileUploadResponse;
using DemoEFBO.ProjectSchedule;
using System.IO;
using DemoEFDAL.Users;
using DemoEFDAL.CommonProjectsList;
using DemoEFDAL.ProjectSchedule;
using SpreadsheetLight;

namespace DemoEFBL.ProjectSchedule
{
    public class ProjectScheduleBL
    {
        #region SaveUploadedFile Function Code and Sub function Block

        // file name only shoud be passed from api project
        public FileUploadResponse SaveUploadedFile(string FileName, string FilePath)
        {
            FileUploadResponse objResponse = new FileUploadResponse();
            try
            {
                if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(FilePath))
                {
                    ReadExcelFile(FilePath + FileName ,ref objResponse);
                    return objResponse;
                }
                else
                {
                    objResponse.FileName = "";
                    objResponse.Error = "Cannot process empty file name";
                    objResponse.isSuccess = false;
                    objResponse.ResponseMsg = "File name is empty";
                    return objResponse;
                }
            }
            catch (Exception ex)
            {
                objResponse.FileName = FileName;
                objResponse.Error = ex.ToString();
                objResponse.isSuccess = false;
                objResponse.ResponseMsg = "Error occured during processing...";
                return objResponse;
            }
        }

        private bool ReadExcelFile(string filepath, ref FileUploadResponse objResponse)
        {
            List<ProjectsScheduleListBO> lstProject = new List<ProjectsScheduleListBO>();
            List<ColumnIndexListBO> lstColIndex = new List<ColumnIndexListBO>();
            List<ExcelMappingBO> objFinalLst = new List<ExcelMappingBO>();
            try
            {
                using (SLDocument sl = new SLDocument())
                {
                    FileStream fs = new FileStream(filepath, FileMode.Open);
                    SLDocument sheet = new SLDocument(fs, "Sheet1");
                    SLWorksheetStatistics stats = sheet.GetWorksheetStatistics();
                    int RowCount = stats.EndRowIndex;
                    int Columncount = stats.EndColumnIndex;
                    if (RowCount > 0 && Columncount > 0)
                    {
                        //Loop for adding data to Column Header List 
                        for (int c = 1; c <= Columncount; c++) // Loop on column Count
                        {
                            if (!string.IsNullOrEmpty(sheet.GetCellValueAsString(1, c)))
                            {
                                if (lstColIndex.Count > 0)
                                {
                                    lstColIndex[lstColIndex.Count - 1].EndIndex = c - 1;
                                }
                                string _colName = sheet.GetCellValueAsString(1, c);
                                lstColIndex.Add(new ColumnIndexListBO
                                {
                                    ColumnName = _colName,
                                    StartIndex = c,
                                    EndIndex = c //-1
                                });
                            }
                        }

                        // for getting Projects Name from file with start and End Index
                        for (int r = 4; r <= RowCount; r++)
                        {
                            if (!string.IsNullOrEmpty(sheet.GetCellValueAsString(r, 1)))
                            {
                                if (lstProject.Count > 0)
                                {
                                    lstProject[lstProject.Count - 1].EndRowIndex = r - 1;
                                }
                                string _ProjectName = sheet.GetCellValueAsString(r, 1);
                                lstProject.Add(new ProjectsScheduleListBO
                                {
                                    ProjectName = _ProjectName,
                                    StartRowIndex = r,
                                    EndRowIndex = RowCount
                                });
                            }
                        }

                        if (lstColIndex.Count > 0)
                        {
                            int _ProjectEstimateColumnIndex = lstColIndex[lstColIndex.Count - 1].StartIndex;
                            int TotalCount = lstColIndex.Count;
                            for (int i = 0; i < TotalCount; i++)
                            {
                                // For skipping first two column
                                if (i > 1)
                                {
                                    int _startIndex = lstColIndex[i].StartIndex;
                                    int _endIndex = lstColIndex[i].EndIndex;
                                    string _projectType = lstColIndex[i].ColumnName;
                                    // For specific Project type
                                    for (int j = _startIndex; j <= _endIndex; j++) // column updating j value for getting column values
                                    {
                                        for (int k = 0; k < lstProject.Count; k++) // row list extraction
                                        {
                                            int _startIndexProject = lstProject[k].StartRowIndex;
                                            int _endIndexProject = lstProject[k].EndRowIndex;
                                            ExcelMappingBO objFinalClass = new ExcelMappingBO();
                                            objFinalClass.ProjectType = _projectType;
                                            objFinalClass.ProjectName = lstProject[k].ProjectName;
                                            if (string.IsNullOrEmpty(sheet.GetCellValueAsString(2, j)))
                                            {
                                                break;
                                            }
                                            objFinalClass.ResourceName = sheet.GetCellValueAsString(2, j);
                                            for (int l = _startIndexProject; l <= _endIndexProject; l++) // row
                                            {
                                                if (_startIndexProject == l)
                                                {
                                                    if (!string.IsNullOrEmpty(sheet.GetCellValueAsString(l, j)))
                                                    {
                                                        string _columnHeaderName = sheet.GetCellValueAsString(3, j);
                                                        if (_columnHeaderName.Trim().ToLower() == "start date")
                                                        {
                                                            objFinalClass.StartDate = sheet.GetCellValueAsString(l, j);
                                                            objFinalClass.EndDate = sheet.GetCellValueAsString(l, j + 1);
                                                            objFinalClass.Estimate = sheet.GetCellValueAsString(l + 1, j);
                                                            objFinalClass.ProjectEstimate = sheet.GetCellValueAsString(l, _ProjectEstimateColumnIndex);
                                                            l++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // case when resource doesn't have any entry found for this project and project type
                                                        objFinalClass.ProjectEstimate = sheet.GetCellValueAsString(l, _ProjectEstimateColumnIndex);
                                                    }

                                                }
                                                else
                                                {
                                                    // update part 
                                                    ProjectScheduleUpdateBO objProjectUpdate = new ProjectScheduleUpdateBO();
                                                    if (!string.IsNullOrEmpty(sheet.GetCellValueAsString(l, j)))
                                                    {
                                                        // Update list entry Case
                                                        objProjectUpdate.Projectcomment = sheet.GetCellValueAsString(l, 2);
                                                        objProjectUpdate.ResourceComments = sheet.GetCellValueAsString(l, j);
                                                        objProjectUpdate.Phase = _projectType;
                                                        if (objFinalClass.lstProUpdate == null)
                                                        {
                                                            objFinalClass.lstProUpdate = new List<ProjectScheduleUpdateBO>();
                                                        }
                                                        objFinalClass.lstProUpdate.Add(objProjectUpdate);
                                                    }
                                                    else
                                                    {
                                                        // case when resource doesn't have any entry found for this project and project type
                                                        if (objFinalClass.lstProUpdate == null)
                                                        {
                                                            objFinalClass.lstProUpdate = new List<ProjectScheduleUpdateBO>();
                                                        }
                                                        objProjectUpdate.Projectcomment = "";
                                                        objProjectUpdate.ResourceComments = "";
                                                        objProjectUpdate.Phase = _projectType; //xlRange.Cells[1, j].Value2; // for getting phase value
                                                        objFinalClass.lstProUpdate.Add(objProjectUpdate);
                                                    }
                                                }
                                            }
                                            objFinalLst.Add(objFinalClass);
                                        }
                                    }
                                }
                            }
                        }

                        // Fetch Data from List and Save
                        if (objFinalLst.Count > 0)
                        {
                            Dictionary<string, int> dict = GetUserList();
                            Dictionary<string, int> projectDict = GetProjectList();
                            for (int i = 0; i < objFinalLst.Count; i++)
                            {
                                int userID = -1;
                                int projectID = -1;
                                try
                                {
                                    if (objFinalLst[i].ResourceName != null)
                                    {
                                        userID = dict[objFinalLst[i].ResourceName];
                                    }
                                    if (objFinalLst[i].ProjectName != null)
                                    {
                                        projectID = projectDict[objFinalLst[i].ProjectName];
                                    }
                                }
                                catch (Exception ex) { var excc = ex.ToString(); }


                                //Region for Adding ProjectInfo 
                                AddReportProject(projectID, objFinalLst[i].ProjectName, objFinalLst[i].ProjectEstimate); // need to add proper param
                                                                                                                         //Region for adding Resource info
                                Nullable<DateTime> dtStart = null;
                                Nullable<DateTime> dtEnd = null;
                                try
                                {
                                    if (!string.IsNullOrEmpty(objFinalLst[i].StartDate))
                                    {
                                        dtStart = converttodate(objFinalLst[i].StartDate);
                                    }
                                    if (!string.IsNullOrEmpty(objFinalLst[i].EndDate))
                                    {
                                        dtEnd = converttodate(objFinalLst[i].EndDate);
                                    }
                                }
                                catch { dtStart = null; dtEnd = null; }

                                AddResource(userID, objFinalLst[i].ProjectType, dtStart, dtEnd, objFinalLst[i].Estimate, projectID);

                                //Region for adding project update
                                if (objFinalLst[i].lstProUpdate != null && objFinalLst[i].lstProUpdate.Count > 0)
                                {
                                    for (int x = 0; x < objFinalLst[i].lstProUpdate.Count; x++)
                                    {
                                        AddProjectUpdate(objFinalLst[i].lstProUpdate[x].Projectcomment, userID, projectID, objFinalLst[i].lstProUpdate[x].ResourceComments, objFinalLst[i].lstProUpdate[x].Phase);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objResponse.FileName = "";
                objResponse.Error = ex.ToString();
                objResponse.isSuccess = false;
                objResponse.ResponseMsg = "Error occured during processing...";
                return false;
            }
            objResponse.FileName = "";
            objResponse.Error = "";
            objResponse.isSuccess = true;
            objResponse.ResponseMsg = "Success";
            return true;
        }

        private Dictionary<string, int> GetUserList()
        {
            try
            {
                UsersDAL objDal = new UsersDAL();
                Dictionary<string, int> dict = new Dictionary<string, int>();
                var result = objDal.GetUserlist(""); // for all users
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (!dict.ContainsKey(result[i].FullName))
                        {
                            dict.Add(result[i].FullName, result[i].Id);
                        }
                    }
                    return dict;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private Dictionary<string, int> GetProjectList()
        {
            try
            {
                ProjectsListDAL objDal = new ProjectsListDAL();
                Dictionary<string, int> dict = new Dictionary<string, int>();
                var result = objDal.GetProjects(""); // for all
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (!dict.ContainsKey(result[i].Name))
                        {
                            dict.Add(result[i].Name, result[i].ID);
                        }
                    }
                    return dict;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private void AddReportProject(int projectid, string ProjectName, string ProjectEstimate)
        {
            try
            {
                if(projectid > 0)
                {
                    ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                    // Need to verify this
                    var result = objDal.AddReportProject(-1, projectid, ProjectName, ProjectEstimate);
                }
            }
            catch (Exception ex) { }
        }

        public DateTime converttodate(string date)
        {
            try
            {
                date = date.TrimStart();
                date = date.TrimEnd();

                string[] datearray = date.Split(new char[] { ' ' });

                if (datearray.Length <= 1)
                {
                    datearray = date.Split(new char[] { '-' });
                }
                string day = datearray[0].ToLower().Replace("th", "");
                day = day.ToLower().Replace("st", "");
                day = day.ToLower().Replace("rd", "");
                day = day.ToLower().Replace("nd", "");

                string month = datearray[datearray.Length - 1].ToString();

                if ("january".Contains(month.ToLower()))
                {
                    month = "01";
                }
                else if ("february".Contains(month.ToLower()))
                {
                    month = "02";
                }
                else if ("march".Contains(month.ToLower()))
                {
                    month = "03";
                }
                else if ("april".Contains(month.ToLower()))
                {
                    month = "04";
                }
                else if ("may".Contains(month.ToLower()))
                {
                    month = "05";
                }
                else if ("june".Contains(month.ToLower()))
                {
                    month = "06";
                }
                else if ("july".Contains(month.ToLower()))
                {
                    month = "07";
                }
                else if ("august".Contains(month.ToLower()))
                {
                    month = "08";
                }
                else if ("september".Contains(month.ToLower()))
                {
                    month = "09";
                }
                else if ("october".Contains(month.ToLower()))
                {
                    month = "10";
                }
                else if ("november".Contains(month.ToLower()))
                {
                    month = "11";
                }
                else if ("december".Contains(month.ToLower()))
                {
                    month = "12";
                }

                string year = DateTime.Now.Year.ToString();

                DateTime newdatetime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));

                return newdatetime;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }

        private void AddResource(int ResourceID, string Department, Nullable<DateTime> startDate, Nullable<DateTime> endDate, string Duration, int projectID)
        {
            try
            {
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                // need to verify this
                objDal.AddResource(-1, ResourceID, Department, startDate, endDate, Duration, projectID);
            }
            catch (Exception ex) { }
        }

        private void AddProjectUpdate(string updateNo, int resourceID, int ProjectID, string vComments, string vProjectPhase)
        {
            try
            {
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                // need to veriffy this 
                objDal.AddProjectUpdate(-1, updateNo, resourceID, ProjectID, vComments, vProjectPhase);
            }
            catch (Exception ex) { }
        }

        #endregion

        #region Utility functions 

        public List<ProjectGridBO> GetScheduleProjects()
        {
            try
            {
                var lst = new List<ProjectGridBO>();
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                var result = objDal.GetScheduleProjects(0); // passing default value 
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ProjectGridBO
                        {
                            ProjectID = Convert.ToInt32(result[i].ProjectID),
                            ProjectName = result[i].SchReport_ProjectName
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }

        public List<ResourceGridBO> GetScheduleResourceData(int ProjectID)
        {
            try
            {
                if(ProjectID > 0)
                {
                    var lst = new List<ResourceGridBO>();
                    ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                    var result = objDal.GetScheduleResourceData(ProjectID);
                    if(result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            lst.Add(new ResourceGridBO
                            {
                                projectID = Convert.ToInt32(result[i].ProjectID),
                                ResourceDetailID = result[i].SchReport_ResourceDetailID,
                                ResourceId = Convert.ToInt32(result[i].ResourceID),
                                Department = result[i].Phase == null ? "" : result[i].Phase,
                                Duration = result[i].Duration == null ? "" : result[i].Duration,
                                EndDate = result[i].EndDate == null ? "" : Convert.ToDateTime(result[i].EndDate).ToString("dd/MM/yyyy"),
                                ResourceName = result[i].FullName == null ? "" : result[i].FullName,
                                StartDate = result[i].StartDate == null ? "" : Convert.ToDateTime(result[i].StartDate).ToString("dd/MM/yyyy")
                            });
                        }
                        return lst;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e) { throw e; }
        }
        
        public List<UpdateGridBO> GetScheduleUpdates(int projectid)
        {
            //
            try
            {
                if(projectid > 0)
                {
                    var lst = new List<UpdateGridBO>();
                    ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                    var result = objDal.GetScheduleUpdates(projectid);
                    if(result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            lst.Add(new UpdateGridBO
                            {
                                ProjectID = Convert.ToInt32(result[i].ProjectID),
                                UpdateID = result[i].SchReport_UpdateID,
                                ResourceID = Convert.ToInt32(result[i].ResourceID),
                                UpdateNumber = result[i].Update_, // need to check that i.e. update#
                                Comments = result[i].Comments,
                                ResourceName = result[i].FullName,
                                phase = result[i].Phase
                            });
                        }
                        return lst;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

        #region Export Template function 
        
        public List<ProjectListExportBO> GetProjectListExport()
        {
            try
            {
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                var lst = new List<ProjectListExportBO>();
                var result = objDal.GetScheduleProjectListExport(0); // default value
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ProjectListExportBO
                        {
                            schProjectID = result[i].SchReport_ProjectID,
                            ProjectID = Convert.ToInt32(result[i].ProjectID),
                            ProjectName = result[i].SchReport_ProjectName,
                            ProjectEstimate = result[i].ProjectEstimate
                        });
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // need to check isdbnull function
        public List<ProjectResourceExportBO> GetResourceListExport()
        {
            try
            {
                var lst = new List<ProjectResourceExportBO>();
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                var result = objDal.GetScheduleResourceListExport(0); // default value
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        string _StartDate = "";
                        string _EndDate = "";
                        var value = result[i].StartDate;;
                        var value1 = result[i].EndDate;
                       
                        if (value != null && !System.Convert.IsDBNull(value))
                        {
                            try
                            {
                                ;
                                _StartDate = Convert.ToDateTime(value).ToShortDateString();
                            }
                            catch { }
                        }
                        if (value1 != null && !System.Convert.IsDBNull(value1))
                        {
                            try
                            {
                                _EndDate = Convert.ToDateTime(value1).ToShortDateString();
                            }
                            catch { }
                        }
                        
                        lst.Add(new ProjectResourceExportBO
                        {
                            ResourceName = result[i].FullName,
                            ResourceDetailID = result[i].SchReport_ResourceDetailID,
                            ResourceID = Convert.ToInt32(result[i].ResourceID),
                            Phase = result[i].Phase,
                            StartDate = _StartDate,
                            EndDate = _EndDate,
                            Duration = result[i].Duration == null ? "" : result[i].Duration,
                            ProjectID = Convert.ToInt32(result[i].ProjectID)
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<UpdateExportBO> GetUpdateListExport()
        {
            try
            {
                var lst = new List<UpdateExportBO>();
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                var result = objDal.GetScheduleUpdateListExport(0);
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new UpdateExportBO
                        {
                            FullName = result[i].FullName,
                            UpdateID = result[i].SchReport_UpdateID,
                            UpdateNumber = result[i].Update_, // need to check i.e. update#
                            ResourceID = Convert.ToInt32(result[i].ResourceID),
                            ProjectID = Convert.ToInt32(result[i].ProjectID),
                            Comments = result[i].Comments,
                            Phase = result[i].Phase,
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ProjectUpdateLengthBO> getProjectLength()
        {
            try
            {
                var lst = new List<ProjectUpdateLengthBO>();
                ProjectScheduleDAL objDal = new ProjectScheduleDAL();
                var result = objDal.getScheduleProjectLength();
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ProjectUpdateLengthBO
                        {
                            ProjectID = Convert.ToInt32(result[i].ProjectID),
                            UpdateCount = Convert.ToInt32(result[i].UpdateCount)
                        });
                    }
                    return lst;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string converttostring(DateTime dateobj)
        {
            string day = dateobj.Day.ToString();

            if (day == "1")
            {
                day = "1st";
            }
            else if (day == "2")
            {
                day = "2nd";
            }
            else if (day == "3")
            {
                day = "3rd";
            }
            else
            {
                day = day + "th";
            }

            string month = dateobj.Month.ToString();

            if (Convert.ToInt16(month) == 1)
            {
                month = "JAN";
            }
            else if (Convert.ToInt16(month) == 2)
            {
                month = "FEB";
            }
            else if (Convert.ToInt16(month) == 3)
            {
                month = "MAR";
            }
            else if (Convert.ToInt16(month) == 4)
            {
                month = "APR";
            }
            else if (Convert.ToInt16(month) == 5)
            {
                month = "MAY";
            }
            else if (Convert.ToInt16(month) == 6)
            {
                month = "JUN";
            }
            else if (Convert.ToInt16(month) == 7)
            {
                month = "JUL";
            }
            else if (Convert.ToInt16(month) == 8)
            {
                month = "AUG";
            }
            else if (Convert.ToInt16(month) == 9)
            {
                month = "SEP";
            }
            else if (Convert.ToInt16(month) == 10)
            {
                month = "OCT";
            }
            else if (Convert.ToInt16(month) == 11)
            {
                month = "NOV";
            }
            else if (Convert.ToInt16(month) == 12)
            {
                month = "DEC";
            }

            return day + " " + month;
        }




        #endregion



    }
}
