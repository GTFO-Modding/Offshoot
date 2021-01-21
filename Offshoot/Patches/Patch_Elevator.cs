using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FX_EffectSystem;
using HarmonyLib;
using UnityEngine;

namespace Offshoot.Patches
{
	[HarmonyPatch(typeof(Elevator_PointLightSpawner), "TurnON")]
	class Patch_ElevatorPointLight
    {
		public static void Prefix(Elevator_PointLightSpawner __instance)
        {
			__instance.enabled = true;
			if (!__instance.HasLight && FX_Manager.TryAllocateFXLight(out FX_PointLight light))
			{
				__instance.m_light = light;
				__instance.m_lightColor = new Color(1f, 0.5f, 0.5f);
				switch (UnityEngine.Random.Range(0, 3))
				{
					case 0:
						__instance.m_lightColor = new Color(1f, 0.2f, 0.15f);
						break;
					case 1:
						__instance.m_lightColor = new Color(0.9f, 0.2f, 0.3f);
						break;
					case 2:
						__instance.m_lightColor = new Color(0.8f, 0.1f, 0.2f);
						break;
				}
				__instance.m_intensity = UnityEngine.Random.Range(0.3f, 0.5f);
				__instance.m_light.m_isOn = true;
				__instance.m_light.m_intensity = __instance.m_intensity;
				__instance.m_light.SetColor(__instance.m_lightColor);
				__instance.m_light.SetRange(__instance.m_range);
				__instance.m_light.UpdateData();
				__instance.m_light.m_position = __instance.transform.position;
				__instance.m_light.UpdateTransform();
			}

		}
	}
}
