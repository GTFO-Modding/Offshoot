using ChainedPuzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Offshoot.Patches;

namespace Offshoot.Managers
{
    class InvoidableManager : MonoBehaviour
    {
        public InvoidableManager(IntPtr intPtr) : base(intPtr)
        {
        }

        CP_PlayerScanner scanner;
        CP_Bioscan_Graphics invGx;
        Color invColor = new Color(1, 1, 1, 1);
        Color offColor = new Color(0, 0, 0, 0);

        public void Awake()
        {
            scanner = GetComponent<CP_PlayerScanner>();
            invGx = GetComponent<CP_Bioscan_Graphics>();
            invGx.m_colors = new ColorModeColor[] {
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Active, col = offColor },
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Waiting, col = offColor },
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.TimedOut, col = offColor }
            };
            invGx.m_currentCol = offColor;

            Patch_StateChange.OnEnd += TriggerEnd;
        }

        void OnDestroy()
        {
            Patch_StateChange.OnEnd -= TriggerEnd;
        }

        private void TriggerEnd()
        {
            scanner = GetComponent<CP_PlayerScanner>();
            var core = GetComponent<CP_Bioscan_Core>();
            invGx = GetComponent<CP_Bioscan_Graphics>();
            //Main.log.LogDebug(scanner.m_scanActive);
            scanner.m_scanRadiusSqr = 2.5f * 2.5f;
            scanner.m_scanSpeeds = new float[] { 0.4f, 0.4f, 0.4f, 0.4f };
            invGx.m_colors = new ColorModeColor[] {
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Active, col = invColor },
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.Waiting, col = invColor },
                new ColorModeColor() { mode = eChainedPuzzleGraphicsColorMode.TimedOut, col = invColor }
            };
            invGx.m_currentCol = invColor;
        }
    }
}
