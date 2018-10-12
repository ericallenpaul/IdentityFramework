using System;
using System.Collections.Generic;
using System.Text;
using NLog;

namespace IdentityFramework.Service
{
    public interface INLogLogger
    {
    }

    public class NLogLogger : ILogger, INLogLogger
    {
        NLog.Logger mylogger;
        public NLogLogger(string fullyQualifiedClassName)
        {
            mylogger = NLog.LogManager.GetLogger(fullyQualifiedClassName);
        }
    }
}
