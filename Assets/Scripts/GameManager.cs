using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public Transform[] pos;
    public int WinScore = 3, RestartTime = 5;

    private void Awake()
    {
        main = this;
    }

    public void AddScore(Player player)
    {
        if (++player.data.Score >= WinScore) Win(player.data);
        else player.RpcSetScore(player.data.Score);
    }

    internal void SpawnPlayers()
    {
        foreach (Player player in PlayerManager.Main.players)
        {
            player.Spawn(pos[Random.Range(0, pos.Length)].position);
        }
    }

    public void Win(PlayerData data)
    {
        foreach (Player player in PlayerManager.Main.players)
        {
            player.TRpcSnowWinner(data);
        }
    }

    [Server]
    public void WinRestart()
    {
        Invoke("Restart", RestartTime);
    }

    [Server]
    void Restart()
    {
        foreach (Player player in PlayerManager.Main.players)
        {
            player.playerState.RpcTransp(pos[Random.Range(0, pos.Length)].position);
            player.RpcSetScore(0);
        }
    }
}
