using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Splash : MonoBehaviour
{
    AudioSource lightOnOff;
    [SerializeField] TMP_Text logoText;
    [SerializeField] GameObject theLight;
    [SerializeField] Material retroMat;

    // Start is called before the first frame update
    void Start()
    {
        // retroMat.SetFloat("_DistortionStrength", 0.1f);
        lightOnOff = GameObject.Find("fluorescent-light-onoff").GetComponent<AudioSource>();
        StartCoroutine("SplashRoutine");
    }
    IEnumerator SplashRoutine(){
        yield return new WaitForSeconds(2);
        lightOnOff.Play();
        theLight.SetActive(true);
        logoText.text = "DREADLESS";
        yield return new WaitForSeconds(5.7f);
        theLight.SetActive(false);
        logoText.text = "";
        yield return new WaitForSeconds(1);
        float fadeTime = GameObject.Find("GameManager")
        .GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene("MainMenu");
    }
}
