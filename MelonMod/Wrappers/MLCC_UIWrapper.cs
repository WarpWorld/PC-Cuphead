#if ML_Il2Cpp
using System;
#endif
using UnityEngine;
#pragma warning disable 1591

namespace ML_CrowdControl.Wrappers
{
    /// <summary>Unity Component Wrapper for MLCC_UI</summary>
    public class MLCC_UIWrapper : MonoBehaviour
    {
        internal MLCC_UI ui;

#if ML_Il2Cpp
        public MLCC_UIWrapper(IntPtr value) : base(value) { }
#endif

        public void Update()
            => ui?.OnUpdate();
        public void OnGUI()
            => ui?.OnGUI();
    }
}
