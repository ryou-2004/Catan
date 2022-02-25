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


public class LoginController : MonoBehaviour
{
    public InputField roomId_inputField;

    private int _myNumber;
    private int _playerCount = 2;
    private int _loadCompleteNum;
    private bool _isStart;
    private string _gameScene = "Game";
    private PhotonView _view;
    private AsyncOperation _asyncOperation;

    public static int s_myNumber;
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();//Photonに接続
        }
        roomId_inputField.text = "room";
    }
    public async void LoadScene()
    {
        await Connect();
        await Load();
    }
    private async UniTask Connect()
    {
        await UniTask.WaitUntil(() => PhotonNetwork.IsConnectedAndReady);

        string roomName = roomId_inputField.text;
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions() { MaxPlayers = (byte)2, }, TypedLobby.Default);

        await UniTask.WaitUntil(() => PhotonNetwork.InRoom);
        print("connect");
    }
    private async UniTask Load()
    {
        if (_asyncOperation == null)
        {
            _asyncOperation = SceneManager.LoadSceneAsync(_gameScene);
            _asyncOperation.allowSceneActivation = false;
        }
        await UniTask.WaitUntil(() => _asyncOperation.progress >= 0.9f);
        print("シーンロード完了");

        _view.RPC("LoadComplete", RpcTarget.MasterClient);
        while (true)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                await UniTask.WaitUntil(() => _loadCompleteNum == _playerCount);
                _view.RPC("GameStart", RpcTarget.AllViaServer);
            }
            await UniTask.WaitUntil(() => _isStart);
            _asyncOperation.allowSceneActivation = true;
            break;
        }


    }
    [PunRPC]
    private void LoadComplete()
    {
        _loadCompleteNum++;
    }
    [PunRPC]
    private void GameStart()
    {
        _isStart = true;
    }
}
