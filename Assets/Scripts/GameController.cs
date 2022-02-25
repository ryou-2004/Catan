using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class GameController : MonoBehaviour
{
    public GameObject _tileParent;
    public ResourceData _resourceData;

    private PhotonView _view;

    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
        {
            PhotonNetwork.OfflineMode= true;
        }
    }
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            List<int> token = new List<int>()
            {
                2,3,3,4,4,5,5,6,6,8,8,9,9,10,10,11,11,12
            };
            List<int> tileId = new List<int>()
            {
                0,0,0,1,1,1,1,2,2,2,2,3,3,3,3,4,4,4,5
            };
            token = token.OrderBy(a => Guid.NewGuid()).ToList();
            tileId = tileId.OrderBy(a => Guid.NewGuid()).ToList();
            int skipId = UnityEngine.Random.Range(0, _tileParent.transform.childCount);
            _view.RPC("BordSet", RpcTarget.AllViaServer,token.ToArray(),tileId.ToArray(),skipId);
        }
    }

    [PunRPC]
    private void BordSet(int[] token, int[] tileId, int skipId)
    {
        int i = 0;
        foreach (Transform child in _tileParent.transform)
        {
            child.GetComponent<Renderer>().material = _resourceData.sheet[tileId[i]].material;
            if (i == skipId)
            {
                child.name = "砂漠";
                child.Find("Token").GetComponent<TextMesh>().text = "";
                skipId = -1;
            }
            else
            {
                child.name = token[i].ToString();
                child.Find("Token").GetComponent<TextMesh>().text = token[i].ToString();
                i++;
            }
        }
    }
}
