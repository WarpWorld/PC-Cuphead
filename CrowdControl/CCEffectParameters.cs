using System.Collections.Generic;
using System;
#if ML_Il2Cpp
using UnhollowerBaseLib.Attributes;
#else
using UnityEngine;
#endif

namespace WarpWorld.CrowdControl
{
    /// <summary>A Crowd Control effect that handles parameters. </summary>
    public abstract class CCEffectParameters : CCEffectBase
    {
#if ML_Il2Cpp
        public CCEffectParameters(IntPtr value) : base(value) { }
#else
        [Tooltip("Parameters this Effect has")]
        [SerializeField]
#endif
        private List<string> parameters = new List<string>();

        private string parametersAsString;

        /// <summary>Function for dynamically adding object(s) to the parameter list. </summary>
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void AddParameters(params object[] prms)
        {
            if (prms.Length == 0)
            {
                CrowdControl.LogError("You cannot pass in zero parameters!");
                return;
            }

            for (int i = 0; i < prms.Length; i++)
                parameters.Add(prms[i].ToString());
        }

        /// <summary>Clearing the established parameter list. </summary>
        public void ClearParameters()
        {
            parameters.Clear();
        }

        /// <summary>Size of the playload for this effect.</summary>
        public override ushort PayloadSize(string userName)
        {
            if (parameters.Count != 0)
                parametersAsString = string.Join(",", parameters.ToArray());

            return Convert.ToUInt16(3 + 4 + 4 + 4 + userName.Length + 1 + parametersAsString.Length + 1);
        }

        /// <summary>All Parameters for this effect as a string.</summary>
        public override string Params()
        {
            return string.Join(",", parameters.ToArray());
        }

        /// <summary>Used for processing the newly received parameter array. Can be overridden by a derived class.</summary>
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public virtual void AssignParameters(string [] prms)
        {

        }
    }
}
