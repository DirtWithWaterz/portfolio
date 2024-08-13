using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerControls : MonoBehaviour
{
    public KeyCode forward = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode backward = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode interact = KeyCode.E;
    public KeyCode flashlight = KeyCode.F;
    public KeyCode replaceBatteries = KeyCode.B;
    public KeyCode jump = KeyCode.Space;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode zoom = KeyCode.Mouse1;


    string PRforw;
    string PRback;
    string PRlef;
    string PRrig;
    string PRintr;
    string PRjum;
    string PRsprin;
    string PRzoo;
    string PRcrouc;
    string PRflash;
    string PRrepBat;

    FirstPersonController fpsc;

    GameObject objectToFind;
    string tagName = "Player";
    public bool inControlsUI = false;

    void Awake(){
        objectToFind = GameObject.FindGameObjectWithTag(tagName);
        fpsc = objectToFind.GetComponent<FirstPersonController>();
        PRforw = forward.ToString();
        PRback = backward.ToString();
        PRlef = left.ToString();
        PRrig = right.ToString();
        PRintr = interact.ToString();
        PRjum = jump.ToString();
        PRsprin = sprint.ToString();
        PRcrouc = crouch.ToString();
        PRzoo = zoom.ToString();
        PRflash = flashlight.ToString();
        PRrepBat = replaceBatteries.ToString();
    }

    private bool keycodesChanged = false;
    void Update()
    {
#if !UNITY_EDITOR
        if(!inControlsUI) return;
#endif
        string forw = forward.ToString();
        string back = backward.ToString();
        string lef = left.ToString();
        string rig = right.ToString();
        string intr = interact.ToString();
        string jum = jump.ToString();
        string sprin = sprint.ToString();
        string crouc = crouch.ToString();
        string zoo = zoom.ToString();
        string flash = flashlight.ToString();
        string repBat = replaceBatteries.ToString();

        if(
            forw != PRforw ||
            back != PRback ||
            lef != PRlef ||
            rig != PRrig ||
            intr != PRintr ||
            jum != PRjum ||
            crouc != PRcrouc ||
            zoo != PRzoo ||
            sprin != PRsprin ||
            flash != PRflash ||
            repBat != PRrepBat
        ){
            keycodesChanged = true;
            UpdateInputs();
            PRforw = forw;
            PRback = back;
            PRlef = lef;
            PRrig = rig;
            PRintr = intr;
            PRjum = jum;
            PRzoo = zoo;
            PRcrouc = crouc;
            PRsprin = sprin;
            PRflash = flash;
            PRrepBat = repBat;
        }
    }
    void UpdateInputs(){
        if(!keycodesChanged) return;

        fpsc.shouldUpdateInputs = true;
        keycodesChanged = false;
    }

}
