using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFCommon.ApiResponseResult
{
    public static class ResponseResultMsg
    {
        static ResponseResultMsg()
        {
            SavedResultMsg = "Saved Successfully";
            ErrorMsg = "Error Occured";
        }

        public static string SavedResultMsg { get; set; }
        public static string ErrorMsg { get; set; }
    }
}
