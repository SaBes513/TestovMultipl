using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct PlayerData : NetworkMessage
{
    public string Name;
    public int Score;
}

public class Player : NetworkBehaviour
{
    [SyncVar] public PlayerData data;
    public PlayerState characterPrefab;
    public PlayerState playerState; //Only Server

    public static Player main { get; private set; }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        main = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerManager.Main.AddPlayer(this);
    }

    public override void OnStopClient()
    {
        PlayerManager.Main.RemovePlayer(this);
        base.OnStopClient();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    [Command]
    public void CmdSetName(string name)
    {
        RpcSetName(name);
    }

    [ClientRpc]
    public void RpcSetName(string name)
    {
        data.Name = name;
        UIManager.main.Lobby.UpdatePlayer(this);
    }

    [ClientRpc]
    public void RpcSetScore(int score)
    {
        data.Score = score;
        UIManager.main.Lobby.UpdatePlayer(this);
    }

    [Server]
    public void Spawn(Vector3 pos)
    {
        if (playerState == null)
        {
            playerState = Instantiate(characterPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(playerState.gameObject, gameObject);
        }
    }

    [TargetRpc]
    internal void TRpcChangeScene(string scene)
    {
        if (!SceneManager.GetSceneByName(scene).IsValid())
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    [TargetRpc]
    internal void TRpcSnowWinner(PlayerData data)
    {
        UIManager.main.SnowWin(data);
        GameManager.main.WinRestart();
    }
}
