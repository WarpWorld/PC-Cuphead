using ML_CrowdControl;
using MelonLoader;
using System.Net.Sockets;
using WarpWorld.CrowdControl;
using HarmonyLib;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Reflection;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using UnityEngine.Networking;

[HarmonyPatch(typeof(LevelPauseGUI), "Awake")]
class Patch1
{
    public static void Postfix(LevelPauseGUI __instance, ref Text[] ___menuItems)
    {
        if (TestUI.TestUI.secret == "") 
        {
            System.Random random = new System.Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            TestUI.TestUI.secret = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        TestUI.TestUI.overon = (PlayerPrefs.GetInt("cc_overon", 1)==1);
        TestUI.TestUI.showcoins = (PlayerPrefs.GetInt("cc_showcoins", 0) == 1);
        TestUI.TestUI.showicons = (PlayerPrefs.GetInt("cc_showicons", 0) == 1);
        TestUI.TestUI.playsfx = (PlayerPrefs.GetInt("cc_playsfx", 0) == 1);
        TestUI.TestUI.volume = (PlayerPrefs.GetInt("cc_volume", 50));

        TestUI.TestUI.menu = __instance;
        TestUI.TestUI.menuItems = ___menuItems;

        Text cc = Text.Instantiate(___menuItems[0], ___menuItems[0].transform);

        cc.text = "CROWD CONTROL SETTINGS";

        float dx = ((float)(Screen.width)) / 1280.0f;
        float dy = ((float)(Screen.height)) / 720.0f;

        if (TestUI.TestUI.inLevel())
        {
            cc.transform.Translate(0.0F, 70.0F * dy, 0);
        } else
        {
            cc.transform.Translate(256.0F * dx, 60.0F * dy, 0);
        }
        

        cc.color = LevelPauseGUI.COLOR_INACTIVE;

        TestUI.TestUI.ccitem = cc;
    }
}

[HarmonyPatch(typeof(LevelPauseGUI), "set_selection")]
class Patch2
{
    public static bool Prefix(LevelPauseGUI __instance, ref int ____selection)
    {
        TestUI.TestUI.sel = ____selection;

        return !TestUI.TestUI.authup;
    }

    public static void Postfix(LevelPauseGUI __instance, ref int ____selection, ref Text[] ___menuItems)
    {
        int second = 0;
        int last = 0;



        for (int i = 1; i < ___menuItems.Length; i++)
        {
            if (___menuItems[i].gameObject.activeSelf && second == 0)
            {
                second = i;
            }
            if (___menuItems[i].gameObject.activeSelf)
            {
                last = i;
            }
        }

        if (____selection != TestUI.TestUI.sel)
        {
            if (TestUI.TestUI.sel == 0 && ____selection != second)
            {
                ____selection = ___menuItems.Length;
                
                for (int i = 1; i < ___menuItems.Length; i++)
                {
                    ___menuItems[i].color = LevelPauseGUI.COLOR_INACTIVE;
                }

                TestUI.TestUI.ccitem.color = LevelPauseGUI.COLOR_SELECTED;

            }
            else if (TestUI.TestUI.sel == last && ____selection == 0)
            {
                ____selection = ___menuItems.Length;

                for (int i = 0; i < ___menuItems.Length; i++)
                {
                    ___menuItems[i].color = LevelPauseGUI.COLOR_INACTIVE;
                }

                TestUI.TestUI.ccitem.color = LevelPauseGUI.COLOR_SELECTED;

            }
            else if (TestUI.TestUI.sel == ___menuItems.Length && ____selection == second)
            {
                ____selection = 0;
                
                TestUI.TestUI.ccitem.color = LevelPauseGUI.COLOR_INACTIVE;

                for (int i = 1; i < ___menuItems.Length; i++)
                {
                    ___menuItems[i].color = LevelPauseGUI.COLOR_INACTIVE;
                }

                ___menuItems[0].color = LevelPauseGUI.COLOR_SELECTED;
            }
            else
            {
                TestUI.TestUI.ccitem.color = LevelPauseGUI.COLOR_INACTIVE;
            }
        }
    }
}

[HarmonyPatch(typeof(LevelPauseGUI), "Select")]
class Patch3
{
    public static bool Prefix(LevelPauseGUI __instance, ref int ____selection, ref Text[] ___menuItems)
    {

        if (TestUI.TestUI.authup) return false;

        if (TestUI.TestUI.menuup)
        {

            return false;
        }

        if (____selection == ___menuItems.Length)
        {
            TestUI.TestUI.openMenu();


            return false;
        }
        return true;
    }
}


[HarmonyPatch(typeof(LevelPauseGUI), "Update")]
class Patch4
{
    public static bool Prefix()
    {
        if (TestUI.TestUI.menuup) return false;
        if (TestUI.TestUI.authup) return false;
        if (TestUI.TestUI.press < 0)
        {
            TestUI.TestUI.press += Time.deltaTime;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(SlotSelectScreen), "Awake")]
class Patch5
{
    public static void Postfix(LevelPauseGUI __instance, ref Text[] ___mainMenuItems)
    {

        try
        {
            Text cc;

            if (TestUI.TestUI.titleText == null)
            {
                cc = Text.Instantiate(___mainMenuItems[1], ___mainMenuItems[1].transform);

                cc.transform.Translate(-200.0F, -350.0F, 0);
                cc.SetAllDirty();

                TestUI.TestUI.titleText = cc;
            }

            if (TestUI.TestUI.titleUpdate == null)
            {
                cc = Text.Instantiate(___mainMenuItems[0], ___mainMenuItems[0].transform);

                cc.transform.Translate(-1200.0F, -350.0F, 0);
                cc.color = UnityEngine.Color.red;
                cc.SetAllDirty();

                TestUI.TestUI.titleUpdate = cc;
            }
            TestUI.TestUI.checkForUpdate();
        }
        catch(Exception e)
        {
            MelonLogger.Msg($"{e}");
        }
    }
}

[HarmonyPatch(typeof(SlotSelectScreen), "UpdateMainMenu")]
class Patch6
{
    public static void Postfix(LevelPauseGUI __instance, ref Text[] ___mainMenuItems)
    {
        try
        {
            TestUI.TestUI.titleText.text = $"Crowd Control v{TestUI.TestUI.version}";
            TestUI.TestUI.titleUpdate.text = $"Update Available";
        }
        catch(Exception e)
        {

        }
    }
}

namespace TestUI
{
    public class TestUI : MLCC_UI
    {
        public static string version = "1.0.0";

        public static Text titleText = null;
        public static Text titleUpdate = null;
        public static Text ccitem = null;

        public static Text toggle = null;
        public static Text deauth = null;
        public static Text menutext = null;
        public static Text username = null;
        public static Text overlay = null;

        public static Text authfield = null;
        public static Text authtext = null;
        public static Text authtext2 = null;
        public static Text authtext3 = null;

        public static Text overtext = null;
        public static Text overcoins= null;
        public static Text overtoggle = null;
        public static Text overicons = null;
        public static Text oversfx = null;
        public static Text overvol = null;


        public static LevelPauseGUI menu = null;
        public static Text[] menuItems = null;
        public static int sel = 0;
        public static int selection = 0;
        public static int oversel = 0;
        public static float press = 0;
        public static bool connect = false;
        public static bool patched = false;
        public static bool authup = false;
        public static bool menuup = false;
        public static bool overup = false;

        public static bool overon = false;
        public static bool showcoins = false;
        public static bool showicons = false;
        public static bool playsfx = false;
        public static int  volume = 50;

        public static System.Diagnostics.Process overprocess = null;
        public static string secret = "";

        public static bool badguid = false;
        public static bool waiting = false;


        // If Overridden then Console will no longer show User Input Prompt for their Activation Code

        public override void OnAuthenticationPrompt(bool isShown)
        {
            //Logger.Msg($"OnAuthenticationPrompt: {isShown}");

            if (isShown)
            {
                if (authup) return;
                authup = true;
                Text cc = Text.Instantiate(ccitem, ccitem.transform.parent);

                cc.text = "ENTER AUTH CODE";
                cc.transform.Translate(-50000.0F, 100.0F, 0);



                cc.color = UnityEngine.Color.white;
                authtext = cc;

                cc = Text.Instantiate(ccitem, ccitem.transform.parent);

                cc.supportRichText = true;
                cc.text = "GO TO crowdcontrol.live/activate";
                cc.fontSize += 4;
                cc.transform.Translate(-50000.0F+2.0F, 148.0F, 0);



                cc.color = UnityEngine.Color.white;
                authtext3 = cc;

                cc = Text.Instantiate(ccitem, ccitem.transform.parent);

                cc.supportRichText = true;
                cc.text = "GO TO <color=#0066ffff>crowdcontrol.live/activate</color>";
                cc.fontSize += 4;

                cc.transform.Translate(-50000.0F, 150.0F, 0);
                


                cc.color = UnityEngine.Color.white;
                authtext2 = cc;





                cc = Text.Instantiate(ccitem, ccitem.transform.parent);

                cc.text = "------";

                cc.transform.Translate(-50000.0F, 60.0F, 0);
                


                cc.color = UnityEngine.Color.white;

                authfield = cc;
                //hideMenu();
            }
            else
            {
                //closePrompt();
            }

        }
        
        public override void OnUpdate()
        {

            if (connect)
            {
                CrowdControl.UpdateTimerEffectStatuses();
            }
            if (authup)
            {
                authInput();
                return;
            }
            if (overup)
            {
                overInput();
                return;
            }
            if (menuup) menuInput();
        }

        public static bool inLevel()
        {
            try
            {
                if (!SceneLoader.SceneName.StartsWith("scene_level")) return false;

            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public override void OnLoad()
        {
            if (!patched)
            {
                patched = true;
                var harmony = new HarmonyLib.Harmony("com.warpworld.cupheadcc");

                // Prefix and finalizer have values, however the postfix does not.
                harmony.PatchAll();

                MelonLogger.Msg("Patched UI");
            }
        }

        public override void OnAuthenticated()
        {
            Logger.Msg($"Auth Success");
            connect = true;
            toggle.text = "CROWD CONTROL <color=green>ON</color>";
            if (CrowdControl.streamerUser != null) username.text = "User: " + CrowdControl.streamerUser.name;
            closePrompt();

            Logger.Msg($"overon: {overon}");

            if (overon)
            {
                if (!openOverlay())
                {
                    updateOverlay();
                }

                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer((obj) =>
                {
                    Task.Factory.StartNew(() => authOverlay());
                    timer.Dispose();
                    waiting = false;
                },
                null, 1000, System.Threading.Timeout.Infinite);
            }
        }

        public static void closePrompt()
        {
            if (!authup) return;
            authup = false;
            //showMenu();
            UnityEngine.Object.Destroy(authtext);
            UnityEngine.Object.Destroy(authtext2);
            UnityEngine.Object.Destroy(authtext3);
            UnityEngine.Object.Destroy(authfield);
        }

        public static void openMenu()
        {
            if (menuup) return;

            selection = 0;

            Text cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.text = "CROWD CONTROL";
            cc.transform.Translate(-50000.0F, 0.0F, 0);


            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            menutext = cc;

            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.supportRichText = true;
            cc.text = "User: ";

            if (CrowdControl.streamerUser != null && CrowdControl.instance.isAuthenticated) cc.text += CrowdControl.streamerUser.name;

            if (inLevel())
            {
                cc.transform.Translate(-50000.0F + 30.0F, -40.0F, 0);
            } else
            {
                cc.transform.Translate(-50230.0F, -20.0F, 0);
            }
            cc.alignment = TextAnchor.UpperLeft;

            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            username = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.supportRichText = true;
            if(connect)
                cc.text = "CROWD CONTROL <color=green>ON</color>";
            else
                cc.text = "CROWD CONTROL <color=red>OFF</color>";
            cc.transform.Translate(-50000.0F, -90.0F, 0);


            cc.color = LevelPauseGUI.COLOR_SELECTED;
            toggle = cc;

            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.text = "OVERLAY SETTINGS";
            cc.transform.Translate(-50000.0F, -140.0F, 0);


            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            overlay = cc;

            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.text = "UNLINK CROWD CONTROL";
            cc.transform.Translate(-50000.0F, -190.0F, 0);


            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            deauth = cc;

            foreach (Text text in menuItems)
            {
                text.transform.Translate(50000.0F, 0, 0);
            }

            menuup = true;
        }

        public static void closeMenu()
        {
            if (!menuup) return;

            UnityEngine.Object.Destroy(menutext);
            UnityEngine.Object.Destroy(username);
            UnityEngine.Object.Destroy(deauth);
            UnityEngine.Object.Destroy(toggle);
            UnityEngine.Object.Destroy(overlay);

            foreach (Text text in menuItems) 
            {
                text.transform.Translate(-50000.0F, 0, 0);
            }

            menuup = false;

            if (overon && connect)
            {
                Task.Factory.StartNew(() => cycleOverlay());
            }
        }

        public static void cycleOverlay()
        {
            openOverlay();
            authOverlay();
            updateOverlay();
        }

        public static void openOver()
        {
            if (overup) return;
            hideMenu();

            oversel = 0;

            Text cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.text = "OVERLAY SETTINGS";
            cc.transform.Translate(-50000.0F, 0.0F, 0);
            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            overtext = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            if(overon)
                cc.text = "SHOW OVERLAY <color=green>ON</color>";
            else
                cc.text = "SHOW OVERLAY <color=red>OFF</color>";
            cc.transform.Translate(-50000.0F, -40.0F, 0);
            cc.fontSize -= 4;            
            cc.color = LevelPauseGUI.COLOR_SELECTED;
            overtoggle = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);
            
            if(showcoins)
                cc.text = "SHOW COIN PURCHASES <color=green>ON</color>";
            else
                cc.text = "SHOW COIN PURCHASES <color=red>OFF</color>";
            cc.transform.Translate(-50000.0F, -75.0F, 0);
            cc.fontSize -= 4;
            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            overcoins = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            if(showicons)
                cc.text = "SHOW PROFILE ICONS <color=green>ON</color>";
            else
                cc.text = "SHOW PROFILE ICONS <color=red>OFF</color>";
            cc.transform.Translate(-50000.0F, -110.0F, 0);
            cc.fontSize -= 4;
            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            overicons = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            if (playsfx)
                cc.text = "PLAY SFX <color=green>ON</color>";
            else
                cc.text = "PLAY SFX <color=red>OFF</color>";
            cc.transform.Translate(-50000.0F, -145.0F, 0);
            cc.fontSize -= 4;
            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            oversfx = cc;


            cc = Text.Instantiate(ccitem, ccitem.transform.parent);

            cc.text = $"SFX VOLUME <color=#333333FF>{volume}</color>";
            cc.transform.Translate(-50000.0F, -180.0F, 0);
            cc.fontSize -= 4;
            cc.color = LevelPauseGUI.COLOR_INACTIVE;
            overvol = cc;

            overup = true;
            
        }

        public static void closeOver()
        {
            waiting = false;
            if (!overup) return;
            showMenu();

            UnityEngine.Object.Destroy(overtext);
            UnityEngine.Object.Destroy(overicons);
            UnityEngine.Object.Destroy(overtoggle);
            UnityEngine.Object.Destroy(overcoins);
            UnityEngine.Object.Destroy(oversfx);
            UnityEngine.Object.Destroy(overvol);

            overup = false;

        }

        public static void hideMenu()
        {
            if (!menuup) return;

            menutext.transform.Translate(50000.0F, 0, 0);
            username.transform.Translate(50000.0F, 0, 0);
            deauth.transform.Translate(50000.0F, 0, 0);
            toggle.transform.Translate(50000.0F, 0, 0);
            overlay.transform.Translate(50000.0F, 0, 0);
        }

        public static void showMenu()
        {
            if (!menuup) return;

            menutext.transform.Translate(-50000.0F, 0, 0);
            username.transform.Translate(-50000.0F, 0, 0);
            deauth.transform.Translate(-50000.0F, 0, 0);
            toggle.transform.Translate(-50000.0F, 0, 0);
            overlay.transform.Translate(-50000.0F, 0, 0);
        }

        public static void updateSelection()
        {
            if (overup)
            {
                switch (oversel)
                {
                    case 0:
                        overtoggle.color = LevelPauseGUI.COLOR_SELECTED;
                        overcoins.color = LevelPauseGUI.COLOR_INACTIVE;
                        overicons.color = LevelPauseGUI.COLOR_INACTIVE;
                        oversfx.color = LevelPauseGUI.COLOR_INACTIVE;
                        overvol.color = LevelPauseGUI.COLOR_INACTIVE;
                        break;
                    case 1:
                        overtoggle.color = LevelPauseGUI.COLOR_INACTIVE;
                        overcoins.color = LevelPauseGUI.COLOR_SELECTED;
                        overicons.color = LevelPauseGUI.COLOR_INACTIVE;
                        oversfx.color = LevelPauseGUI.COLOR_INACTIVE;
                        overvol.color = LevelPauseGUI.COLOR_INACTIVE;
                        break;
                    case 2:
                        overtoggle.color = LevelPauseGUI.COLOR_INACTIVE;
                        overcoins.color = LevelPauseGUI.COLOR_INACTIVE;
                        overicons.color = LevelPauseGUI.COLOR_SELECTED;
                        oversfx.color = LevelPauseGUI.COLOR_INACTIVE;
                        overvol.color = LevelPauseGUI.COLOR_INACTIVE;
                        break;
                    case 3:
                        overtoggle.color = LevelPauseGUI.COLOR_INACTIVE;
                        overcoins.color = LevelPauseGUI.COLOR_INACTIVE;
                        overicons.color = LevelPauseGUI.COLOR_INACTIVE;
                        oversfx.color = LevelPauseGUI.COLOR_SELECTED;
                        overvol.color = LevelPauseGUI.COLOR_INACTIVE;
                        break;
                    case 4:
                        overtoggle.color = LevelPauseGUI.COLOR_INACTIVE;
                        overcoins.color = LevelPauseGUI.COLOR_INACTIVE;
                        overicons.color = LevelPauseGUI.COLOR_INACTIVE;
                        oversfx.color = LevelPauseGUI.COLOR_INACTIVE;
                        overvol.color = LevelPauseGUI.COLOR_SELECTED;
                        break;
                }
                return;
            }

            switch (selection)
            {
                case 0:
                    toggle.color = LevelPauseGUI.COLOR_SELECTED;
                    overlay.color = LevelPauseGUI.COLOR_INACTIVE;
                    deauth.color = LevelPauseGUI.COLOR_INACTIVE;
                    break;
                case 1:
                    toggle.color = LevelPauseGUI.COLOR_INACTIVE;
                    overlay.color = LevelPauseGUI.COLOR_SELECTED;
                    deauth.color = LevelPauseGUI.COLOR_INACTIVE;
                    break;
                case 2:
                    toggle.color = LevelPauseGUI.COLOR_INACTIVE;
                    overlay.color = LevelPauseGUI.COLOR_INACTIVE;
                    deauth.color = LevelPauseGUI.COLOR_SELECTED;
                    break;
            }
        }
        

        public override void OnAuthenticationFailed()
        {
            waiting = false;
            authtext.color = LevelPauseGUI.COLOR_SELECTED;
            authtext.text = "INVALID AUTH CODE";
            deauthOverlay();
            toggle.text = "CROWD CONTROL <color=red>OFF</color>";
        }
        
        public static void updateOverlay()
        {
            if (CrowdControl.streamerUser == null) return;
            if (overprocess == null || overprocess.HasExited) return;

            string url = $"http://localhost:31515/{secret}/update?channel={CrowdControl.streamerUser.name}&coinPurchases=false";

            if (showicons)
                url += "&showProfile=true";
            else
                url += "&showProfile=false";

            if (showcoins)
                url += "&showCoins=true";
            else
                url += "&showCoins=false";

            if (playsfx)
                url += "&playSFX=true";
            else
                url += "&playSFX=false";

            url += $"&sfxVolume={volume}";

            //MelonLogger.Msg($"{url}");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responsetext = reader.ReadToEnd();
                reader.Close();
            }
            catch(Exception e)
            {
                //MelonLogger.Msg($"{e}");
            }

        }

        public static void hideOverlay()
        {
            if (overprocess == null || overprocess.HasExited) return;

            string url = $"http://localhost:31515/{secret}/update";

            //MelonLogger.Msg($"{url}");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responsetext = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                //MelonLogger.Msg($"{e}");
            }

        }


        public static void authOverlay()
        {
            if (CrowdControl.streamerUser == null) return;
            if (overprocess == null || overprocess.HasExited) return;

            string url = $"http://localhost:31515/{secret}/token?token=";

            string guid;

            //var p = CrowdControl.instance.GetType().GetField("_token", BindingFlags.Instance | BindingFlags.NonPublic);
            //guid = (string)p.GetValue(CrowdControl.instance);

            guid = PlayerPrefs.GetString($"CCToken{126}{false}", string.Empty);

            if (guid == null || guid == "")
            {
                MelonLogger.Msg($"No GUID");
                badguid = true;
                return;
            }
            badguid = false;

            url += guid;

            //MelonLogger.Msg($"{url}");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responsetext = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                //MelonLogger.Msg($"{e}");
            }
        }

        public static void deauthOverlay()
        {
            if (overprocess == null || overprocess.HasExited) return;

            string url = $"http://localhost:31515/{secret}/token?token=";
            
            //MelonLogger.Msg($"{url}");

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responsetext = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                //MelonLogger.Msg($"{e}");
            }
        }

        public static bool openOverlay()
        {
            if (overprocess != null)
            {
                if (!overprocess.HasExited) return false;

            }

            if (!CrowdControl.instance.isAuthenticated) return false;

            string guid;

            //var p = CrowdControl.instance.GetType().GetField("_token", BindingFlags.Instance | BindingFlags.NonPublic);
            //guid = (string)p.GetValue(CrowdControl.instance);

            guid = PlayerPrefs.GetString($"CCToken{126}{false}", string.Empty);

            if (guid == null || guid == "")
            {
                badguid = true;
                return false;
            }

            //MelonLogger.Msg($"Launching CC Process cc-overlay.exe \"Cuphead\" \"{secret}\" \"{guid}\" {version}");

            overprocess = new System.Diagnostics.Process();
            overprocess.StartInfo.Arguments = $"\"Cuphead\" \"{secret}\" \"{guid}\" \"126\" \"{version}\"";
            overprocess.StartInfo.FileName = "cc-overlay.exe";
            overprocess.Start();
            Task.Factory.StartNew(() => updateOverlay());
            return true;
        }

        

        public static void closeOverlay()
        {
            try {
                if (overprocess != null)
                {

                    string url = $"http://localhost:31515/{secret}/quit";

                    //MelonLogger.Msg($"{url}");

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responsetext = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch(Exception e)
            {

            }
            overprocess = null;
        }

        public void overInput()
        {
            press += Time.deltaTime;

            if (press < 0.15f) return;

            float t;
            Rewired.Keyboard k = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.Keyboard;

            MethodInfo m = menu.GetType().GetMethod("GetButton", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            bool escape = (bool)m.Invoke(menu, new object[] { CupheadButton.Cancel });

            t = k.GetKeyTimePressed(KeyCode.Escape);
            if ((t > 0 && t < 0.10) || escape)
            {
                press = -0.5f;
                closeOver();
                return;
            }

            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuDown }))
            {
                m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                m.Invoke(menu, new object[] { });

                oversel++;
                oversel = oversel % 5;

                updateSelection();
                press = 0;
                return;
            }
            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuUp }))
            {
                m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                m.Invoke(menu, new object[] { });

                oversel--;
                oversel += 5;
                oversel = oversel % 5;

                updateSelection();
                press = 0;
                return;
            }

            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuLeft }))
            {
                if (oversel == 4)
                {


                    press = 0F;
                    if (volume > 0)
                    {
                        volume -= 10;
                        m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        m.Invoke(menu, new object[] { });
                        overvol.text = $"SFX VOLUME <color=#333333FF>{volume}</color>";
                        PlayerPrefs.SetInt("cc_volume", volume);
                        Task.Factory.StartNew(() => updateOverlay());
                    }
                    return;
                }
                press = 0;
                return;
            }

            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuRight }))
            {
                if (oversel == 4)
                {


                    press = 0F;
                    if (volume < 100)
                    {
                        volume += 10;
                        m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                        m.Invoke(menu, new object[] { });
                        overvol.text = $"SFX VOLUME <color=#333333FF>{volume}</color>";
                        PlayerPrefs.SetInt("cc_volume", volume);
                        Task.Factory.StartNew(() => updateOverlay());
                    }
                    return;
                }
                press = 0;
                return;
            }


            if ((bool)m.Invoke(menu, new object[] { CupheadButton.Accept }))
            {
                if (oversel == 0)
                {
                    press = -0.4F;
                    overon = !overon;
                    if (overon)
                    {
                        overtoggle.text = "SHOW OVERLAY <color=green>ON</color>";
                        Task.Factory.StartNew(() => updateOverlay());
                    }
                    else
                    {
                        overtoggle.text = "SHOW OVERLAY <color=red>OFF</color>";
                        Task.Factory.StartNew(() => hideOverlay());
                    }
                    PlayerPrefs.SetInt("cc_overon", overon ? 1 : 0);
                    return;
                }

                if (oversel == 1)
                {
                    press = -0.1F;
                    showcoins = !showcoins;
                    if (showcoins)
                    {
                        overcoins.text = "SHOW COIN PURCHASES <color=green>ON</color>";
                    }
                    else
                    {
                        overcoins.text = "SHOW COIN PURCHASES <color=red>OFF</color>";
                    }
                    PlayerPrefs.SetInt("cc_showcoins", showcoins ? 1 : 0);
                    Task.Factory.StartNew(() => updateOverlay());
                    return;
                }

                if (oversel == 2)
                {
                    press = -0.1F;
                    showicons = !showicons;
                    if (showicons)
                    {
                        overicons.text = "SHOW PROFILE ICONS <color=green>ON</color>";
                    }
                    else
                    {
                        overicons.text = "SHOW PROFILE ICONS <color=red>OFF</color>";
                    }
                    PlayerPrefs.SetInt("cc_showicons", showicons ? 1 : 0);
                    Task.Factory.StartNew(() => updateOverlay());
                    return;
                }
                if (oversel == 3)
                {
                    press = -0.1F;
                    playsfx = !playsfx;
                    if (playsfx)
                    {
                        oversfx.text = "PLAY SFX <color=green>ON</color>";
                    }
                    else
                    {
                        oversfx.text = "PLAY SFX <color=red>OFF</color>";
                    }
                    PlayerPrefs.SetInt("cc_playsfx", playsfx ? 1 : 0);
                    Task.Factory.StartNew(() => updateOverlay());
                    return;
                }
            }
        }

        public void menuInput()
        {
            press += Time.deltaTime;

            if (press < 0.15f) return;

            float t;
            Rewired.Keyboard k = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.Keyboard;

            MethodInfo m = menu.GetType().GetMethod("GetButton", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            bool escape = (bool)m.Invoke(menu, new object[] { CupheadButton.Cancel });

            t = k.GetKeyTimePressed(KeyCode.Escape);
            if ((t > 0 && t < 0.10) || escape)
            {
                press = -0.5f;
                if (!waiting) closeMenu();
                return;
            }

            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuDown }))
            {
                m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                m.Invoke(menu, new object[] { });

                selection++;
                selection = selection % 3;

                updateSelection();
                press = 0;
                if (badguid)
                {
                    openOverlay();
                    authOverlay();
                }

                return;
            }
            if ((bool)m.Invoke(menu, new object[] { CupheadButton.MenuUp }))
            {
                m = menu.GetType().GetMethod("MenuSelectSound", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                m.Invoke(menu, new object[] { });

                selection--;
                selection += 3;
                selection = selection % 3;

                updateSelection();
                press = 0;
                if (badguid)
                {
                    openOverlay();
                    authOverlay();
                }
                return;
            }



            if ((bool)m.Invoke(menu, new object[] { CupheadButton.Accept })){
                if (badguid)
                {
                    openOverlay();
                    authOverlay();
                }
                if (selection == 0)
                {
                    press = -0.5F;
                    connect = !connect;
                    if (connect)
                    {
                        toggle.text = "CROWD CONTROL <color=green>ON</color>";

                        //var p = CrowdControl.instance.GetType().GetField("_staging", BindingFlags.Instance | BindingFlags.NonPublic);
                        //p.SetValue(CrowdControl.instance, true);

                        CrowdControl.instance.Connect();
                        if (overon)
                        {
                            openOverlay();
                            waiting = true;
                        }
                    }
                    else
                    {
                        toggle.text = "CROWD CONTROL <color=red>OFF</color>";
                        CrowdControl.instance.Disconnect();
                        if (overon) closeOverlay();

                        TestEffectPack.Base.StopAll();
                    }
                    return;
                }
                if (selection == 1)
                {
                    press = -0.5F;
                    openOver();
                }
                if (selection == 2)
                {
                    press = -0.5F;
                    connect = false;
                    toggle.text = "CROWD CONTROL <color=red>OFF</color>";
                    username.text = "User: ";
                    deauthOverlay();
                    try
                    {
                        DeauthorizeUser();
                    }
                    catch
                    {

                    }
                }
            }



        }

       

        public static async Task checkForUpdate()
        {
            try
            {
                string url = "http://unity.crowdcontrol.live/gamenews/version/126";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {

                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responsetext = reader.ReadToEnd();

                    responsetext = responsetext.Split(':')[1];

                    responsetext = responsetext.Trim();
                    responsetext = responsetext.Replace("\"", "");
                    responsetext = responsetext.Replace("}", "");

                    MelonLogger.Msg($"CC Version {responsetext}");

                    if (responsetext != version)
                    {
                        titleUpdate.transform.Translate(1000.0F, 0, 0);
                    }

                    reader.Close();
                }


            }
            catch (Exception e)
            {
                //MelonLogger.Msg($"{e}");
            }
            
        }

        public void authInput()
        {
            press += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {

                float dx = ((float)(Screen.width)) / 1280.0f;
                float dy = ((float)(Screen.height)) / 720.0f;

                if (Input.mousePosition.x >= 480.0f * dx && Input.mousePosition.x <= 900.0f * dx && Input.mousePosition.y >= 600.0f * dy && Input.mousePosition.y <= 640.0f * dy)
                {
                    Application.OpenURL("http://crowdcontrol.live/activate");
                    return;
                }


            }

            if (press < 0.125) return;

            float t;
            Rewired.Keyboard k = PlayerManager.GetPlayerInput(PlayerId.PlayerOne).controllers.Keyboard;

            
            t = k.GetKeyTimePressed(KeyCode.Escape);
            if ((t > 0 && t < 0.10))
            {
                press = 0;
                closePrompt();

                CrowdControl.instance.Disconnect();

                toggle.text = "CROWD CONTROL <color=red>OFF</color>";
                connect = false;
                return;
            }

            t = k.GetKeyTimePressed(KeyCode.Return) + k.GetKeyTimePressed(KeyCode.KeypadEnter);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                CrowdControl.instance.SubmitTempToken(authfield.text);
                return;
            }

            authfield.text = authfield.text.Replace("-", "");

            if (authfield.text.Length > 0)
            {
                t = k.GetKeyTimePressed(KeyCode.Backspace);
                if (t > 0 && t < 0.10)
                {
                    press = 0;
                    authfield.text = authfield.text.Substring(0, authfield.text.Length - 1);
                }
            }

            if (authfield.text.Length > 5) return;

            float shift = k.GetModifierKeyTimePressed(Rewired.ModifierKey.Shift);

            t = k.GetKeyTimePressed(KeyCode.A);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "A";
                else
                    authfield.text += "a";
            }

            t = k.GetKeyTimePressed(KeyCode.B);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "B";
                else
                    authfield.text += "b";
            }

            t = k.GetKeyTimePressed(KeyCode.C);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "C";
                else
                    authfield.text += "c";
            }

            t = k.GetKeyTimePressed(KeyCode.D);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "D";
                else
                    authfield.text += "d";
            }

            t = k.GetKeyTimePressed(KeyCode.E);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "E";
                else
                    authfield.text += "e";
            }

            t = k.GetKeyTimePressed(KeyCode.F);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "F";
                else
                    authfield.text += "f";
            }

            t = k.GetKeyTimePressed(KeyCode.G);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "G";
                else
                    authfield.text += "g";
            }

            t = k.GetKeyTimePressed(KeyCode.H);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "H";
                else
                    authfield.text += "h";
            }

            t = k.GetKeyTimePressed(KeyCode.I);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "I";
                else
                    authfield.text += "i";
            }

            t = k.GetKeyTimePressed(KeyCode.J);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "J";
                else
                    authfield.text += "j";
            }

            t = k.GetKeyTimePressed(KeyCode.K);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "K";
                else
                    authfield.text += "k";
            }

            t = k.GetKeyTimePressed(KeyCode.L);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "L";
                else
                    authfield.text += "l";
            }

            t = k.GetKeyTimePressed(KeyCode.M);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "M";
                else
                    authfield.text += "m";
            }

            t = k.GetKeyTimePressed(KeyCode.N);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "N";
                else
                    authfield.text += "n";
            }

            t = k.GetKeyTimePressed(KeyCode.O);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "O";
                else
                    authfield.text += "o";
            }

            t = k.GetKeyTimePressed(KeyCode.P);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "P";
                else
                    authfield.text += "p";
            }

            t = k.GetKeyTimePressed(KeyCode.Q);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "Q";
                else
                    authfield.text += "q";
            }

            t = k.GetKeyTimePressed(KeyCode.R);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "R";
                else
                    authfield.text += "r";
            }

            t = k.GetKeyTimePressed(KeyCode.S);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "S";
                else
                    authfield.text += "s";
            }

            t = k.GetKeyTimePressed(KeyCode.T);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "T";
                else
                    authfield.text += "t";
            }

            t = k.GetKeyTimePressed(KeyCode.U);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "U";
                else
                    authfield.text += "u";
            }

            t = k.GetKeyTimePressed(KeyCode.V);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "V";
                else
                    authfield.text += "v";
            }

            t = k.GetKeyTimePressed(KeyCode.W);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "W";
                else
                    authfield.text += "w";
            }

            t = k.GetKeyTimePressed(KeyCode.X);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "X";
                else
                    authfield.text += "x";
            }

            t = k.GetKeyTimePressed(KeyCode.Y);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "Y";
                else
                    authfield.text += "y";
            }

            t = k.GetKeyTimePressed(KeyCode.Z);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                if (shift > 0)
                    authfield.text += "Z";
                else
                    authfield.text += "z";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha0) + k.GetKeyTimePressed(KeyCode.Keypad0);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "0";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha1) + k.GetKeyTimePressed(KeyCode.Keypad1);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "1";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha2) + k.GetKeyTimePressed(KeyCode.Keypad2);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "2";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha3) + k.GetKeyTimePressed(KeyCode.Keypad3);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "3";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha4) + k.GetKeyTimePressed(KeyCode.Keypad4);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "4";
            }


            t = k.GetKeyTimePressed(KeyCode.Alpha5) + k.GetKeyTimePressed(KeyCode.Keypad5);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "5";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha6) + k.GetKeyTimePressed(KeyCode.Keypad6);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "6";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha7) + k.GetKeyTimePressed(KeyCode.Keypad7);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "7";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha8) + k.GetKeyTimePressed(KeyCode.Keypad8);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "8";
            }

            t = k.GetKeyTimePressed(KeyCode.Alpha9) + k.GetKeyTimePressed(KeyCode.Keypad9);
            if (t > 0 && t < 0.10)
            {
                press = 0;
                authfield.text += "9";
            }

            while (authfield.text.Length < 6) authfield.text += "-";
        }
    }
}
