using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;


[DisallowMultipleComponent]
[AddComponentMenu("Network/_ServerHUD")]
[RequireComponent(typeof(NetworkDiscovery))]
public class ServersHUD : MonoBehaviour
{
    readonly Dictionary<string, ServerResponse> discoveredServers = new Dictionary<string, ServerResponse>();
    public NetworkDiscovery networkDiscovery;
    public ListOfServers listOfServers;

#if UNITY_EDITOR
    void OnValidate() {
        if (networkDiscovery == null) {
            networkDiscovery = GetComponent<NetworkDiscovery>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnDiscoveredServer);
            UnityEditor.Undo.RecordObjects(new Object[] { this, networkDiscovery }, "Set NetworkDiscovery");
        }
    }

    public void ClearList() {
        discoveredServers.Clear();
	}
#endif
    public void LookForServers() {
        listOfServers.DestroyAllButtons();
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
    }

    void Connect(ServerResponse info) {
        NetworkManager.singleton.StartClient(info.uri);
    }

    public void OnDiscoveredServer(ServerResponse info) {
        //Debug.Log(info.adress);

        // Note that you can check the versioning to decide if you can connect to the server or not using this method
        if (!discoveredServers.ContainsKey(info.name)) {
            discoveredServers[info.name] = info;
            var button = listOfServers.CreateButton(info.adress, info.EndPoint.Address.ToString());
        }
    }

    }


