using System;
using WarpWorld.CrowdControl;

namespace ML_CrowdControl.Effects.Data
{
    /// <summary>Effect data attribute for bid war effects.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MLCC_BidWarEffectData : MLCC_EffectData
    {
        private CCEffectBidWar Effect;

        internal void ApplyData(CCEffectBidWar effect)
        {
            Effect = effect;
            ApplyData((CCEffectBase)Effect);

            Effect.SetBidFor(_bidFor);
            Effect.SetCost(_cost);
        }

        /// <summary>Name of what the user is bidding towards.</summary>
        public string BidFor
        {
            get => (Effect == null) ? _bidFor : _bidFor = Effect.GetBidFor();
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = string.Empty;
                
                _bidFor = value;
                if (Effect != null)
                    Effect.SetBidFor(_bidFor);
            }
        }
        private string _bidFor = string.Empty;

        /// <summary>
        /// How many coins will be bid towards.
        /// Must be a uint value greater than 0
        /// </summary>
        public uint Cost
        {
            get => (Effect == null) ? _cost : _cost = Effect.GetCost();
            set
            {
                if (value < 0)
                    throw new Exception("Cost must be set to a uint value greater than 0!");

                _cost = value;
                if (Effect != null)
                    Effect.SetCost(_cost);
            }
        }
        private uint _cost = 0;
    }
}
