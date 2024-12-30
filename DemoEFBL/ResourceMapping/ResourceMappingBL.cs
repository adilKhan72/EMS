using DemoEFBO.Projects;
using DemoEFBO.ResourceMapping;
using DemoEFDAL.ResourceMappings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DemoEFBL.ResourceMapping
{
    public class ResourceMappingBL
    {
        public List<ResourceMappingBO> getResourceMapping(GetResourceMappingBO obj)
        {

            try
            {
                 List<ResourceMappingBO> ResourceMappingResponse = new List<ResourceMappingBO>();
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();

                var objResult = ObjDAL.ResourceMapping(obj.UserId);
                   if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                               {
                                    ResourceMappingResponse.Add(new ResourceMappingBO
                                    {
                                        ResourceMappingId = Convert.ToInt32(objResult[i].ID),
                                        ProjectId = Convert.ToInt32(objResult[i].ProjectID),
                                        ProjectNames =  objResult[i].ProjectName,
                                        UserId =  objResult[i].UserIDs.ToString(),
                                        IsActive = Convert.ToBoolean(objResult[i].IsActive),
                                    });
                                }
                                return ResourceMappingResponse;
                            }
                        return null;
            }
            catch(Exception e) { return null; }
        }
        public List<ResourceMappingBO> getProjectwithPercentage(int TaskOwnerID)
        {

            try
            {
                List<ResourceMappingBO> ResourceMappingResponse = new List<ResourceMappingBO>();
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();

                var objResult = ObjDAL.projectwithpercentage(TaskOwnerID);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        ResourceMappingResponse.Add(new ResourceMappingBO
                        {
                            ProjectId = Convert.ToInt32(objResult[i].ProjectID),
                            ProjectNames = objResult[i].Name,
                            IsActive = Convert.ToBoolean(objResult[i].Status),
                            percentage = Convert.ToDecimal(objResult[i].Percentage)
                        });
                    }
                    return ResourceMappingResponse;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }

        public List<ResourceMappingBO> FetchProjectUserBL(int ID)
        {

            try
            {
                List<ResourceMappingBO> ResourceMappingResponse = new List<ResourceMappingBO>();
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();

                var objResult = ObjDAL.FetchProjectUser(ID);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        ResourceMappingResponse.Add(new ResourceMappingBO
                        {
                            ResourceMappingId = Convert.ToInt32(objResult[i].ID),
                            ProjectId = Convert.ToInt32(objResult[i].ProjectID),
                            ProjectNames = objResult[i].ProjectName,
                            UserId = objResult[i].UserIDs.ToString(),
                            Name = objResult[i].FullName,
                            IsActive = Convert.ToBoolean(objResult[i].IsActive)
                        });
                    }
                    ResourceMappingResponse = ResourceMappingResponse.Where(x => x.IsActive == true).ToList();
                    return ResourceMappingResponse;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }
        public bool SaveProjectMappingBL(SaveProjectMapping Obj)
        {
            try
            {
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();
                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("ProjectID");
                dt.Columns.Add("UserID");
                if (Obj.UserID != null)
                {
                    for (int i = 0; i < Obj.UserID.Length; i++)
                    {
                        dt.Rows.Add(Obj.ProjectId, Obj.UserID[i]);
                    }
                }
                
                var objResult = ObjDAL.SaveProjectMappingDAL(Obj.ProjectId, dt);

                return true;

            }
            catch (Exception e) { return false; }
        }
        public bool SaveUserMappingBL(SaveUserMapping Obj)
        {
            try
            {
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();
                var objResult = ObjDAL.SaveUserMappingDAL(Obj);

                return true;

            }
            catch (Exception e) { return false; }
        }

        //public bool SaveUpdateMapping(SaveUpdateMappingBO Obj)
        //{

        //    try
        //    {

        //        ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();

        //        var objResult = ObjDAL.SaveUpdateMapping(Obj.ProjectId,Obj.ProjectNames,Obj.MapString);

        //        return true;

        //    }
        //    catch (Exception e) { return false; }
        //}
        public List<ResourceMaintaskMappingBO> FetchMappedProjectMaintaskBL(int ID)
        {

            try
            {
                List<ResourceMaintaskMappingBO> MainTaskMappingResponse = new List<ResourceMaintaskMappingBO>();
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();

                var objResult = ObjDAL.FetchMaintaskProject(ID);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        MainTaskMappingResponse.Add(new ResourceMaintaskMappingBO
                        {
                            ID = Convert.ToInt32(objResult[i].ID),
                            MainTaskID = Convert.ToInt32(objResult[i].MainTaskID),
                            MainTaskName = objResult[i].MainTaskName,
                            ProjectId = Convert.ToInt32(objResult[i].ProjectID),
                            Name = objResult[i].Name,
                            ProjectType = objResult[i].ProjectType,
                            IsActive = objResult[i].IsActive,
                            CreationDate = objResult[i].CreationDate,
                        });
                    }
                    return MainTaskMappingResponse;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }
        
        public bool SaveMainTaskMappingBL(SaveMainTaskMapping Obj)
        {
            try
            {
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();
                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("MainTaskID");
                dt.Columns.Add("ProjectID");
                if (Obj.ProjectId != null)
                {
                    for (int i = 0; i < Obj.ProjectId.Length; i++)
                    {
                        dt.Rows.Add(Obj.MainTaskID, Obj.ProjectId[i]);
                    }
                }
                
                var objResult = ObjDAL.SaveMainTaskMappingDAL(Obj.MainTaskID, dt);

                return true;

            }
            catch (Exception e) { return false; }
        }
        public List<ProjectsBO> FetchProject()
        {
            try
            {
                List<ProjectsBO> projects = new List<ProjectsBO>();
                ResourceMappingsDAL ObjDAL = new ResourceMappingsDAL();
                var objResult = ObjDAL.Fetchprojects();
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        projects.Add(new ProjectsBO
                        {
                            ID = Convert.ToInt32(objResult[i].ID),
                            Name = objResult[i].Name.ToString(),
                            ClientID = Convert.ToInt32(objResult[i].ClientID),

                        });
                    }
                }
                    return projects;

            }
            catch (Exception e) { return null; }
        }
    }
}
