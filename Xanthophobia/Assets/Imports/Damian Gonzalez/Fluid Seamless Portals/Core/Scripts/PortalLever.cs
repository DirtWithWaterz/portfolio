using UnityEngine;

namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(102)] //after portal cameras
    public class PortalLever : MonoBehaviour {

        [SerializeField] float angleOff = 180;
        [SerializeField] float angleOn = 0;
        public PortalSetup portalSet;
        private float currentAngle = 0;

        void Start() {
            if (portalSet.onOffSwitch.initialState == PortalSetup.OnOffSwitch.OnOff.On) SetOn(suddenly: true);
            if (portalSet.onOffSwitch.initialState == PortalSetup.OnOffSwitch.OnOff.Off) SetOff(suddenly: true);

        }

        public void SetOn(bool suddenly = false) {
            portalSet.onOffSwitch.isOn = true;
            portalSet.onOffSwitch.desiredAngle = angleOn;
            portalSet.onOffSwitch.momentLastOn = Time.time;
            portalSet.EnableBothPortals();

            if (suddenly) currentAngle = angleOn;

        }

        public void SetOff(bool suddenly = false) {
            portalSet.onOffSwitch.isOn = false;
            portalSet.onOffSwitch.desiredAngle = angleOff;
            portalSet.DisableBothPortals();

            if (suddenly) currentAngle = angleOff;
        }

        public void ToggleOnOff() {
            if (portalSet.onOffSwitch.isTimed) {
                SetOn();
            } else { 
                if (portalSet.onOffSwitch.isOn) SetOff(); else SetOn();
            }
        }

        void Update() {
            //move slowly to desired angle
            currentAngle = Mathf.Lerp(currentAngle, portalSet.onOffSwitch.desiredAngle, 10f * Time.deltaTime);
            transform.localEulerAngles = new Vector3(currentAngle, 0f, 0f);

            //is time ticking?
            if (portalSet.onOffSwitch.isOn && portalSet.onOffSwitch.isTimed) {
                if (Time.time < portalSet.onOffSwitch.momentLastOn + portalSet.onOffSwitch.durationSeconds) {
                    //still ticking...
                    float progress = (Time.time - portalSet.onOffSwitch.momentLastOn) / portalSet.onOffSwitch.durationSeconds;
                    portalSet.onOffSwitch.desiredAngle = Mathf.Lerp(angleOn, angleOff, progress);
                } else { 
                    //time has expired
                    if (portalSet.onOffSwitch.isOn) {
                        //must turn off
                        SetOff();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.P)) ToggleOnOff();
        }
    }
}