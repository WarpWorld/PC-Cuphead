using WarpWorld.CrowdControl;
using System;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "Heal",
        Name = "Heal",
        Description = "Restores one health to the player",
        Price = 50,
        Categories = new string[] {"Player 1", "Heal" },
        Morality = Morality.Good
    )]
    public class Heal : MLCC_Effect {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance) {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.isMausoleum()) return EffectResult.Retry;

            try {
                int h = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;//heal
                if (h >= 3) return EffectResult.Retry;

                    PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.SetHealth(h + 1);
                    AudioManager.Play(Sfx.Player_Revive);

            }
            catch (Exception e) {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }

        public override void OnLoad() {
            Base.patch();
        }
    }
}
