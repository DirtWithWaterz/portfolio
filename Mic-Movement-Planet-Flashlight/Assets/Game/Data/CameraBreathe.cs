using System.Collections;
using System.Collections.Generic;
using FirstGearGames.SmoothCameraShaker;
using UnityEngine;

public class CameraBreathe : MonoBehaviour
{
    public ShakeData shakeData;
    // Start is called before the first frame update
    void Start()
    {
        CameraShakerHandler.Shake(shakeData);
    }
}
