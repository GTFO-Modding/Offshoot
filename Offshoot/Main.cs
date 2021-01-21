using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnhollowerRuntimeLib;
using Offshoot.Managers;
using UnityEngine;
using AssetShards;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using BepInEx;
using ChainedPuzzles;
using Offshoot.Components;

namespace Offshoot
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class OffshootMain : BasePlugin
    {
        public const string
            MODNAME = "OFFSHOOT",
            AUTHOR = "Dak",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "2.0.0";

        public static ManualLogSource log;
        public override void Load()
        {
            log = Log;
            Log.LogMessage("Application Start");
            ClassInjector.RegisterTypeInIl2Cpp<LightManager>();
            ClassInjector.RegisterTypeInIl2Cpp<InvoidableManager>();
            ClassInjector.RegisterTypeInIl2Cpp<InvoidableEndTrigger>();
            ClassInjector.RegisterTypeInIl2Cpp<ElevatorLight>();

            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            AssetShardManager.add_OnEnemyAssetsLoaded((Action)(() =>
            {
                GameObject shooterProjectile = AssetShardManager.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Characters/Enemies/Abilities/ProjTargetingSmall.prefab", false);
                GameObject hybridProjectile = AssetShardManager.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Characters/Enemies/Abilities/ProjSemiTargetingQuick.prefab", false);

                AssetShardManager.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Characters/Enemies/Abilities/ProjTargetingSmall.prefab", false);
                ProjectileBase projectile = shooterProjectile.GetComponent<ProjectileBase>();
                ProjectileBase hybrid = hybridProjectile.GetComponent<ProjectileBase>();
                projectile.m_maxDamage = 1.0f;
                hybrid.m_maxDamage = 2f;
            }));
        }
    }
}
