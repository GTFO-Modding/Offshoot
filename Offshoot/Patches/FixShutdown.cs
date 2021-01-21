using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AK;
using HarmonyLib;
using LevelGeneration;
using Offshoot;

namespace Offshoot.Patches
{
    [HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "ReceiveCommand")]
    public class FixShutdown
    {
        public static void Postfix(TERM_Command cmd, string inputLine, string param1, string param2, LG_ComputerTerminalCommandInterpreter __instance)
        {
            OffshootMain.log.LogDebug("TERMINAL COMMAND: " + cmd);
            if (cmd == TERM_Command.ReactorShutdown)
            {
                OffshootMain.log.LogDebug("Triggered Shutdown");
                __instance.ReactorShutdown();
            }
        }
    }

    [HarmonyPatch(typeof(LG_WardenObjective_Reactor), "AttemptInteract", typeof(pReactorInteraction))]
    public class FixShutdown2
    {
        public static bool Prefix(pReactorInteraction interaction, LG_WardenObjective_Reactor __instance)
        {
            var state = __instance.m_stateReplicator.State;

            if (interaction.type == eReactorInteraction.WaitForVerify_shutdown)
            {
                OffshootMain.log.LogDebug("It was verify shutdown!");
                state.status = eReactorStatus.Shutdown_complete;
                state.stateProgress = 0f;
                __instance.m_stateReplicator.InteractWithState(state, interaction);
                return false;
            }


            return true;
        }
    }

    [HarmonyPatch(typeof(LG_WardenObjective_Reactor), "OnStateChange")]
    public class FixShutdown3
    {
        public static void Postfix(pReactorState newState, pReactorState oldState, LG_WardenObjective_Reactor __instance)
        {
            if (newState.status == oldState.status) return;

            if (newState.status == eReactorStatus.Shutdown_intro)
            {
                __instance.m_sound.Post(EVENTS.LIGHTS_OFF_GLOBAL);
                __instance.m_sound.Post(EVENTS.REACTOR_SHUTDOWN);
                var noise = new NM_NoiseData() {
                    noiseMaker = null,
                    position = __instance.m_terminalAlign.position,
                    radiusMin = 7,
                    radiusMax = 100,
                    yScale = 1f,
                    node = __instance.SpawnNode,
                    type = NM_NoiseType.InstaDetect,
                    includeToNeightbourAreas = false,
                    raycastFirstNode = false
                };
                NoiseManager.MakeNoise(noise);
            }
        }
    }
}
