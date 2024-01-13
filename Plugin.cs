using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LCTestMod.Component;
using LCTestMod.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LCTestMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestModBase : BaseUnityPlugin
    {
        private const string modGUID = "dolphin.LCTestMod";
        private const string modName = "LCTestMod";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static TestModBase Instance;

        internal ManualLogSource mls;

        internal static GUILoader myGUI;

        internal AssetBundle MainAssetBundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("The test mod has awaken :)");

            var gameObject = new UnityEngine.GameObject("GUILoader");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.AddComponent<GUILoader>();
            myGUI = (GUILoader)gameObject.GetComponent("GUILoader");

            MainAssetBundle = AssetBundle.LoadFromMemory(NetworkAsset.assets);

            NetcodePatcher();

            harmony.PatchAll(typeof(TestModBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(GiftBoxItemPatch));
            harmony.PatchAll(typeof(ShovelPatch));
            harmony.PatchAll(typeof(ShotgunPatch));
            harmony.PatchAll(typeof(ShipLightsPatch));
        }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
