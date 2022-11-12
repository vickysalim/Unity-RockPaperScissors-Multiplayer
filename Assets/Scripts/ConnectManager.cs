using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_Text feedbackText;

    [SerializeField] string lobbySceneName = "Lobby";

    private void Start()
    {
        usernameInput.text = PlayerPrefs.GetString(PropertyNames.Player.NickName, "");
    }

    public void ClickConnect()
    {
        feedbackText.text = "";

        if(usernameInput.text.Length < 3)
        {
            feedbackText.text = "Nickname min 3 characters";
            return;
        }

        PlayerPrefs.SetString(PropertyNames.Player.NickName, usernameInput.text);
        PhotonNetwork.NickName = usernameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "Connecting...";
        Debug.Log("connecting");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connected");
        feedbackText.text = "Connected";

        StartCoroutine(LoadLevelAfterConnectedAndReady());
    }

    IEnumerator LoadLevelAfterConnectedAndReady()
    {
        while(PhotonNetwork.IsConnectedAndReady == false)
            yield return null;

        SceneManager.LoadScene(lobbySceneName);
    }
}
