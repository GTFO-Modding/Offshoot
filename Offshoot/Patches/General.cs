using ChainedPuzzles;
using GameData;
using HarmonyLib;
using Offshoot.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Offshoot.Patches
{
    //[HarmonyPatch(typeof(LG_PowerGeneratorCluster), "Setup")]
    //public class Patch_Generator
    //{
    //    [HarmonyPostfix]
    //    public static void Prefix(LG_PowerGeneratorCluster __instance)
    //    {
    //        if (RundownManager.ActiveExpedition.Descriptive.PublicName == "HEADRUSH")
    //        {
    //            MelonLogger.Log("Patched PowerGen");
    //            __instance.m_fogDataSteps = new FogDataStep[] { new FogDataStep() { m_fogDataId = 301, m_transitionToTime = 4800 } };
    //        }
    //    }
    //}
}
