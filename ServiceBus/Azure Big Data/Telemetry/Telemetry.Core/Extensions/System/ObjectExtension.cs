using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace System
{
    public static class ObjectExtension
    {
        public static Logger GetLogger(this object obj)
        {
            return obj.GetType().GetLogger();
        }
    }
}
