using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelGeneration;
using HarmonyLib;

namespace Offshoot.Util
{
    public static class RandomUtil
    {
        public static TerminalRoundProvider TerminalRound;

        public static string GetPassword(int length)
        {
            return PasswordProvider.GetPassword(length);
        }

        public static void Setup()
        {
            Log.Debug("Called Setup for random util");
            TerminalRound = new TerminalRoundProvider(3, new List<int>() { 2, 4, 6 });
        }
    }


    [HarmonyPatch(typeof(Builder), "Build")]
    public static class Patch_Builder
    {
        public static void Postfix()
        {
            RandomUtil.Setup();
        }
    }



    public static class PasswordProvider
    {
        private static readonly string validCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ.?!#";

        public static string GetPassword(int length)
        {
            var strBuild  = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                strBuild.Append(validCharacters[Builder.SessionSeedRandom.Range(0, validCharacters.Length-1)]);
            }

            return strBuild.ToString();
        }
    }

    public class TerminalRoundProvider
    {
        private readonly List<int> rounds;

        public TerminalRoundProvider(int terminalCount, List<int> possibleRounds)
        {
            if (possibleRounds.Count < terminalCount)
            {
                throw new IndexOutOfRangeException("Possible rounds must be greater than the terminal count!");
            }

            rounds = new List<int>();
            for (int i = 0; i < terminalCount; i++)
            {
                int index = Builder.SessionSeedRandom.Range(0, possibleRounds.Count);
                rounds.Add(possibleRounds[index]);
                possibleRounds.RemoveAt(index);
            }

            foreach (var item in rounds)
            {
                Log.Debug(item);
            }
        }

        public int GetRound()
        {
            var round = rounds[0];
            rounds.RemoveAt(0);
            return round;
        }
    }

}
