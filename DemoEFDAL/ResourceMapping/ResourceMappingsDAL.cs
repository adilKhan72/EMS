using DemoEFBO.ResourceMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.ResourceMappings
{
   public class ResourceMappingsDAL
    {
        public List<sp_GetResourceMapping_Result> ResourceMapping(int ID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetResourceMapping(ID).ToList();

                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<sp_GetProjectWithPercentage_Result> projectwithpercentage(int ID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_GetProjectWithPercentage(ID).ToList();

                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<sp_FetchUserMappingProject_Result> FetchProjectUser(int ID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {

                    var result = dbcontext.sp_FetchUserMappingProject(ID).OrderBy(o=>o.FullName).ToList();

                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public bool SaveUpdateMapping(int ProjectID, string ProjectName, string MapString)
        //{
        //    try
        //    {
        //        using (var dbcontext = new EMSEntities())
        //        {

        //            var result = dbcontext.sp_SaveUpdateMapping(ProjectID, ProjectName, MapString);

        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public bool SaveProjectMappingDAL(int ProjectID , DataTable dt)
        {
            try
            {
                using (var context = new EMSEntities())
                {
                    var ConnString = context.Database.Connection.ConnectionString;
                    using (SqlConnection Connec = new SqlConnection(ConnString))
                    {
                        Connec.Open();
                        SqlCommand cmd = new SqlCommand("sp_SaveProjectMapping", Connec);
                        cmd.Parameters.Add("@projectID", SqlDbType.Int).Value = ProjectID;
                        cmd.Parameters.Add("@ProjectMappedUserID", SqlDbType.Structured).Value = dt;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        Connec.Close();
                    }
                }
                return true;
                //using (var dbcontext = new EMSEntities())
                //{
                //   var result = dbcontext.sp_SaveProjectMapping(ProjectID);
                //    return true;
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool SaveUserMappingDAL(SaveUserMapping model)
        {
            try
            {
                using (var context = new EMSEntities())
                {
                    var ConnString = context.Database.Connection.ConnectionString;
                    using (SqlConnection Connec = new SqlConnection(ConnString))
                    {
                        Connec.Open();
                        SqlCommand cmds = new SqlCommand("sp_UpdateClientIDNull", Connec);
                        cmds.Parameters.Add("@ClientID", SqlDbType.Int).Value = model.UserID;
                        cmds.CommandType = CommandType.StoredProcedure;
                        cmds.ExecuteNonQuery();
                        Connec.Close();
                        if (model.ProjectId != null)
                        {
                            if (model.ProjectId.Length > 0)
                            {
                                for (int i = 0; i < model.ProjectId.Length; i++)
                                {
                                    var asd = model.ProjectId[i];
                                    Connec.Open();
                                    SqlCommand cmd = new SqlCommand("sp_UpdateClientIDinProject", Connec);
                                    cmd.Parameters.Add("@ProjectID", SqlDbType.Int).Value = model.ProjectId[i];
                                    cmd.Parameters.Add("@ClientID", SqlDbType.Int).Value = model.UserID;
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.ExecuteNonQuery();
                                    Connec.Close();
                                }
                            }
                        }
                        
                    }
                }
                return true;
                //using (var dbcontext = new EMSEntities())
                //{
                //   var result = dbcontext.sp_SaveProjectMapping(ProjectID);
                //    return true;
                //}
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<sp_FetchProjectMappingMainTask_Result> FetchMaintaskProject(int ID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_FetchProjectMappingMainTask(ID).OrderBy(x=>x.Name).ToList();

                    if (result != null)
                    {
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool SaveMainTaskMappingDAL(int MainTaskId, DataTable dt)
        {
            try
            {
                using (var context = new EMSEntities())
                {
                    var ConnString = context.Database.Connection.ConnectionString;
                    using (SqlConnection Connec = new SqlConnection(ConnString))
                    {
                        Connec.Open();
                        SqlCommand cmd = new SqlCommand("sp_SaveMainTaskMapping", Connec);
                        cmd.Parameters.Add("@maintaskid", SqlDbType.Int).Value = MainTaskId;
                        cmd.Parameters.Add("@MainTaskMappedProjectID", SqlDbType.Structured).Value = dt;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                        Connec.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<sp_GetProjects_Result> Fetchprojects()
        {
            try
            {
                List<sp_GetProjects_Result> projectlist = new List<sp_GetProjects_Result>();
                using (var context = new EMSEntities())
                {
                    projectlist = context.sp_GetProjects(null).Where(x => x.IsActive == true).ToList();
                }
                return projectlist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
