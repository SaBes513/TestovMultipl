using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    public UILobby Lobby;
    public GameObject Menu, TurnManager, BStartGame, BHost, BJoin;
    public TMP_InputField Name, Adr;
    public VerticalLayoutGroup SRPlayers;
    public static bool Host = false;
    public TMP_Text WinText;

    private void Awake()
    {
        main = this;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Host)
        {
            if (scene.name == "Game")
            {
                NetManager.singleton._GameIsActive = true;
                GameManager.main.SpawnPlayers();
                BStartGame.SetActive(false);
            }
            if (scene.name == "Lobby")
            {
                BStartGame.SetActive(true);
            }
        }
        else
        {
            if (scene.name == "Lobby")
            {
                BStartGame.SetActive(false);
            }
            if (scene.name == "Game")
            {
                NetManager.singleton._GameIsActive = true;
            }
        }
    }

    public void HostStart()
    {
        NetManager.singleton.StartHost();
        Host = true;
    }

    public void ClientStart()
    {
        NetManager.singleton.networkAddress = Adr.text != "" ? Adr.text : "localhost";
        NetManager.singleton.StartClient();
        Host = false;
    }

    public void AppName()
    {
        PlayerManager.Main._LocalData.Name = Name.text;
        if (NetworkClient.active)
        {
            Player.main.CmdSetName(Name.text);
        }
    }

    internal void SnowWin(PlayerData data)
    {
        NetManager.singleton._GameIsActive = false;
        WinText.text = "Player " + data.Name + " Win\nRestart after " + GameManager.main.RestartTime + " sec.";
        WinText.transform.parent.gameObject.SetActive(true);
        Invoke("CloseWin", GameManager.main.RestartTime);
    }

    internal void CloseWin()
    {
        NetManager.singleton._GameIsActive = true;
        Cursor.lockState = CursorLockMode.Locked;
        WinText.transform.parent.gameObject.SetActive(false);
        Menu.gameObject.SetActive(false);
    }

    public void Disconnect()
    {
        if (NetworkServer.active)
            NetManager.singleton.StopHost();
        else
            NetManager.singleton.StopClient();
    }

    public void StartGame()
    {
        PlayerManager.Main.ChangeScene("Game");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
Application.Quit();
#endif
    }
}
