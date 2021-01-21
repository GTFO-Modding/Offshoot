using ChainedPuzzles;
using GameData;
using HarmonyLib;
using Offshoot.Managers;
using System;
using UnityEngine;
using Offshoot.Util;

namespace Offshoot.Patches
{
    [HarmonyPatch(typeof(ChainedPuzzleInstance), "Setup")]
    public class Patch_PuzzleInstance
    {
        public static void Postfix(ChainedPuzzleInstance __instance, ChainedPuzzleDataBlock data)
        {
            foreach (var item in data.ChainedPuzzle)
            {
                if (item.PuzzleType == 999)
                {
                    __instance.gameObject.AddComponent<LightManager>();
                }
            }
        }
    }

    [HarmonyPatch(typeof(ChainedPuzzleInstance), "OnStateChange")]
    public class Patch_StateChange
    {
        public static void Postfix(pChainedPuzzleState newState, ChainedPuzzleInstance __instance)
        {
            OnInteract?.Invoke(newState, __instance.GetInstanceID());
        }

        public static void TriggerEnd()
        {
            OnEnd?.Invoke();
        }

        public static event Action<pChainedPuzzleState, int> OnInteract;
        public static event Action OnEnd;
    }

    [HarmonyPatch(typeof(ChainedPuzzleManager), "OnAssetsLoaded")]
    public class Patch_Puzzles
    {
        public static void Postfix(ChainedPuzzleManager __instance)
        {
            //ChainedPuzzleTypeDataBlock[] allBlocks = GameDataBlockBase<ChainedPuzzleTypeDataBlock>.GetAllBlocks();
            //This needs to be cleaned up

            var invoidable = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[8]);

            var invoidableEndTrigger = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[8]);

            //System Error
            var sysEr1 = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[9]);
            var sysEr1Core = sysEr1.GetComponent<CP_Cluster_Core>();

            var sysEr1Prefab = UnityEngine.Object.Instantiate(sysEr1Core.m_childPuzzlePrefab);
            var sysEr1PrefabCore = sysEr1Prefab.GetComponent<CP_PlayerScanner>();
            sysEr1PrefabCore.m_requireAllPlayers = true;
            sysEr1PrefabCore.m_scanProgression = 2.0f;
            sysEr1Core.m_childPuzzlePrefab = sysEr1Prefab;

            //Sys er 2
            var sysEr2 = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[6]);
            var sysEr2Scan = sysEr2.GetComponent<CP_PlayerScanner>();
            sysEr2Scan.m_requireAllPlayers = false;

            //Sys Er 3
            var sysEr3 = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[4]); // Cluster
            var sysEr4 = UnityEngine.Object.Instantiate(__instance.m_puzzleComponentPrefabs[5]); // Cluster
            var sysEr3Core = sysEr3.GetComponent<CP_Cluster_Core>();
            var sysEr4Core = sysEr4.GetComponent<CP_Cluster_Core>();
            var sysEr3Prefab = UnityEngine.Object.Instantiate(sysEr3Core.m_childPuzzlePrefab);

            //Sys Er 4
            var sysEr4Prefab = UnityEngine.Object.Instantiate(sysEr4Core.m_childPuzzlePrefab);
            var sysEr3Scan = sysEr3Prefab.GetComponent<CP_PlayerScanner>();
            var sysEr4Scan = sysEr4Prefab.GetComponent<CP_PlayerScanner>();
            var er3Speed = sysEr3Scan.m_scanSpeeds;
            var er4Speed = sysEr4Scan.m_scanSpeeds;
            sysEr4Scan.m_scanSpeeds = er3Speed;
            sysEr3Scan.m_scanSpeeds = er4Speed;


            sysEr3Core.m_childPuzzlePrefab = sysEr3Prefab;
            sysEr4Core.m_childPuzzlePrefab = sysEr4Prefab;

            //Setup INVOIDABLE Alarm
            invoidable.AddComponent<InvoidableManager>();
            CP_PlayerScanner ivScan = invoidable.GetComponent<CP_PlayerScanner>();
            CP_Bioscan_Graphics invGx = invoidable.GetComponent<CP_Bioscan_Graphics>();
            Color invColor = new Color(0, 0, 0, 0);
            ivScan.m_scanSpeeds = new float[] { 0, 0, 0, 0 };
            ivScan.m_scanRadius = 0;
            ivScan.m_requireAllPlayers = false;

            invGx.m_colors = new ColorModeColor[] {
                    new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Active, col = invColor },
                    new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Waiting, col = invColor },
                    new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.TimedOut, col = invColor }
                };


            //Setup INV Alarm ender
            invoidableEndTrigger.AddComponent<InvoidableEndTrigger>();
            invoidable.transform.position = new Vector3(1000, 1000, 1000);
            invoidableEndTrigger.transform.position = new Vector3(1000, 1000, 1000);

            sysEr1.transform.position = new Vector3(1000, 1000, 1000);
            sysEr2.transform.position = new Vector3(1000, 1000, 1000);
            sysEr3.transform.position = new Vector3(1000, 1000, 1000);
            sysEr4.transform.position = new Vector3(1000, 1000, 1000);

            sysEr1Prefab.transform.position = new Vector3(1000, 1000, 1000);
            sysEr3Prefab.transform.position = new Vector3(1000, 1000, 1000);
            sysEr4Prefab.transform.position = new Vector3(1000, 1000, 1000);



            __instance.m_puzzleComponentPrefabs.Add(402, sysEr1);
            __instance.m_puzzleComponentPrefabs.Add(403, sysEr2);
            __instance.m_puzzleComponentPrefabs.Add(404, sysEr3);
            __instance.m_puzzleComponentPrefabs.Add(405, sysEr4);
            __instance.m_puzzleComponentPrefabs.Add(999, invoidable);
            __instance.m_puzzleComponentPrefabs.Add(998, invoidableEndTrigger);
        }
    }
}
