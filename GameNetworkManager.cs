using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using LCTestMod;
using Unity.Netcode;
using UnityEngine;

namespace LCTestMod
{
    public class NetworkObjectManager
    {

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        public static void Init()
        {

            NetworkManager.Singleton.AddNetworkPrefab(TestModBase.GUIObject);

        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {

                var networkHandlerHost = UnityEngine.Object.Instantiate(TestModBase.GUIObject, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();

            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.GenerateNewFloor))]
        static void SubscribeToHandler()
        {
            NetworkHandler.TestEvent += ReceivedEventFromServer;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
        static void UnsubscribeFromHandler()
        {
            NetworkHandler.TestEvent -= ReceivedEventFromServer;
        }

        static void ReceivedEventFromServer(string eventName)
        {
            // Event Code Here
        }

        static void SendEventToClients(string eventName)
        {
            if (!(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
                return;

            NetworkHandler.Instance.EventClientRpc(eventName);
        }

        static GameObject networkPrefab;
    }
}