using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fading : MonoBehaviour
{
    public Texture2D fadeOutTexture;
    public float fadeSpeed = 2f;
    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;
    [SerializeField] Material retroMat;
    float strength;

    private void Awake(){
        SceneManager.sceneLoaded
        +=SceneLoaded;
    } void OnGUI(){
        alpha += fadeDir * fadeSpeed * Time.deltaTime;

        if(SceneManager.GetActiveScene().name == "DreadlessSplash" || SceneManager.GetActiveScene().name == "MainMenu"){

            strength += fadeDir * fadeSpeed * Time.deltaTime;
            strength = Mathf.Clamp(strength, 0.1f, 1f);
            retroMat.SetFloat("_DistortionStrength", strength);
            // retroMat.SetInt("_Invert_UV_Y_Axis", 0);
        } else{

            strength += fadeDir * fadeSpeed * Time.deltaTime;
            strength = Mathf.Clamp(strength, 0.0001f, 1f);
            retroMat.SetFloat("_DistortionStrength", strength);
            // retroMat.SetInt("_Invert_UV_Y_Axis", 1);
        }
        
        alpha = Mathf.Clamp01(alpha);
        GUI.color = new Color(
            GUI.color.r,
            GUI.color.g,
            GUI.color.b,
            alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(
            new Rect(
                0,
                0,
                Screen.width,
                Screen.height),
                fadeOutTexture);
    } public float BeginFade(int direction){
        fadeDir = direction;
        return(fadeSpeed);
    } void SceneLoaded(Scene scene, LoadSceneMode mode){
        alpha = 1;
        BeginFade(-1);
    }
}
