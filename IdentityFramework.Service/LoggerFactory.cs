using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace IdentityFramework.Service
{
    public class LoggerFactory : ILoggerFactory
    {
        public ILogger GetLogger(string fullyQualifiedClassName)
        {
            return new NLogLogger(fullyQualifiedClassName);
        }
    }
}
