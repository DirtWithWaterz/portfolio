using UnityEngine;

//this script is not part of the portals system, it's only for the demo scene,
//only to provide an example of how grabbed objects work with the portal's clones system

namespace DamianGonzalez {
    public class PickUpObjects : MonoBehaviour {

        private Vector3 positionOffset;
        private Quaternion rotationOffset;


        [SerializeField] Transform player;
        [SerializeField] private Transform originalGrabbedObject;
        [SerializeField] private Transform simplifiedGrabbedObject;


        [SerializeField] private GameObject interactionMsgInCanvas;
        [SerializeField] private float maxInteractionDistance = 3f;


        void Start() {
            //try getting "interactionMsgInCanvas" automatically, to simplify this demo implementation
            if (interactionMsgInCanvas == null) {
                interactionMsgInCanvas = GameObject.Find("/Canvas/interaction message");
            }

            //try getting the player
            if (player == null) player = transform.parent;
        }


        void Update() {
            InteractionWithPickables();
            UpdateGrabbedObject();
        }

        private void InteractionWithPickables() {
            //interaction demo example
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxInteractionDistance)) {
                if (hit.transform.CompareTag("Pickable")) {
                    //player can interact
                    interactionMsgInCanvas?.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E)) {
                        DropObject();
                        PickUpObject(hit.transform);
                    }
                } else {
                    interactionMsgInCanvas?.SetActive(false);
                }
            } else {
                interactionMsgInCanvas?.SetActive(false);
            }

            //offer to drop
            if (!interactionMsgInCanvas.activeSelf) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    DropObject();
                }
            }
        }


        void PickUpObject(Transform obj) {

            originalGrabbedObject = obj;

            //now, create a simplified clone, destroying some components from itself and childrens
            //note: leave the collider and rigidbody on, otherwise it won't enter the portal trigger

            simplifiedGrabbedObject = Instantiate(originalGrabbedObject, originalGrabbedObject.parent);
            simplifiedGrabbedObject.name += " (grabbed clone)";

            foreach (Rigidbody rb in simplifiedGrabbedObject.GetComponentsInChildren<Rigidbody>()) rb.isKinematic=true;
            foreach (Camera cam in simplifiedGrabbedObject.GetComponentsInChildren<Camera>()) cam.enabled = false; //can't destroy in HDRP
            foreach (CharacterController cc in simplifiedGrabbedObject.GetComponentsInChildren<CharacterController>()) Destroy(cc);
            foreach (AudioListener lis in simplifiedGrabbedObject.GetComponentsInChildren<AudioListener>()) Destroy(lis);
            foreach (MonoBehaviour scr in simplifiedGrabbedObject.GetComponentsInChildren<MonoBehaviour>()) {
                bool destroyScript = true;
                if (typeof(TMPro.TextMeshPro) != null && scr.GetType() == typeof(TMPro.TextMeshPro)) destroyScript = false;
                if (destroyScript) Destroy(scr);
            }

            //also, remove the tag so the clone it's not pickable 
            simplifiedGrabbedObject.tag = "Untagged";

            //and hide the original object
            originalGrabbedObject.gameObject.SetActive(false);

            positionOffset = player.InverseTransformPoint(obj.position); 
            rotationOffset = obj.rotation * Quaternion.Inverse(player.rotation);

        }

        void DropObject() { 
            if (originalGrabbedObject == null) return;

            originalGrabbedObject.SetPositionAndRotation(
                simplifiedGrabbedObject.position,
                simplifiedGrabbedObject.rotation
            );

            originalGrabbedObject.gameObject.SetActive(true);

            Destroy(simplifiedGrabbedObject.gameObject);

            simplifiedGrabbedObject = null;
            originalGrabbedObject = null;
        }

        void UpdateGrabbedObject() {
            if (simplifiedGrabbedObject == null) return;

            simplifiedGrabbedObject.SetPositionAndRotation(
                player.TransformPoint(positionOffset),
                player.rotation * rotationOffset
            );
        }
    }
}