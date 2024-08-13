using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {
    public class ThirdPersonOffsetAndFov : MonoBehaviour {
        [SerializeField] private float speed = .1f;
        private Camera cam;

        [HideInInspector] public float initialFov, desiredFov;
        [HideInInspector] public Vector3 initialOffset, desiredOffset;

        public static ThirdPersonOffsetAndFov instance;
        
        void Awake() {
            //quick singleton
            if (instance == null) instance = this; else Destroy(this);
            
            //self components
            cam = GetComponent<Camera>();

            //initial values
            initialFov = cam.fieldOfView;
            initialOffset = transform.localPosition;

            //desired values
            desiredFov = initialFov;
            desiredOffset = initialOffset;
        }

        void FixedUpdate() {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFov, speed);
            transform.localPosition = Vector3.Lerp(transform.localPosition, desiredOffset, speed);

            if (Portal3rdPersonArea.listOfActiveAreas != null && Portal3rdPersonArea.listOfActiveAreas.Count == 0) {
                desiredFov = initialFov;
                desiredOffset = initialOffset;

            }
        }
    }
}