using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {
    public class Portal3rdPersonArea : MonoBehaviour {
        [HideInInspector] public Teleport triggerScript; //assigned in PortalSetup
        public static List<Portal3rdPersonArea> listOfActiveAreas = new List<Portal3rdPersonArea>();

        private void Awake() {
        }

        private void OnTriggerEnter(Collider other) {
            if (triggerScript.IsPlayer(other.transform)) {
                if (!listOfActiveAreas.Contains(this)) listOfActiveAreas.Add(this);
                triggerScript.PlayerEntered3rdPersonArea();

            }
        }

        private void OnTriggerExit(Collider other) {
            if (listOfActiveAreas.Contains(this)) listOfActiveAreas.Remove(this);
            if (triggerScript.IsPlayer(other.transform)) triggerScript.PlayerExited3rdPersonArea();
        }
    }
}