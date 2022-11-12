using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;

    [SerializeField] TMP_Text playerNameText;

    public void Set(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.TryGetValue(PropertyNames.Player.AvatarIndex, out var value))
            avatarImage.sprite = avatarSprites[(int)value];

        playerNameText.text = player.NickName;

        if (player == PhotonNetwork.MasterClient)
            playerNameText.text += "(Host)";
    }
}
