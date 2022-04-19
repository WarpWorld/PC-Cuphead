using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarpWorld.CrowdControl
{
    /// <summary> Base effect for bid war effects. </summary>
    public abstract class CCEffectBidWar : CCEffectBase
    {
#if ML_Il2Cpp
        public CCEffectBidWar(IntPtr value) : base(value) { }
#endif
        /// <summary>Name of what the user is bidding towards.</summary>
#if !ML_Il2Cpp
        [SerializeField]
#endif
        protected string bidFor = "";
        /// <summary>How many coins will be bid towards <see cref="bidFor"/></summary>
#if !ML_Il2Cpp
        [SerializeField]
#endif
        protected uint cost = 0;
        private string formattedString;

        /// <summary>Size of the playload for this effect.</summary>
        public override ushort PayloadSize(string userName)
        {
            formattedString = String.Format("{0},{1}", bidFor, cost); 
            return Convert.ToUInt16(3 + 4 + 4 + 4 + userName.Length + 1 + formattedString.Length + 1);
        }

        /// <summary>All Parameters for this effect as a string.</summary>
        public override string Params()
        {
            return String.Format("{0},{1}", bidFor, cost);
        }
    }
}
