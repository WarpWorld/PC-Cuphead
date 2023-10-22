using System;
using WarpWorld.CrowdControl;
using ML_CrowdControl.Effects;
using ML_CrowdControl.Effects.Data;
using System.Reflection;

namespace TestEffectPack
{
    [MLCC_EffectData(
        ID = "RandomWeapon",
        Name = "Random Weapon",
        Description = "Equips a random weapon",
        Price = 75,
        Categories = new string[] { "Weapons" }
    )]
    class RandomWeap : MLCC_Effect
    {
        public override EffectResult OnTriggerEffect(CCEffectInstance effectInstance)
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
