using ChainedPuzzles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Offshoot.Patches;

namespace Offshoot
{
    class InvoidableEndTrigger : MonoBehaviour
    {
        public InvoidableEndTrigger(IntPtr intPtr) : base(intPtr)
        {
        }

        CP_Bioscan_Core core;

        void Awake()
        {
            OffshootMain.log.LogDebug("Triggered");
            core = GetComponent<CP_Bioscan_Core>();
            core.add_OnPuzzleDone((Action<int>)((number) => {
                Patch_StateChange.TriggerEnd();
            }));
        }
    }
}
