using System;
using System.Collections.Generic;
using DemoEFBL.Login;

namespace DemoEFUI
{
    class Program
    {
        static void Main(string[] args)
        {
            LoginBL objDBCall = new LoginBL();
            var result = objDBCall.LoginUser("faisal_hameed", "faisalhameed");
            if(result != null)
            {

            }
        }
    }
}
