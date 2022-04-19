using ML_CrowdControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = 32,
        Name = "Random Weapon"
    )]
    class RandomWeap : MLCC_Effect
    {
        public override EffectResult OnTrigger(CCEffectInstance effectInstance)
        {
            if (!Base.isReady() || !Base.inLevel() || !Base.P1Ready()) return EffectResult.Retry;

            if (Base.ultra) return EffectResult.Retry;
            if (Base.isMausoleum()) return EffectResult.Retry;
            if (Base.isPlane()) return EffectResult.Retry;

            try
            {
                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                LevelPlayerController levelPlayerController2 = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;

                Weapon[] list = new Weapon[] { Weapon.level_weapon_charge, Weapon.level_weapon_bouncer, Weapon.level_weapon_peashot, Weapon.level_weapon_spreadshot, Weapon.level_weapon_homing, Weapon.level_weapon_boomerang };
                Random random = new Random();



                int ind = random.Next(0, list.Length);
                Weapon w = list[ind];
                var p = levelPlayerController.weaponManager.GetType().GetField("currentWeapon", BindingFlags.Instance | BindingFlags.NonPublic);
                Weapon cw = (Weapon)p.GetValue(levelPlayerController.weaponManager);
                while (w == cw)
                {
                    ind = random.Next(0, list.Length);
                    w = list[ind];
                }
                var m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                m.Invoke(levelPlayerController.weaponManager, new object[] { w });

                if (levelPlayerController2)
                {

                    ind = random.Next(0, list.Length);
                    w = list[ind];
                    p = levelPlayerController2.weaponManager.GetType().GetField("currentWeapon", BindingFlags.Instance | BindingFlags.NonPublic);
                    cw = (Weapon)p.GetValue(levelPlayerController2.weaponManager);
                    while (w == cw)
                    {
                        ind = random.Next(0, list.Length);
                        w = list[ind];
                    }
                    m = levelPlayerController2.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    m.Invoke(levelPlayerController2.weaponManager, new object[] { w });

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
