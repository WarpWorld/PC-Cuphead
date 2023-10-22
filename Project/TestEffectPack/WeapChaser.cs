using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "EquipChaser",
        Name = "Equip Chaser",
        Description = "Swaps the player's weapon to the chaser",
        Price = 75,
        Categories = new string[] { "Weapons" }
    )]
    class WeapChaser : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.ultra || Base.noFire) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;//switch weapon
                var m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                m.Invoke(levelPlayerController.weaponManager, new object[] { Weapon.level_weapon_homing });

                levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;//switch weapon
                if (levelPlayerController)
                {
                    m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(levelPlayerController.weaponManager, new object[] { Weapon.level_weapon_homing });
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
