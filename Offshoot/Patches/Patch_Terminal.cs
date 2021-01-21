using LevelGeneration;
using HarmonyLib;
using Offshoot.Util;
using System;

namespace Offshoot.Patches
{
    [HarmonyPatch(typeof(TerminalUplinkPuzzle), "Setup")]
    public static class Patch_Terminal
    {
        public static bool Prefix(LG_ComputerTerminal terminal, TerminalUplinkPuzzle __instance)
        {
			__instance.m_rounds = new Il2CppSystem.Collections.Generic.List<TerminalUplinkPuzzleRound>();
			__instance.TerminalUplinkIP = SerialGenerator.GetIpAddress();
			__instance.m_roundIndex = 0;
			__instance.m_lastRoundIndexToUpdateGui = -1;
			__instance.m_position = terminal.transform.position;
			int num = RandomUtil.TerminalRound.GetRound();
			for (int i = 0; i < num; i++)
			{
				int num2 = 6;
				TerminalUplinkPuzzleRound terminalUplinkPuzzleRound = new TerminalUplinkPuzzleRound();
				terminalUplinkPuzzleRound.CorrectIndex = Builder.SessionSeedRandom.Range(0, num2, "NO_TAG");
				terminalUplinkPuzzleRound.Prefixes = new string[num2];
				terminalUplinkPuzzleRound.Codes = new string[num2];
				for (int j = 0; j < num2; j++)
				{
					terminalUplinkPuzzleRound.Codes[j] = RandomUtil.GetPassword(5);
					terminalUplinkPuzzleRound.Prefixes[j] = SerialGenerator.GetCodeWordPrefix();
				}
				__instance.m_rounds.Add(terminalUplinkPuzzleRound);
			}


			Log.Debug($"Terminal {terminal.PublicName} has {__instance.m_rounds.Count} rounds");
			return false;
        }
    }

	[HarmonyPatch(typeof(LG_ComputerTerminalCommandInterpreter), "TerminalUplinkVerify")]
	public static class Patch_LG_CommandInterp
    {
		public static bool Prefix(LG_ComputerTerminalCommandInterpreter __instance, string param1, string param2, ref bool __result)
        {
			if (__instance.m_terminal.UplinkPuzzle.Connected)
			{
				__instance.AddOutput(TerminalLineType.SpinningWaitNoDone, "Attempting uplink verification ", 5f);
				if (__instance.m_terminal.UplinkPuzzle.CurrentRound.CorrectCode.ToUpper() == param1.ToUpper())
				{
					__instance.AddOutput("Verfication code ", true);

					__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
					__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
					__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
					__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
					__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);


					__instance.AddOutput("correct!", true);
					if (__instance.m_terminal.UplinkPuzzle.TryGoToNextRound())
					{
						__instance.AddOutput(TerminalLineType.ProgressWait, "Building uplink verification signature", 12f);
						__instance.AddOutput("", true);
						__instance.AddOutput("Verfication code", true);

						__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
						__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
						__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);
						__instance.AddOutput(TerminalLineType.Warning, StringTools.RandomChars(true, 20, 40), 0.8f);

						__instance.AddOutput(string.Concat(new string[]
						{
							"needed! Use public key <color=orange>",
							__instance.m_terminal.UplinkPuzzle.CurrentRound.CorrectPrefix,
							"</color> to obtain the code."
						}), true);


						__instance.OnEndOfQueue = new Action(() => {
							__instance.m_terminal.UplinkPuzzle.CurrentRound.ShowGui = true;
						});
					}
					else
					{
						__instance.AddOutput(TerminalLineType.SpinningWaitNoDone, "Waiting for uplink host response..", 3f);
						__instance.AddOutput("", true);
						__instance.AddOutput(TerminalLineType.Normal, "SUCCESS! Uplink to " + __instance.m_terminal.UplinkPuzzle.TerminalUplinkIP + " Established Successfully!", 2f);
						__instance.AddOutput("", true);
						__instance.AddOutput(TerminalLineType.Warning, "<color=red>An unknown system error occured!</color>", 0.8f);
						__instance.AddOutput(TerminalLineType.Normal, "Please contact your network administrator.");

						__instance.OnEndOfQueue = new Action(() =>
						{
							//WardenObjectiveManager.StopAllWardenObjectiveEnemyWaves();
							__instance.m_terminal.UplinkPuzzle.Solved = true;
							Il2CppSystem.Action onPuzzleSolved = __instance.m_terminal.UplinkPuzzle.OnPuzzleSolved;
							if (onPuzzleSolved == null)
							{
								return;
							}
							onPuzzleSolved.Invoke();
						});
					}
				}
				else
				{
					__instance.AddOutput("", true);
					__instance.AddOutput(TerminalLineType.Fail, "Verfication failed! Use public key <color=orange>" + __instance.m_terminal.UplinkPuzzle.CurrentRound.CorrectPrefix + "</color> to obtain the code", 0f);
					__instance.AddOutput(TerminalLineType.Normal, "Returning to root..", 6f);
				}
			}
			else
			{
				__instance.AddOutput("", true);
				__instance.AddOutput("Uplink not connected! Use UPLINK_CONNECT to initialize a connection.", true);
			}
			__result = false;
			return false;
        }
    }
}
