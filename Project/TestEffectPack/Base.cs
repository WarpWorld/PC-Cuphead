using ML_CrowdControl;
using MelonLoader;
using System.Net.Sockets;
using WarpWorld.CrowdControl;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.PostProcessing;
using System.Collections.Generic;

[HarmonyPatch(typeof(LevelPlayerAnimationController), "Update")]
class PatchA
{
    public static void Prefix() {
        if (TestEffectPack.Base.invis) {
            LevelPlayerAnimationController component = PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerAnimationController>();
             
            MethodInfo m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            m.Invoke(component, new object[] { 0.0f });

            if (!TestEffectPack.Base.P2Ready())
                return;

            LevelPlayerAnimationController component2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerAnimationController>();

            m = component2.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            m.Invoke(component2, new object[] { 0.0f });
        }

        if (CrowdControl.ActionQueue.Count == 0)
            return;

        Queue<Action> recycledActions = new Queue<Action>();

        while (CrowdControl.ActionQueue.Count > 0) {
            Action action = CrowdControl.ActionQueue.Dequeue();

            try {
                action.Invoke();
            }
            catch (Exception e) {
                recycledActions.Enqueue(action);
            }
        }

        if (recycledActions.Count > 0)
            CrowdControl.ActionQueue = recycledActions;
    }

    public static void Postfix()
    {
        if (TestEffectPack.Base.inLevel() && TestEffectPack.Base.isReady() && TestEffectPack.Base.P1Ready())
        {
            //MelonLogger.Msg($"{ PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.ToString()}");
            if (TestEffectPack.Base.giant)
            {
                float sx = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.x;
                float sy = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.y;
                float sz = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.z;

                if (Math.Abs(sx) < 1.5f) sx *= 2.5f;
                if (Math.Abs(sy) < 1.5f) sy *= 2.5f;
                if (Math.Abs(sz) < 1.5f) sz *= 2.5f;


                PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.SetScale(sx, sy, sz);

            }
            if (TestEffectPack.Base.tiny)
            {
                float sx = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.x;
                float sy = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.y;
                float sz = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.z;

                if (Math.Abs(sx) > 0.75f) sx *= 0.5f;
                if (Math.Abs(sy) > 0.75f) sy *= 0.5f;
                if (Math.Abs(sz) > 0.75f) sz *= 0.5f;


                PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.SetScale(sx, sy, sz);

            }

            if (PlayerManager.GetPlayer(PlayerId.PlayerTwo))
            {
                if (TestEffectPack.Base.giant2)
                {
                    float sx = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.x;
                    float sy = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.y;
                    float sz = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.z;

                    if (Math.Abs(sx) < 1.5f) sx *= 2.5f;
                    if (Math.Abs(sy) < 1.5f) sy *= 2.5f;
                    if (Math.Abs(sz) < 1.5f) sz *= 2.5f;


                    PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.SetScale(sx, sy, sz);

                }
                if (TestEffectPack.Base.tiny2)
                {
                    float sx = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.x;
                    float sy = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.y;
                    float sz = PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.localScale.z;

                    if (Math.Abs(sx) > 0.75f) sx *= 0.5f;
                    if (Math.Abs(sy) > 0.75f) sy *= 0.5f;
                    if (Math.Abs(sz) > 0.75f) sz *= 0.5f;


                    PlayerManager.GetPlayer(PlayerId.PlayerTwo).baseTransform.SetScale(sx, sy, sz);

                }
            }
        }
    }
}

[HarmonyPatch(typeof(PlanePlayerAnimationController), "FixedUpdate")]
class PatchAB
{
    public static void Prefix()
    {
        if (TestEffectPack.Base.invis)
        {
            LevelPlayerAnimationController component = PlayerManager.GetPlayer(PlayerId.PlayerOne).GetComponent<LevelPlayerAnimationController>();

            MethodInfo m = component.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            m.Invoke(component, new object[] { 0.0f });

            if (!TestEffectPack.Base.P2Ready())
                return;

            LevelPlayerAnimationController component2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).GetComponent<LevelPlayerAnimationController>();

            m = component2.GetType().GetMethod("SetAlpha", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            m.Invoke(component2, new object[] { 0.0f });
        }
    }

    public static void Postfix()
    {
        if (TestEffectPack.Base.inLevel() && TestEffectPack.Base.isReady() && TestEffectPack.Base.P1Ready())
        {
            //MelonLogger.Msg($"{ PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.ToString()}");
            if (TestEffectPack.Base.giant)
            {
                float sx = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.x;
                float sy = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.y;
                float sz = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.z;

                if (Math.Abs(sx) < 1.5f) sx *= 2.5f;
                if (Math.Abs(sy) < 1.5f) sy *= 2.5f;
                if (Math.Abs(sz) < 1.5f) sz *= 2.5f;


                PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.SetScale(sx, sy, sz);

            }
            if (TestEffectPack.Base.tiny)
            {
                float sx = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.x;
                float sy = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.y;
                float sz = PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.localScale.z;

                if (Math.Abs(sx) > 0.75f) sx *= 0.5f;
                if (Math.Abs(sy) > 0.75f) sy *= 0.5f;
                if (Math.Abs(sz) > 0.75f) sz *= 0.5f;


                PlayerManager.GetPlayer(PlayerId.PlayerOne).baseTransform.SetScale(sx, sy, sz);

            }
        }
    }
}

[HarmonyPatch(typeof(Cuphead), "Update")]
class Patch
{
    public static void Prefix()
    {
        try
        {
            if (TestEffectPack.Base.inLevel() && TestEffectPack.Base.invul)
            {
                if (!PlayerStatsManager.DebugInvincible)
                {
                    PlayerStatsManager.DebugToggleInvincible();


                }

                if (TestEffectPack.Base.isPlane())
                {
                    PlanePlayerController lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                }
                else
                {
                    LevelPlayerController lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                }
            }



            TestEffectPack.Base.frame++;
            if (TestEffectPack.Base.spin && TestEffectPack.Base.inLevel())
            {
                if (!TestEffectPack.Base.isRunAndGun())
                {
                    CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, 0.25F);
                    TestEffectPack.Base.rot += 0.25F;
                }
            }
            if (!TestEffectPack.Base.spin && TestEffectPack.Base.inLevel() && TestEffectPack.Base.rot != 0)
            {
                if (!TestEffectPack.Base.isRunAndGun())
                {
                    CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, 0.25F);
                    TestEffectPack.Base.rot += 0.25F;

                    if (Math.Abs(TestEffectPack.Base.rot - 360.0F) < 1.0F)
                    {
                        CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, -TestEffectPack.Base.rot);
                        TestEffectPack.Base.rot = 0;
                    }
                }
                else TestEffectPack.Base.rot = 0;
            }


            int rate = 4;
            if (CupheadTime.GlobalSpeed != 1.0f) rate = 8;

            //if (TestEffectPack.Base.isPlane()) rate /= 2;

            if (TestEffectPack.Base.inLevel() && TestEffectPack.Base.isReady() && TestEffectPack.Base.P1Ready())
            {

                if (TestEffectPack.Base.ultra && (TestEffectPack.Base.frame % rate) == 0)
                {
                    if (!TestEffectPack.Base.isPlane())
                    {
                        LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                        AbstractLevelWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;
                        var m = lw.GetType().GetMethod("fireBasic", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        m.Invoke(lw, new object[] { });
                    }
                    else
                    {
                        PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                        AbstractPlaneWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;

                        var m = lw.GetType().GetMethod("fireBasic", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        m.Invoke(lw, new object[] { });
                    }
                }

                if (TestEffectPack.Base.ultra2 && (TestEffectPack.Base.frame % rate) == 0)
                {
                    if (!TestEffectPack.Base.isPlane())
                    {
                        LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;
                        if (levelPlayerController)
                        {
                            AbstractLevelWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;
                            var m = lw.GetType().GetMethod("fireBasic", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            m.Invoke(lw, new object[] { });
                        }
                    }
                    else
                    {
                        PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                        if (levelPlayerController)
                        {
                            AbstractPlaneWeapon lw = levelPlayerController.weaponManager.CurrentWeapon;

                            var m = lw.GetType().GetMethod("fireBasic", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                            m.Invoke(lw, new object[] { });
                        }
                    }
                }

                if (TestEffectPack.Base.ultra && (TestEffectPack.Base.frame % rate) == 0)
                {
                    Weapon[] list;
                    if (!TestEffectPack.Base.isPlane())
                    {
                        list = new Weapon[] { Weapon.level_weapon_charge, Weapon.level_weapon_bouncer, Weapon.level_weapon_peashot, Weapon.level_weapon_spreadshot, Weapon.level_weapon_homing, Weapon.level_weapon_boomerang };
                    }
                    else
                    {
                        list = new Weapon[] { Weapon.plane_weapon_bomb, Weapon.plane_weapon_peashot };
                    }
                    System.Random random = new System.Random();
                    int ind = random.Next(0, list.Length);
                    Weapon w = list[ind];

                    if (!TestEffectPack.Base.isPlane())
                    {
                        LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;

                        var p = levelPlayerController.weaponManager.GetType().GetField("currentWeapon", BindingFlags.Instance | BindingFlags.NonPublic);
                        Weapon cw = (Weapon)p.GetValue(levelPlayerController.weaponManager);
                        while (w == cw)
                        {
                            ind = random.Next(0, list.Length);
                            w = list[ind];
                        }
                        var m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        m.Invoke(levelPlayerController.weaponManager, new object[] { w });
                    }
                    else
                    {
                        PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);

                        var p = levelPlayerController.weaponManager.GetType().GetField("currentWeapon", BindingFlags.Instance | BindingFlags.NonPublic);
                        Weapon cw = (Weapon)p.GetValue(levelPlayerController.weaponManager);
                        while (w == cw)
                        {
                            ind = random.Next(0, list.Length);
                            w = list[ind];
                        }
                        var m = levelPlayerController.weaponManager.GetType().GetMethod("SwitchWeapon", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                        m.Invoke(levelPlayerController.weaponManager, new object[] { w });
                    }
                }
            }
        } catch(Exception e)
        {

        }
    }

}

[HarmonyPatch(typeof(ChromaticAberrationFilmGrain), "OnRenderImage")]
class PatchD
{
    public static void Prefix()
    {
        //MelonLogger.Msg("OnRenderImage");

        if (!TestEffectPack.Base.isReady() || !TestEffectPack.Base.inLevel() || !TestEffectPack.Base.P1Ready()) return;
        if (TestEffectPack.Base.isRunAndGun()) return;
        if (TestEffectPack.Base.isMausoleum()) return;

        if (!TestEffectPack.Base.bosshp) return;

        
        var p = Level.Current.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);

        var prop = p.GetValue(Level.Current);

  
        var p2 = prop.GetType().GetField("TotalHealth", BindingFlags.Instance | BindingFlags.Public);
        var p3 = prop.GetType().GetProperty("CurrentHealth", BindingFlags.Instance | BindingFlags.Public);

        float tot = (float)p2.GetValue(prop);
        float h   = (float)p3.GetValue(prop, new object[] { });

        if (Level.Current.CurrentLevel == Levels.Veggies)
        {
            tot = TestEffectPack.Base.hp1 + TestEffectPack.Base.hp2 + TestEffectPack.Base.hp3;



            FieldInfo bossField = Level.Current.GetType().GetField("currentBoss", BindingFlags.NonPublic | BindingFlags.Instance);

            object potato = Level.Current.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Potato").GetValue(Level.Current);
            object onion = Level.Current.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Onion").GetValue(Level.Current);
            object carrot = Level.Current.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Carrot").GetValue(Level.Current);

            object current = bossField.GetValue(Level.Current);

            FieldInfo hpfield = TestEffectPack.Base.vegboss.GetType().GetField("hp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            h = (float)hpfield.GetValue(TestEffectPack.Base.vegboss);

            //MelonLogger.Msg($"h: {h} current: {current} onion: {onion} hp2: {TestEffectPack.Base.hp2}");

            FieldInfo pb;

            if (current.ToString() == carrot.ToString())
                pb = TestEffectPack.Base.vegboss.GetType().GetField("carrot", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            else
                pb = TestEffectPack.Base.vegboss.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            object bossprop = null;
            if(pb!=null)bossprop = pb.GetValue(TestEffectPack.Base.vegboss);

            //MelonLogger.Msg($"vegboss: {TestEffectPack.Base.vegboss} bossprop: {bossprop}");
            
            if (current.ToString() == potato.ToString())
            {
                if (bossprop == null) h = TestEffectPack.Base.hp1;
                h += TestEffectPack.Base.hp2 + TestEffectPack.Base.hp3;
            }
            if (current.ToString() == onion.ToString())
            {
                if (bossprop == null || bossprop.ToString().Contains("Potato")) h = TestEffectPack.Base.hp2;
                h += TestEffectPack.Base.hp3;
            }
            if (current.ToString() == carrot.ToString())
            {
                if (bossprop == null || bossprop.ToString().Contains("Onion")) h = TestEffectPack.Base.hp3;
            }

            //MelonLogger.Msg($"h2: {h}");
        }

        if (Level.Current.CurrentLevel == Levels.Baroness || Level.Current.CurrentLevel == Levels.Train || Level.Current.CurrentLevel == Levels.DicePalaceBooze)
        {
            tot = Level.Current.timeline.health;

            h = tot - Level.Current.timeline.damage;
        }

        UnityEngine.GL.PushMatrix();
        UnityEngine.GL.LoadPixelMatrix(0, UnityEngine.Screen.width, UnityEngine.Screen.height, 0);

        int dw = UnityEngine.Screen.width * 40 / 100;

        UnityEngine.Rect r;
        UnityEngine.Color c;

        

        r = new UnityEngine.Rect(UnityEngine.Screen.width/2 - dw/2 - 2, 20 - 2, dw + 4, 40 + 4);
        c = new UnityEngine.Color(0, 0, 0, 1.0F);
        TestEffectPack.Base.GUIDrawRect(r, c);

        int hw = (int)((dw * h) / tot);

        //MelonLogger.Msg($"health: {h} tot: {tot} hw: {hw} dw: {dw}");

        r = new UnityEngine.Rect(UnityEngine.Screen.width / 2 - dw / 2, 20, hw, 40);
        c = new UnityEngine.Color(1.0F, 0, 0, 1.0F);
        TestEffectPack.Base.GUIDrawRect(r, c);

        UnityEngine.GL.PopMatrix();
    }
}

[HarmonyPatch(typeof(LevelPlayerWeaponManager), "HandleWeaponSwitch")]
class Patch2
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noSwitch;
    }
}

[HarmonyPatch(typeof(PlanePlayerWeaponManager), "HandleWeaponSwitch")]
class Patch2B
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noSwitch;
    }
}

[HarmonyPatch(typeof(LevelPlayerWeaponManager), "HandleWeaponFiring")]
class Patch3
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noFire;
    }
}

[HarmonyPatch(typeof(PlanePlayerWeaponManager), "CheckBasic")]
class Patch3B
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noFire;
    }
}

[HarmonyPatch(typeof(PlanePlayerWeaponManager), "CheckEx")]
class Patch3C
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noFire;
    }
}


[HarmonyPatch(typeof(Level), "Start")]
class Patch4
{
    public static void Postfix(Level __instance)
    {
        try
        {
            MelonLogger.Msg($"Level Start - {SceneLoader.CurrentLevel}");


            if (TestEffectPack.Base.invul)
            {
                if (!PlayerStatsManager.DebugInvincible) PlayerStatsManager.DebugToggleInvincible();
            }

            if (PlayerStatsManager.DebugInvincible)
            {
                if (TestEffectPack.Base.isPlane())
                {
                    PlanePlayerController lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    lpc = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                } else
                {
                    LevelPlayerController lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerOne);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                    lpc = PlayerManager.GetPlayer<LevelPlayerController>(PlayerId.PlayerTwo);
                    if (lpc) lpc.animationController.SetColor(UnityEngine.Color.yellow);
                }
            }

            if (__instance.CurrentLevel == Levels.Veggies)
            {
                VeggiesLevel vl = __instance as VeggiesLevel;

                var p = vl.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);

                LevelProperties.Veggies prop = (LevelProperties.Veggies)p.GetValue(vl);

                TestEffectPack.Base.hp1 = prop.CurrentState.potato.hp;
                TestEffectPack.Base.hp2 = prop.CurrentState.onion.hp;
                TestEffectPack.Base.hp3 = prop.CurrentState.carrot.hp;

            }


            TestEffectPack.Base.rot = 0;

            if (TestEffectPack.Base.flipped && !TestEffectPack.Base.isRunAndGun()) CupheadLevelCamera.Current.baseTransform.Rotate(0, 0, 180.0F);
            if (TestEffectPack.Base.zoom) CupheadLevelCamera.Current.Zoom(0.5F, 5.0F, EaseUtils.EaseType.linear);
            if (TestEffectPack.Base.slowmo) CupheadTime.GlobalSpeed = 0.25F;
            if (!TestEffectPack.Base.isPlane())
            {

                LevelPlayerController levelPlayerController = PlayerManager.GetPlayer(PlayerId.PlayerOne) as LevelPlayerController;
                LevelPlayerController levelPlayerController2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo) as LevelPlayerController;

                if (TestEffectPack.Base.frozen) levelPlayerController.weaponManager.FreezePosition = true;
                if (TestEffectPack.Base.frozen2 && levelPlayerController2) levelPlayerController2.weaponManager.FreezePosition = true;

                if (TestEffectPack.Base.fastwalk)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.moveSpeed = 1000f;
                }

                if (TestEffectPack.Base.slowwalk)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.moveSpeed = 250f;
                }

                if (TestEffectPack.Base.slowwalk2 && levelPlayerController2)
                {
                    var p = levelPlayerController2.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController2.motor);
                    prop.moveSpeed = 250f;
                }

                if (TestEffectPack.Base.shortdash)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.dashSpeed = 200f;
                }

                if (TestEffectPack.Base.shortdash2 && levelPlayerController2)
                {
                    var p = levelPlayerController2.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController2.motor);
                    prop.dashSpeed = 200f;
                }

                if (TestEffectPack.Base.longdash)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.dashSpeed = 2800f;
                }

                if (TestEffectPack.Base.longdash2 && levelPlayerController2)
                {
                    var p = levelPlayerController2.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController2.motor);
                    prop.dashSpeed = 2800f;
                }

                if (TestEffectPack.Base.highjump)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.jumpPower -= 0.5f;
                }

                if (TestEffectPack.Base.highjump2 && levelPlayerController2)
                {
                    var p = levelPlayerController2.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController2.motor);
                    prop.jumpPower -= 0.5f;
                }

                if (TestEffectPack.Base.lowjump)
                {
                    var p = levelPlayerController.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController.motor);
                    prop.jumpPower += 0.3f;
                }

                if (TestEffectPack.Base.lowjump2 && levelPlayerController2)
                {
                    var p = levelPlayerController2.motor.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                    LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p.GetValue(levelPlayerController2.motor);
                    prop.jumpPower += 0.3f;
                }
            }
            else
            {
                PlanePlayerController levelPlayerController = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerOne);
                PlanePlayerController levelPlayerController2 = PlayerManager.GetPlayer<PlanePlayerController>(PlayerId.PlayerTwo);

                if (TestEffectPack.Base.frozen)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;
                    prop.speed = 0;
                    prop.shrunkSpeed = 0;
                }

                if (TestEffectPack.Base.frozen2 && levelPlayerController2)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController2.motor.properties;
                    prop.speed = 0;
                    prop.shrunkSpeed = 0;
                }


                if (TestEffectPack.Base.fastwalk)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;
                    prop.speed = 1000f;
                    prop.shrunkSpeed = 1500f;
                }

                if (TestEffectPack.Base.fastwalk2 && levelPlayerController2)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController2.motor.properties;
                    prop.speed = 1000f;
                    prop.shrunkSpeed = 1500f;
                }

                if (TestEffectPack.Base.slowwalk)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController.motor.properties;
                    prop.speed = 250f;
                    prop.shrunkSpeed = 375f;
                }

                if (TestEffectPack.Base.slowwalk2 && levelPlayerController2)
                {
                    PlanePlayerMotor.Properties prop = levelPlayerController2.motor.properties;
                    prop.speed = 250f;
                    prop.shrunkSpeed = 375f;
                }
            }
        } catch(Exception e)
        {

        }
    }
}


[HarmonyPatch(typeof(PlayerInput), "GetAxisInt")]
class Patch5
{
    public static void Postfix(ref int __result)
    {
        if (TestEffectPack.Base.invert) __result *= -1;
    }
}

[HarmonyPatch(typeof(PlayerInput), "GetAxis")]
class Patch6
{
    public static void Postfix(ref float __result)
    {
        if (TestEffectPack.Base.invert) __result *= -1;
    }
}

[HarmonyPatch(typeof(PlanePlayerMotor), "HandleInput")]
class Patch9
{
    public static void Postfix(PlanePlayerMotor __instance)
    {
        if (TestEffectPack.Base.invert)
        {
            var p = __instance.GetType().GetProperty("MoveDirection", BindingFlags.Instance | BindingFlags.Public);

            Trilean2 m = (Trilean2)p.GetValue(__instance, new object[] { });
            m.x *= -1.0f;
            m.y *= -1.0f;
            p.SetValue(__instance, m, new object[] { });
        }
    }
}

[HarmonyPatch(typeof(LevelPlayerMotor), "HandleParry")]
class Patch7
{
    public static bool Prefix(LevelPlayerMotor __instance)
    {
        TestEffectPack.Base.check = __instance.Parrying;

        return !TestEffectPack.Base.noParry;
    }

    public static void Postfix(LevelPlayerMotor __instance)
    {
        //MelonLogger.Msg($"Check: {TestEffectPack.Base.check} Parry: {__instance.Parrying}");

        if (TestEffectPack.Base.check == false && __instance.Parrying && TestEffectPack.Base.doublejump)
        {
            var p = __instance.GetType().GetField("velocityManager", BindingFlags.Instance | BindingFlags.NonPublic);

            LevelPlayerMotor.VelocityManager jm = (LevelPlayerMotor.VelocityManager)p.GetValue(__instance);


            var p2 = __instance.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);

            LevelPlayerMotor.Properties prop = (LevelPlayerMotor.Properties)p2.GetValue(__instance);

            jm.y = prop.jumpPower * 1.5F;
        }
    }
}

[HarmonyPatch(typeof(PlanePlayerParryController), "StartParry")]
class Patch7B
{
    public static bool Prefix()
    {
        return !TestEffectPack.Base.noParry;
    }
}

[HarmonyPatch(typeof(LevelPlayerMotor), "HandleDash")]
class Patch8
{
    public static void Prefix(LevelPlayerMotor __instance)
    {

        if (TestEffectPack.Base.dashes)
        {
            var p = __instance.GetType().GetField("dashManager", BindingFlags.Instance | BindingFlags.NonPublic);

            LevelPlayerMotor.DashManager dm = (LevelPlayerMotor.DashManager)p.GetValue(__instance);

            if (dm.state == LevelPlayerMotor.DashManager.State.End)
            {
                dm.state = LevelPlayerMotor.DashManager.State.Ready;
            }
        }
    }
}

[HarmonyPatch(typeof(VeggiesLevelPotato), "Awake")]
class PatchV1
{
    public static void Prefix(VeggiesLevelPotato __instance)
    {
        TestEffectPack.Base.vegboss = __instance;
    }
}

[HarmonyPatch(typeof(VeggiesLevelOnion), "Start")]
class PatchV2
{
    public static void Prefix(VeggiesLevelOnion __instance)
    {
        TestEffectPack.Base.vegboss = __instance;
    }
}

[HarmonyPatch(typeof(VeggiesLevelCarrot), "Start")]
class PatchV3
{
    public static void Prefix(VeggiesLevelCarrot __instance)
    {
        TestEffectPack.Base.vegboss = __instance;
    }
}


[HarmonyPatch(typeof(DamageReceiver), "TakeDamage")]
class Patch10
{
    public static void Prefix(DamageDealer.DamageInfo info, DamageReceiver __instance)
    {
        if (__instance.type != DamageReceiver.Type.Enemy) return;

        var m = info.GetType().GetMethod("set_damage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (TestEffectPack.Base.dmgup)
        {
            m.Invoke(info, new object[] { info.damage * 2.0F });
        }
        if (TestEffectPack.Base.dmgdown)
        {
            m.Invoke(info, new object[] { info.damage * 0.5F });
        }
    }
}


namespace TestEffectPack
{

    public class Base
    {
        public static bool patched = false;
        public static bool noSwitch = false;
        public static bool noFire = false;
        public static bool noParry = false;
        public static bool invert = false;
        public static bool ultra = false;
        public static bool ultra2 = false;
        public static bool ff = false;
        public static bool dashes = false;
        public static bool doublejump = false;
        public static bool autofire = false;
        public static bool flipped = false;
        public static bool frozen = false;
        public static bool frozen2 = false;
        public static bool invis = false;
        public static bool spin = false;
        public static bool zoom = false;
        public static bool slowmo = false;
        public static bool dmgup = false;
        public static bool dmgdown = false;
        public static bool bosshp = false;
        public static bool invul = false;


        public static bool highjump = false;
        public static bool lowjump = false;
        public static bool longdash = false;
        public static bool shortdash = false;
        public static bool fastwalk = false;
        public static bool slowwalk = false;
        public static bool giant = false;
        public static bool tiny = false;

        public static bool highjump2 = false;
        public static bool lowjump2 = false;
        public static bool longdash2 = false;
        public static bool shortdash2 = false;
        public static bool fastwalk2 = false;
        public static bool slowwalk2 = false;
        public static bool giant2 = false;
        public static bool tiny2 = false;


        public static int frame = 0;

        public static float rot = 0;

        public static float hp1 = 0;
        public static float hp2 = 0;
        public static float hp3 = 0;
        public static LevelProperties.Veggies.Entity vegboss = null;

        public static bool check = false;

        public static FieldInfo GetBackingField(Type type, string propertyName)
        {
            return type.GetField(string.Format("<{0}>k__BackingField", propertyName), BindingFlags.Static | BindingFlags.NonPublic);
        }

        public void start()
        {
            
        }

        public static void patch()
        {
            if (!patched)
            {
                patched = true;
                var harmony = new HarmonyLib.Harmony("com.warpworld.cupheadcc");

                // Prefix and finalizer have values, however the postfix does not.
                harmony.PatchAll();

                MelonLogger.Msg("Patched");
            }
        }


        public static bool isReady()
        {
            try
            {
                if (SceneLoader.CurrentlyLoading) return false;
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool isRunAndGun()
        {
            try
            {
                if (SceneLoader.CurrentlyLoading) return false;
                if (Map.Current != null) return false;

                switch (SceneLoader.CurrentLevel)
                {
                    case Levels.Platforming_Level_1_1:
                    case Levels.Platforming_Level_1_2:
                    case Levels.Platforming_Level_2_1:
                    case Levels.Platforming_Level_2_2:
                    case Levels.Platforming_Level_3_1:
                    case Levels.Platforming_Level_3_2:
                    case Levels.Tutorial:
                        return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return false;

        }

        public static bool isPlane()
        {
            try
            {
                if (!SceneLoader.SceneName.StartsWith("scene_level_flying") && !string.Equals(SceneLoader.SceneName, "scene_level_robot")) return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool isMausoleum()
        {
            try
            {
                if (SceneLoader.CurrentlyLoading) return false;
                if (!SceneLoader.SceneName.StartsWith("scene_level_mausoleum")) return false;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool inLevel()
        {
            try
            {
                if (SceneLoader.CurrentlyLoading) return false;
                if (!SceneLoader.SceneName.StartsWith("scene_level")) return false;


                switch (SceneLoader.CurrentLevel)
                {
                    case Levels.House:
                    case Levels.DiceGate:
                        return false;
                }

                return Level.Current.Started && !Level.Current.Ending && PauseManager.state != PauseManager.State.Paused;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool onMap()
        {
            try
            {
                if (SceneLoader.CurrentlyLoading) return false;

                //MelonLogger.Msg($"Scene: {SceneLoader.SceneName}");

                return (SceneLoader.SceneName.StartsWith("scene_map"));
            }
            catch (Exception e) {
                return false;
            }
        }

        public static bool inGame()
        {
            return inLevel() || onMap();
        }

        public static bool P1Ready()
        {
            try
            {
                int h = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.Health;
                if (h <= 0) return false;

                if (PlayerManager.GetPlayer(PlayerId.PlayerOne).stats.State != PlayerStatsManager.PlayerState.Ready) return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool P2Ready()
        {
            try
            {
                int h = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.Health;
                if (h <= 0) return false;

                if (PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.State != PlayerStatsManager.PlayerState.Ready) return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool damageBoss(Level level, float d)
        {

            try
            {
                if (level.CurrentLevel == Levels.Veggies) return damageVeggie(level as VeggiesLevel, d);
                if (level.CurrentLevel == Levels.Baroness) return damageBaroness(level as BaronessLevel, d);
                if (level.CurrentLevel == Levels.Train) return damageTrain(level as TrainLevel, d);
                if (level.CurrentLevel == Levels.DicePalaceBooze) return damageBooze(level as DicePalaceBoozeLevel, d);

                var p = level.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);

                var prop = p.GetValue(level);

                var p3 = prop.GetType().GetProperty("CurrentHealth", BindingFlags.Instance | BindingFlags.Public);
                float ch = (float)p3.GetValue(prop, new object[] { });

                if (ch <= 0) return false;

                if (d < 0)
                {

                    var p2 = prop.GetType().GetField("TotalHealth", BindingFlags.Instance | BindingFlags.Public);



                    //MelonLogger.Msg($"p2: {p2} p3: {p3}");

                    
                    float th = (float)p2.GetValue(prop);

                    MelonLogger.Msg($"Damage: {d} Current: {ch} Total: {th}");

                    if (ch - d > th) return false;
                }

                MethodInfo m = prop.GetType().GetMethod("DealDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                m.Invoke(prop, new object[] { d });
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool damageVeggie(VeggiesLevel level, float d)
        {
            try
            {
                if (vegboss == null) return false;
                float ch = 0;
                float th = 0;


                FieldInfo bossField = level.GetType().GetField("currentBoss", BindingFlags.NonPublic | BindingFlags.Instance);

                object potato = level.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Potato").GetValue(level);
                object onion = level.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Onion").GetValue(level);
                object carrot = level.GetType().GetNestedType("CurrentBoss", BindingFlags.NonPublic).GetField("Carrot").GetValue(level);

                object current = bossField.GetValue(level);

                var p = level.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);

                LevelProperties.Veggies prop = (LevelProperties.Veggies)p.GetValue(level);


                FieldInfo hpfield = vegboss.GetType().GetField("hp", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                ch = (float)hpfield.GetValue(vegboss);

                th = hp1 + hp2 + hp3;

                if (ch <= 0) return false;

                if (d < 0)
                {
                    if (ch - d > th) return false;
                }


                level.timeline.DealDamage(d);
                hpfield.SetValue(vegboss, ch - d);

                if (ch - d <= 0f)
                {
                    MethodInfo m = vegboss.GetType().GetMethod("Die", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    m.Invoke(vegboss, new object[] { });
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool damageBaroness(BaronessLevel level, float d)
        {
            try
            {
                float ch = 0;
                float th = 0;

                var p = level.GetType().GetField("currentMiniBoss", BindingFlags.Instance | BindingFlags.NonPublic);
                BaronessLevelMiniBossBase boss = (BaronessLevelMiniBossBase)p.GetValue(level);

                p = level.GetType().GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
                LevelProperties.Baroness prop = (LevelProperties.Baroness)p.GetValue(level);

                if (prop.CurrentState.stateName != LevelProperties.Baroness.States.Chase && boss)
                {
                    if (boss)
                    {

                        FieldInfo hpfield = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        ch = (float)hpfield.GetValue(boss);

                        if (ch <= 0) return false;

                        if (d < 0)
                        {
                            if (level.timeline.damage < -d) return false;

                            level.timeline.DealDamage(d);
                            hpfield.SetValue(boss, ch - d);
                        }
                        else
                        {
                            DamageDealer.DamageInfo info = new DamageDealer.DamageInfo(d, DamageDealer.Direction.Neutral, new UnityEngine.Vector2(0, 0), DamageDealer.DamageSource.Neutral);

                            MethodInfo m = boss.GetType().GetMethod("OnDamageTaken", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                            m.Invoke(boss, new object[] { info });
                        }

                    }
                    else return false;
                } else
                {

                    p = level.GetType().GetField("castle", BindingFlags.Instance | BindingFlags.NonPublic);
                    BaronessLevelCastle castle = (BaronessLevelCastle)p.GetValue(level);


                    ch = prop.CurrentHealth;

                    th = prop.TotalHealth;

                    if (ch <= 0) return false;

                    if (d < 0)
                    {
                        if (ch - d > th) return false;
                    }

                    DamageDealer.DamageInfo info = new DamageDealer.DamageInfo(d, DamageDealer.Direction.Neutral, new UnityEngine.Vector2(0, 0), DamageDealer.DamageSource.Neutral);

                    MethodInfo m = castle.GetType().GetMethod("OnDamageTaken", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    m.Invoke(castle, new object[] { info });

                }


            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public static bool damageTrain(TrainLevel level, float d)
        {
            try
            {
                float ch = 0;
                float th = 0;

                LevelProperties.Train.Entity boss = null;

                var p = level.GetType().GetField("currentPhase", BindingFlags.Instance | BindingFlags.NonPublic);
                int phase = (int)p.GetValue(level);

                switch (phase)
                {
                    case 1:
                        p = level.GetType().GetField("blindSpecter", BindingFlags.Instance | BindingFlags.NonPublic);
                        boss = (LevelProperties.Train.Entity)p.GetValue(level);
                        break;
                    case 2:
                        p = level.GetType().GetField("skeleton", BindingFlags.Instance | BindingFlags.NonPublic);
                        boss = (LevelProperties.Train.Entity)p.GetValue(level);
                        break;
                    case 3:
                        p = level.GetType().GetField("ghouls", BindingFlags.Instance | BindingFlags.NonPublic);
                        var ghouls = (LevelProperties.Train.Entity)p.GetValue(level);
                        p = ghouls.GetType().GetField("ghoulLeft", BindingFlags.Instance | BindingFlags.NonPublic);
                        boss = (LevelProperties.Train.Entity)p.GetValue(ghouls);
                        var f = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        ch = (float)f.GetValue(boss);
                        if (ch <= 0)
                        {
                            p = ghouls.GetType().GetField("ghoulRight", BindingFlags.Instance | BindingFlags.NonPublic);
                            boss = (LevelProperties.Train.Entity)p.GetValue(ghouls);
                        }
                        break;
                    case 4:
                        p = level.GetType().GetField("engine", BindingFlags.Instance | BindingFlags.NonPublic);
                        boss = (LevelProperties.Train.Entity)p.GetValue(level);
                        break;
                }

                if (boss == null) return false;

                FieldInfo hpfield = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                ch = (float)hpfield.GetValue(boss);

                if (ch <= 0) return false;

                if (d < 0)
                {
                    if (level.timeline.damage < -d) return false;
                }

                DamageDealer.DamageInfo info = new DamageDealer.DamageInfo(d, DamageDealer.Direction.Neutral, new UnityEngine.Vector2(0, 0), DamageDealer.DamageSource.Neutral);

                MethodInfo m = boss.GetType().GetMethod("OnDamageTaken", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                m.Invoke(boss, new object[] { info });



            }
            catch (Exception e)
            {
                MelonLogger.Msg($"{e}");
                return false;
            }

            return true;
        }


        public static bool damageBooze(DicePalaceBoozeLevel level, float d)
        {
            try
            {
                float ch = 0;
                float th = 0;

                DicePalaceBoozeLevelBossBase boss = null;
                
                var p = level.GetType().GetField("decanter", BindingFlags.Instance | BindingFlags.NonPublic);
                boss = (DicePalaceBoozeLevelBossBase)p.GetValue(level);

                var f = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                ch = (float)f.GetValue(boss);
                if (ch <= 0)
                {
                    p = level.GetType().GetField("martini", BindingFlags.Instance | BindingFlags.NonPublic);
                    boss = (DicePalaceBoozeLevelBossBase)p.GetValue(level);



                    f = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    ch = (float)f.GetValue(boss);
                    if (ch <= 0)
                    {
                        p = level.GetType().GetField("tumbler", BindingFlags.Instance | BindingFlags.NonPublic);
                        boss = (DicePalaceBoozeLevelBossBase)p.GetValue(level);
                    }

                }



                if (boss == null) return false;

                FieldInfo hpfield = boss.GetType().GetField("health", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                ch = (float)hpfield.GetValue(boss);

                if (ch <= 0) return false;

                if (d < 0)
                {
                    if (level.timeline.damage < -d) return false;
                }

                DamageDealer.DamageInfo info = new DamageDealer.DamageInfo(d, DamageDealer.Direction.Neutral, new Vector2(0, 0), DamageDealer.DamageSource.Neutral);

                MethodInfo m = boss.GetType().GetMethod("OnDamageTaken", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

                m.Invoke(boss, new object[] { info });



            }
            catch (Exception e)
            {
                MelonLogger.Msg($"{e}");
                return false;
            }

            return true;
        }


        public static void StopAll()
        {
            if(PlayerStatsManager.DebugInvincible)
            {
                PlayerStatsManager.DebugToggleInvincible();
            }

            noSwitch = false;
            noFire = false;
            noParry = false;
            invert = false;
            ultra = false;
            ultra2 = false;
            ff = false;
            dashes = false;
            doublejump = false;
            autofire = false;
            flipped = false;
            frozen = false;
            frozen2 = false;
            invis = false;
            spin = false;
            zoom = false;
            slowmo = false;
            dmgup = false;
            dmgdown = false;
            bosshp = false;
            invul = false;


            highjump = false;
            lowjump = false;
            longdash = false;
            shortdash = false;
            fastwalk = false;
            slowwalk = false;
            giant = false;
            tiny = false;

            highjump2 = false;
            lowjump2 = false;
            longdash2 = false;
            shortdash2 = false;
            fastwalk2 = false;
            slowwalk2 = false;
            giant2 = false;
            tiny2 = false;
    }

        private static UnityEngine.Texture2D _staticRectTexture = null;

        public static void GUIDrawRect(UnityEngine.Rect position, UnityEngine.Color color) {
            if (_staticRectTexture == null)
            {
                _staticRectTexture = new UnityEngine.Texture2D(1, 1);
            }

            _staticRectTexture.SetPixel(0, 0, color);
            _staticRectTexture.Apply();

            UnityEngine.Graphics.DrawTexture(position, _staticRectTexture);
        }
    }
}


