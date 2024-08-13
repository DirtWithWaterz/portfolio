using UnityEngine;

namespace DamianGonzalez.Portals {
    public class PortalSwitchInteraction : MonoBehaviour {

        [SerializeField] private GameObject interactionMsgInCanvas;

        void Start() {
            //try getting "interactionMsgInCanvas" automatically, to simplify this demo implementation
            if (interactionMsgInCanvas == null) {
                interactionMsgInCanvas = GameObject.Find("/Canvas/interaction message");
            }
        }

        void Update() {
            InteractionWithSwitches();
        }

        private void InteractionWithSwitches() {
            //interaction demo example
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 6f)) {
                if (hit.transform.CompareTag("PortalSwitch")) {
                    //player can interact
                    interactionMsgInCanvas?.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E)) {
                        Portals.PortalLever lever = hit.transform.GetComponentInChildren<Portals.PortalLever>();
                        lever?.ToggleOnOff();
                    }
                } else {
                    interactionMsgInCanvas?.SetActive(false);
                }
            } else {
                interactionMsgInCanvas?.SetActive(false);
            }
        }
    }
}