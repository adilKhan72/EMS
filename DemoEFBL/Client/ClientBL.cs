using DemoEFBO.Client;
using DemoEFDAL.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBL.Client
{
   public class ClientBL
    {
        public static List<ClientViewModel> GetAllClient(ClientViewModel model)
        {
            try
            {

                List<ClientViewModel> lst = new List<ClientViewModel>();
                ClientDAL objDAL = new ClientDAL();
                var result = objDAL.GetClientList(model);
                if (result != null && result.Count > 0)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        lst.Add(new ClientViewModel
                        {
                            ID = result[i].ID,
                            ClientName = result[i].ClientName,
                            ClientEmail = result[i].Email,
                            ContactNumber = result[i].ContactNumber,
                            Address = result[i].Address,
                            Website_URL = result[i].Website_URL,
                            Facebooklink = result[i].Facebooklink,
                            Twitter = result[i].Twitter,
                            instagramlink = result[i].instagramlink,
                            Active = Convert.ToBoolean(result[i].isActive),
                        });
                    }
                    return lst;
                }
                if(result.Count() == 0)
                {
                    return lst;
                }
                return null;
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
                if (model != null)
                {
                    ClientDAL objDal = new ClientDAL();
                    return objDal.InsertUpdateClient(model);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteClient(ClientViewModel model)
        {
            try
            {
                if (model != null)
                {
                    ClientDAL objDal = new ClientDAL();
                    return objDal.DeleteClient(model.ID);
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
