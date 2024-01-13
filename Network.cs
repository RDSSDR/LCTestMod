using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LCTestMod
{
    public class NetworkHandler : NetworkBehaviour
    {
        public static NetworkHandler Instance { get; private set; }

        public static event Action<String> TestEvent;

        public override void OnNetworkSpawn()
        {
            TestEvent = null;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;


            base.OnNetworkSpawn();
        }


        [ClientRpc]
        public void EventClientRpc(string eventName)
        {
            //TestEvent?.Invoke(eventName);
            if (TestModBase.Instance != null)
            {
                TestModBase.Instance.mls.LogInfo(eventName);
            }
            else
            {
                Debug.Log("no testmod instance");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void EventServerRpc(string eventName)
        {
            EventClientRpc(eventName);
        }

        public static void SpawnExplosion(Vector3 explosionPosition, bool spawnExplosionEffect = false, float killRange = 1f, float damageRange = 1f)
        {
            Debug.Log("Spawning explosion at pos: {explosionPosition}");
            if (spawnExplosionEffect)
            {
                GameObject explodeObj = UnityEngine.Object.Instantiate(StartOfRound.Instance.explosionPrefab, explosionPosition, Quaternion.Euler(-90f, 0f, 0f), RoundManager.Instance.mapPropsContainer.transform);
                explodeObj.SetActive(value: true);
                Destroy(explodeObj, 5f);
            }
            float num = Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, explosionPosition);
            if (num < 14f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Big);
            }
            else if (num < 25f)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }
            Collider[] array = Physics.OverlapSphere(explosionPosition, 6f, 2621448, QueryTriggerInteraction.Collide);
            PlayerControllerB playerControllerB = null;
            for (int i = 0; i < array.Length; i++)
            {
                float num2 = Vector3.Distance(explosionPosition, array[i].transform.position);
                if (num2 > 4f && Physics.Linecast(explosionPosition, array[i].transform.position + Vector3.up * 0.3f, 256, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }
                if (array[i].gameObject.layer == 3)
                {
                    playerControllerB = array[i].gameObject.GetComponent<PlayerControllerB>();
                    if (playerControllerB != null && playerControllerB.IsOwner)
                    {
                        if (num2 < killRange)
                        {
                            Vector3 bodyVelocity = (playerControllerB.gameplayCamera.transform.position - explosionPosition) * 80f / Vector3.Distance(playerControllerB.gameplayCamera.transform.position, explosionPosition);
                            playerControllerB.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                        }
                        else if (num2 < damageRange)
                        {
                            playerControllerB.DamagePlayer(50);
                        }
                    }
                }
                else if (array[i].gameObject.layer == 21)
                {
                    Landmine componentInChildren = array[i].gameObject.GetComponentInChildren<Landmine>();
                    if (componentInChildren != null && !componentInChildren.hasExploded && num2 < 6f)
                    {
                        Debug.Log("Setting off other mine");
                        componentInChildren.StartCoroutine(componentInChildren.TriggerOtherMineDelayed(componentInChildren));
                    }
                }
                else if (array[i].gameObject.layer == 19)
                {
                    EnemyAICollisionDetect componentInChildren2 = array[i].gameObject.GetComponentInChildren<EnemyAICollisionDetect>();
                    if (componentInChildren2 != null && componentInChildren2.mainScript.IsOwner && num2 < 4.5f)
                    {
                        componentInChildren2.mainScript.HitEnemyOnLocalClient(6);
                    }
                }
            }
            int num3 = ~LayerMask.GetMask("Room");
            num3 = ~LayerMask.GetMask("Colliders");
            array = Physics.OverlapSphere(explosionPosition, 10f, num3);
            for (int j = 0; j < array.Length; j++)
            {
                Rigidbody component = array[j].GetComponent<Rigidbody>();
                if (component != null)
                {
                    component.AddExplosionForce(70f, explosionPosition, 10f);
                }
            }
        }
    }
}
