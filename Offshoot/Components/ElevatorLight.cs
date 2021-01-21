using FX_EffectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Offshoot.Components
{
    public class ElevatorLight : MonoBehaviour
    {
        public ElevatorLight(IntPtr intPtr) : base(intPtr)
        {
        }

		private bool HasLight = false;

		private FX_PointLight light;

		void Awake()
        {
			HasLight = FX_Manager.TryAllocateFXLight(out light);

			if (HasLight)
            {
				light.SetColor(Color.white);
				light.SetRange(100);
				light.m_intensity = 0.1f;
				light.m_isOn = true;
				light.UpdateData();
				light.UpdateTransform();
			}
		}

		void OnDestroy()
        {
			if (HasLight)
			{
				FX_Manager.DeallocateFXLight(light);
				light = null;
			}
		}



		void Update()
		{
			if (!HasLight)
			{
				return;
			}
			light.m_position = transform.position;
			light.UpdateTransform();
		}

		private void LateUpdate()
		{
			if (!HasLight)
			{
				return;
			}
			light.m_position = transform.position;
			light.UpdateTransform();
		}
	}
}
