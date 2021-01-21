using AK;
using ChainedPuzzles;
using LevelGeneration;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Offshoot.Patches;
using Offshoot.Util;

namespace Offshoot.Managers
{
    public class LightManager : MonoBehaviour
    {

        public LightManager(IntPtr intPtr) : base(intPtr)
        {
        }

        ChainedPuzzleInstance chainedPuzzleInstance;
        LG_LightCollection lightCollection;
        CellSoundPlayer sound;
        int id;
        private bool trigger = false;
        private showHelp HelpText;
        private enum showHelp
        {
            notActive,
            show
        }

        public void Awake()
        {
            chainedPuzzleInstance = GetComponentInChildren<ChainedPuzzleInstance>();
            id = chainedPuzzleInstance.GetInstanceID();
            sound = new CellSoundPlayer(new Vector3(transform.position.x, transform.position.y + 50, transform.position.z));
            //lightCollection = LG_LightCollection.Create(chainedPuzzleInstance.m_sourceArea.m_courseNode, new Vector3(0, 0, 0), LG_LightCollectionSorting.Distance, float.MaxValue);
            Patch_StateChange.OnInteract += onStateChange;
        }

        private void onStateChange(pChainedPuzzleState arg1, int arg2)
        {
            if (arg1.status == eChainedPuzzleStatus.Active && id == arg2)
            {
                DoorSequence();
              
            }
            if (arg1.status == eChainedPuzzleStatus.Solved && id == arg2)
            {
                Patch_StateChange.TriggerEnd();
                GuiManager.PlayerLayer.m_wardenIntel.ShowSubObjectiveMessage("", "<color=orange><size=200%>PRIORITY WARNING</size></color>\nLarge active bio-mass detected in <color=orange>ZONE_894</color>", false, null);
                try
                {
                    foreach (CollectedLight light in lightCollection.collectedLights)
                    {
                        light.light.ChangeIntensity(0.8f);
                        light.light.ChangeColor(new Color(1, 1, 1, 1));
                    }
                }
                catch { }
                sound.Post(EVENTS.LIGHTS_ON_INTENSITY_1);
            }
        }

        public void Update()
        {
            switch (HelpText)
            {
                case showHelp.show:
                    var wardenObj = WardenObjectiveManager.Current.m_activeWardenObjectives[LG_LayerType.MainLayer];
                    wardenObj.ShowHelpDelay = 0;
                    WardenObjectiveManager.Current.AttemptInteract(new pWardenObjectiveInteraction() { forceUpdate = true, inLayer = LG_LayerType.MainLayer, newSubObj = eWardenSubObjectiveStatus.GoToZoneHelp, type = eWardenObjectiveInteractionType.UpdateSubObjective });
                    WardenObjectiveManager.Current.UpdateFromInLevel();
                    HelpText = showHelp.notActive;
                    Utils.UnlockDoor();
                    break;
            }
        }

        private async Task DoorSequence()
        {
            if (trigger == true) return;
            trigger = true;
            Utils.ShowWardenMessage("msgCat::Obj.Info" + "\n>\n<size=200%><color=red>" + "Issue with security bypass, standby." + "</color></size>");

            await Task.Delay(1 * 1000);
            lightCollection = LG_LightCollection.Create(chainedPuzzleInstance.m_sourceArea.m_courseNode, new Vector3(0, 0, 0), LG_LightCollectionSorting.Distance, float.MaxValue);

            sound.Post(EVENTS.LIGHTS_OFF_GLOBAL);
            foreach (CollectedLight light in lightCollection.collectedLights)
            {
                light.light.ChangeIntensity(0);
                if (light.light.m_category == LG_Light.LightCategory.Emergency ||
                    light.light.m_category == LG_Light.LightCategory.Door ||
                    light.light.m_category == LG_Light.LightCategory.DoorImportant ||
                    light.light.m_category == LG_Light.LightCategory.Sign)
                {
                    light.light.ChangeColor(new Color(1, 0, 0, 1));
                }
            }

            await Task.Delay(4 * 1000);

            sound.Post(EVENTS.LIGHTS_ON_INTENSITY_1);
            LightsOnForCatagory(LG_Light.LightCategory.Emergency);
            //Main.log.LogDebug("1");
            await Task.Delay(1 * 1000);

            sound.Post(EVENTS.LIGHTS_ON_INTENSITY_2);
            LightsOnForCatagory(LG_Light.LightCategory.DoorImportant);
            //Main.log.LogDebug("2");
            await Task.Delay(1 * 1000);

            sound.Post(EVENTS.LIGHTS_ON_INTENSITY_3);
            LightsOnForCatagory(LG_Light.LightCategory.Door);
            //Main.log.LogDebug("3");
            await Task.Delay(1 * 1000);

            sound.Post(EVENTS.LIGHTS_ON_INTENSITY_4);
            LightsOnForCatagory(LG_Light.LightCategory.Sign);
            //Main.log.LogDebug("4");

            await Task.Delay(5 * 1000);
            HelpText = showHelp.show;
        }



        void OnDestroy()
        {
            Patch_StateChange.OnInteract -= onStateChange;
        }

        private void LightsOnForCatagory(LG_Light.LightCategory lightCategory)
        {

            foreach (var light in lightCollection.collectedLights)
            {
                if (light.light.m_category == lightCategory)
                {
                    light.light.ChangeIntensity(1);
                }
            }
        }

        private void CreateLightCollection()
        {
            if (lightCollection == null)
            {
                chainedPuzzleInstance = GetComponentInChildren<ChainedPuzzleInstance>();
                lightCollection = LG_LightCollection.Create(chainedPuzzleInstance.m_sourceArea.m_courseNode, new Vector3(0, 0, 0), LG_LightCollectionSorting.Distance, float.MaxValue);
            }
        }

        private void SetEmitterMesh(LG_Light light, Color col)
        {
            var lightMesh = light.m_emitterMesh;
            if (lightMesh.m_emitterMeshes != null)
            {
                for (int i = 0; i < lightMesh.m_emitterMeshes.Length; i++)
                {
                    if (lightMesh.m_emitterMeshes[i] != null)
                    {
                        lightMesh.m_emitterMeshes[i].material.SetColor("_EmissionColor", col);
                    }
                }
            }
        }

        private string FormatText(string text)
        {
            return string.Concat(new string[]
            {
                "<size=60%>//:Decoded warden transmission</size>\n\n",
                text,
                "\n>\n> MSGdec_STATUS::E_DONE\n> _Transmission checksum ",
                UnityEngine.Random.Range(99999, int.MaxValue).ToString(),
                "\n\n<size=60%>//:End of warden transmission</size>"
            });
        }

    }
}
