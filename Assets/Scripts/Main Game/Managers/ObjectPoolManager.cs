using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace PhotonCourse.Scripts.MainGame.Managers
{
    public class ObjectPoolManager : MonoBehaviour, INetworkObjectPool
    {
        private Dictionary<NetworkObject, List<NetworkObject>> _instansiatedPrefabs = new Dictionary<NetworkObject, List<NetworkObject>>();




        // Game Loop Methods-----------------------------------------------------------------------

        private void Start()
        {
            if (GlobalsManager.Instance != null)
            {
                GlobalsManager.Instance.ObjectPoolManagerInstance = this;
            }
        }
        
        // Memeber Methods-------------------------------------------------------------------------

        private NetworkObject CreateObjectInstance(NetworkObject prefab)
        {
            var createdObj = Instantiate(prefab);

            if (_instansiatedPrefabs.TryGetValue(prefab, out var networkObjectsList))
            {
                // Updated the instantiate items list
                networkObjectsList.Add(createdObj);
            }
            else
            {
                // first create a new key value pair
                List<NetworkObject> list = new List<NetworkObject>() { createdObj };
                _instansiatedPrefabs.Add(prefab, list);
            }


            return createdObj;
        }

        public void RemoveGameobjectFromDictionary(NetworkObject targetNetworkObject)
        {
            if (_instansiatedPrefabs?.Count > 0)
            {
                foreach (var item in _instansiatedPrefabs)
                {
                    foreach (var networkObject in item.Value.Where(networkObject => networkObject == targetNetworkObject))
                    {
                        item.Value.Remove(networkObject);

                        break;
                    }
                }
            }
        }

        // Interface Methods-----------------------------------------------------------------------
        
        public NetworkObject AcquireInstance(NetworkRunner runner, NetworkPrefabInfo info)
        {
            NetworkObject networkObject = null;

            NetworkProjectConfig.Global.PrefabTable.TryGetPrefab(info.Prefab, out var prefab);
            _instansiatedPrefabs.TryGetValue(prefab, out var networkObjects);

            bool matchFound = false;

            if (networkObjects?.Count > 0)
            {
                foreach (var item in networkObjects)
                {
                    if (item && !item.gameObject.activeSelf)
                    {
                        // TODO - Recycle prefabs
                        networkObject = item;
                        matchFound = true;

                        break;
                    }
                }
            }

            if (!matchFound)
            {
                //TODO - Instantiate a new prefab
                networkObject = CreateObjectInstance(prefab);
            }

            return networkObject;
        }

        public void ReleaseInstance(NetworkRunner runner, NetworkObject instance, bool isSceneObject)
        {
            instance.gameObject.SetActive(false);
        }
    }
}
