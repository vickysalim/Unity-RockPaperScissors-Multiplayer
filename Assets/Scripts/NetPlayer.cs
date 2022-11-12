using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetPlayer : MonoBehaviourPun
{
    public static List<NetPlayer> NetPlayers = new List<NetPlayer>(2);
    //private Player player;

    private Card[] cards;

    public void Set(Player player)
    {
        player.Name.text = photonView.Owner.NickName;
        //this.player = player;
        cards = player.GetComponentsInChildren<Card>();
        foreach(var card in cards)
        {
            var button = card.GetComponent<Button>();
            button.onClick.AddListener(() => RemoteClickButton(card.AttackValue));
        }
    }

    private void RemoteClickButton(Attack value)
    {
        if(photonView.IsMine)
            photonView.RPC("RemoteClickButtonRPC", RpcTarget.Others, (int) value);
    }

    [PunRPC]
    private void RemoteClickButtonRPC(int value)
    {
        foreach(var card in cards)
        {
            if (card.AttackValue == (Attack)value)
            {
                var button = card.GetComponent<Button>();
                button.onClick.Invoke();
                break;
            }
                
        }
    }

    private void OnEnable()
    {
        NetPlayers.Add(this);    
    }

    private void OnDisable()
    {
        foreach (var card in cards)
        {
            var button = card.GetComponent<Button>();
            button.onClick.RemoveListener(() => RemoteClickButton(card.AttackValue));
        }

        NetPlayers.Remove(this);
    }
}