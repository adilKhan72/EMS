using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFBO.Client
{
   public class ClientViewModel
    {
        public int ID { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string Website_URL { get; set; }
        public string Facebooklink { get; set; }
        public string Twitter { get; set; }
        public string instagramlink { get; set; }
        public bool Active { get; set; }
    }
}
