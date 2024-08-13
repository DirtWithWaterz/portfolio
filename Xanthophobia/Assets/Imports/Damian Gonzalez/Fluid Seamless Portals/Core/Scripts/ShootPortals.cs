using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {


    public class ShootPortals : MonoBehaviour {

        [SerializeField] private float offsetPos;
        [SerializeField] private GameObject prefabCircularPortals;
        [SerializeField] private PortalSetup surfacePortalSet;

        Transform[] portals = new Transform[2];
        Teleport[] teleportScripts = new Teleport[2];

        LayerMask _layerMaskNotPlane = ~2;

        private void Start() { //after all the "Awakes"
            if (surfacePortalSet != null) GetReferences();

        }

        void GetReferences() { 

            portals[0] = surfacePortalSet.refs.portalB;
            portals[1] = surfacePortalSet.refs.portalA;

            teleportScripts[0] = surfacePortalSet.refs.scriptPortalB;
            teleportScripts[1] = surfacePortalSet.refs.scriptPortalA;
        }

        private void CreatePortal() {
            surfacePortalSet = Instantiate(prefabCircularPortals).GetComponent<PortalSetup>();
            GetReferences();
        }

        void Update() {
            if (Input.GetMouseButton(0)) PlacePortal(0, 1, false);
            if (Input.GetMouseButton(1)) PlacePortal(1, -1, false);

            if (Input.GetMouseButtonUp(0)) PlacePortal(0, 1, true);
            if (Input.GetMouseButtonUp(1)) PlacePortal(1, -1, true);
        }

        void PlacePortal(int id, float offsetRot, bool manageColliders) {
            if (portals[0] == null) CreatePortal();

            if (Physics.Raycast(
                transform.position, 
                transform.forward, 
                out RaycastHit hit, 
                100f,
                _layerMaskNotPlane, 
                QueryTriggerInteraction.Ignore
            )) {

                //place the portal
                portals[id].SetPositionAndRotation(
                    hit.point + (hit.normal * offsetPos),
                    Quaternion.LookRotation(hit.normal * offsetRot)
                );

                if (manageColliders) {
                    //restore previously disabled colliders (just in case)
                    teleportScripts[id].RestoreColliders();

                    teleportScripts[id].nearCollidersToDisable.Clear();

                    //add this surface collider(s) to the list
                    foreach (Collider col in hit.transform.GetComponentsInChildren<Collider>()) {
                        if (surfacePortalSet.IsSurfaceColliderAllowed(col)) {
                            teleportScripts[id].AddColliderToDisable(col);
                        }
                    }
                }
            }
        }

    }
}