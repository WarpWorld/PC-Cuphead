using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
    ID = 39,
    Name = "Equip Roundabout"
    )]
    class WeapRound : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.ultra || Base.noFire) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;//switch weapon
                var m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                m.Invoke(levelPlayerController.weaponManager, new object[] { Weapon.level_weapon_boomerang });

                levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;//switch weapon
                if (levelPlayerController)
                {
                    m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(levelPlayerController.weaponManager, new object[] { Weapon.level_weapon_boomerang });
                }
            }
            catch (Exception e)
            {
                return EffectResult.Retry;
            }

            return EffectResult.Success;
        }
    }
}
