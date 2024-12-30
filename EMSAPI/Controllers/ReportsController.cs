using DemoEFBL.Reports;
using DemoEFBO.Reports;
using DemoEFCommon.ApiResponseResult;
using DemoEFDAL;
using DemoEFDAL.Reports;
using EMSAPI.ActionFilter;
using EMSAPI.Common;
using EMSAPI.Utility.Nlog;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Results;


namespace EMSAPI.Controllers
{
    public class DelAssign
    {
        public int ID { get; set; }
    }
    public class CheckUsersData
    {
        public int UserID { get; set; }
        public int ProjectID { get; set; }
    }
    public class CheckUsersHours
    {
        public int UserID { get; set; }
        public int ProjectID { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Hours { get; set; }
    }
    [Authorize]
    [CheckCustomFilter]
    public class ReportsController : ApiController
    {
        //[HttpPost]
        //[Route("api/Reports/GetAssignments")]
        //public HttpResponseMessage GetAssignments(AssignmentDTOBO objRptParam)
        //{
        //    MyLogger.GetInstance().Info($"Entering the Reports Controller.  GetAssignments Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objRptParam)}");
        //    try
        //    {
        //        ReportsBL objRptBL = new ReportsBL();
        //        var result = objRptBL.GetAssignments(objRptParam);
        //        if(result != null)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
        //            MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Success)  ");
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Failed)  ");
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}
        [HttpPost]
        [Route("api/Reports/GetAssignments")]
        public HttpResponseMessage GetAssignments(GetAssignmentBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  GetAssignments Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    var result = objRptBL.GetAssignments(obj);
                    if (obj.ClientID > 0)
                    {
                        result = result.Where(x => x.ClientID == obj.ClientID).ToList();
                    }
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        //[HttpPost]
        //[Route("api/Reports/SaveAssignments")]
        //public HttpResponseMessage SaveAssignemntToDb(AssignmentsBO objRptParam)
        //{
        //    MyLogger.GetInstance().Info($"Entering the Reports Controller.  SaveAssignemntToDb Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objRptParam)}");
        //    try
        //    {
        //        ReportsBL objRptBL = new ReportsBL();
        //        bool result = objRptBL.SaveAssignemntToDb(objRptParam);
        //        if (result)
        //        {
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
        //            MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Success)  ");
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Failed)  ");
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}
        [HttpPost]
        [Route("api/Reports/CheckUserAssigned")]
        public HttpResponseMessage CheckUserAssigned(CheckUsersData data)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportDAL objDal = new ReportDAL();
                    bool result = objDal.CheckAssigneduser(data.UserID, data.ProjectID);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.NotFound };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  CheckUserAssigned Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                               System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Reports/SaveAssignments")]
        public HttpResponseMessage SaveAssignemntToDb(AddRecordBO objRptParam)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  SaveAssignemntToDb Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(objRptParam)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    bool CheckMaintasksubtask = objRptBL.CheckMaintaskSubTaskToDb(objRptParam);
                    if (!CheckMaintasksubtask)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "SubTask not available in the MainTask.", MainTaskCheck = true };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  CheckMaintaskSubTaskToDb Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    bool result = objRptBL.SaveAssignemntToDb(objRptParam);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  SaveAssignemntToDb Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }

            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Reports/ExportToExcelWeeklyAll")]
        public HttpResponseMessage ExportToExcelWeeklyAll(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    if (assignmentWeekly.ProjectId == 0)
                    {
                        assignmentWeekly.ProjectId = -1;
                    }
                    //if (assignmentWeekly.TaskOwnerId == 0)
                    //{
                    //    assignmentWeekly.TaskOwnerId = -1;
                    //}
                    if (assignmentWeekly.TaskId == 0)
                    {
                        assignmentWeekly.TaskId = -1;
                    }
                    if (assignmentWeekly.SubTaskId == 0)
                    {
                        assignmentWeekly.SubTaskId = -1;
                    }
                    List<string> path = new List<string>();
                    var Filename = assignmentWeekly.fileName;
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
                    DateTime checkdate = Directory.GetCreationTime(fileDirectory);
                    if (checkdate.Date != DateTime.Now.Date)
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}
                    var getProjectwithnoClientID = new List<ProjectAllArray>();
                    if (assignmentWeekly.ExportType)
                    {
                        getProjectwithnoClientID.AddRange(assignmentWeekly.AllArrays.Where(x => x.ClientID == 0).ToList());
                    }
                    assignmentWeekly.AllArrays = assignmentWeekly.AllArrays.Where(x => x.ProjectIdAll > 0).ToList();
                    if (assignmentWeekly.ExportType == false || getProjectwithnoClientID.Count > 0)
                    {
                        var getArrayProject = getProjectwithnoClientID.Count > 0 ? getProjectwithnoClientID : assignmentWeekly.AllArrays;
                        foreach (var item in getArrayProject)
                        {
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = item.ProjectNameAll + Filename;

                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.ProjectId = item.ProjectIdAll;
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".xlsx";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            assignmentWeekly.Billable = true;
                            Tuple<string, bool> result = objRptBL.GenerateExcelTSWeekly(assignmentWeekly, filePath, workSheetName);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".xlsx";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }

                    }
                    if (assignmentWeekly.ExportType == true)
                    {
                        var RemoveClientwithZero = assignmentWeekly.AllArrays.Where(x => x.ClientID > 0).ToList();
                        var groupbyprojects = RemoveClientwithZero.GroupBy(x => x.ClientID).ToList();
                        foreach (var groupbyproject in groupbyprojects)
                        {
                            var getClientName = assignmentWeekly.ClientArray.Where(c => c.ID == groupbyproject.Key).FirstOrDefault();
                            var getProjectIDs = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectIdAll).ToList();
                            var getProjectName = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectNameAll).ToList();
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = getClientName.ClientName + Filename;

                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.projectwise = string.Join(",", getProjectIDs);
                            var getProject_Name = string.Join(",", getProjectName);
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".xlsx";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            Tuple<string, bool> result = objRptBL.GenerateExcelTSWeeklyClientWise(assignmentWeekly, filePath, workSheetName, getProject_Name);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".xlsx";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }
                    }
                    if (path.Count > 0)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = path };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (path.Count == 0)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = "No Record Found" };
                        }
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        [Route("api/Reports/ExportToPdfWeekly")]
        public HttpResponseMessage ExportToPdfWeekly(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int CountName = 0;
                    string File_Name = string.Empty;
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");
                    var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";
                    DateTime date = Directory.GetCreationTime(fileDirectory);
                    if (date.Date == DateTime.Now.Date)
                    {
                        string[] excellist = Directory.GetFiles(fileDirectory, "*.pdf");
                        //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                        foreach (var item in excellist)
                        {
                            var file = System.IO.Path.GetFileNameWithoutExtension(item);
                            var lastWord = file[file.Length - 1];
                            if (lastWord == ')')
                            {
                                var split = file.Substring(0, file.Length - 3);
                                if (split == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                                else
                                {
                                    var split_File = file.Substring(0, file.Length - 4);
                                    if (split_File == assignmentWeekly.fileName)
                                    {
                                        CountName++;
                                    }
                                }
                            }
                            else
                            {
                                if (file == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                            }
                        }
                        if (CountName > 0)
                        {
                            File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".pdf";
                            filePath = fileDirectory + File_Name;
                        }
                        if (CountName == 0)
                        {
                            File_Name = assignmentWeekly.fileName + ".pdf";
                        }
                    }
                    else
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");
                    //System.IO.DirectoryInfo di = new DirectoryInfo(fileDirectory);

                    //foreach (FileInfo file in di.GetFiles())
                    //{
                    //    file.Delete();
                    //}
                    //var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");
                    // var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}
                    ReportsBL objRptBL = new ReportsBL();
                    DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                    DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                    assignmentWeekly.FromDate = sdate.Date;
                    assignmentWeekly.ToDate = edate.Date;
                    //assignmentWeekly.pdftextboxvalue = item.ReferenceNumber;
                    Tuple<string, bool> result = objRptBL.GeneratePdfTSWeekly(assignmentWeekly, filePath);
                    if (string.IsNullOrEmpty(File_Name))
                    {
                        File_Name = assignmentWeekly.fileName + ".pdf";
                    }
                    if (result.Item2)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = filePath, File_Name = File_Name };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (result.Item1 != null)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = result.Item1 };
                        }

                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("api/Reports/DeleteAssignment")]
        public HttpResponseMessage DeleteAssignment(DelAssign obj)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  DeleteAssignment Method   Arguments:: {obj.ID}");

            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    bool result = objRptBL.DeleteAssignment(obj.ID);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  DeleteAssignment Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  DeleteAssignment Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  DeleteAssignment Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
               
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        public HttpResponseMessage ApproveAssignment([FromBody] int projectId)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  ApproveAssignment Method   Arguments:: {projectId}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    bool result = objRptBL.ApproveAssignment(projectId);
                    if (result)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = ResponseResultMsg.SavedResultMsg };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  ApproveAssignment Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  ApproveAssignment Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  ApproveAssignment Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        
        //[HttpPost]
        //public HttpResponseMessage ExportToExcel(AssignmentDTOBO objRptParam)
        //{
        //    try
        //    {
        //        ReportsBL objRptBL = new ReportsBL();
        //        if(objRptParam != null)
        //        {
        //            var assignments = new List<AssignmentsModelForExcelBO>();
        //            if (objRptParam.taskTypePrefixes != null && objRptParam.taskTypePrefixes.Count() > 0)
        //            {
        //                assignments = objRptBL.GetAllAssignmentResult(objRptParam).Where(e =>
        //                objRptParam.taskTypePrefixes.Contains(e.TaskTypePrefix)).ToList();
        //            }
        //            else
        //            {
        //                assignments = objRptBL.GetAllAssignmentResult(objRptParam);
        //            }
        //            var assignmentsWithTotals = objRptBL.GetTotalHoursOfMonth(assignments);
        //            var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
        //            var filePath = fileDirectory + "TimeSheet.xlsx";
        //            if (!System.IO.Directory.Exists(fileDirectory))
        //            {
        //                System.IO.Directory.CreateDirectory(fileDirectory);
        //            }
        //            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
        //            this.WriteListToExcel(assignmentsWithTotals, filePath, workSheetName);
        //            var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = filePath };
        //            return new HttpResponseMessage
        //            {
        //                Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
        //                                    System.Text.Encoding.UTF8, "application/json")
        //            };
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
        //    }
        //}

        // skip due to error and discussion with faisal bhai
        [HttpPost]
        public HttpResponseMessage ExportComparisonReportToExcel(AssignmentDTOBO assignment)
        {
            //try
            //{
            //    var comparedTasks = this.GetActualVsEstimatedComparedTasks(assignment);
            //    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
            //    var filePath = fileDirectory + "ComparisonReport.xlsx";
            //    if (!System.IO.Directory.Exists(fileDirectory))
            //    {
            //        System.IO.Directory.CreateDirectory(fileDirectory);
            //    }
            //    var file = this.WriteComparisonReportToExcel(comparedTasks, filePath, "Comparison Report");
            //    return Json(filePath, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            return null;
        }

        // Same why use 2 ?
        [HttpPost]
        public HttpResponseMessage DownloadFileTimeSheet([FromBody] string filepath, [FromBody] string fileName)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                    response.Content = new ByteArrayContent(filedata);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
                    response.Content.Headers.ContentLength = filedata.Length;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    return response;
                }else
                {
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                    response.Content = new ByteArrayContent(filedata);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
                    response.Content.Headers.ContentLength = filedata.Length;
                    response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    return response;
                }
                   
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }


        [HttpPost]
        public HttpResponseMessage DownloadFile([FromBody] string filepath, [FromBody] string fileName)
        {
            try
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                response.Content = new ByteArrayContent(filedata);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/ms-excel");
                response.Content.Headers.ContentLength = filedata.Length;
                response.StatusCode = System.Net.HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }


        }

        #region Methods 

        private void WriteListToExcel(List<AssignmentsModelForExcelBO> assignments, string filePath, string workSheetName)
        {
            try
            {
                FileInfo newFile = new FileInfo(filePath);

                if (newFile.Exists)
                {
                    newFile.Delete();
                    newFile = new FileInfo(filePath);
                }
                ExcelPackage pck = new ExcelPackage(newFile);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(workSheetName);

                ws.Cells["A1"].Value = "Task Detail Report";
                ws.Cells["A3"].Value = "Project";
                ws.Cells["B3"].Value = "Date";
                ws.Cells["C3"].Value = "Task Owner";
                ws.Cells["D3"].Value = "Duration (Hours)";
                ws.Cells["E3"].Value = "Days";
                ws.Cells["F3"].Value = "Main Task";
                ws.Cells["G3"].Value = "Task/PBI";
                ws.Cells["H3"].Value = "Description";
                ws.Cells["I3"].Value = "Task Type";
                ws.Cells["J3"].Value = "Phase #";

                int totalCols = ws.Dimension.End.Column;
                var headerCells = ws.Cells[1, 1, 3, totalCols];
                headerCells.Style.Font.Bold = true;
                int rowStart = 4;
                foreach (var item in assignments)
                {
                    if (item.Project.Equals("TOTAL") && (item.Task.Substring(0, 4).Equals("Week") || item.Task.Substring(0, 5).Equals("Month")))
                    {
                        ws.Cells[string.Format("F{0}", rowStart)].Value = item.Task + " (hrs/days)" + Convert.ToDecimal(item.Duration.ToString("0.00")) + " and " + Convert.ToDecimal((item.Days).ToString("0.00"));
                        ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#bdd5fc")));
                        ws.Row(rowStart).Style.Font.Bold = true;
                    }
                    else
                    {
                        ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        if (item.IsApproved)
                        {
                            ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("#b4ff9b")));
                        }
                        else
                        {
                            ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("pink")));
                        }

                        ws.Cells[string.Format("A{0}", rowStart)].Value = item.Project;
                        ws.Cells[string.Format("B{0}", rowStart)].Value = item.Date;
                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.TaskOwner;
                        ws.Cells[string.Format("D{0}", rowStart)].Value = Convert.ToDecimal(item.Duration.ToString("0.00"));
                        ws.Cells[string.Format("E{0}", rowStart)].Value = item.Days;
                        ws.Cells[string.Format("F{0}", rowStart)].Value = item.MainTaskName;
                        ws.Cells[string.Format("G{0}", rowStart)].Value = item.Task;
                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.SubTask;
                        ws.Cells[string.Format("I{0}", rowStart)].Value = item.TaskType;
                        ws.Cells[string.Format("J{0}", rowStart)].Value = Convert.ToDecimal(item.Phase);

                    }
                    rowStart++;
                }

                ws.Cells["A:AZ"].AutoFitColumns();
                const double minWidth = 50.00;
                const double maxWidth = 100.00;
                ws.Cells["G3"].AutoFitColumns(minWidth, maxWidth);

                pck.Save();

                //byte[] filedata = System.IO.File.ReadAllBytes(filePath);
                //string contentType = MimeMapping.GetMimeMapping(filePath);

                //var cd = new System.Net.Mime.ContentDisposition
                //{
                //    FileName = "Timesheet_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx",
                //    Inline = true,
                //};

                //Response.AppendHeader("Content-Disposition", cd.ToString());
                //return File(filedata, contentType);
            }
            catch (Exception ex) { }
        }
        [HttpPost]
        [Route("api/Reports/ExportToExcelWeekly")]
        public HttpResponseMessage ExportToExcelWeekly(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int CountName = 0;
                    string File_Name = string.Empty;
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
                    var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";
                    DateTime date = Directory.GetCreationTime(fileDirectory);
                    if (date.Date == DateTime.Now.Date)
                    {
                        string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                        //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                        foreach (var item in excellist)
                        {
                            var file = System.IO.Path.GetFileNameWithoutExtension(item);
                            var lastWord = file[file.Length - 1];
                            if (lastWord == ')')
                            {
                                var split = file.Substring(0, file.Length - 3);
                                if (split == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                                else
                                {
                                    var split_File = file.Substring(0, file.Length - 4);
                                    if (split_File == assignmentWeekly.fileName)
                                    {
                                        CountName++;
                                    }
                                }
                            }
                            else
                            {
                                if (file == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                            }
                        }
                        if (CountName > 0)
                        {
                            File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                            filePath = fileDirectory + File_Name;
                        }
                        if (CountName == 0)
                        {
                            File_Name = assignmentWeekly.fileName + ".xlsx";
                        }
                    }
                    else
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //var FileName = assignmentWeekly.fileName + ".xlsx";
                    //if (File.Exists(Path.Combine(fileDirectory, FileName)))
                    //{
                    //    File.Delete(Path.Combine(fileDirectory, FileName));
                    //}
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}

                    var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                    ReportsBL objRptBL = new ReportsBL();
                    DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                    DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                    assignmentWeekly.FromDate = sdate.Date;
                    assignmentWeekly.ToDate = edate.Date;
                    Tuple<string, bool> result = objRptBL.GenerateExcelTSWeekly(assignmentWeekly, filePath, workSheetName);
                    if (string.IsNullOrEmpty(File_Name))
                    {
                        File_Name = assignmentWeekly.fileName + ".xlsx";
                    }
                    if (result.Item2)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = filePath, File_Name = File_Name };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (result.Item1 != null)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = result.Item1 };
                        }
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                    
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        [HttpPost]
        [Route("api/Reports/ExportToPdfWeeklyAll")]
        public HttpResponseMessage ExportToPdfWeeklyAll(ExportAssignmentBO assignmentWeekly)
        {
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");
                    List<string> path = new List<string>();
                    var Filename = assignmentWeekly.fileName;
                    DateTime checkdate = Directory.GetCreationTime(fileDirectory);
                    if (checkdate.Date != DateTime.Now.Date)
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }

                    // var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");

                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}
                    assignmentWeekly.AllArrays = assignmentWeekly.AllArrays.Where(x => x.ProjectIdAll > 0).ToList();
                    var getProjectwithnoClientID = new List<ProjectAllArray>();
                    if (assignmentWeekly.ExportType == true)
                    {
                        getProjectwithnoClientID.AddRange(assignmentWeekly.AllArrays.Where(x => x.ClientID == 0).ToList());
                    }
                    if (assignmentWeekly.ExportType == false || getProjectwithnoClientID.Count > 0)
                    {
                        var getArrayProject = getProjectwithnoClientID.Count > 0 ? getProjectwithnoClientID : assignmentWeekly.AllArrays;
                        foreach (var item in getArrayProject)
                        {
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = item.ProjectNameAll + Filename;
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.ProjectId = item.ProjectIdAll;
                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.pdf");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }

                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".pdf";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".pdf";
                                }
                            }
                            //else
                            //{
                            //    System.IO.Directory.Delete(fileDirectory, true);
                            //}
                            assignmentWeekly.pdftextboxvalue = item.ReferenceNumber;
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            Tuple<string, bool> result = objRptBL.GeneratePdfTSWeekly(assignmentWeekly, filePath);
                             if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName;
                                
                            }
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                            
                        }

                   }
                    if (assignmentWeekly.ExportType == true)
                    {
                        var RemoveClientwithZero = assignmentWeekly.AllArrays.Where(x => x.ClientID > 0).ToList();
                        var groupbyprojects = RemoveClientwithZero.GroupBy(x => x.ClientID).ToList();
                        foreach (var groupbyproject in groupbyprojects)
                        {
                            var getClientName = assignmentWeekly.ClientArray.Where(c => c.ID == groupbyproject.Key).FirstOrDefault();
                            var getProjectIDs = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectIdAll).ToList();
                            var getProjectName = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectNameAll).ToList();
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = getClientName.ClientName + Filename;

                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.projectwise = string.Join(",", getProjectIDs);
                            var getProject_Name = string.Join(",", getProjectName);
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.pdf");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".pdf";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".pdf";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            assignmentWeekly.pdftextboxvalue = groupbyproject.First().ReferenceNumber;
                            Tuple<string, bool> result = objRptBL.GeneratePdfTSWeeklyClientWise(assignmentWeekly, filePath, workSheetName, getProject_Name);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".pdf";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }
                    }
                    if (path.Count > 0)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = path };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (path.Count == 0)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = "No Record Found" };
                        }
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
        [HttpPost]
        [Route("api/Reports/GetTotalHoursForTasks")]
        public HttpResponseMessage GetTotalHoursForTasks(AssignmentDTO assignment)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  GetTotalHoursForTasks Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(assignment)}");
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    var result = objRptBL.GetReportshours(assignment);
                    if (assignment.ClientID > 0 && result != null)
                    {
                        result = result.Where(x => x.ClientID == assignment.ClientID).ToList();
                    }
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetTotalHoursForTasks Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetTotalHoursForTasks Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetTotalHoursForTasks Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        [Route("api/Reports/CheckUserHours")]
        public HttpResponseMessage CheckUserHours(CheckUsersHours data)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportDAL objDal = new ReportDAL();
                    decimal result = objDal.CheckuserHours(data.UserID, data.ProjectID, data.DateTime.Date);
                    result = result + data.Hours;
                    if (result <= 24)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  CheckUserHours Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.NotFound, Result = result, MainTaskCheck = true };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  CheckUserHours Method(Failed)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                               System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  CheckUserHours Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Reports/GetAssignmentsGrid")]
        public HttpResponseMessage GetAssignmentsGrid(GetAssignmentBO obj)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  GetAssignments Method   Arguments:: {Newtonsoft.Json.JsonConvert.SerializeObject(obj)}");
            try
            {

                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    ReportsBL objRptBL = new ReportsBL();
                    var result = objRptBL.GetAssignmentsList(obj);
                    //if (obj.ClientID > 0)
                    //{
                    //    result = result.Where(x => x.ClientID == obj.ClientID).ToList();
                    //}
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = result };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  GetAssignments Method(Failed)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Reports/ExportToPdfMainTaskBreakDown")]
        public HttpResponseMessage ExportToPdfMainTaskBreakDown(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    //int CountName = 0;
                    List<string> path = new List<string>();
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/PdfFiles/");
                    DateTime checkdate = Directory.GetCreationTime(fileDirectory);
                    if (checkdate.Date != DateTime.Now.Date)
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }

                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}
                    assignmentWeekly.AllArrays = assignmentWeekly.AllArrays.Where(x => x.ProjectIdAll > 0).ToList();
                    var getProjectwithnoClientID = new List<ProjectAllArray>();
                    if (assignmentWeekly.ExportType == true)
                    {
                        getProjectwithnoClientID.AddRange(assignmentWeekly.AllArrays.Where(x => x.ClientID == 0).ToList());
                    }
                    if (assignmentWeekly.ExportType == false || getProjectwithnoClientID.Count > 0)
                    {
                        var getArrayProject = getProjectwithnoClientID.Count > 0 ? getProjectwithnoClientID : assignmentWeekly.AllArrays;
                        foreach (var item in getArrayProject)
                        {
                            int CountName = 0;
                            string File_Name = string.Empty;
                            DateTime FromDate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime ToDate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = FromDate.Date;
                            assignmentWeekly.ToDate = ToDate.Date;
                            if (FromDate.Month == ToDate.Month)
                            {
                                assignmentWeekly.fileName = item.ProjectNameAll + " MAIN TASK " + FromDate.Day + " - " + ToDate.Day + " " + FromDate.ToString("MMM") + " REPORT";
                            }
                            else
                            {
                                assignmentWeekly.fileName = item.ProjectNameAll + " MAIN TASK " + FromDate.Day + " " + FromDate.ToString("MMM") + " - " + ToDate.Day + " " + ToDate.ToString("MMM") + " REPORT";
                            }

                            //assignmentWeekly.fileName = item.ProjectNameAll;
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.ProjectId = item.ProjectIdAll;
                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.pdf");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".pdf";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".pdf";
                                }
                            }
                            Tuple<string, bool> result = objRptBL.GeneratepdfBreakDown(assignmentWeekly, filePath);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".pdf";
                            }
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }
                    }
                    if (assignmentWeekly.ExportType == true)
                    {
                        var RemoveClientwithZero = assignmentWeekly.AllArrays.Where(x => x.ClientID > 0).ToList();
                        var groupbyprojects = RemoveClientwithZero.GroupBy(x => x.ClientID).ToList();
                        foreach (var groupbyproject in groupbyprojects)
                        {

                            var getClientName = assignmentWeekly.ClientArray.Where(c => c.ID == groupbyproject.Key).FirstOrDefault();
                            var getProjectIDs = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectIdAll).ToList();
                            var getProjectName = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectNameAll).ToList();
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = getClientName.ClientName;
                            DateTime FromDate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime ToDate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = FromDate.Date;
                            assignmentWeekly.ToDate = ToDate.Date;
                            if (FromDate.Month == ToDate.Month)
                            {
                                assignmentWeekly.fileName = getClientName.ClientName + " MAIN TASK " + FromDate.Day + " - " + ToDate.Day + " " + FromDate.ToString("MMM") + " REPORT";
                            }
                            else
                            {
                                assignmentWeekly.fileName = getClientName.ClientName + " MAIN TASK " + FromDate.Day + " " + FromDate.ToString("MMM") + " - " + ToDate.Day + " " + ToDate.ToString("MMM") + " REPORT";
                            }
                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.projectwise = string.Join(",", getProjectIDs);
                            var getProject_Name = string.Join(",", getProjectName);
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".pdf";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.pdf");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".pdf";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".pdf";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            Tuple<string, bool> result = objRptBL.GeneratepdfBreakDownClientWise(assignmentWeekly, filePath, getProject_Name);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".pdf";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }
                    }
                    //ReportsBL objRptBL = new ReportsBL();

                    if (path.Count() > 0)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = path };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (path.Count() != 0)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = "No Record Found" };
                        }

                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        [Route("api/Reports/WBSExcelExport")]
        public HttpResponseMessage WBSExcelExport(int ProjectID, string ProjectName)
        {
            MyLogger.GetInstance().Info($"Entering the Reports Controller.  WBSExcelExport Method   Arguments:: ProjectID" + ProjectID);
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
                    DateTime checkdate = Directory.GetCreationTime(fileDirectory);
                    if (checkdate.Date != DateTime.Now.Date)
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }

                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    string filepath = string.Empty;
                    var FileName = ProjectName + "-WBS";
                    int CountName = 0;
                    string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                    foreach (var itemfile in excellist)
                    {
                        var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                        var lastWord = file[file.Length - 1];
                        if (lastWord == ')')
                        {
                            var split = file.Substring(0, file.Length - 3);
                            if (split == FileName)
                            {
                                CountName++;
                            }
                            else
                            {
                                var split_File = file.Substring(0, file.Length - 4);
                                if (split_File == ProjectName)
                                {
                                    CountName++;
                                }
                            }
                        }
                        else
                        {
                            if (file == FileName)
                            {
                                CountName++;
                            }
                        }
                    }
                    if (CountName > 0)
                    {
                        FileName = FileName + "(" + CountName + ")" + ".xlsx";
                        filepath = fileDirectory + FileName;
                    }
                    if (CountName == 0)
                    {
                        FileName = FileName + ".xlsx";
                    }
                    if (string.IsNullOrEmpty(filepath))
                    {
                        filepath = fileDirectory + FileName;
                    }
                    ReportsBL objRptBL = new ReportsBL();
                    Tuple<string, bool> result = objRptBL.ExportToWBSReport(ProjectID, FileName, filepath, workSheetName);
                    if (result != null)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = FileName };
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  WBSExcelExport Method(Success)  ");
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        MyLogger.GetInstance().Info($"Exit the Reports Controller.  WBSExcelExport Method(Failed)  ");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResponseResultMsg.ErrorMsg);
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    MyLogger.GetInstance().Info($"Exit the Reports Controller.  WBSExcelExport Method(Success)  ");
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                   
            }
            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Exception in Notification Controller!  " + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Route("api/Reports/ExportToExcelctualHrsWeeklyAll")]
        public HttpResponseMessage ExportToExcelctualHrsWeeklyAll(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
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
                    List<string> path = new List<string>();
                    var Filename = assignmentWeekly.fileName;
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
                    DateTime checkdate = Directory.GetCreationTime(fileDirectory);
                    if (checkdate.Date != DateTime.Now.Date)
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}
                    var getProjectwithnoClientID = new List<ProjectAllArray>();
                    if (assignmentWeekly.ExportType)
                    {
                        getProjectwithnoClientID.AddRange(assignmentWeekly.AllArrays.Where(x => x.ClientID == 0).ToList());
                    }
                    assignmentWeekly.AllArrays = assignmentWeekly.AllArrays.Where(x => x.ProjectIdAll > 0).ToList();
                    if (assignmentWeekly.ExportType == false || getProjectwithnoClientID.Count > 0)
                    {
                        var getArrayProject = getProjectwithnoClientID.Count > 0 ? getProjectwithnoClientID : assignmentWeekly.AllArrays;
                        foreach (var item in getArrayProject)
                        {
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = item.ProjectNameAll + Filename;

                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.ProjectId = item.ProjectIdAll;
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".xlsx";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            //assignmentWeekly.Billable = true;
                            Tuple<string, bool> result = objRptBL.GenerateExcelTSWeeklyActualHrs(assignmentWeekly, filePath, workSheetName);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".xlsx";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }

                    }
                    if (assignmentWeekly.ExportType == true)
                    {
                        var RemoveClientwithZero = assignmentWeekly.AllArrays.Where(x => x.ClientID > 0).ToList();
                        var groupbyprojects = RemoveClientwithZero.GroupBy(x => x.ClientID).ToList();
                        foreach (var groupbyproject in groupbyprojects)
                        {
                            var getClientName = assignmentWeekly.ClientArray.Where(c => c.ID == groupbyproject.Key).FirstOrDefault();
                            var getProjectIDs = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectIdAll).ToList();
                            var getProjectName = assignmentWeekly.AllArrays.Where(c => c.ClientID == groupbyproject.Key).Select(b => b.ProjectNameAll).ToList();
                            int CountName = 0;
                            string File_Name = string.Empty;
                            assignmentWeekly.fileName = getClientName.ClientName + Filename;

                            var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                            ReportsBL objRptBL = new ReportsBL();
                            assignmentWeekly.projectwise = string.Join(",", getProjectIDs);
                            var getProject_Name = string.Join(",", getProjectName);
                            var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";

                            DateTime date = Directory.GetCreationTime(fileDirectory);
                            if (date.Date == DateTime.Now.Date)
                            {
                                string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                                //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                                foreach (var itemfile in excellist)
                                {
                                    var file = System.IO.Path.GetFileNameWithoutExtension(itemfile);
                                    var lastWord = file[file.Length - 1];
                                    if (lastWord == ')')
                                    {
                                        var split = file.Substring(0, file.Length - 3);
                                        if (split == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                        else
                                        {
                                            var split_File = file.Substring(0, file.Length - 4);
                                            if (split_File == assignmentWeekly.fileName)
                                            {
                                                CountName++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (file == assignmentWeekly.fileName)
                                        {
                                            CountName++;
                                        }
                                    }
                                }
                                if (CountName > 0)
                                {
                                    File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                                    filePath = fileDirectory + File_Name;
                                }
                                if (CountName == 0)
                                {
                                    File_Name = assignmentWeekly.fileName + ".xlsx";
                                }
                            }
                            DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                            DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                            assignmentWeekly.FromDate = sdate.Date;
                            assignmentWeekly.ToDate = edate.Date;
                            Tuple<string, bool> result = objRptBL.GenerateExcelTSWeeklyClientWiseActualHrs(assignmentWeekly, filePath, workSheetName, getProject_Name);
                            if (string.IsNullOrEmpty(File_Name))
                            {
                                File_Name = assignmentWeekly.fileName + ".xlsx";
                            }
                            assignmentWeekly.fileName = string.Empty;
                            if (result.Item2 == false)
                            {

                            }
                            else
                            {
                                path.Add(File_Name);
                            }
                        }
                    }
                    if (path.Count > 0)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = path };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (path.Count == 0)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = "No Record Found" };
                        }
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized};
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                  

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("api/Reports/ExportToExcelWeeklyActualHrs")]
        public HttpResponseMessage ExportToExcelWeeklyActualHrs(ExportAssignmentBO assignmentWeekly)
        {
            try
            {
                GlobalFunctions globalfunctions = new GlobalFunctions();

                bool CheckUserType = globalfunctions.getHeadervalue(Request);
                if (CheckUserType)
                {
                    int CountName = 0;
                    string File_Name = string.Empty;
                    var fileDirectory = System.Web.Hosting.HostingEnvironment.MapPath("~/ExcelFiles/");
                    var filePath = fileDirectory + assignmentWeekly.fileName + ".xlsx";
                    DateTime date = Directory.GetCreationTime(fileDirectory);
                    if (date.Date == DateTime.Now.Date)
                    {
                        string[] excellist = Directory.GetFiles(fileDirectory, "*.xlsx");
                        //string[] filePaths = Directory.GetFiles(fileDirectory, assignmentWeekly.fileName + ".xlsx");
                        foreach (var item in excellist)
                        {
                            var file = System.IO.Path.GetFileNameWithoutExtension(item);
                            var lastWord = file[file.Length - 1];
                            if (lastWord == ')')
                            {
                                var split = file.Substring(0, file.Length - 3);
                                if (split == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                                else
                                {
                                    var split_File = file.Substring(0, file.Length - 4);
                                    if (split_File == assignmentWeekly.fileName)
                                    {
                                        CountName++;
                                    }
                                }
                            }
                            else
                            {
                                if (file == assignmentWeekly.fileName)
                                {
                                    CountName++;
                                }
                            }
                        }
                        if (CountName > 0)
                        {
                            File_Name = assignmentWeekly.fileName + "(" + CountName + ")" + ".xlsx";
                            filePath = fileDirectory + File_Name;
                        }
                        if (CountName == 0)
                        {
                            File_Name = assignmentWeekly.fileName + ".xlsx";
                        }
                    }
                    else
                    {
                        System.IO.Directory.Delete(fileDirectory, true);
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //var FileName = assignmentWeekly.fileName + ".xlsx";
                    //if (File.Exists(Path.Combine(fileDirectory, FileName)))
                    //{
                    //    File.Delete(Path.Combine(fileDirectory, FileName));
                    //}
                    if (!System.IO.Directory.Exists(fileDirectory))
                    {
                        System.IO.Directory.CreateDirectory(fileDirectory);
                    }
                    //else
                    //{
                    //    System.IO.Directory.Delete(fileDirectory, true);
                    //    System.IO.Directory.CreateDirectory(fileDirectory);
                    //}

                    var workSheetName = WebConfigurationManager.AppSettings["workSheetName"];
                    ReportsBL objRptBL = new ReportsBL();
                    DateTime sdate = Convert.ToDateTime(assignmentWeekly.FromDate);
                    DateTime edate = Convert.ToDateTime(assignmentWeekly.ToDate);
                    assignmentWeekly.FromDate = sdate.Date;
                    assignmentWeekly.ToDate = edate.Date;
                    Tuple<string, bool> result = objRptBL.GenerateExcelTSWeeklyActualHrs( assignmentWeekly, filePath, workSheetName);
                    if (string.IsNullOrEmpty(File_Name))
                    {
                        File_Name = assignmentWeekly.fileName + ".xlsx";
                    }
                    if (result.Item2)
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.OK, Result = filePath, File_Name = File_Name };
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                    else
                    {
                        var ResponseMsg = new { StatusCode = HttpStatusCode.BadRequest, Result = "Failed" };

                        if (result.Item1 != null)
                        {
                            ResponseMsg = new { StatusCode = HttpStatusCode.Conflict, Result = result.Item1 };
                        }
                        return new HttpResponseMessage
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                                System.Text.Encoding.UTF8, "application/json")
                        };
                    }
                }
                else
                {
                    var ResponseMsg = new { StatusCode = HttpStatusCode.Unauthorized };
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(ResponseMsg),
                                            System.Text.Encoding.UTF8, "application/json")
                    };
                }
                  
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}