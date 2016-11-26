using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Telemetry.Core;

namespace System
{
    public static class TypeExtension
    {
        public static Logger GetLogger(this Type type)
        {
            LogSetup.Run();
            return LogManager.GetLogger(type.Name);
        }
    }
}
