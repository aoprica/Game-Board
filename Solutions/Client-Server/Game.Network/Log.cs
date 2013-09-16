using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Game
{
    public static class Log
    {
        private readonly static log4net.ILog logger = log4net.LogManager.GetLogger("default");
        public static void Info(string str)
        {
            logger.Info(str);
        }
        public static void Error(string str)
        {
            logger.Error(str);
        }
        public static void Error(Exception ex)
        {
            logger.Error(string.Empty, ex);
        }
    }
}
