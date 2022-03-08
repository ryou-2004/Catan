using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PunCallback : MonoBehaviourPunCallbacks
{
    public GameObject error;
    public Toggle toggle;
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GameController._isGameNow)
        {
            error.SetActive(true);
            toggle.interactable = false;
        }
    }
}
