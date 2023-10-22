using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "ChargeUp",
        Name = "Charge Up",
        Description = "Increases the ex charge meter",
        Price = 50,
        Categories = new string[] { "Firing" },
        Morality = Morality.Good
    )]
    class ChargeUp : MLCC_Effect {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance) {
            if (!Base.isReady() || !Base.inLevel() || (!Base.P1Ready() && !Base.P2Ready())) return EffectResult.Retry;

            if (Base.isMausoleum()) return EffectResult.Retry;

            bool go = false;

            try {
                if (Base.P1Ready()) {
                    if (PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.SuperMeter <= 40.0F) {

                            PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.AddEx();

                        go = true;
                    }
                }

                if (Base.P2Ready()) {
                    if (PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.SuperMeter <= 40.0F) {
                       
                            PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.AddEx();
                        

                        go = true;
                    }
                }
            }
            catch (Exception e) {
                return EffectResult.Retry;
            }

            if (go) AudioManager.Play("player_parry_power_up");
            else return EffectResult.Retry;

            return EffectResult.Success;
        }
    }
}
