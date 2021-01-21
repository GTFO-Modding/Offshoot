using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace Offshoot.Patches
{
    [HarmonyPatch(typeof(PUI_LocalPlayerStatus), "StartHealthWarning")]
    public static class Patch_LowHealth
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
