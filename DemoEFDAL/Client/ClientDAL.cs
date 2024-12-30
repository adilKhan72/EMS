
using DemoEFBO.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFDAL.Client
{
    public class ClientDAL
    {
        public List<sp_GetClients_Result> GetClientList(ClientViewModel model)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_GetClients(null, false).OrderBy(g => g.ClientName).ToList();
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
        public bool InsertUpdateClient(ClientViewModel model)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.sp_AddUpdateClient(model.ID, model.ClientName, model.ContactNumber, model.Address, model.Website_URL, model.Facebooklink, model.Twitter, model.instagramlink, model.Active, model.ClientEmail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteClient(int ID)
        {
            try
            {
                using (var dbcontext = new EMSEntities())
                {
                    var result = dbcontext.ClientTables.Find(ID);
                    dbcontext.ClientTables.Remove(result);
                    dbcontext.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
