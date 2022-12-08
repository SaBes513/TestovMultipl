using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILobby : MonoBehaviour
{
    public UIPlayer playerPrefab;

    Dictionary<uint, UIPlayer> list = new Dictionary<uint, UIPlayer>();

    private void OnEnable()
    {
        PlayerManager.Main.playerAdded += AddPlayer;
        PlayerManager.Main.playerRemoved += RemovePlayer;
    }
    private void OnDisable()
    {
        PlayerManager.Main.playerAdded -= AddPlayer;
        PlayerManager.Main.playerRemoved -= RemovePlayer;
    }

    public void AddPlayer(Player player)
    {
        var item = Instantiate(playerPrefab);
        item.SetPlayer(player);
        item.transform.SetParent(transform);
        list.Add(player.netId, item);
    }

    public void RemovePlayer(Player player)
    {
        Destroy(list[player.netId].gameObject);
        list.Remove(player.netId);
    }
    public void UpdatePlayer(Player player)
    {
        list[player.netId].SetPlayer(player);
    }
}
