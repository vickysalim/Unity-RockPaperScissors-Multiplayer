using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Button button;
    
    LobbyManager manager;

    RoomInfo roomInfo;
    public void Set(LobbyManager manager, RoomInfo roomInfo)
    {
        this.manager = manager;
        this.roomInfo = roomInfo;

        var defaultRoomInfoText = roomInfo.Name + $" ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
        roomNameText.text = defaultRoomInfoText;

        if (roomInfo.PlayerCount == roomInfo.MaxPlayers)
        {
            button.interactable = false;
            roomNameText.text = defaultRoomInfoText + " (FULL)";
        }

        if (roomInfo.IsOpen == false)
        {
            button.interactable = false;
            roomNameText.text = defaultRoomInfoText + " (IN-GAME)";
        }

    }

    public void ClickRoomName()
    {
        manager.JoinRoom(this.roomInfo.Name);
    }
}
