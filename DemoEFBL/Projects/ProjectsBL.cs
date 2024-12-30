using DemoEFBO.Projects;
using DemoEFDAL.CommonProjectsList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Projects
{
    public class ProjectsBL
    {
        public static List<ProjectsBO> GetProjects(string ProjectName)
        {
            try
            {
                // ***need exception handling in this function
                List<ProjectsBO> lst = new List<ProjectsBO>();
                ProjectsListDAL objDAL = new ProjectsListDAL();
                var result = objDAL.GetProjects(ProjectName);
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        var estimatedHours = float.Parse(result[i].EstimatedHours.ToString()).ToString("0.00");
                        float _ContractHours = 0;
                        float.TryParse(result[i].ContractHoursPerMonth.ToString(), out _ContractHours);

                        float _ContractJan = 0;
                        float _ContractFeb = 0;
                        float _ContractMar = 0;
                        float _ContractApr = 0;
                        float _ContractMay = 0;
                        float _ContractJun = 0;
                        float _ContractJul = 0;
                        float _ContractAug = 0;
                        float _ContractSep = 0;
                        float _ContractOct = 0;
                        float _ContractNov = 0;
                        float _ContractDec = 0;
                        if (result[i].ContractHoursInJanuary != null)
                        {
                            float.TryParse(result[i].ContractHoursInJanuary.ToString(), out _ContractJan);
                        }
                        if (result[i].ContractHoursInFebruary != null)
                        {
                            float.TryParse(result[i].ContractHoursInFebruary.ToString(), out _ContractFeb);
                        }
                        if (result[i].ContractHoursInMarch != null)
                        {
                            float.TryParse(result[i].ContractHoursInMarch.ToString(), out _ContractMar);
                        }
                        if (result[i].ContractHoursInApril != null)
                        {
                            float.TryParse(result[i].ContractHoursInApril.ToString(), out _ContractApr);
                        }
                        if (result[i].ContractHoursInMay != null)
                        {
                            float.TryParse(result[i].ContractHoursInMay.ToString(), out _ContractMay);
                        }
                        if (result[i].ContractHoursInJune != null)
                        {
                            float.TryParse(result[i].ContractHoursInJune.ToString(), out _ContractJun);
                        }
                        if (result[i].ContractHoursInJuly != null)
                        {
                            float.TryParse(result[i].ContractHoursInJuly.ToString(), out _ContractJul);
                        }
                        if (result[i].ContractHoursInAugust != null)
                        {
                            float.TryParse(result[i].ContractHoursInAugust.ToString(), out _ContractAug);
                        }
                        if (result[i].ContractHoursInSeptember != null)
                        {
                            float.TryParse(result[i].ContractHoursInSeptember.ToString(), out _ContractSep);
                        }
                        if (result[i].ContractHoursInOctober != null)
                        {
                            float.TryParse(result[i].ContractHoursInOctober.ToString(), out _ContractOct);
                        }
                        if (result[i].ContractHoursInNovember != null)
                        {
                            float.TryParse(result[i].ContractHoursInNovember.ToString(), out _ContractNov);
                        }
                        if (result[i].ContractHoursInDecember != null)
                        {
                            float.TryParse(result[i].ContractHoursInDecember.ToString(), out _ContractDec);
                        }

                        lst.Add(new ProjectsBO
                        {
                            ID = result[i].ID,
                            Name = result[i].Name,
                            Type = result[i].Type,
                            IsActive = Convert.ToBoolean(result[i].IsActive),
                            IsExternal = Convert.ToBoolean(result[i].IsExternal),
                            EstimatedHours = float.Parse(estimatedHours),
                            ContractHours = float.Parse(_ContractHours.ToString("0.00")),
                            JanContract = float.Parse(_ContractJan.ToString("0.00")),
                            FebContract = float.Parse(_ContractFeb.ToString("0.00")),
                            MarContract = float.Parse(_ContractMar.ToString("0.00")),
                            AprContract = float.Parse(_ContractApr.ToString("0.00")),
                            MayContract = float.Parse(_ContractMay.ToString("0.00")),
                            JunContract = float.Parse(_ContractJun.ToString("0.00")),
                            JulContract = float.Parse(_ContractJul.ToString("0.00")),
                            AugContract = float.Parse(_ContractAug.ToString("0.00")),
                            SepContract = float.Parse(_ContractSep.ToString("0.00")),
                            OctContract = float.Parse(_ContractOct.ToString("0.00")),
                            NovContract = float.Parse(_ContractNov.ToString("0.00")),
                            DecContract = float.Parse(_ContractDec.ToString("0.00")),
                            ProjectDescription = result[i].ProjectDescription,
                            ClientID = Convert.ToInt32(result[i].ClientID)
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

        public static List<ProjectsBO> GetProjectsLazyLoadingBL(int page, int recsPerPage)
        {
            try
            {
               
                List<ProjectsBO> lst = new List<ProjectsBO>();
                ProjectsListDAL objDAL = new ProjectsListDAL();
                var result = objDAL.GetProjectsLazyLoading(page, recsPerPage);
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        var estimatedHours = float.Parse(result[i].EstimatedHours.ToString()).ToString("0.00");
                        lst.Add(new ProjectsBO
                        {
                            ID = result[i].ID,
                            Name = result[i].Name,
                            Type = result[i].Type,
                            IsActive = Convert.ToBoolean(result[i].IsActive),
                            IsExternal = Convert.ToBoolean(result[i].IsExternal),
                            EstimatedHours = float.Parse(estimatedHours),
                            ProjectDescription = result[i].ProjectDescription
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

        public bool InsertProject(ProjectsBO project)
        {
            try
            {
                if(project != null)
                {
                    ProjectsListDAL objDal = new ProjectsListDAL();
                    return objDal.InsertProject(project);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static List<ProjectsBO> GetProjectsBudgetDetailBL(ProjectsBO project)
        {
            try
            {
                List<ProjectsBO> ProjectsBudgetDetail = new List<ProjectsBO>();
                ProjectsListDAL objDAL = new ProjectsListDAL();
                var result = objDAL.GetProjectsBudgetDetail(project);

                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        float _ContractHours = 0;
                        float.TryParse(result[i].ContractHoursPerMonth.ToString(), out _ContractHours);

                        float _ContractJan = 0;
                        float _ContractFeb = 0;
                        float _ContractMar = 0;
                        float _ContractApr = 0;
                        float _ContractMay = 0;
                        float _ContractJun = 0;
                        float _ContractJul = 0;
                        float _ContractAug = 0;
                        float _ContractSep = 0;
                        float _ContractOct = 0;
                        float _ContractNov = 0;
                        float _ContractDec = 0;
                        if (result[i].ContractHoursInJanuary != null)
                        {
                            float.TryParse(result[i].ContractHoursInJanuary.ToString(), out _ContractJan);
                        }
                        if (result[i].ContractHoursInFebruary != null)
                        {
                            float.TryParse(result[i].ContractHoursInFebruary.ToString(), out _ContractFeb);
                        }
                        if (result[i].ContractHoursInMarch != null)
                        {
                            float.TryParse(result[i].ContractHoursInMarch.ToString(), out _ContractMar);
                        }
                        if (result[i].ContractHoursInApril != null)
                        {
                            float.TryParse(result[i].ContractHoursInApril.ToString(), out _ContractApr);
                        }
                        if (result[i].ContractHoursInMay != null)
                        {
                            float.TryParse(result[i].ContractHoursInMay.ToString(), out _ContractMay);
                        }
                        if (result[i].ContractHoursInJune != null)
                        {
                            float.TryParse(result[i].ContractHoursInJune.ToString(), out _ContractJun);
                        }
                        if (result[i].ContractHoursInJuly != null)
                        {
                            float.TryParse(result[i].ContractHoursInJuly.ToString(), out _ContractJul);
                        }
                        if (result[i].ContractHoursInAugust != null)
                        {
                            float.TryParse(result[i].ContractHoursInAugust.ToString(), out _ContractAug);
                        }
                        if (result[i].ContractHoursInSeptember != null)
                        {
                            float.TryParse(result[i].ContractHoursInSeptember.ToString(), out _ContractSep);
                        }
                        if (result[i].ContractHoursInOctober != null)
                        {
                            float.TryParse(result[i].ContractHoursInOctober.ToString(), out _ContractOct);
                        }
                        if (result[i].ContractHoursInNovember != null)
                        {
                            float.TryParse(result[i].ContractHoursInNovember.ToString(), out _ContractNov);
                        }
                        if (result[i].ContractHoursInDecember != null)
                        {
                            float.TryParse(result[i].ContractHoursInDecember.ToString(), out _ContractDec);
                        }

                        ProjectsBudgetDetail.Add(new ProjectsBO
                        {
                            ID = Convert.ToInt32(result[i].ProjectID),
                            ProjectBudgetYear = result[i].ProjectBudgetYear,
                            ContractHours = float.Parse(_ContractHours.ToString("0.00")),
                            JanContract = float.Parse(_ContractJan.ToString("0.00")),
                            FebContract = float.Parse(_ContractFeb.ToString("0.00")),
                            MarContract = float.Parse(_ContractMar.ToString("0.00")),
                            AprContract = float.Parse(_ContractApr.ToString("0.00")),
                            MayContract = float.Parse(_ContractMay.ToString("0.00")),
                            JunContract = float.Parse(_ContractJun.ToString("0.00")),
                            JulContract = float.Parse(_ContractJul.ToString("0.00")),
                            AugContract = float.Parse(_ContractAug.ToString("0.00")),
                            SepContract = float.Parse(_ContractSep.ToString("0.00")),
                            OctContract = float.Parse(_ContractOct.ToString("0.00")),
                            NovContract = float.Parse(_ContractNov.ToString("0.00")),
                            DecContract = float.Parse(_ContractDec.ToString("0.00")),
                           
                        });
                    }
                    return ProjectsBudgetDetail;
                }
                return null;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteProject(int ID)
        {
            try
            {
                if(ID > 0)
                {
                    ProjectsListDAL objDal = new ProjectsListDAL();
                    return objDal.DeleteProject(ID);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<SearchProjectBO> SearchProjectBL(string ProjectName)
        {

            try
            {
                if ( ProjectName != null)
                {
                    List<SearchProjectBO> lstSearchProject = new List<SearchProjectBO>();
                    ProjectsListDAL objDAL = new ProjectsListDAL();
                    var objResult = objDAL.SearchProjctDAL(ProjectName);
                    if (objResult != null)
                    {
                        for (int i = 0; i < objResult.Count; i++)
                        {
                            lstSearchProject.Add(new SearchProjectBO
                            {
                                ProjectID = objResult[i].ID,
                                ProjectName = objResult[i].Name,
                                isActive = Convert.ToBoolean(objResult[i].IsActive)
                            });
                        }
                        return lstSearchProject;
                    }
                }
                return null;
            }
            catch { return null; }
        }




    }
}
