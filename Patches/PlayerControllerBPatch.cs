using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace LCTestMod.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch("AllowPlayerDeath")]
        [HarmonyPrefix]
        static bool GodModePatch()
        {
            return !TestModBase.myGUI.guiEnableGod;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePatch()
        {
            if (StartOfRound.Instance.localPlayerController == null)
            {
                return;
            }

            if (StartOfRound.Instance.localPlayerController.isPlayerDead)
            {
                TestModBase.myGUI.wasKeyDown = false;
                TestModBase.myGUI.isMenuOpen = false;
                return;
            }

            StartOfRound.Instance.localPlayerController.jumpForce = TestModBase.myGUI.guiPlayerJumpForce;

            if (TestModBase.myGUI.guiEnableGod)
            {
                StartOfRound.Instance.localPlayerController.voiceMuffledByEnemy = false;
                StartOfRound.Instance.localPlayerController.playersManager.fearLevel = 0f;
                StartOfRound.Instance.localPlayerController.health = 100;
                StartOfRound.Instance.localPlayerController.criticallyInjured = false;
                StartOfRound.Instance.localPlayerController.isSinking = false;
                StartOfRound.Instance.localPlayerController.sinkingValue = 0;
            }

            if (TestModBase.myGUI.guiEnableSpeedCheat)
            {
                StartOfRound.Instance.localPlayerController.isSpeedCheating = true;
            }
            else
            {
                StartOfRound.Instance.localPlayerController.isSpeedCheating = false;
            }

            if (TestModBase.myGUI.guiEnableInfiniteSprint)
            {
                StartOfRound.Instance.localPlayerController.sprintMeter = 1f;
            }

            if (TestModBase.myGUI.guiEnableInfiniteWeight)
            {
                StartOfRound.Instance.localPlayerController.carryWeight = 1f;
            }

            if (TestModBase.myGUI.guiEnableTwoHanded)
            {
                StartOfRound.Instance.localPlayerController.twoHanded = false;
            }

            if (TestModBase.myGUI.guiEnableInfiniteReach)
            {
                StartOfRound.Instance.localPlayerController.grabDistance = 9999f;
            }
            else
            {
                StartOfRound.Instance.localPlayerController.grabDistance = 5f;
            }

            if (TestModBase.myGUI.guiEnableNightVision)
            {
                StartOfRound.Instance.localPlayerController.nightVision.enabled = true;
            }
            else
            {
                StartOfRound.Instance.localPlayerController.nightVision.enabled = false;
            }

            if (TestModBase.myGUI.guiEnableInfiniteBattery)
            {
                foreach (GrabbableObject item in StartOfRound.Instance.localPlayerController.ItemSlots)
                {
                    if (item != null && item.itemProperties.requiresBattery)
                    {
                        item.insertedBattery = new Battery(isEmpty: false, 1f);
                        item.SyncBatteryServerRpc(100);
                    }
                }
            }

            if (TestModBase.myGUI.guiEnableMuteMic)
            {
                IngamePlayerSettings micSettings = UnityEngine.Object.FindObjectOfType<IngamePlayerSettings>();
                micSettings.settings.micEnabled = false;
            }
            else
            {
                IngamePlayerSettings micSettings = UnityEngine.Object.FindObjectOfType<IngamePlayerSettings>();
                micSettings.settings.micEnabled = true;
            }

            if (TestModBase.myGUI.guiEnableFloating)
            {
                StartOfRound.Instance.localPlayerController.fallValue = 1f;
                StartOfRound.Instance.localPlayerController.fallValueUncapped = 0f;
            }

            if (TestModBase.myGUI.guiSpawnGun)
            {
                TestModBase.myGUI.guiSpawnGun = false;
                SpawnItem("Shotgun");
            }

            if (TestModBase.myGUI.guiSpawnStopSign)
            {
                TestModBase.myGUI.guiSpawnStopSign = false;
                SpawnItem("stop sign");
            }

            if (TestModBase.myGUI.guiSpawnYieldSign)
            {
                TestModBase.myGUI.guiSpawnYieldSign = false;
                SpawnItem("yield sign");
            }

            if (TestModBase.myGUI.guiTeleportToShipButtonPressed)
            {
                TestModBase.myGUI.guiTeleportToShipButtonPressed = false;
                TeleportToShip();
            }

            if (TestModBase.myGUI.guiTeleportToScrapButtonPressed)
            {
                TestModBase.myGUI.guiTeleportToScrapButtonPressed = false;
                TeleportToScrap();
            }

            if (TestModBase.myGUI.guiFunnyButtonPressed)
            {
                TestModBase.myGUI.guiFunnyButtonPressed = false;
                FunnyButton();
            }

            if (TestModBase.myGUI.guiTeleportToEntranceButtonPressed)
            {
                TestModBase.myGUI.guiTeleportToEntranceButtonPressed = false;
                TeleportToEntrance();
            }

            if (TestModBase.myGUI.guiTeleportForwardButtonPressed)
            {
                TestModBase.myGUI.guiTeleportForwardButtonPressed = false;
                TeleportForward();
            }

            if (TestModBase.myGUI.guiTeleportMenuOpened)
            {
                for (int i = 0; i < TestModBase.myGUI.guiPlayerList.Count; i++)
                {
                    if (TestModBase.myGUI.guiTeleportToPlayersButtonPressed[i])
                    {
                        TestModBase.myGUI.guiTeleportToPlayersButtonPressed[i] = false;
                        StartOfRound.Instance.localPlayerController.TeleportPlayer(TestModBase.myGUI.guiPlayerList[i].transform.position);
                    }
                }
            }

            CleanupExplosions();
        }

        internal static void TeleportToShip()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead) { return; }
            if ((bool)UnityEngine.Object.FindObjectOfType<AudioReverbPresets>())
            {
                UnityEngine.Object.FindObjectOfType<AudioReverbPresets>().audioPresets[3].ChangeAudioReverbForPlayer(StartOfRound.Instance.localPlayerController);
            }
            StartOfRound.Instance.localPlayerController.isInHangarShipRoom = true;
            StartOfRound.Instance.localPlayerController.isInElevator = true;
            StartOfRound.Instance.localPlayerController.isInsideFactory = false;
            StartOfRound.Instance.localPlayerController.TeleportPlayer(StartOfRound.Instance.middleOfShipNode.position);
        }

        internal static void TeleportToEntrance()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead) { return; }
            if ((bool)UnityEngine.Object.FindObjectOfType<AudioReverbPresets>())
            {
                UnityEngine.Object.FindObjectOfType<AudioReverbPresets>().audioPresets[2].ChangeAudioReverbForPlayer(StartOfRound.Instance.localPlayerController);
            }
            StartOfRound.Instance.localPlayerController.isInHangarShipRoom = false;
            StartOfRound.Instance.localPlayerController.isInElevator = false;
            StartOfRound.Instance.localPlayerController.isInsideFactory = true;
            StartOfRound.Instance.localPlayerController.TeleportPlayer(RoundManager.FindMainEntrancePosition());
        }

        internal static void TeleportToScrap()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead) { return; }
            var rng = new System.Random();
            GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
            rng.Shuffle(array);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null && array[i].itemProperties.isScrap && !array[i].isHeld && !array[i].isInShipRoom && array[i].gameObject.GetComponent<GiftTag>() == null)
                {
                    if (array[i])
                        TestModBase.Instance.mls.LogInfo("Found item" + array[i].itemProperties.itemName);
                    StartOfRound.Instance.localPlayerController.TeleportPlayer(array[i].transform.position);
                    break;
                }
            }
        }

        internal static void FunnyButton()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead) { return; }
            Landmine[] funnys = UnityEngine.Object.FindObjectsOfType<Landmine>();
            bool foundFunny = false;
            for (int i = 0; i < funnys.Length; i++)
            {
                if (funnys[i] != null)
                {
                    if (!funnys[i].hasExploded)
                    {
                        StartOfRound.Instance.localPlayerController.TeleportPlayer(funnys[i].transform.position);
                        foundFunny = true;
                        break;
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(funnys[i].gameObject);
                    }
                }
            }
            if (!foundFunny)
            {
                //Landmine.SpawnExplosion(StartOfRound.Instance.localPlayerController.transform.position, true);
                if (NetworkHandler.Instance != null)
                {
                    NetworkHandler.Instance.EventServerRpc("hello world!");
                }
                else
                {
                    TestModBase.Instance.mls.LogInfo("no instance");
                }
            }
        }

        internal static void TeleportToSpace()
        {
            if (StartOfRound.Instance.localPlayerController.isPlayerDead) { return; }
            StartOfRound.Instance.localPlayerController.isInHangarShipRoom = false;
            StartOfRound.Instance.localPlayerController.isInElevator = false;
            StartOfRound.Instance.localPlayerController.isInsideFactory = false;
            StartOfRound.Instance.localPlayerController.TeleportPlayer(StartOfRound.Instance.middleOfSpaceNode.position);
        }

        internal static void TeleportForward()
        {
            StartOfRound.Instance.localPlayerController.TeleportPlayer((StartOfRound.Instance.localPlayerController.transform.position + StartOfRound.Instance.localPlayerController.transform.forward * 5) + new Vector3(0f, 0.5f, 0f));
        }

        internal static void CleanupExplosions()
        {
            TagComponent[] objects = UnityEngine.Object.FindObjectsOfType<TagComponent>();
            foreach (TagComponent component in objects)
            {
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component.gameObject, 3f);
                }
            }
        }

        internal static void SpawnItem(string itemName)
        {
            itemName = itemName.ToLower();

            // Get a list of all items
            var allItems = StartOfRound.Instance.allItemsList;
            var item = allItems.itemsList.Find(i => i.itemName.ToLower().StartsWith(itemName));
            //for (int i = 0; i < allItems.itemsList.Count; i++)
            //{
            //    TestModBase.Instance.mls.LogInfo(allItems.itemsList[i].itemName.ToLower());
            //}

            if (item == null)
            {
                TestModBase.Instance.mls.LogInfo($"Could not find item: {itemName}");
                HUDManager.Instance.DisplayTip($"Could not find item: {itemName}", "", isWarning: true);
                return;
            }

            TestModBase.Instance.mls.LogInfo($"Spawned item: {item.itemName}");

            // Get the transform of the player
            var playerTransform = StartOfRound.Instance.localPlayerController.transform;
            // Spawn the item
            var prop = UnityEngine.Object.Instantiate(item.spawnPrefab, playerTransform.position + playerTransform.forward * 2.0f, playerTransform.rotation, RoundManager.Instance.spawnedScrapContainer);
            prop.transform.position = playerTransform.position + playerTransform.forward * 2.0f;
            var grabbable = prop.GetComponent<GrabbableObject>();
            grabbable.fallTime = 1.0f;
            grabbable.scrapPersistedThroughRounds = false;
            grabbable.grabbable = true;

            // Check if it's scrap
            if (item.isScrap)
            {
                // Set the scrap value
                grabbable.SetScrapValue(UnityEngine.Random.Range(item.minValue, item.maxValue));
                TestModBase.Instance.mls.LogInfo($"Set scrap value to {grabbable.scrapValue}");

                grabbable.NetworkObject.Spawn(false);
            }
        }
    }

    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("MeetsScanNodeRequirements")]
        private static void alterReqs(ScanNodeProperties node, ref bool __result)
        {
            if ((UnityEngine.Object)(object)node == (UnityEngine.Object)null)
            {
                __result = false;
                return;
            }

            if (TestModBase.myGUI.guiEnableAlwaysScannable)
            {
                __result = true;
            }
        }
    }

    internal class TagComponent : MonoBehaviour
    {
        public string stringTag = "explosionTag";
    }

    internal class GiftTag : MonoBehaviour
    {
        public string stringTag = "GiftBoxTag";
    }

    static class RandomExtensions
    {
        public static void Shuffle<T>(this System.Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Awake")]
        private static void AwakePatch(StartOfRound __instance)
        {
            __instance.explosionPrefab.AddComponent<TagComponent>();
        }
    }

    [HarmonyPatch(typeof(GiftBoxItem))]
    internal class GiftBoxItemPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ItemActivate")]
        private static void ItemActivatePatch(GiftBoxItem __instance)
        {
            __instance.gameObject.AddComponent<GiftTag>();
            UnityEngine.Object.Destroy(__instance.gameObject, 1f);
        }
    }

    [HarmonyPatch(typeof(Shovel))]
    internal class ShovelPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("HitShovel")]
        private static void HitShovelPatch(Shovel __instance, ref List<RaycastHit> ___objectsHitByShovelList, ref PlayerControllerB ___previousPlayerHeldBy)
        {
            //shovelHitForce, itemProperties.weight
            //shovel = 1, 1.18
            //stop sign = 1, 1.2
            //yield sign = 1, 1.4
            TestModBase.Instance.mls.LogInfo($"{__instance.shovelHitForce}, {__instance.itemProperties.weight}");
            if (__instance.itemProperties.weight == 1.4f)
            {
                Vector3 start = ___previousPlayerHeldBy.gameplayCamera.transform.position;
                for (int i = 0; i < ___objectsHitByShovelList.Count; i++)
                {
                    IHittable component;
                    RaycastHit hitInfo;
                    if (___objectsHitByShovelList[i].transform.gameObject.layer == 8 || ___objectsHitByShovelList[i].transform.gameObject.layer == 11)
                    {
                        Landmine.SpawnExplosion(___objectsHitByShovelList[i].point, true);
                        break;
                    }
                    else if (___objectsHitByShovelList[i].transform.TryGetComponent<IHittable>(out component) && !(___objectsHitByShovelList[i].transform == ___previousPlayerHeldBy.transform) && (___objectsHitByShovelList[i].point != Vector3.zero && !Physics.Linecast(start, ___objectsHitByShovelList[i].point, out hitInfo, StartOfRound.Instance.collidersAndRoomMaskAndDefault)))
                    {
                        Landmine.SpawnExplosion(___objectsHitByShovelList[i].point, true);
                        break;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        private static void InfiniteAmmoPatch(ShotgunItem __instance)
        {
            if (TestModBase.myGUI.guiEnableInfiniteShells)
            {
                __instance.shellsLoaded = 2;
            }
        }
    }

    [HarmonyPatch(typeof(ShipLights))]
    internal class ShipLightsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ToggleShipLights")]
        private static void RandomLightKill()
        {
            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
            int num = UnityEngine.Random.Range(0, 10);
            if (num == 3)
            {
                TestModBase.myGUI.guiEnableGod = false;
                Landmine.SpawnExplosion(StartOfRound.Instance.localPlayerController.transform.position, true);
            }
        }
    }
}
