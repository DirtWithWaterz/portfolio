using System.Collections.Generic;
using System;
using UnityEngine;


/* 
 * this script controls what the plane on THIS portal displays,
 * moving the camera around the OTHER portal
 */

namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(112)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalSetup (rendering) > 114 PortalRenderer
    public class PortalCamMovement : MonoBehaviour {
        //these 10 variables are assigned automatically by the Setup script

        [HideInInspector] public Transform playerCamera;
        [HideInInspector] public Transform currentViewer;            //usually playerCamera, except on nesting portals
        [HideInInspector] public Camera currentViewerCameraComp;     
        [HideInInspector] public PortalSetup setup;
        [HideInInspector] public PortalCamMovement otherScript;
        [HideInInspector] public Transform portal;
        [HideInInspector] public Camera _camera;
        [HideInInspector] public Transform _plane;
        [HideInInspector] public Renderer _renderer;
        [HideInInspector] public MeshFilter _filter;
        [HideInInspector] public Collider _collider;
        [HideInInspector] public bool inverted;
        [HideInInspector] public Transform shadowClone;
        [HideInInspector] public Camera playerCameraComp;
        [HideInInspector] public float currentClippingOffset = 0;
        [HideInInspector] public Teleport thisSidePortalScript;


        public List<PortalCamMovement> nested = new List<PortalCamMovement>();


        public string cameraId;                     //useful if some debugging is needed. This is automatically assigned by portalSetup


        [HideInInspector] public string renderPasses;   //as text, for a visual info
        [HideInInspector] public int int_renderPasses;  //as int, just for counting

        [System.Serializable]
        public class PosAndRot {
            public Vector3 position = Vector3.zero;
            public Quaternion rotation = Quaternion.identity;

            public void Zero() { 
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }
        }
        

        List<PosAndRot> recursions = new List<PosAndRot>();


        private void Start() {

            if (playerCamera == null) playerCamera = Camera.main.transform;
            if (playerCameraComp == null) playerCameraComp = Camera.main;

            setup.refs.scriptPortalA.RestorePlaneOriginalPosition();
        }


        public PosAndRot CalculateTeleportedPositionAndRotation(Transform tr, Transform reference, Transform thisPortal, Transform otherPortal) {
            
            //rotation
            Quaternion _rotation =
                 (thisPortal.rotation
                 * Quaternion.Inverse(otherPortal.rotation))
                 * reference.rotation
            ;


            //position
            Vector3 distanceFromPlayerToPortal = (reference.position) - (otherPortal.position);
            Vector3 whereTheOtherCamShouldBe = thisPortal.position + (distanceFromPlayerToPortal);// + offset.position + tempPos;
            Vector3 _position = RotatePointAroundPivot(
                whereTheOtherCamShouldBe,
                thisPortal.position ,
                (thisPortal.rotation  * Quaternion.Inverse(otherPortal.rotation ) ).eulerAngles
            );

            return new PosAndRot() {
                position = _position,
                rotation = _rotation
            };
        }

        
        public void ApplyAdvancedOffset(bool ignoreDistance = false) {
            //if player is too near to the plane, don't apply the advanced offset
            if (!ignoreDistance && Vector3.Distance(
                    otherScript._collider.ClosestPoint(currentViewer.position),
                    currentViewer.position 
            ) < setup.advanced.dontAlignNearerThan) {
                _camera.projectionMatrix = playerCameraComp.projectionMatrix;
                return;
            }

            //not too near (or forced). continue.

            Vector3 point =
                _plane.position
                + portal.forward * setup.advanced.dotCalculationOffset * (inverted ? -1f : 1f)
                - transform.position;

            int dot = Math.Sign(Vector3.Dot(portal.forward, point));

            //rotate near clipping plane, so it matches portal rotation

            Plane p = new Plane(portal.forward * dot, _plane.position);
            Vector4 clipPlane = new Vector4(
                p.normal.x, 
                p.normal.y, 
                p.normal.z,
                p.distance + playerCameraComp.nearClipPlane + currentClippingOffset
            );

            Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(_camera.worldToCameraMatrix)) * clipPlane;
            var newMatrix = playerCameraComp.CalculateObliqueMatrix(clipPlaneCameraSpace);

            _camera.projectionMatrix = newMatrix; //


        }


        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
            Vector3 dir = point - pivot; // get point direction relative to pivot
            dir = Quaternion.Euler(angles) * dir; // rotate it
            point = dir + pivot; // calculate rotated point
            return point; // return it
        }


        public void Recalculate() {
            
            PosAndRot pr = CalculateTeleportedPositionAndRotation(transform, currentViewer, portal, otherScript.portal);
            transform.SetPositionAndRotation(pr.position, pr.rotation);
        }


        public void ManualRenderIfNecessary() {
            
            if (currentViewer == null) currentViewer = playerCamera;
            if (currentViewerCameraComp == null) currentViewerCameraComp = playerCameraComp;

            Recalculate();


            //only render cameras when player is seeing the planes that render them
            thisCameraIsVisible = ShouldRenderCamera(otherScript._renderer, currentViewerCameraComp, otherScript._plane);
            if (thisCameraIsVisible) {
                //this camera will be rendered below

                //if any other portal is nested (visible from this portal),
                //trick those other cameras to render as if player was the in other side of this portal
                CheckNestedRendering();

            } else {

                //has nested portals? then set default camera to those planes
                foreach (PortalCamMovement pcm in nested) {
                    pcm.otherScript.currentViewer = playerCamera;
                    pcm.otherScript.currentViewerCameraComp = playerCameraComp;

                }

                return;

            }


            //if asked, mimic the field of view of the main camera
            if (setup.player.alwaysMimicPlayersFOV) {
                _camera.fieldOfView = playerCameraComp.fieldOfView;
            }


            //surface portals? temporally remove the obstructing surface
            if (setup.surfacePortals.isSurfacePortals && setup.surfacePortals.hideSurfacesBeforeRendering) {
                foreach (Collider col in thisSidePortalScript.nearCollidersToDisable) col.gameObject.SetActive(false);
            }


            //render
            if (setup.recursiveRendering.useRecursiveRendering) {
                ManualRenderRecursive();
            } else {
                ManualRenderNotRecursive();
            }


            //restore hidden surfaces
            if (setup.surfacePortals.isSurfacePortals && setup.surfacePortals.hideSurfacesBeforeRendering) {
                foreach (Collider col in thisSidePortalScript.nearCollidersToDisable) col.gameObject.SetActive(true);
            }
        }

        public void ManualRenderNotRecursive() {

            if (setup.advanced.alignNearClippingPlane) ApplyAdvancedOffset();



            if (setup.doubleSided) _renderer.enabled = false; //the other plane may get in the way


            
            try {
                _camera.Render();
            }
            catch (Exception) {
            }

            int_renderPasses++;
            if (setup.doubleSided) _renderer.enabled = true;

        }

        
        void ManualRenderRecursive() {

            if (setup.doubleSided) _renderer.enabled = false; //if double sided, the other plane gets in the way

            recursions.Clear();

            Matrix4x4 localToWorldMatrix = currentViewer.transform.localToWorldMatrix;
            _camera.projectionMatrix = playerCameraComp.projectionMatrix;


            //from first "normal" position, it calculates the inner rendering positions until the plane is not visible
            for (int i = 0; i < setup.recursiveRendering.maximumRenderPasses; i++) {

                //calculate
                localToWorldMatrix = portal.localToWorldMatrix * otherScript.portal.transform.worldToLocalMatrix * localToWorldMatrix;

                transform.SetPositionAndRotation(
                    localToWorldMatrix.GetColumn(3),
                    localToWorldMatrix.rotation
                );

                recursions.Insert(0, new PosAndRot() {
                    position = transform.position,
                    rotation = transform.rotation
                });

                //to add in future versions: check if player can see the interior of this recursion. If not, stop recursion

            }


            //with positions and rotations calculated, now render them in reverse order
            //(inner recursions first)
            for (int i = 0; i < recursions.Count; i++) {
                transform.SetPositionAndRotation(recursions[i].position, recursions[i].rotation);
                
                //apply offset, but only consider distance in main render (last one)
                ApplyAdvancedOffset(i < recursions.Count - 1);

                //render
                try {
                    _camera.Render();
                }
                catch (Exception ) { }


                int_renderPasses++;
            }

            if (setup.doubleSided) _renderer.enabled = true;

        }


        void CheckNestedRendering() {

            foreach (PortalCamMovement pcm in nested) {
                bool shouldOverrideCamera = true;
                if (setup.nestedPortals.priorityWhenBothVisible == PortalSetup.NestedPortals.NestedPriorityOptions.ShowDirectly) {
                    if (pcm.otherScript.thisCameraIsVisible) shouldOverrideCamera = false;
                }


                pcm.otherScript.currentViewer = shouldOverrideCamera ? _camera.transform : playerCamera;
                pcm.otherScript.currentViewerCameraComp = shouldOverrideCamera ? _camera : playerCameraComp;
            }

        }

        public bool thisCameraIsVisible=false;
        public bool ShouldRenderCamera(Renderer renderer, Camera camera, Transform plane) {
            if (setup.forceActivateCamsInNextFrame) return true;
            if (!setup.optimization.disableUnnecessaryCameras) return true;
            

            //1st filter: distance
            if (setup.optimization.disableDistantCameras) {
                if (Vector3.Distance(plane.position, currentViewer.position) > setup.optimization.fartherThan) return false;
            }

            //2nd filter: portal is visible?
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds)) return false;

            //3rd filter (only for one-sided portals): portal visible, but is the "good" side visible?
            if (setup.doubleSided) return true;

            float dotProduct = Vector3.Dot(
                inverted ? -otherScript.portal.forward : otherScript.portal.forward,
                currentViewer.position - otherScript.portal.position
            );

            float dotMarginForCams = .1f;

            return (dotProduct > 0 - dotMarginForCams); //instead of 0, let's give it a safe margin in case player is crossing sideways
                                                        //(my worst nightmare!)

        }


    }
}