using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoEFCommon.FileUploadResponse
{
    public class FileUploadResponse
    {
        public bool isSuccess { get; set; }
        public string FileName { get; set; }
        public string ResponseMsg { get; set; }
        public string Error { get; set; }
    }
}
