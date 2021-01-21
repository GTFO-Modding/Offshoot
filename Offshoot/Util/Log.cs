using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offshoot.Util
{
    public static class Log
    {
        public static void Debug(object msg)
        {
            OffshootMain.log.LogDebug(msg);
        }

        public static void Error(object msg)
        {
            OffshootMain.log.LogError(msg);
        } 
    }
}
