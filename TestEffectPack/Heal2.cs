using ML_CrowdControl;
using MelonLoader;
using System.Net.Sockets;
using WarpWorld.CrowdControl;
using System.Reflection;
using System;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = 58,
        Name = "Heal P2"
    )]
    public class Heal2 : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P2Ready()) return EffectResult.Retry;

            if (Base.isMausoleum()) return EffectResult.Retry;

            try
            {
                int h = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health;
                if (h >= 3) return EffectResult.Retry;
                PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.SetHealth(h + 1);
                AudioManager.Play(Sfx.Player_Revive);
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }

}
