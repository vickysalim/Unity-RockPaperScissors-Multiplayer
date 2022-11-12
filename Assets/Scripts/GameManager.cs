using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour, IOnEventCallback
{
    public GameObject netPlayerPrefab;

    public Player P1;
    public Player P2;

    public float maxHealth = 100;
    public float restoreValue = 5;
    public float damageValue = 10;

    public GameState State, NextState = GameState.NetPlayersInitialization;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text gameOverText;
    [SerializeField] TMP_Text pingText;

    private Player damagedPlayer;
    
    HashSet<int> syncReadyPlayers = new HashSet<int>(2);

    [SerializeField] Player getLastPlayerWin;
    [SerializeField] int winStreak = 0;

    public enum GameState
    {
        SyncState,
        NetPlayersInitialization,
        ChooseAttack,
        Attacks,
        Damages,
        Draw,
        GameOver
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        
        PhotonNetwork.Instantiate(netPlayerPrefab.name, Vector3.zero, Quaternion.identity);

        StartCoroutine(PingCoroutine());

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.RestoreValue, out var restoreValue))
        {
            this.restoreValue = (float)restoreValue;
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PropertyNames.Room.DamageValue, out var damageValue))
        {
            this.damageValue = (float)damageValue;
        }
    }

    void Update()
    {
        // Choose Attack
        switch (State)
        {
            case GameState.SyncState:
                if(syncReadyPlayers.Count == 2)
                {
                    syncReadyPlayers.Clear();
                    State = NextState;
                }
                break;
            case GameState.NetPlayersInitialization:
                if(NetPlayer.NetPlayers.Count == 2)
                {
                    foreach(var netPlayer in NetPlayer.NetPlayers)
                    {
                        if(netPlayer.photonView.IsMine)
                        {
                            netPlayer.Set(P1);
                        } else
                        {
                            netPlayer.Set(P2);
                        }
                    }

                    ChangeState(GameState.ChooseAttack);
                }
                break;
            case GameState.ChooseAttack:
                if(P1.AttackValue != null && P2.AttackValue != null)
                {
                    P1.AnimateAttack();
                    P2.AnimateAttack();

                    P1.IsClickable(false);
                    P2.IsClickable(false);

                    ChangeState(GameState.Attacks);
                }
                break;
            case GameState.Attacks:
                if(P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    damagedPlayer = GetDamagedPlayer();
                    if(damagedPlayer != null)
                    {
                        damagedPlayer.AnimateDamage();

                        ChangeState(GameState.Damages);
                    } else
                    {
                        P1.AnimateDraw();
                        P2.AnimateDraw();

                        getLastPlayerWin = null;
                        winStreak = 0;

                        ChangeState(GameState.Draw);
                    }

                    P2.ResetAnimateChoose();
                }
                break;
            case GameState.Damages:
                if(P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    if(damagedPlayer == P1)
                    {
                        if (getLastPlayerWin == P2)
                        {
                            winStreak++;
                        } else
                        {
                            winStreak = 1;
                        }

                        P1.ChangeHealth(-(damageValue * (1 * winStreak)));
                        P2.ChangeHealth(restoreValue);

                        getLastPlayerWin = P2;
                        

                    } else
                    {
                        if (getLastPlayerWin == P1)
                        {
                            winStreak++;
                        } else
                        {
                            winStreak = 1;
                        }

                        P1.ChangeHealth(restoreValue);
                        P2.ChangeHealth(-(damageValue * (1 * winStreak)));

                        getLastPlayerWin = P1;
                    }

                    var winner = GetWinner();

                    if(winner == null)
                    {
                        ResetPlayers();

                        P1.IsClickable(true);
                        P2.IsClickable(true);

                        ChangeState(GameState.ChooseAttack);
                    } else
                    {
                        gameOverPanel.SetActive(true);
                        gameOverText.text = winner == P1 ? $"{P1.Name.text} win" : $"{P2.Name.text} win";

                        ResetPlayers();

                        ChangeState(GameState.GameOver);
                    }
                }
                break;
            case GameState.Draw:
                if(P1.IsAnimating() == false && P2.IsAnimating() == false)
                {
                    ResetPlayers();
                    P1.IsClickable(true);
                    P2.IsClickable(true);

                    ChangeState(GameState.ChooseAttack);
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void ChangeState(GameState newState)
    {
        if (this.NextState == newState)
            return;

        var actorNum = PhotonNetwork.LocalPlayer.ActorNumber;
        
        var raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(1, actorNum, raiseEventOptions, SendOptions.SendReliable);
        this.State = GameState.SyncState;
        this.NextState = newState;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            var actorNum = (int)photonEvent.CustomData;

            syncReadyPlayers.Add(actorNum);
        }
    }

    IEnumerator PingCoroutine()
    {
        var wait = new WaitForSeconds(1);
        while(true)
        {
            pingText.text = "Client Ping: " + PhotonNetwork.GetPing() + " ms";
            yield return wait;
        }
    }

    private void ResetPlayers()
    {
        damagedPlayer = null;
        P1.Reset();
        P2.Reset();
    }

    private Player GetDamagedPlayer()
    {
        Attack? P1Attack = P1.AttackValue;
        Attack? P2Attack = P2.AttackValue;

        if(P1Attack == Attack.Ant && P2Attack == Attack.Human)
        {
            return P1;
        } else if(P1Attack == Attack.Ant && P2Attack == Attack.Elephant)
        {
            return P2;
        }
        else if (P1Attack == Attack.Human && P2Attack == Attack.Ant)
        {
            return P2;
        }
        else if (P1Attack == Attack.Human && P2Attack == Attack.Elephant)
        {
            return P1;
        }
        else if (P1Attack == Attack.Elephant && P2Attack == Attack.Ant)
        {
            return P1;
        }
        else if (P1Attack == Attack.Elephant && P2Attack == Attack.Human)
        {
            return P2;
        }

        return null;
    }

    private Player GetWinner()
    {
        if(P1.Health == 0)
        {
            return P2;
        }
        else if(P2.Health == 0)
        {
            return P1;
        } else
        {
            return null;
        }
    }
}
