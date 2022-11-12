using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Toggle audioToggle;

    public void Awake()
    {
        if (!PlayerPrefs.HasKey("audio"))
            PlayerPrefs.SetInt("audio", 1);

        if (PlayerPrefs.GetInt("audio") == 1)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }

        if (audioToggle != null)
        {
            if (PlayerPrefs.GetInt("audio") == 1)
            {
                audioToggle.isOn = true;
            }
            else
            {
                audioToggle.isOn = false;
            }
        }
    }

    public void ToggleAudio()
    {
        if (audioToggle.isOn)
        {
            AudioListener.volume = 1;
            PlayerPrefs.SetInt("audio", 1);
        }
        else
        {
            AudioListener.volume = 0;
            PlayerPrefs.SetInt("audio", 0);
        }
    }
}
