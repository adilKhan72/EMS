using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMSAPI.Utility.Nlog
{
    public class MyLogger : ILogger
    {
        //Singleton Pattern. Only Instancetiate one time

        private static MyLogger instance;
        private static Logger logger; //hold the single instance of nLog logger

        private MyLogger() // default constructor 
        {

        }

        public static MyLogger GetInstance() //check and create only instance of class
        {
            if(instance == null)
            {
                instance = new MyLogger();
            }
            return instance;
        }

        private Logger GetLogger(string theLogger) //check and create only instance of nLog
        {
            if (MyLogger.logger == null)
            {
                MyLogger.logger = LogManager.GetLogger(theLogger);
            }
            return MyLogger.logger;
        }

        public void Debug(string message, string arg = null)
        {
          if(arg == null)
            {
                GetLogger("EmsApiLoggerRules").Debug(message);
            }
            else
            {
                GetLogger("EmsApiLoggerRules").Debug(message, arg);
            }
        }

        public void Error(string message, string arg = null)
        {
            if (arg == null)
            {
                GetLogger("EmsApiLoggerRules").Error(message);
            }
            else
            {
                GetLogger("EmsApiLoggerRules").Error(message, arg);
            }
        }

        public void Info(string message, string arg = null)
        {
            if (arg == null)
            {
                GetLogger("EmsApiLoggerRules").Info(message);
            }
            else
            {
                GetLogger("EmsApiLoggerRules").Info(message, arg);
            }
        }

        public void Warning(string message, string arg = null)
        {
            if (arg == null)
            {
                GetLogger("EmsApiLoggerRules").Warn(message);
            }
            else
            {
                GetLogger("EmsApiLoggerRules").Warn(message, arg);
            }
        }
    }
}