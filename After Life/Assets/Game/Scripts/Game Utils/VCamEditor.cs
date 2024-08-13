using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VCamEditor : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cmvcam;

    // Update is called once per frame
    void Update(){
        
        cmvcam.m_Lens.OrthographicSize = (transform.position.y / 5) + 5f;
    }
}
