using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(113)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalRenderer > 114 PortalSetup
    public class PortalRenderer : MonoBehaviour {

        public static bool sceneHasPortalRenderer = false;
        public static Dictionary<int, PortalSetup> manualOrder = new Dictionary<int, PortalSetup>();
        public static int minId, maxId;


        private void Awake() {
            sceneHasPortalRenderer = true;

            manualOrder = new Dictionary<int, PortalSetup>();
            minId = 0;
            maxId = 0;
        }

        private void Start() {
        }

        void Update() {
            for (int i = minId; i <= maxId; i++) {
                if (manualOrder.ContainsKey(i)) {
                    Debug.Log($"Render {i}");

                    manualOrder[i].refs.scriptCamA.int_renderPasses = 0;
                    manualOrder[i].refs.scriptCamB.int_renderPasses = 0;

                    manualOrder[i].refs.scriptCamA.ManualRenderIfNecessary();
                    manualOrder[i].refs.scriptCamB.ManualRenderIfNecessary();
                }
            }

            /*
             * if you want to do something AFTER every portal has requested rendering, this is when.
             * For example...
             
                playerCamera.Render(); //last

            */
        }
    }
}