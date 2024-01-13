using BepInEx;
using BepInEx.Configuration;
using GameNetcodeStuff;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace LCTestMod.Component
{
    internal class GUILoader : MonoBehaviour
    {
        private KeyboardShortcut openCloseMenu;
        public bool isMenuOpen;

        internal bool wasKeyDown;

        public int toolbarInt = 0;
        private string[] toolbarStrings = { "Player Mods", "Teleport Options", "Ship Options" };

        private int MENUWIDTH = 600;
        private int MENUHEIGHT = 800;
        private int MENUX;
        private int MENUY;
        private int ITEMWIDTH = 300;
        private int CENTERX;

        #region Public Values
        public bool guiEnableGod;
        public bool guiEnableSpeedCheat;
        public bool guiEnableInfiniteSprint;
        public bool guiEnableInfiniteWeight;
        public bool guiEnableTwoHanded;
        public bool guiEnableAlwaysScannable;
        public bool guiEnableInfiniteReach;
        public bool guiEnableNightVision;
        public bool guiEnableInfiniteBattery;
        public bool guiEnableInfiniteShells;
        public bool guiEnableMuteMic;
        public bool guiEnableFloating;

        public float guiPlayerJumpForce = 13f;
        public string guiPlayerStringJumpForce = "13";

        public bool guiSpawnGun;
        public bool guiSpawnStopSign;
        public bool guiSpawnYieldSign;
        public bool guiStartShipAnimation;
        public bool guiOpenShipDoor;
        public bool guiCloseShipDoor;

        public bool guiTeleportToScrapButtonPressed;
        public bool guiTeleportToShipButtonPressed;
        public bool guiFunnyButtonPressed;
        public bool guiTeleportToEntranceButtonPressed;
        public bool guiTeleportForwardButtonPressed;

        public bool guiTeleportMenuOpened = false;

        public List<PlayerControllerB> guiPlayerList;
        public List<bool> guiTeleportToPlayersButtonPressed;

        public string guiCreditString = "";
        public int guiCredits;
        public bool guiSetCredits;

        public string guiQuotaNeededString = "";
        public int guiQuotaNeeded;
        public bool guiSetQuotaNeeded;

        public string guiDeadlineString = "";
        public int guiDeadline;
        public bool guiSetDeadline;
        #endregion

        private GUIStyle menuStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle toggleStyle;
        private GUIStyle hScrollStyle;
        private GUIStyle textFeildStyle;

        private void Awake()
        {
            TestModBase.Instance.mls.LogInfo("The GUILoader has awaken :)");
            openCloseMenu = new KeyboardShortcut(KeyCode.F1);
            isMenuOpen = false;
            MENUX = (Screen.width / 2);
            MENUY = (Screen.height / 2);
            CENTERX = MENUX + ((MENUWIDTH / 2) - (ITEMWIDTH / 2));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void intitializeMenu()
        {
            if (menuStyle == null)
            {
                menuStyle = new GUIStyle(GUI.skin.box);
                buttonStyle = new GUIStyle(GUI.skin.button);
                labelStyle = new GUIStyle(GUI.skin.label);
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                hScrollStyle = new GUIStyle(GUI.skin.horizontalSlider);
                textFeildStyle = new GUIStyle(GUI.skin.textField);

                menuStyle.normal.textColor = Color.white;
                menuStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                menuStyle.fontSize = 18;
                menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                buttonStyle.normal.textColor = Color.white;
                buttonStyle.fontSize = 18;

                labelStyle.normal.textColor = Color.white;
                labelStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                labelStyle.fontSize = 18;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                toggleStyle.normal.textColor = Color.white;
                toggleStyle.fontSize = 18;

                hScrollStyle.normal.textColor = Color.white;
                hScrollStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                hScrollStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                textFeildStyle.normal.textColor = Color.white;
                textFeildStyle.fontSize = 18;
                hScrollStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                hScrollStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
            }
        }


        public void OnDestroy()
        {
            TestModBase.Instance.mls.LogInfo("The GUILoader was destroyed :(");
        }

        public void Update()
        {
            if (StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null || StartOfRound.Instance.OtherClients == null)
            {
                wasKeyDown = false;
                isMenuOpen = false;
                return;
            }


            if (isMenuOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                StartOfRound.Instance.localPlayerController.disableLookInput = true;
            }

            //if (openCloseMenu.IsDown())
            if (UnityInput.Current.GetKeyDown(KeyCode.F1))
            {
                TestModBase.Instance.mls.LogInfo("blah");
                if (!wasKeyDown)
                {
                    wasKeyDown = true;
                }
            }
            //if (openCloseMenu.IsUp())
            if (UnityInput.Current.GetKeyUp(KeyCode.F1))
            {
                if (wasKeyDown)
                {
                    wasKeyDown = false;
                    isMenuOpen = !isMenuOpen;
                    if (isMenuOpen)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                        StartOfRound.Instance.localPlayerController.disableLookInput = true;

                        guiPlayerList = StartOfRound.Instance.OtherClients;
                        guiTeleportToPlayersButtonPressed = new List<bool>();

                        while (guiTeleportToPlayersButtonPressed.Count < guiPlayerList.Count)
                        {
                            guiTeleportToPlayersButtonPressed.Add(false);
                        }
                    }
                    else
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        StartOfRound.Instance.localPlayerController.disableLookInput = false;
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (menuStyle == null) { intitializeMenu(); }

            if (isMenuOpen)
            {
                GUI.Box(new Rect(MENUX, MENUY, MENUWIDTH, MENUHEIGHT), "TestMod", menuStyle);

                toolbarInt = GUI.Toolbar(new Rect(MENUX, MENUY - 30, MENUWIDTH, 30), toolbarInt, toolbarStrings, buttonStyle);

                switch (toolbarInt)
                {
                    case 0:
                        guiEnableGod = GUI.Toggle(new Rect(MENUX, MENUY + 30, ITEMWIDTH, 30), guiEnableGod, "God Mode", toggleStyle);
                        guiEnableSpeedCheat = GUI.Toggle(new Rect(MENUX, MENUY + 60, ITEMWIDTH, 30), guiEnableSpeedCheat, "Speed Cheat", toggleStyle);
                        guiEnableInfiniteSprint = GUI.Toggle(new Rect(MENUX, MENUY + 90, ITEMWIDTH, 30), guiEnableInfiniteSprint, "Infinite Sprint", toggleStyle);
                        guiEnableInfiniteWeight = GUI.Toggle(new Rect(MENUX, MENUY + 120, ITEMWIDTH, 30), guiEnableInfiniteWeight, "Infinite Weight", toggleStyle);
                        guiEnableTwoHanded = GUI.Toggle(new Rect(MENUX, MENUY + 150, ITEMWIDTH, 30), guiEnableTwoHanded, "Pocket Twohanded Scrap", toggleStyle);
                        guiEnableAlwaysScannable = GUI.Toggle(new Rect(MENUX, MENUY + 180, ITEMWIDTH, 30), guiEnableAlwaysScannable, "Scan Everything Everywhere", toggleStyle);
                        guiEnableInfiniteReach = GUI.Toggle(new Rect(MENUX, MENUY + 210, ITEMWIDTH, 30), guiEnableInfiniteReach, "Infinite Reach", toggleStyle);
                        guiEnableNightVision = GUI.Toggle(new Rect(MENUX, MENUY + 240, ITEMWIDTH, 30), guiEnableNightVision, "Night Vision", toggleStyle);
                        guiEnableInfiniteBattery = GUI.Toggle(new Rect(MENUX, MENUY + 270, ITEMWIDTH, 30), guiEnableInfiniteBattery, "Infinite Battery", toggleStyle);
                        guiEnableInfiniteShells = GUI.Toggle(new Rect(MENUX, MENUY + 300, ITEMWIDTH, 30), guiEnableInfiniteShells, "Infinite Bullets", toggleStyle);
                        guiEnableMuteMic = GUI.Toggle(new Rect(MENUX, MENUY + 330, ITEMWIDTH, 30), guiEnableMuteMic, "Mute Microphone", toggleStyle);
                        guiEnableFloating = GUI.Toggle(new Rect(MENUX, MENUY + 360, ITEMWIDTH, 30), guiEnableFloating, "Enable Floating", toggleStyle);

                        GUI.Label(new Rect(MENUX, MENUY + 390, ITEMWIDTH, 30), $"Player Jump Force: {guiPlayerJumpForce.ToString()} (default 13)", labelStyle);
                        guiPlayerStringJumpForce = GUI.TextField(new Rect(MENUX, MENUY + 420, ITEMWIDTH, 30), guiPlayerStringJumpForce, 5, textFeildStyle);

                        bool result = float.TryParse(guiPlayerStringJumpForce, out guiPlayerJumpForce);
                        if (result)
                        {
                            if (guiPlayerJumpForce > 9999f)
                            {
                                guiPlayerJumpForce = 9999f;
                                guiPlayerStringJumpForce = "9999";
                            }
                            else if (guiPlayerJumpForce < -9999f)
                            {
                                guiPlayerJumpForce = -9999f;
                                guiPlayerStringJumpForce = "-9999";
                            }
                        }
                        else
                        {
                            guiPlayerJumpForce = 13f;
                        }

                        guiSpawnGun = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 30, ITEMWIDTH, 30), "Spawn Gun", buttonStyle);
                        guiSpawnStopSign = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 60, ITEMWIDTH, 30), "Spawn Stop Sign", buttonStyle);
                        guiSpawnYieldSign = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 90, ITEMWIDTH, 30), "Spawn Yield Sign", buttonStyle);
                        guiStartShipAnimation = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 120, ITEMWIDTH, 30), "?????", buttonStyle);
                        guiOpenShipDoor = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 150, ITEMWIDTH, 30), "Open Ship Door", buttonStyle);
                        guiCloseShipDoor = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 180, ITEMWIDTH, 30), "Close Ship Door", buttonStyle);

                        if (guiStartShipAnimation)
                        {
                            StartOfRound startofround = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                            startofround.shipAnimator.ResetTrigger("ShipLeave");
                            startofround.shipAnimator.SetTrigger("ShipLeave");
                        }

                        if (guiOpenShipDoor)
                        {
                            StartOfRound startofround = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                            HangarShipDoor shipDoor = UnityEngine.Object.FindFirstObjectByType<HangarShipDoor>();
                            shipDoor.PlayDoorAnimation(false);
                            startofround.SetDoorsClosedServerRpc(false);
                            startofround.hangarDoorsClosed = false;
                        }

                        if (guiCloseShipDoor)
                        {
                            StartOfRound startofround = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                            HangarShipDoor shipDoor = UnityEngine.Object.FindFirstObjectByType<HangarShipDoor>();
                            shipDoor.PlayDoorAnimation(true);
                            startofround.SetDoorsClosedServerRpc(true);
                            startofround.hangarDoorsClosed = false;
                        }

                        break;
                    case 1:
                        guiTeleportToShipButtonPressed = GUI.Button(new Rect(MENUX, MENUY + 30, ITEMWIDTH, 30), "Teleport To Ship", buttonStyle);
                        guiTeleportToEntranceButtonPressed = GUI.Button(new Rect(MENUX, MENUY + 60, ITEMWIDTH, 30), "Teleport To Entrance", buttonStyle);
                        guiTeleportToScrapButtonPressed = GUI.Button(new Rect(MENUX, MENUY + 90, ITEMWIDTH, 30), "Teleport To Scrap", buttonStyle);
                        guiFunnyButtonPressed = GUI.Button(new Rect(MENUX, MENUY + 120, ITEMWIDTH, 30), "Funny Button", buttonStyle);
                        guiTeleportForwardButtonPressed = GUI.Button(new Rect(MENUX, MENUY + 150, ITEMWIDTH, 30), "Teleport Forward", buttonStyle);

                        int offset = 0;
                        for (int i = 0; i < guiPlayerList.Count; i++)
                        {
                            PlayerControllerB player = guiPlayerList[i];
                            if (player != null && player.playerSteamId != 0)
                            {
                                guiTeleportToPlayersButtonPressed[i] = GUI.Button(new Rect(MENUX + ITEMWIDTH, MENUY + 30 + offset, ITEMWIDTH, 30), $"Teleport To {player.playerUsername}", buttonStyle);
                                offset += 30;
                            }
                            else
                            {
                                guiTeleportToPlayersButtonPressed[i] = false;
                            }
                        }
                        if (!guiTeleportMenuOpened)
                        {
                            guiTeleportMenuOpened = true;
                        }
                        break;
                    case 2:
                        GUI.Label(new Rect(MENUX, MENUY + 30, ITEMWIDTH, 30), $"Quota Needed", labelStyle);
                        guiQuotaNeededString = GUI.TextField(new Rect(MENUX, MENUY + 60, ITEMWIDTH - 30, 30), guiQuotaNeededString, 5, textFeildStyle);
                        guiSetQuotaNeeded = GUI.Button(new Rect(MENUX + ITEMWIDTH - 30, MENUY + 60, 30, 30), "=", buttonStyle);

                        GUI.Label(new Rect(MENUX + ITEMWIDTH, MENUY + 30, ITEMWIDTH, 30), $"Deadline", labelStyle);
                        guiDeadlineString = GUI.TextField(new Rect(MENUX + ITEMWIDTH, MENUY + 60, ITEMWIDTH - 30, 30), guiDeadlineString, 5, textFeildStyle);
                        guiSetDeadline = GUI.Button(new Rect(MENUX + ITEMWIDTH + ITEMWIDTH - 30, MENUY + 60, 30, 30), "=", buttonStyle);

                        GUI.Label(new Rect(MENUX, MENUY + 90, ITEMWIDTH, 30), $"Group Credits", labelStyle);
                        guiCreditString = GUI.TextField(new Rect(MENUX, MENUY + 120, ITEMWIDTH - 30, 30), guiCreditString, 5, textFeildStyle);
                        guiSetCredits = GUI.Button(new Rect(MENUX + ITEMWIDTH - 30, MENUY + 120, 30, 30), "=", buttonStyle);

                        result = int.TryParse(guiQuotaNeededString, out guiQuotaNeeded);
                        if (result && guiSetQuotaNeeded)
                        {
                            TimeOfDay timeofday = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
                            if (timeofday != null)
                            {
                                timeofday.profitQuota = guiQuotaNeeded;
                                float baseIncrease = timeofday.quotaVariables.baseIncrease;
                                timeofday.quotaVariables.baseIncrease = 0f;
                                timeofday.SetNewProfitQuota();
                                timeofday.quotaVariables.baseIncrease = baseIncrease;
                            }
                        }

                        result = int.TryParse(guiDeadlineString, out guiDeadline);
                        if (result && guiSetDeadline)
                        {
                            TimeOfDay timeofday = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
                            if (timeofday != null)
                            {
                                timeofday.quotaVariables.deadlineDaysAmount = guiDeadline;
                                float baseIncrease = timeofday.quotaVariables.baseIncrease;
                                timeofday.quotaVariables.baseIncrease = 0f;
                                timeofday.SetNewProfitQuota();
                                timeofday.quotaVariables.baseIncrease = baseIncrease;
                            }
                        }

                        result = int.TryParse(guiCreditString, out guiCredits);
                        if (result && guiSetCredits)
                        {
                            guiSetCredits = false;
                            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                            if (terminal != null)
                            {
                                terminal.groupCredits = guiCredits;
                            }
                        }

                        result = int.TryParse(guiCreditString, out guiCredits);
                        if (result && guiSetCredits)
                        {
                            guiSetCredits = false;
                            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                            if (terminal != null)
                            {
                                terminal.groupCredits = guiCredits;
                                terminal.SyncGroupCreditsServerRpc(terminal.groupCredits, terminal.numberOfItemsInDropship);
                            }
                        }
                        break;
                }
            }
        }
    }
}