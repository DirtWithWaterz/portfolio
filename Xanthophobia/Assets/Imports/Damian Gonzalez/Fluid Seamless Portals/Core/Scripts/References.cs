using System.Collections.Generic;
using UnityEngine;

namespace DamianGonzalez.Portals {

    public class References  {
        public static Dictionary<string, PortalSetup> portalSets = new Dictionary<string, PortalSetup>();
        public static Dictionary<string, Teleport> individualPortals = new Dictionary<string, Teleport>();


        public static void DeclareNewPortalSet(PortalSetup script) {
            if (portalSets.ContainsKey(script.groupId)) {
                Debug.LogWarning($"Portal set '{script.groupId}' already exists. In order to use automatic references, give each portal set an unique name.");
                portalSets.Remove(script.groupId); 
                //preserves the new one, in case the references persist from previous gameplay
            }
            
            portalSets.Add(script.groupId, script);

        }

        public static void DeclareNewIndividualPortal(Teleport script, string name) {
            if (individualPortals.ContainsKey(name)) {
                Debug.LogWarning($"Portal '{name}' already exists. In order to use automatic references, give each portal an unique name.");
                individualPortals.Remove(name);
            }

            individualPortals.Add(name, script);
        }
    }
}