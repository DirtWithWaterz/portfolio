using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject settingsScreen;
    float secondsToFadeOut = 0.8f;
    [SerializeField] Material retroMat;

    public void ContinueGame(){

    }

    public void newGame(){
        fadeAudio();
        StartCoroutine("fadeToCamp");
    }

    IEnumerator fadeToCamp(){
        
        // retroMat.SetFloat("_DistortionStrength", 2);
        float fadeTime = GameObject.Find("GameManager")
        .GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        // retroMat.SetFloat("_DistortionStrength", 0);
        SceneManager.LoadScene("Camp");
    }

    void fadeAudio(){
        StartCoroutine("fadeAudioOut");
    }

    IEnumerator fadeAudioOut(){
        AudioSource audioMusic = GameObject.Find("Audio").GetComponent<AudioSource>();
        AudioSource audioMusic1 = GameObject.Find("Audio1").GetComponent<AudioSource>();
 
        // Check Music Volume and Fade Out
        while (audioMusic.volume > 0.01f || audioMusic1.volume > 0.01f)
        {
            audioMusic.volume -= Time.deltaTime / secondsToFadeOut;
            audioMusic1.volume -= Time.deltaTime / secondsToFadeOut;
            yield return null;
        }

        // Make sure volume is set to 0
        audioMusic.volume = 0;
        audioMusic1.volume =0;
 
        // Stop Music
        audioMusic.Stop();
    }
    
    public void OpenSettings(){
        settingsScreen.SetActive(true);
    }

    public void CloseSettings(){
        settingsScreen.SetActive(false);
    }

    public void Quit(){
        Application.Quit();
    }

}
