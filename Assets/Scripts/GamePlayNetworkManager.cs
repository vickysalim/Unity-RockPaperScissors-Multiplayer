using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayNetworkManager : MonoBehaviourPunCallbacks
{
    public void ExitToMenu(string sceneName)
    {
        StartCoroutine(LoadMenuScene(sceneName));
    }

    IEnumerator LoadMenuScene(string sceneName)
    {
        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
            yield return null;

        SceneManager.LoadScene(sceneName);
    }

    public void LeaveMatch(string sceneName)
    {
        StartCoroutine(LoadLobbyScene(sceneName));
    }

    IEnumerator LoadLobbyScene(string sceneName)
    {
        PhotonNetwork.LeaveRoom();
        
        while (PhotonNetwork.InRoom || PhotonNetwork.IsConnectedAndReady == false)
            yield return null;

        SceneManager.LoadScene(sceneName);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            LeaveMatch("Lobby");
        }
    }
}
