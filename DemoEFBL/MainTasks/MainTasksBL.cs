using DemoEFBO.Tasks;
using DemoEFDAL.CommonTasksList;
using DemoEFDAL.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.MainTasks
{
    public class MainTasksBL
    {
        public List<GetMainTaskBO> GetMainTask()
        {
            try
            {
                List<GetMainTaskBO> GetMainTasks = new List<GetMainTaskBO>();
                //var GetMainTasks = new List<GetMainTaskBO>();
                MainTasksDAL objDal = new MainTasksDAL();

                var result = objDal.MainTaskDAL();
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].ProjectIDs == null)
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = "-1",
                                ProjectType = result[i].ProjectType

                            });
                        }
                        else
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = result[i].ProjectIDs,
                                ProjectType = result[i].ProjectType

                            });
                        }
                    }
                    return GetMainTasks;
                }
                return null;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<GetMainTaskBO> GetFilterMainTaskList(string MainTaskName,bool isFilter)
        {
            try
            {
                var GetMainTasks = new List<GetMainTaskBO>();
                TasksListDAL objDal = new TasksListDAL();

                var result = objDal.GetFilterMainTaskList(MainTaskName, isFilter);
                if(result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                        if (result[i].ProjectIDs == null)
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = "-1",
                                ProjectType = result[i].ProjectType

                            });
                        }
                        else
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = result[i].ProjectIDs,
                                ProjectType = result[i].ProjectType

                            });
                        }
                }
               
               // GetMainTasks = GetMainTasks.OrderBy(x => x.MainTaskName).ToList();
                return GetMainTasks;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<GetMainTaskBO> GetMainTaskMappedListBL(int ProjectID,int UserID)
        {
            try
            {
                var GetMainTasks = new List<GetMainTaskBO>();
                TasksListDAL objDal = new TasksListDAL();

                var result = objDal.GetMainTaskMappedListDAL(ProjectID, UserID);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                        if (result[i].ProjectID == null)
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = (int)result[i].MainTaskID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = "-1",
                                ProjectType = result[i].ProjectType

                            });
                        }
                        else
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = (int)result[i].MainTaskID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = result[i].ProjectID.ToString(),
                                ProjectType = result[i].ProjectType

                            });
                        }
                }

                // GetMainTasks = GetMainTasks.OrderBy(x => x.MainTaskName).ToList();
                return GetMainTasks;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<GetMainTaskBO> GetMainTaskMappedListBLbyDepartment(int ProjectID, int DepartmentID)
        {
            try
            {
                var GetMainTasks = new List<GetMainTaskBO>();
                TasksListDAL objDal = new TasksListDAL();
                if (ProjectID > 0 && DepartmentID == 0) 
                {
                    var result = objDal.GetMainTaskMappedListDALByDepartmentID(ProjectID, DepartmentID);
                    if (result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                            if (result[i].ProjectID == null)
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = (int)result[i].MainTaskID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = "-1",
                                    ProjectType = result[i].ProjectType

                                });
                            }
                            else
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = (int)result[i].MainTaskID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = result[i].ProjectID.ToString(),
                                    ProjectType = result[i].ProjectType

                                });
                            }
                    }
                }
                if (ProjectID == 0 && DepartmentID > 0)
                {
                    var result = objDal.GetMainTasByDepartmentID(DepartmentID);
                    if (result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = result[i].ID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = "-1",
                                    ProjectType = result[i].ProjectType

                                });
                    }
                }

                if (ProjectID > 0 && DepartmentID > 0)
                {
                    var result = objDal.sp_GetMappedMainTaskByProjectID_DepartmentID(ProjectID, DepartmentID);
                    if (result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                            if (result[i].ProjectID == null)
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = (int)result[i].MainTaskID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = "-1",
                                    ProjectType = result[i].ProjectType

                                });
                            }
                            else
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = (int)result[i].MainTaskID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = result[i].ProjectID.ToString(),
                                    ProjectType = result[i].ProjectType

                                });
                            }
                    }
                }
                // GetMainTasks = GetMainTasks.OrderBy(x => x.MainTaskName).ToList();
                return GetMainTasks;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<MainTaskBO> GetMainTaskList(string MainTaskName)
        {
            try
            {
                var MainTasks = new List<MainTaskBO>();
                TasksListDAL objDal = new TasksListDAL();
                // setting false because default value is false in SP
                var result = objDal.GetFilterMainTaskList(MainTaskName, false);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        MainTasks.Add(new MainTaskBO
                        {
                            Id = result[i].ID,
                            MainTaskName = result[i].MainTaskName,
                            Active = Convert.ToBoolean(result[i].IsActive)
                        });
                    }
                    return MainTasks;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public  List<GetMainTaskBO> GetMainTaskLazyLoadingBL(int page, int recsPerPage)
        {
            try
            {

                List<GetMainTaskBO> GetMainTasks = new List<GetMainTaskBO>();
                MainTasksDAL objDAL = new MainTasksDAL();
                var result = objDAL.GetMainTasksLazyLoading(page, recsPerPage);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].ProjectIDs == null)
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = "-1",
                                ProjectType = result[i].ProjectType

                            });
                        }
                        else
                        {
                            GetMainTasks.Add(new GetMainTaskBO
                            {
                                Id = result[i].ID,
                                MainTaskName = result[i].MainTaskName,
                                Active = Convert.ToBoolean(result[i].IsActive),
                                CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                ProjectIDs = result[i].ProjectIDs,
                                ProjectType = result[i].ProjectType

                            });
                        }
                    }
                    return GetMainTasks;
                }   
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool InsertMainTask(MainTaskBO task)
        {
            try
            {
                if(task != null)
                {
                    bool Param = false;
                    if (task.Case == "add")
                    {
                        Param = false;
                    }
                    else
                    {
                        Param = true;
                    }
                    TasksListDAL objDal = new TasksListDAL();
                    return objDal.InsertMainTask(task, Param);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<GetMainTaskBO> SearchMainTaskBL(string MainTaskName)
        {

            try
            {
                if (MainTaskName != null)
                {
                    List<GetMainTaskBO> GetMainTasks = new List<GetMainTaskBO>();
                    //var GetMainTasks = new List<GetMainTaskBO>();
                    MainTasksDAL objDal = new MainTasksDAL();
                    var result = objDal.SearchMainTaskDAL(MainTaskName);

                    if (result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].ProjectIDs == null)
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = result[i].ID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = "-1",
                                    ProjectType = result[i].ProjectType

                                });
                            }
                            else
                            {
                                GetMainTasks.Add(new GetMainTaskBO
                                {
                                    Id = result[i].ID,
                                    MainTaskName = result[i].MainTaskName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    CreationDate = Convert.ToDateTime(result[i].CreationDate),
                                    ProjectIDs = result[i].ProjectIDs,
                                    ProjectType = result[i].ProjectType

                                });
                            }
                        }
                        return GetMainTasks;
                    }
                }
                return null;
            }
            catch { return null; }
        }



        public List<GetClientBO> GetClientLazyLoadingBL(int page, int recsPerPage)
        {
            try
            {

                List<GetClientBO> GetClients = new List<GetClientBO>();
                MainTasksDAL objDAL = new MainTasksDAL();
                var result = objDAL.GetClientLazyLoading(page, recsPerPage);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].ID>0)
                        {
                            GetClients.Add(new GetClientBO
                            {
                                ID = result[i].ID,
                                ClientName = result[i].ClientName,
                                Active = Convert.ToBoolean(result[i].isActive),
                                ProjectIDs = "-1"
                            });
                        }
                        else
                        {
                            GetClients.Add(new GetClientBO
                            {
                                ID = result[i].ID,
                                ClientName = result[i].ClientName,
                                Active = Convert.ToBoolean(result[i].isActive),
                                ProjectIDs = "-1"

                            });
                        }
                    }
                    return GetClients;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<GetClientBO> SearchClientBL(string MainTaskName)
        {

            try
            {
                if (MainTaskName != null)
                {
                    List<GetClientBO> GetMainTasks = new List<GetClientBO>();
                    //var GetMainTasks = new List<GetMainTaskBO>();
                    MainTasksDAL objDal = new MainTasksDAL();
                    var result = objDal.SearchClientDAL(MainTaskName);

                    if (result != null)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result[i].ID >0)
                            {
                                GetMainTasks.Add(new GetClientBO
                                {
                                    ID = result[i].ID,
                                    ClientName = result[i].ClientName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    ProjectIDs = "-1"

                                });
                            }
                            else
                            {
                                GetMainTasks.Add(new GetClientBO
                                {
                                    ID = result[i].ID,
                                    ClientName = result[i].ClientName,
                                    Active = Convert.ToBoolean(result[i].IsActive),
                                    ProjectIDs = "-1"

                                });
                            }
                        }
                        return GetMainTasks;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        public bool CheckMainTask(CheckMainTaskMappedParms model)
        {
            bool IsSuceess = false;
            try
            {
                MainTasksDAL objDal = new MainTasksDAL();
                return objDal.CheckMainTaskID(model);
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
