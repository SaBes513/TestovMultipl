using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public static class SingletonExtension
{
    public static bool InitializeSingleton<T>(this T _this, ref T _main) where T : MonoBehaviour
    {
        if (_main != null && _main == _this)
            return true;
        if (_main != null)
        {
            MonoBehaviour.Destroy(_this.gameObject);
            return false;
        }
        _main = _this;
        if (Application.isPlaying)
        {
            _this.transform.SetParent(null);
            MonoBehaviour.DontDestroyOnLoad(_this.gameObject);
        }
        return true;
    }
}

public class NetManager : NetworkManager
{
    public new static NetManager singleton => (NetManager)NetworkManager.singleton;
    public bool Play = false, _GameIsActive = false;
    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<PlayerData>(OnAcceptPlayerData);
    }

    private void OnAcceptPlayerData(NetworkConnectionToClient conn, PlayerData playerData)
    {
        if (_GameIsActive) { conn.Disconnect(); return; }
        var player = Instantiate(playerPrefab).GetComponent<Player>();
        player.data = playerData;
        NetworkServer.AddPlayerForConnection(conn, player.gameObject);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.Send(PlayerManager.Main._LocalData);
    }
}
