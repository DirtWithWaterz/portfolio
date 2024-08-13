using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsScreen : MonoBehaviour
{
    public Toggle fullscreenTog, vsyncTog;
    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;
    public TMP_Text resolutionLabel;
    public AudioMixer theMixer;
    public Slider masterSlider, musicSlider, sfxSlider;
    void Start()
    {
        fullscreenTog.isOn = Screen.fullScreen;
        if(QualitySettings.vSyncCount == 0){
            vsyncTog.isOn = false;
        } else{
            vsyncTog.isOn = true;
        } bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++){
            if(Screen.width == resolutions[i].horizontal
            && Screen.height == resolutions[i].vertical){
                foundRes = true;
                selectedResolution = i;
                UpdateResLabel();
            }} if(!foundRes){
                ResItem newRes = new ResItem();
                newRes.horizontal = Screen.width;
                newRes.vertical = Screen.height;
                resolutions.Add(newRes);
                selectedResolution 
                = resolutions.Count - 1;
                UpdateResLabel();}
            float vol = 0f;
        theMixer.GetFloat(
            "MasterVol", out vol);
        masterSlider.value = 
        (vol + 80) / 10;
        theMixer.GetFloat(
            "MusicVol", out vol);
        musicSlider.value = 
        (vol + 80) / 10;
        theMixer.GetFloat(
            "SFxVol", out vol);
        sfxSlider.value = 
        (vol + 80) / 10;
    } void Update(){
        
    } public void ResLeft(){
        selectedResolution--;
        if(selectedResolution < 0){
            selectedResolution = resolutions.Count - 1;
        } UpdateResLabel();
    } public void ResRight(){
        selectedResolution++;
        if(selectedResolution > resolutions.Count - 1){
            selectedResolution = 0;
        } UpdateResLabel();
    } public void UpdateResLabel(){
        resolutionLabel.text = 
            resolutions[selectedResolution].horizontal.ToString() + " x " 
            + resolutions[selectedResolution].vertical.ToString();
    } public void ApplyGraphics(){
        //Screen.fullScreen = fullscreenTog.isOn;
        if(vsyncTog.isOn){
            QualitySettings.vSyncCount = 1;
        } else{
            QualitySettings.vSyncCount = 0;
        } Screen.SetResolution(
            resolutions[selectedResolution].horizontal,
            resolutions[selectedResolution].vertical,
            fullscreenTog.isOn
        );
    } public void SetMasterVol(){
        theMixer.SetFloat("MasterVol",
        masterSlider.value * 10 - 80);
        PlayerPrefs.SetFloat("MasterVol",
            masterSlider.value * 10 - 80);
    } public void SetMusicVol(){
        theMixer.SetFloat("MusicVol",
        musicSlider.value * 10 - 80);
        PlayerPrefs.SetFloat("MusicVol",
            musicSlider.value * 10 - 80);
    } public void SetSFxVol(){
        theMixer.SetFloat("SFxVol",
        sfxSlider.value * 10 - 80);
        PlayerPrefs.SetFloat("SFxVol",
            sfxSlider.value * 10 - 80);
    }
} [System.Serializable]
public class ResItem{
    public int horizontal, vertical;
}
