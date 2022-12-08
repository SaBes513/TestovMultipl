using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<Player> players;
    public PlayerData _LocalData;
    static PlayerManager _main;
    public static PlayerManager Main => _main;

    public Action<Player> playerRemoved { get; internal set; }
    public Action<Player> playerAdded { get; internal set; }

    private void Awake()
    {
        this.InitializeSingleton(ref _main);
    }

    internal void AddPlayer(Player player)
    {
        if (!players.Contains(player))
            players.Add(player);
        playerAdded?.Invoke(player);
    }

    internal void RemovePlayer(Player player)
    {
        players.Remove(player);
        playerRemoved?.Invoke(player);
    }

    internal void ChangeScene(string scene)
    {
        foreach (Player player in players)
        {
            player.TRpcChangeScene(scene);
        }
    }
}
