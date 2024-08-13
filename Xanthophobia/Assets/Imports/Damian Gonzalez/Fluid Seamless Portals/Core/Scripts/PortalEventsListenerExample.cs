using System.Collections.Generic;
using UnityEngine;


namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(999)]
    public class PortalEventsListenerExample : MonoBehaviour {
        private void Start() {

            //subscription to public events
            PortalEvents.teleport += SomethingTeleported;
            PortalEvents.setupComplete += PortalSetupComplete;
            PortalEvents.gameResized += GameWindowHasResized;
            PortalEvents.rayThroughPortals += RayWentThroughPortals;
            PortalEvents.onOffStateChanged += StateChanged;


        }

        void SomethingTeleported(string groupId, Transform portalFrom, Transform portalTo, Transform objectTeleported, Vector3 positionFrom, Vector3 positionTo) {
            Debug.Log(
                $"{objectTeleported.name} teleported " +
                $" from {groupId}.{portalFrom.name} to {groupId}.{portalTo.name}"
            , objectTeleported);
        }


        void PortalSetupComplete(string groupId, PortalSetup portalSet) {

        }

        void GameWindowHasResized(string groupId, Transform portal, Vector2 oldSize, Vector2 newSize) {
            Debug.Log(
                $"Game window has resized from {oldSize} to {newSize}. " +
                $"Therefore, portal '{groupId}' updated its cameras."
            );
        }

        void RayWentThroughPortals(Vector3 from, Vector3 lastHitPos, List<PortalRaycast.RayInfo> subrays) {

            string[] names = new string[subrays.Count];
            for (int i = 0; i < subrays.Count; i++) {
                names[i] = subrays[i].portalId;
            }

            Debug.Log($"A ray went through {(subrays.Count - 1)} portals ({string.Join(", ", names)})");
        }

        void StateChanged(string groupId, PortalSetup portalSet, bool newState) {
            Debug.Log($"Portal {groupId} is now {(newState ? "on" : "off")}");
        }
        


    }
}