#if (UNITY_EDITOR) 

using UnityEngine;
using UnityEditor;

/*
 *  All this code provides is visual help and cosmetics.
 *  So, if you eventually get any warnings or errors, 
 *  consider deleting this file althogheter or moving it temporally out of the \Editor folder
 */

namespace DamianGonzalez.Portals {
    [CustomEditor(typeof(PortalSetup))]
    [CanEditMultipleObjects]
    [HelpURL("https://www.damian-gonzalez.com/portals/")]

    public class PortalSetupEditor : Editor {
        public Texture editorLogo;
        public override void OnInspectorGUI() {
            //logo
            if (editorLogo != null) {
                GUI.DrawTexture(new Rect(0, 20, Screen.width, 90), editorLogo, ScaleMode.ScaleAndCrop);
                GUILayout.Space(20 + 90 + 20);
            }

            GUILayout.Space(20);
			
            //help box
            EditorGUILayout.HelpBox(
                "Hover the mouse over the variables for a description. " +
                "If you need assistance, contact the developer.",
                
                MessageType.Info, true
            );

            GUILayout.Space(20);
																			  
																									  

            //display exposed variables
            base.OnInspectorGUI();


            //and buttons below
            PortalSetup script = (PortalSetup)target;

            GUILayout.Space(50);


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Switch to 'Runtime' mode", GUILayout.Height(20))) {
                script.Undeploy();
                Debug.Log("Changed to 'runtime' mode");
            }
            if (GUILayout.Button("Switch to 'Deployed' mode", GUILayout.Height(20))) {
                script.DeployAndSetupMaterials();
                Debug.Log("Changed to 'deployed' mode");
            }

            GUILayout.EndHorizontal();

            //margin below
            GUILayout.Space(10);
        }

    }
}
#endif