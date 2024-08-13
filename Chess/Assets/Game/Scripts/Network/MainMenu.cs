using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenu : MonoBehaviourPun
{
    public static bool _34View = false;
    public Toggle _34Toggle;

    [SerializeField] GameObject settingsPage;

    bool settingsToggle;

    void Start(){
        PhotonNetwork.ConnectUsingSettings();
        _34Toggle.isOn = MainMenu._34View;
    }


    public void StartGame(){
        SceneManager.LoadScene("LoadingLobby");
    }
    public void QuitGame(){
        MainMenu._34View = false;
        Debug.Log(_34View);
        Application.Quit();
    }
    public void ToggleSettings(){
        settingsToggle = !settingsToggle;
        settingsPage.SetActive(settingsToggle);
    }
    public void Toggle34View(){

        MainMenu._34View = !MainMenu._34View;
        Debug.Log(_34View);
    }
}
