using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {

    [DefaultExecutionOrder(111)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalSetup (rendering) > 114 PortalRenderer
    public class Teleport : MonoBehaviour {

        /*
         * Quick reminder (for visitors and for myself) of how this works.
         * 
         * Player uses elastic plane, so they teleport on Update, the trigger does nothing on them.
         * Other objects follows this plan:
         *  - when they enter the trigger, a clone is made on the other side, but the original (and it physics) keeps on this side
         *  - while they are inside, the clone is updated
         *  - if player crosses sides while the object is still inside, clone and original are swapped (for physics)
         *  - when it exits the trigger, it teleports and the clone is destroyed
         * 
         * That's the plan, BUT...
         * Players have other 2 fallback methods, in case they can't count with the Update method
         * (either because the trigger is too short, or the player too fast, or the PC too slow) 
         * and they only stay inside the trigger a few frames, or none at all, so:
         * 1) they can teleport directly on "OnTriggerEnter", when its speed is greater than the declared limit
         * 2) they can teleport on "OnTriggerExit", when it's actually too late, this is called "emergency teleport"
         * 
         * */

        [HideInInspector] public bool portalIsEnabled = true;

        private Vector3 originalPlanePosition;


        [HideInInspector] public BoxCollider _collider;               //
        [HideInInspector] public Transform plane;                     //
        [HideInInspector] public Transform portal;                    // these variables are automatically writen
        [HideInInspector] public PortalSetup setup;                   // by PortalSetup 
        [HideInInspector] public PortalCamMovement cameraScript;      //
        [HideInInspector] public Teleport otherScript;                //


        [HideInInspector] public bool planeIsInverted; //only for elastic mode
        [HideInInspector] public bool teleportPlayerOnExit = false;
        [HideInInspector] public bool dontTeleport = false;

        private Dictionary<Transform, Transform> clones = new Dictionary<Transform, Transform>(); //original => clone
        private GameObject cloneParent;

        private float initialFOV;
        private Vector3 initialCameraOffset;



        [FancyInfo("This info is only for debugging, do not set values here")]

        /* [HideInInspector] */ public float trespassProgress;              // these 3 variables
        /* [HideInInspector] */ public bool playerInsideTrigger = false;    // can be hidden
        /* [HideInInspector] */ public int framesInsideCount=0;             // from the inspector
        Collider playerCollider;

        private void Start() {
            originalPlanePosition = plane.localPosition;
            initialFOV = setup.player.playerCameraComp.fieldOfView;
            initialCameraOffset = setup.player.playerCameraTr.localPosition;

        }

        public void DisableThisPortal() {
            SetEnabled(false);
        }

        public void EnableThisPortal() {
            SetEnabled(true);
        }

        public void SetEnabled(bool _enabled) {
            portalIsEnabled = _enabled;
            gameObject.SetActive(portalIsEnabled);                //trigger (functional)
            cameraScript.gameObject.SetActive(portalIsEnabled);   //camera (functional)
            plane.gameObject.SetActive(portalIsEnabled);          //plane (visual)
        }



        Vector3 GetVelocity(Transform obj) {

            if (obj == setup.player.playerMainObj) {
                if (setup.player.controllerType == PortalSetup.Player.ControllerType.CharacterController) return setup.player.playerCc.velocity;
                if (setup.player.controllerType == PortalSetup.Player.ControllerType.Rigidbody) return setup.player.playerRb.velocity;
            } else {
                if (obj.TryGetComponent(out Rigidbody rb)) return rb.velocity;
                if (obj.TryGetComponent(out CharacterController cc)) return cc.velocity;
            }
            return Vector3.zero;
        }

        Vector3 TowardDestination(Transform obj) {
            //single-sided: crossing the plane
            if (!setup.doubleSided) return TowardDestinationSingleSided();

            //double-sided: whichever direction the object is going
            return IsGoodSide(obj) ? TowardDestinationSingleSided() : -TowardDestinationSingleSided();
        }

        public Vector3 TowardDestinationSingleSided() => planeIsInverted ? -portal.forward : portal.forward;

        public bool IsGoodSide(Transform obj) {

            //not about facing, but about velocity. where is it going?
            Vector3 velocityOrPosition = GetVelocity(obj);
            float dotProduct;
            if (velocityOrPosition != Vector3.zero) {
                //it has velocity
                dotProduct = Vector3.Dot(-TowardDestinationSingleSided(), velocityOrPosition);
            } else {
                //it hasn't velocity, let's try with its position (it may fail with very fast objects)
                dotProduct = Vector3.Dot(-TowardDestinationSingleSided(), portal.position - velocityOrPosition);
            }

            //if (setup.debugOptions.verboseDebug) Debug.Log($"{obj.name} crossing. Good side: {dotProduct < 0}");
            return dotProduct < 0;
        }

        public bool IsGoodSide(Vector3 dir) => Vector3.Dot(-TowardDestinationSingleSided(), dir) < 0;




        bool CandidateToTeleport(Transform objectToTeleport) {
            /*
             * an object is candidate to teleport now if:
             * a) it's player, or not player but it passes the tag filters
             * and
             * b) portal is double-sided, or single-sided but in the good side
             * and
             * c) object is not too far from the portal -> no longer necessary from v1.4
             */

            //a)
            //bool isPlayer = objectToTeleport.CompareTag(setup.filters.playerTag); //for better readability
            if (!ThisObjectCanCross(objectToTeleport)) {
                if (setup.debugOptions.verboseDebug) Debug.Log($"{objectToTeleport.name} will not teleport. Reason: filters");
                return false;
            }

            //b) 
            if (!setup.doubleSided && !IsGoodSide(objectToTeleport)) {
                if (setup.debugOptions.verboseDebug) Debug.Log($"{objectToTeleport.name} will not teleport. Reason: not the good side"); 
                return false;
            }

            //c)
            /*
            bool tooFar = Vector3.Distance(objectToTeleport.position, portal.position) > setup.advanced.maximumDistance;
            if (tooFar) {
                if (setup.debugOptions.verboseDebug) Debug.Log($"{objectToTeleport.name} will not teleport. Reason: too far");
                return false;
            }
            */
            return true;
        }

        void DoTeleport(Transform objectToTeleport, bool fireEvents = true) { //note: we're still inside OnTrigger...
            if (objectToTeleport == null) return;

            bool isPlayer = IsPlayer(objectToTeleport);
            if (isPlayer) {
                playerInsideTrigger = false;
                playerIn3rdPersonArea = false;

                //for 3rd person:
                otherScript.cameraEffectInitialDistance = cameraEffectInitialDistance;
                if (setup.debugOptions.verboseDebug) Debug.Log($"new cameraEffectInitialDistance on teleport: {cameraEffectInitialDistance}");
            }

            if (isPlayer) {
                /*
                 * 
                 *  If you need to do something to your player RIGHT BEFORE teleporting,
                 *  this is when.
                 * 
                */
            }


            // Teleport the object
            Vector3 oldPosition = objectToTeleport.position;
            Vector3 rbOffset = Vector3.zero;
            Rigidbody rb = objectToTeleport.GetComponent<Rigidbody>();
            if (rb != null) rbOffset = rb.position - transform.position;


            //position
            objectToTeleport.position = otherScript.portal.TransformPoint(
                portal.InverseTransformPoint(objectToTeleport.position)
            );

            //rotation
            objectToTeleport.rotation =
                otherScript.portal.rotation
                * Quaternion.Inverse(portal.rotation)
                * objectToTeleport.rotation;

            //velocity (if object has rigidbody)
            if (rb != null) {
                rb.velocity = otherScript.portal.TransformDirection(
                    portal.InverseTransformDirection(rb.velocity)
                );
                rb.position = transform.position + rbOffset; //not entirely necessary
            }

            //change of gravity
            if (setup.multiGravity.applyMultiGravity) {
                if (objectToTeleport.TryGetComponent(out CustomGravity gr)) {
                    Vector3 oldDirection = gr.down;
                    gr.ChangeGravity(-otherScript.portal.up);

                    //changed?
                    if (setup.debugOptions.verboseDebug && oldDirection != gr.down) {
                        Debug.Log($"Changed gravity from {oldDirection} to {gr.down}");
                    }
                    
                }
            }


            if (isPlayer) {

                //player has crossed. If using clones, may be necessary to swap clones and originals (see documentation)
                
                if (setup.clones.useClones) {
                    SwapOriginalsAndClones();
                }
                


                //avoid miscalulating the distance from previous frame
                lastFramePosition = Vector3.zero;
                otherScript.lastFramePosition = Vector3.zero;


                //refresh camera position before rendering, in order to avoid flickering
                otherScript.cameraScript.Recalculate();
                cameraScript.Recalculate();
                //MoveElasticPlane(.1f);
                //otherScript.MoveElasticPlane(.1f);

                if (setup.afterTeleport.tryResetCharacterController) {
                    //reset player's character controller (if there is one)
                    //otherwise it won't allow the change of position
                    if (objectToTeleport.TryGetComponent(out CharacterController cc)) {
                        cc.enabled = false;
                        cc.enabled = true;
                    }
                }

                if (setup.afterTeleport.tryResetCameraObject) {
                    //reset player's camera (may solve issues)
                    setup.player.playerCameraTr.gameObject.SetActive(false);
                    setup.player.playerCameraTr.gameObject.SetActive(true);
                }

                if (setup.afterTeleport.tryResetCameraScripts) {
                    //reset scripts in camera
                    foreach (MonoBehaviour scr in setup.player.playerCameraTr.GetComponents<MonoBehaviour>()) {
                        if (scr.isActiveAndEnabled) {
                            scr.enabled = false;
                            scr.enabled = true;
                        }
                    }
                }

                /*
                 * 
                 * If you need to do something to your player AFTER teleporting, this is when.
                 * See online documentation about controllers (pipasjourney.com/damianGonzalez/portals/#controller)
                 * and how to implement a 3rd party controller with these portals
                 * 
                */

                if (setup.debugOptions.pauseWhenPlayerTeleports) Debug.Break();

            } else {
                // If you need to do something to other crossing object, this is when.
                if (setup.debugOptions.pauseWhenOtherTeleports) Debug.Break();
            }

            //finally, fire event
            if (fireEvents) {
                PortalEvents.teleport?.Invoke(
                    setup.groupId,
                    portal,
                    otherScript.portal,
                    objectToTeleport,
                    oldPosition,
                    objectToTeleport.position
                );
            }
        }

        
        public void SwapOriginalsAndClones() {
            if (clones.Count == 0 && otherScript.clones.Count == 0) return;

            //first, let's destroy all clones (they are no longer necessary)
            // - get originals and clones from both sides
            List<Transform> originalsHere = new List<Transform>();
            List<Transform> clonesHere = new List<Transform>();
            foreach (KeyValuePair<Transform, Transform> clone in clones) {
                clonesHere.Add(clone.Value);
                if (clone.Key != setup.player.playerMainObj) originalsHere.Add(clone.Key);
            }

            List<Transform> originalsThere = new List<Transform>();
            List<Transform> clonesThere = new List<Transform>();
            foreach (KeyValuePair<Transform, Transform> clone in otherScript.clones) {
                clonesThere.Add(clone.Value);
                originalsThere.Add(clone.Key);
            }

            // - delete the actual objects and remove from "clones"
            foreach (Transform clone in clonesHere) TryDestroyClone(clone);
            foreach (Transform clone in clonesThere) otherScript.TryDestroyClone(clone);


            //then, let's teleport the originals (they will "enter" the trigger on the other side and make a clone in this side)

            foreach (Transform original in originalsHere) DoTeleport(original);
            foreach (Transform original in originalsThere) otherScript.DoTeleport(original);
            


        }
        

        bool thisOne = false;
        [HideInInspector] public float cameraEffectInitialDistance = 0;
        int approxFpsCount;
        void OnTriggerEnter(Collider other) {

            if (setup.debugOptions.verboseDebug) {
                float _vel = GetVelocity(other.transform).magnitude;
                Debug.Log($"{other.name} entering the trigger (at {_vel} m/s, {approxFpsCount} FPS). ");
            }

            bool isPlayer = IsPlayer(other.transform);

            //player detection
            if (isPlayer) { 
                playerCollider = other;
                playerInsideTrigger = true;
                framesInsideCount = 0;
                goodSideWhenEnteredTrigger = IsGoodSide(other.transform);
            }

            //Try to prevent high speed on trigger enter
            if (!setup.teleportInProgress && setup.advanced.tryPreventTooFastFailure) {
                float _vel = GetVelocity(other.transform).magnitude;
                if (isPlayer && _vel > setup.advanced.maxSpeedForElasticPlane.Evaluate(approxFpsCount)) {
                    if (setup.debugOptions.verboseDebug) {
                        Debug.Log(
                            $"Player crossing too fast ({_vel} m/s at {approxFpsCount} FPS). " +
                            $"Trying to teleport earlier (on trigger enter)."
                        );
                    }
                    ConsiderTeleporting(other);
                    return;
                }
            }

            //surface portal?
            if (isPlayer && setup.surfacePortals.isSurfacePortals && setup.surfacePortals.disableCollidersNearPortals) {
                DisableColliders();
            }

            //make clone?
            if (setup.clones.useClones && (!isPlayer || setup.clones.clonePlayerToo)) {
                CreateCloneOnTheOtherSide(other.transform);
            }


        }

        private bool playerIn3rdPersonArea = false;
        public void PlayerEntered3rdPersonArea() {
            if (setup.debugOptions.verboseDebug) Debug.Log("Player entered 3rd person area", transform);
            playerIn3rdPersonArea = true;
            //start camera effect
            if (setup.player.cameraType == PortalSetup.Player.CameraType.ThirdPerson && !setup.teleportInProgress) {
                float value = DistanceCameraPlane();
                if (value < -.5f) {
                    if (setup.debugOptions.verboseDebug) Debug.Log($"new cameraEffectInitialDistance on enter: {value}");
                    cameraEffectInitialDistance = value;
                }
            }
        }

        public void PlayerExited3rdPersonArea() {
            if (setup.debugOptions.verboseDebug) Debug.Log("Player exited 3rd person area", transform);
            playerIn3rdPersonArea = false;
        }

        bool ConsiderTeleporting(Collider other) { //note: this can be called from OnTrigger... or from Update

            //process ends, but doesn't continue
            if (setup.teleportInProgress) {
                if (!thisOne) {
                    otherScript.thisOne = false;
                    setup.teleportInProgress = false;
                }
                if (setup.debugOptions.verboseDebug) Debug.Log($"{other.name} will not teleport. Reason: too soon");
                return false;
            }


            if (!CandidateToTeleport(other.transform)) {
                if (setup.debugOptions.verboseDebug) Debug.Log($"{other.name} will not teleport. Reason: not candidate");
                return false;
            }

            //ok, it's candidate to teleport
            if (setup.debugOptions.verboseDebug) Debug.Log($"{other.name} passed the filters, will teleport.");

            setup.teleportInProgress = true;
            setup.lastTeleportTime = Time.time;
            thisOne = true;


            //bool createClone = setup.clones.useClones && vel.magnitude > setup.advanced.maxVelocityForClones;

            DoTeleport(other.transform);
            
            return true;
        }

        void TryDestroyClone(Transform clone_or_original) {
            foreach (KeyValuePair<Transform, Transform> cl in clones) {
                if (cl.Value.Equals(clone_or_original) || cl.Key.Equals(clone_or_original)) {
                    //this is a clone. Delete the object
                    Destroy(cl.Value.gameObject);

                    //and the reference
                    clones.Remove(cl.Key);

                    return;
                }
            }

        }


        public List<Collider> nearCollidersToDisable = new List<Collider>();

        public void AddColliderToDisable(Collider col) {
            
            if (!col.isTrigger) {
                nearCollidersToDisable.Add(col);
            }
        }
        
        public void DisableColliders() {
            foreach (Collider col in nearCollidersToDisable) col.enabled = false;
        }

        public void RestoreColliders() {
            foreach (Collider col in nearCollidersToDisable) col.enabled = true;
        }

        public void GetInitialColliders() {
            foreach (Collider col in Physics.OverlapBox(_collider.center, Vector3.one * 1f)) {
                AddColliderToDisable(col);
            }
        }

        private bool goodSideWhenEnteredTrigger = false;

        void OnTriggerExit(Collider other) {
            if (setup.debugOptions.verboseDebug) Debug.Log($"On trigger exit. {other.name} has left the trigger. TresspasProgress value is {trespassProgress}");

            bool isPlayer = IsPlayer(other.transform);
            if (isPlayer) playerInsideTrigger = false;

            //make clone?
            if (setup.clones.useClones && (!isPlayer || setup.clones.clonePlayerToo)) {
                TryDestroyClone(other.transform);
                if (!isPlayer) ConsiderTeleporting(other);
            }

            if (setup.teleportInProgress) return; //do not disturb

            //surface portal?
            if (isPlayer && setup.surfacePortals.isSurfacePortals && setup.surfacePortals.disableCollidersNearPortals) {
                RestoreColliders();
            }



            if (setup.advanced.tryEmergencyTeleport) {
                //if using elastic plane and it's NOT declared as a big portal,
                //and player crosses too fast (1 or 2 frames), it should consider to be teleported
                trespassProgress = DistanceCameraPlane();

                if (
                    setup.advanced.useElasticPlane
                    && !setup.teleportInProgress
                    && goodSideWhenEnteredTrigger
                    && trespassProgress > 0
                    && trespassProgress <= setup.advanced.maxElasticPlaneValueForEmergency
                ) {
                    if (setup.debugOptions.verboseDebug) Debug.Log($"Emergency teleport! {other.name} has left the trigger. TresspasProgress was {trespassProgress}");
                    ConsiderTeleporting(other);
                }
            }

        }

        public void RestorePlaneOriginalPosition() {
            plane.localPosition = originalPlanePosition;
            
            SetClippingOffset(setup.advanced.clippingOffset);
            cameraScript.ApplyAdvancedOffset();

        }

      

        void OnTriggerStay(Collider other) {

            //elastic plane
            if (IsPlayer(other.transform)) { 
                playerInsideTrigger = true;
                playerCollider = other;
            }
 
        }

        private void Update() {
            if (playerInsideTrigger) framesInsideCount++;


            trespassProgress = DistanceCameraPlane();

            ManageElasticPlane();

            Manage3rdPersonCamera();

            UpdateClones();

            CheckTeleportingTimeout();

            approxFpsCount = (int)(1f / Time.deltaTime);
        }

        void CheckTeleportingTimeout() {
            if (Time.time > setup.lastTeleportTime + .05f) {
                thisOne = false;
                otherScript.thisOne = false;
                setup.teleportInProgress = false;
            }
        }

        private void ManageElasticPlane() {
            if (playerInsideTrigger && setup.advanced.useElasticPlane) {

                //move this plane
                MoveElasticPlane(trespassProgress);

                //teleport player when the progress treshold is reached
                if (
                    setup.advanced.useElasticPlane 
                    && tresspasProgressInTeleportWindow
                ) {
                    if (setup.debugOptions.verboseDebug) {
                        Debug.Log($"teleported because {trespassProgress} > {setup.advanced.elasticPlaneTeleportWindowFrom}");
                    }
                    ConsiderTeleporting(playerCollider);
                }
            } else {
            }

            if (!playerInsideTrigger && !otherScript.playerInsideTrigger) {
                RestorePlaneOriginalPosition();
            }
        }

        bool tresspasProgressInTeleportWindow => (
                trespassProgress >= setup.advanced.elasticPlaneTeleportWindowFrom
                && trespassProgress <= setup.advanced.elasticPlaneTeleportWindowTo
        );

        private void Manage3rdPersonCamera() {

            if (playerIn3rdPersonArea) { 

                float progress01 = Mathf.InverseLerp(cameraEffectInitialDistance, 0, trespassProgress);
                //0 is extrem far, 1 is extrem near

                if (setup.player.adjustFieldOfView) {
                    ThirdPersonOffsetAndFov.instance.desiredFov = Mathf.Lerp(initialFOV, setup.player.nearFovValue, progress01);
                }

                if (setup.player.adjustOffsetPosition) {
                    ThirdPersonOffsetAndFov.instance.desiredOffset = Vector3.Lerp(initialCameraOffset, setup.player.nearOffsetPosition, progress01);
                }

            }

        }

        private void UpdateClones() {

            if (!setup.clones.useClones) return;
            foreach (var cl in clones) { 
                if (cl.Key != null) UpdateClone(cl.Key.transform);
            }
        }

        public float DistanceCameraPlane() {
            Transform reference = 
                (setup.player.cameraType == PortalSetup.Player.CameraType.ThirdPerson)
                ? setup.player.playerMainObj 
                : setup.player.playerCameraTr
            ;

            return Vector3.Dot(
                -TowardDestination(setup.player.playerMainObj), 
                portal.position - reference.position
            );
        }

        Vector3 lastFramePosition = Vector3.zero;
        Vector3 deltaMov;


        private void MoveElasticPlane(float _trespassProgress, bool forced = false) {
            if (_trespassProgress > setup.advanced.elasticPlaneMinTreshold  && _trespassProgress <= setup.advanced.elasticPlaneTeleportWindowFrom) {
                
                //calculate offset by velocity
                float relativeSpeed = 0;
                if (setup.advanced.dynamicOffsetBasedOnVelocity) {

                    //if player just crossed this frame, don't recalculate deltaMov, use last frame's speed
                    if (lastFramePosition != Vector3.zero) {
                        deltaMov = setup.player.playerCameraTr.position - lastFramePosition;
                    }

                    relativeSpeed = Vector3.Dot(TowardDestination(setup.player.playerMainObj), deltaMov);
                } 

                
                //calculate offset
                float totalOffset = 
                    setup.advanced.elasticPlaneOffset * 1                           //1. the constant minimum offset
                    + _trespassProgress                                             //2. how much player has crossed
                    + relativeSpeed * setup.advanced.elasticPlaneVelocityFactor     //3. an extra according to velocity
                    ;


                //apply
                plane.localPosition = originalPlanePosition;
                plane.position += TowardDestination(setup.player.playerMainObj) * totalOffset;

                SetClippingOffset(-(_trespassProgress) + setup.advanced.clippingOffset);
                
            } else {
                //RestorePlaneOriginalPosition();
            }

            lastFramePosition = setup.player.playerCameraTr.position;
        }

        void SetClippingOffset(float value) {
            cameraScript.currentClippingOffset = value;
            otherScript.cameraScript.currentClippingOffset = value;
        }


        Transform CreateCloneOnTheOtherSide(Transform original) {
            if (setup.debugOptions.verboseDebug) Debug.Log($"Creating clone for {original.name}");
            if (cloneParent == null) {
                cloneParent = GameObject.Find("Portal Clones") ?? new GameObject("Portal Clones");
            }

            //this clone already exists? remove it
            if (clones.ContainsKey(original)) TryDestroyClone(original);


            Transform clone = Instantiate(original, original.parent);
            //clone.SetParent(cloneParent.transform, true);
            clone.name = "(portal clone) " + original.name;

            clones.Add(original, clone);

            //destroy some components from itself and childrens, to obtain a simplified version of the object
            foreach (Rigidbody rb in clone.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
            foreach (Collider col in clone.GetComponentsInChildren<Collider>()) Destroy(col);
            foreach (Camera cam in clone.GetComponentsInChildren<Camera>()) cam.enabled = false; //can't destroy in HDRP
            foreach (CharacterController cc in clone.GetComponentsInChildren<CharacterController>()) Destroy(cc);
            foreach (AudioListener lis in clone.GetComponentsInChildren<AudioListener>()) Destroy(lis);
            foreach (MonoBehaviour scr in clone.GetComponentsInChildren<MonoBehaviour>()) {
                bool destroyScript = true;
                if (typeof(TMPro.TextMeshPro) != null && scr.GetType() == typeof(TMPro.TextMeshPro)) destroyScript = false;
                if (destroyScript) Destroy(scr);
            }


            clone.gameObject.tag = "Untagged";
            return clone;
        }

        void UpdateClone(Transform original) {
            if (!clones.ContainsKey(original)) return;

            Transform clone = clones[original];


            //similar calculations to the actual teleporting

            //--position
            clone.position = otherScript.portal.TransformPoint(
                portal.InverseTransformPoint(original.position)
            );

            //--rotation
            clone.rotation =
                 (Quaternion.Inverse(portal.rotation)
                 * otherScript.portal.rotation)
                 * original.rotation
            ;

        }

        public bool IsPlayer(Transform tr) => tr.CompareTag(setup.player.playerTag) || tr == setup.player.playerMainObj;

        bool ThisObjectCanCross(Transform obj) {
            //player always can cross
            if (IsPlayer(obj)) return true;

            //negative filter
            if (setup.filters.tagsCannotCross.Contains(obj.tag)) return false;

            //main filter
            switch (setup.filters.otherObjectsCanCross) {
                case PortalSetup.Filters.OthersCanCross.Everything: 
                    return true;

                case PortalSetup.Filters.OthersCanCross.NothingOnlyPlayer:
                    return false;

                case PortalSetup.Filters.OthersCanCross.OnlySpecificTags:
                    if (setup.filters.tagsCanCross.Count > 0 && !setup.filters.tagsCanCross.Contains(obj.tag)) return false;
                    return true;
            }

            return true;
        }


    }
}