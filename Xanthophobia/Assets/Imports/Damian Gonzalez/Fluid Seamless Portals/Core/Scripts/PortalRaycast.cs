using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {
    static public class PortalRaycast {
        public class RayInfo {
            public Vector3 from;
            public Vector3 direction;
            public bool didHit;
            public RaycastHit hit;
            public string portalId;
            public Teleport portalScript;
        }
        static int currentDepthRays = 0;
        static float gap = .1f;
        static LayerMask default_LayerMask = ~0;
        const int default_maxDepthPortalRays = 20;
        const QueryTriggerInteraction default_queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;


        //definition for 2 parameters:
        static public bool Raycast(
            Vector3 from,
            Vector3 direction
        ) {
            return Raycast(
                from, 
                direction, 
                out RaycastHit lastHit, 
                out List<RayInfo> subRays, 
                default_LayerMask
            );
        }


        //definition for 3 parameters (since subRays can't be default):
        static public bool Raycast(
            Vector3 from,
            Vector3 direction,
            out RaycastHit lastHit
        ) {
            return Raycast(
                from, 
                direction, 
                out lastHit, 
                out List<RayInfo> subRays,
                default_LayerMask
            );
        }


        //definition for 4 parameters (since layerMask can't be default):
        static public bool Raycast(
            Vector3 from,
            Vector3 direction,
            out RaycastHit lastHit,
            out List<RayInfo> subRays
        ) {
            return Raycast(
                from, 
                direction, 
                out lastHit, 
                out subRays,
                default_LayerMask
            );
        }


        //definition for 5, 6 or 7 parameters:
        static public bool Raycast(
            Vector3 from,
            Vector3 direction,
            out RaycastHit lastHit,
            out List<RayInfo> subRays,
            LayerMask layerMask,
            int maxDepthPortalRays = default_maxDepthPortalRays,
            QueryTriggerInteraction queryTriggerInteraction = default_queryTriggerInteraction
        ) {

            subRays = new List<RayInfo>();

            currentDepthRays++;
            if (currentDepthRays >= maxDepthPortalRays) {
                lastHit = new RaycastHit();
                currentDepthRays = 0;
                return false;
            }


            //emit a "normal" raycast
            bool didHit = Physics.Raycast(
               from,
               direction,
               out lastHit,
               500f,
               layerMask,
               QueryTriggerInteraction.Collide  //we need to collide with the trigger
            );

            RayInfo thisRay = new RayInfo() {
                from = from,
                direction = direction,
                didHit = didHit,
                hit = lastHit,
                portalId = "",
                portalScript = null
            };

            if (didHit) {
                if ("triggerTrigger".Contains(lastHit.transform.gameObject.name)) {
                    //raycast hit a portal plane. 
                    //good or bad side, it will go into recursion

                    Vector3 newFrom;
                    Vector3 newDirection;
                    Teleport portalScript = lastHit.transform.GetComponent<Teleport>();

                    thisRay.portalScript = portalScript;
                    thisRay.portalId = portalScript.cameraScript.cameraId;
                    
                    //remember: lastHit.transform is the trigger, not the plane or the portal

                    //if visible side of the plane was hit OR if this portal is double sided...
                    if (portalScript.IsGoodSide(direction) || portalScript.setup.doubleSided ) {
                        //...then "teleport" the ray.

                        Transform thisPlane = lastHit.transform;
                        Transform otherPlane = thisPlane.GetComponent<Teleport>().otherScript.plane;

                        newDirection = otherPlane.parent.TransformDirection(
                            thisPlane.parent.InverseTransformDirection(direction)
                        );

                        newFrom = otherPlane.parent.TransformPoint(
                            thisPlane.parent.InverseTransformPoint(lastHit.point)
                        );

                    } else {
                        //it didn't hit the visible side. Continue without teleporting
                        newDirection = direction;
                        newFrom = lastHit.point;

                    }


                    //go deeper in recursion
                    bool lastBoolean = PortalRaycast.Raycast(
                        newFrom + newDirection * gap,
                        newDirection,
                        out lastHit,
                        out List<RayInfo> newSubRays,
                        layerMask,
                        maxDepthPortalRays,
                        queryTriggerInteraction
                    );

                    subRays = newSubRays;
                    subRays.Insert(0, thisRay);
                    return lastBoolean; //not finished yet, just step out of this level


                } else {
                    //hit something else. Recursion ends.
                    currentDepthRays = 0;
                    subRays.Insert(0, thisRay);

                }
                //portalEvents.rayThroughPortals?.Invoke(from, lastHit.point, subRays);
                return true;
            } else {
                //didn't hit anything. Recursion ends.
                subRays.Insert(0, thisRay);
                currentDepthRays = 0;
                return false;
            }

        }
    }
}