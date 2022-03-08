using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using System.Linq;
using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;


public class LoginController : MonoBehaviourPunCallbacks
{
    public GameObject clickToStart;
    public GameObject connect;
    public GameObject home;
    public GameObject create;
    public GameObject join;
    public GameObject inRoom;
    public GameObject setName;
    public GameObject kick;
    public GameObject disconnected;

    public GameObject inRoomMaster;
    public GameObject inRoomClient;
    public GameObject createError;
    public GameObject joinError;

    public InputField nameInput;
    public InputField createRoomId;
    public InputField joinRoomId;

    public GameObject nameContent;
    public GameObject nameElement;

    public Button joinButton;
    public Button createButton;
    public Button leaveButton;
    public Button startButton;
    public Button clickToStartButton;
    public Slider playerCountSlider;
    public Text playerCountText;
    public Text kickPlayerNameText;
    public Text playerCount;
    public Image connectImg;

    private Dictionary<int, bool> _redy = new Dictionary<int, bool>();

    private int _playerCount = 3;
    private int _loadCompleteNum;
    private int _kickId;
    private bool _isStart;
    private string _gameScene = "Game";
    private PhotonView _view;
    private AsyncOperation _asyncOperation;

    public static bool isTitle;

    public static int s_myNumber;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        isTitle = true;
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();//Photonに接続
        }
        setName.SetActive(false);
        createRoomId.text = "room";
        PlayerCountChange();
        DisplayChange(clickToStart);
        clickToStartButton.onClick.AddListener(() => DisplayClick());
    }
    public async void DisplayClick()
    {
        connect.SetActive(true);
        clickToStart.SetActive(false);
        //フェード
        await AlphaChange(1);
        await UniTask.WhenAny(UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady),UniTask.Delay(2000));
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            disconnected.SetActive(true);
            return;
        }
        //フェード
        home.SetActive(true);
        setName.SetActive(true);
        await AlphaChange(0);
        connect.SetActive(false);
    }
    private async UniTask AlphaChange(float alpha)
    {
        await DOTween.ToAlpha(
               () => connectImg.color,
               color => connectImg.color = color,
               alpha, // 目標値
               0.5f // 所要時間
           );
    }

    public void RoomCreate()
    {
        string roomName = createRoomId.text;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)_playerCount, }, TypedLobby.Default);
        createButton.interactable = false;
    }
    public void RoomJoin()
    {
        string roomName = joinRoomId.text;
        PhotonNetwork.NickName = nameInput.text;
        PhotonNetwork.JoinRoom(roomName);
    }
    public void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreatedRoom()
    {
        print("部屋立ちました");
        create.SetActive(false);
        inRoom.SetActive(true);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("部屋立ちませんでした");
        createError.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            inRoomMaster.SetActive(true);
            inRoomClient.SetActive(false);
            startButton.gameObject.SetActive(true);
        }
        else
        {
            inRoomMaster.SetActive(false);
            inRoomClient.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
        DisplayChange(inRoom);
        PlayerListUpdate();
        GameSceneLoad();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        joinError.SetActive(true);
    }
    public override void OnLeftRoom()
    {
        DisplayChange(home);
    }
    public override void OnPlayerEnteredRoom(Player newPlaer)
    {
        PlayerListUpdate();
        _redy.Add(newPlaer.ActorNumber, false);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerListUpdate();
        _redy.Remove(otherPlayer.ActorNumber);
        StartCheck();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnected.SetActive(true);
    }
    public async void ReConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
        connect.SetActive(true);
        await AlphaChange(1);
        await UniTask.WhenAny(UniTask.Delay(500), UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady));
        if (PhotonNetwork.IsConnectedAndReady)
        {
            disconnected.SetActive(false);
            DisplayChange(home);
        }
        else
        {
            disconnected.SetActive(true);
            setName.SetActive(true);
        }
        connect.SetActive(true);
        await AlphaChange(0);
        connect.SetActive(false);
    }
    private void KickConf(int id)
    {
        _kickId = id;
        kick.SetActive(true);
    }
    public void Kick()
    {
        _view.RPC("P_Kick", RpcTarget.All, _kickId);
        kickPlayerNameText.text = GetName(PhotonNetwork.PlayerList.ToList().Find(x => x.ActorNumber == _kickId)) + "を強制退出させます";
        kick.SetActive(false);
    }
    [PunRPC]
    private void P_Kick(int id)
    {
        if (id == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            print("蹴られた");
            PhotonNetwork.LeaveRoom();
        }
    }

    public void Dissolve()
    {
        _view.RPC("P_Dissolve", RpcTarget.All);
    }
    [PunRPC]
    private void P_Dissolve()
    {
        print("部屋が解散した");
        PhotonNetwork.LeaveRoom();
        _redy.Clear();
    }

    public void DisplayChange(GameObject displayObj)
    {
        clickToStart.SetActive(false);
        home.SetActive(false);
        create.SetActive(false);
        join.SetActive(false);
        inRoom.SetActive(false);
        connect.SetActive(false);

        displayObj.SetActive(true);
    }
    public void NameChange()
    {
        PhotonNetwork.NickName = nameInput.text;
        _view.RPC("PlayerListUpdate", RpcTarget.All);
    }
    [PunRPC]
    private void PlayerListUpdate()
    {
        foreach (Transform child in nameContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var element = Instantiate(nameElement, nameContent.transform);
            var button = element.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => KickConf(player.ActorNumber));
            var text = element.GetComponentInChildren<Text>();
            text.text = GetName(player);
            if (PhotonNetwork.IsMasterClient && player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    private string GetName(Player player)
    {
        if (string.IsNullOrEmpty(player.NickName) || player.NickName == "Player")
        {
            return "Player" + PhotonNetwork.PlayerList.ToList().IndexOf(player);
        }
        else
        {
            return player.NickName;
        }
    }

    private async void GameSceneLoad()
    {
        if (_asyncOperation == null)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(_gameScene);
            _asyncOperation.allowSceneActivation = false;
        }
        await UniTask.WaitUntil(() => _asyncOperation.progress >= 0.9f);
        _view.RPC("LoadComplete", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void LoadComplete(int id)
    {
        _redy[id] = true;
        StartCheck();
    }
    private void StartCheck()
    {
        var redyCount = _redy.Where(x => x.Value).Count();
        if (3 <= redyCount)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    public void GameStart()
    {
        _view.RPC("P_GameStart", RpcTarget.AllViaServer);
    }
    [PunRPC]
    private void P_GameStart()
    {
        Dice.Clear();
        isTitle = false;
        _asyncOperation.allowSceneActivation = true;
    }

    public void PlayerCountChange()
    {
        _playerCount = (int)playerCountSlider.value;
        playerCountText.text = _playerCount.ToString();
    }
}
