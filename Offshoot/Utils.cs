using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offshoot.Util
{
    public class Utils
    {
        public static void UnlockDoor()
        {
            Log.Debug("Door Unlocked");
            Builder.Current.m_currentFloor.TryGetZoneByLocalIndex(LG_LayerType.MainLayer, GameData.eLocalZoneIndex.Zone_4, out LG_Zone lgZone);
            lgZone.m_sourceGate.SpawnedDoor.AttemptOpenCloseInteraction(true);
        }

        public static void ShowWardenMessage(string message, WMsgType wMsgType = WMsgType.Info, int time = 4)
        {
            GuiManager.PlayerLayer.m_wardenIntel.m_intelText.text = FormatText(message);
            GuiManager.PlayerLayer.m_wardenIntel.SetVisible(true, time);
        }


        public static void SetSubObjective(string message)
        {
        }

        public enum WMsgType
        {
            Info,
            New
        }

        private static string FormatText(string text)
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
