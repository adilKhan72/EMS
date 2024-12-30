using DemoEFBO.Department;
using DemoEFBO.DepartmentMapping;
using DemoEFBO.ResourceMapping;
using DemoEFBO.Tasks;
using DemoEFDAL.Department;
using DemoEFDAL.Department_Mapping;
using DemoEFDAL.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoEFBL.DepartmentMapping
{
    public class DepartmentMappingBL
    {
        public List<DepartmentMappingBO> getDefaultMapping(bool CheckBoxStatus)
        {

            try
            {
                List<DepartmentMappingBO> list = new List<DepartmentMappingBO>();
                DepartmentMappingDAL ObjDAL = new DepartmentMappingDAL();
                var objResult = ObjDAL.DefaultMapping(CheckBoxStatus);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        list.Add(new DepartmentMappingBO
                        {
                            Id = Convert.ToInt32(objResult[i].ID),
                            Name = objResult[i].MainTaskName.ToString(),
                            IsActive = Convert.ToBoolean(objResult[i].IsActive)

                        });
                    }
                    return list;
                }
                return null;
            }
            catch (Exception e) { return null; }
        }
        public List<DepartmentModel> GetDepartmentLazyLoadingBL(int page, int recsPerPage)
        {
            try
            {

                List<DepartmentModel> modellist = new List<DepartmentModel>();
                DepartmentMappingDAL objDAL = new DepartmentMappingDAL();
                var result = objDAL.GetDepartmentLazyLoading(page, recsPerPage);
                if (result != null)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].ID > 0)
                        {
                            modellist.Add(new DepartmentModel
                            {
                                ID = result[i].ID,
                                DepartmentName = result[i].DepartmentName,
                                IsActive = Convert.ToBoolean(result[i].IsActive),
                            });
                        }
                        else
                        {
                            modellist.Add(new DepartmentModel
                            {
                                ID = result[i].ID,
                                DepartmentName = result[i].DepartmentName,
                                IsActive = Convert.ToBoolean(result[i].IsActive),

                            });
                        }
                    }
                    return modellist;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<MapMainTaskinDepartmentMapping> getMapMainTask(int DeptID)
        {

            try
            {
                List<MapMainTaskinDepartmentMapping> list = new List<MapMainTaskinDepartmentMapping>();
                DepartmentMappingDAL ObjDAL = new DepartmentMappingDAL();
                var objResult = ObjDAL.getMapMainTask(DeptID);
                if (objResult != null)
                {
                    for (int i = 0; i < objResult.Count; i++)
                    {
                        list.Add(new MapMainTaskinDepartmentMapping
                        {
                            MaintaskID = Convert.ToInt32(objResult[i].MaintaskID),
                            DepartID = Convert.ToInt32(objResult[i].DepartID),
                            DepartMappingID = Convert.ToInt32(objResult[i].DepartMappingID),
                            Name = objResult[i].MainTaskName.ToString(),
                            AdditionalCheck = Convert.ToBoolean(objResult[i].AdditionalCheck)
                        });
                    }
                    return list.OrderBy(o => o.Name).ToList();
                }
                return null;
            }
            catch (Exception e) { return null; }
        }
        public bool InsertDepartmentMaintaskMap(SaveDepartmentMapping model)
        {
            bool IsSuccess = false;
            try
            {
                DepartmentMappingDAL ObjDAL = new DepartmentMappingDAL();
                IsSuccess = ObjDAL.InsertDepartmentMap(model);
                return IsSuccess;
            }
            catch(Exception ex)
            {
                return IsSuccess;
            }
        }
    }
}
