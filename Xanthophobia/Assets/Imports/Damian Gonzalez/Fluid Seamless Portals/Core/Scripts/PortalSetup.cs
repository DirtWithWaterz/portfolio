using System.Collections.Generic;
using UnityEngine;
using System;


namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(114)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalSetup (rendering) > 114 PortalRenderer
    public class PortalSetup : MonoBehaviour {

        #region declarations

        public static PortalSetup lastInstance; //useful if you only have 1 set of portals

        [Tooltip("Optional. An unique name, for static references. If blank, an auto numerated name will be generated.")]
        public string groupId = "";

        [Serializable]
        public class Player {

            [FancyInfo("This setup is not necessary. Set these values only if the script fail to auto-setup itself.")]

            [Header("Player setup (not necessary)")]
            [Tooltip("Optional. Default value is main camera")]
            public Camera playerCameraComp;
            [HideInInspector] public Transform playerCameraTr; 
            public string playerTag = "Player";

            [Tooltip("Optional. Default value is main camera's parent")]
            public Transform playerMainObj;

            public enum ControllerType { Rigidbody, CharacterController, AutoDetect }
            [Tooltip("Optional. How the player moves. 'Auto' will try to guess")]
            public ControllerType controllerType = ControllerType.AutoDetect;

            [Tooltip("Optional. Ref. to player's character controller")]
            public CharacterController playerCc;

            [Tooltip("Optional. Ref. to player's rigid body")]
            public Rigidbody playerRb;

            [Tooltip("Check this if your player changes its 'field of view'")]
            public bool alwaysMimicPlayersFOV = false;


            [FancyInfo("The following setup is only for 3rd person controllers")]
            [Header("Only for 3rd person players")]
            public CameraType cameraType = CameraType.AutoDetect;
            public enum CameraType { FirstPerson, ThirdPerson, AutoDetect }


            [HideInInspector] public bool adjustOffsetPosition = true;
            public Vector3 nearOffsetPosition = new Vector3(0, .4f , 0);
            public bool adjustFieldOfView;
            public float nearFovValue = 40f;

            public enum TransitionAreaDetectionMode { ByCustomBox, ByRadialDistance, None }
            public TransitionAreaDetectionMode transitionAreaDetectionMode = TransitionAreaDetectionMode.ByRadialDistance;
            public float transitionRadius = 4f;
            public Vector3 customBoxCenter = new Vector3(0,1,0);
            public Vector3 customBoxSize = new Vector3(5,2,10);

            public bool letAddScriptToPlayerCam = true;
        }
        public Player player;																  
		 
        [HideInInspector] public bool setupComplete = false;

        [System.Serializable]
        public class InternalReferences {
            [HideInInspector] public int screenHeight;
            [HideInInspector] public int screenWidth;

            [FancyInfo(
                "This setup is not necessary while in 'runtime' workflow, " +
                "and will automatically set when you enter the 'deployed' workflow." +
                " It's recommended to leave these values as they are.",
                FancyInfoAttribute.FancyInfoType.Warning, 100
            )]


            [Tooltip("Already assigned by default. It should be 'screenRender.shader'")]
            public Shader planeShader;

            [Header("References")]
            public GameObject objCamA;
            public GameObject objCamB;

            public Camera cameraA;
            public Camera cameraB;

            public Transform planeA;
            public Transform planeB;

            public Transform portalA;
            public Transform portalB;

            public Teleport scriptPortalA;
            public Teleport scriptPortalB;

            public PortalCamMovement scriptCamA;
            public PortalCamMovement scriptCamB;

            public Renderer rendererA;
            public Renderer rendererB;

            public Transform functionalFolderA;
            public Transform functionalFolderB;
        }

		public InternalReferences refs;

        private float timeStartResize = -1;

        [HideInInspector] public bool doubleSided = false;



        [Serializable]
        public class Filters {

            public enum OthersCanCross { Everything, OnlySpecificTags, NothingOnlyPlayer };
            [FancyInfo("Obviously, player always can cross this set of portals. But what about other objects?")]
            [Header("Main filter")]
            public OthersCanCross otherObjectsCanCross;

            [Header("Positive tags")]
            [Tooltip("In case you chose 'specific tags' above, name here which objects can cross.")]
            public List<string> tagsCanCross = new List<string>();

            [Header("Gandalf filter")]
            [Tooltip("Whatever you chose above, objects with these tags shall not pass.")]
            public List<string> tagsCannotCross = new List<string>();
        }
        public Filters filters;



        [Serializable]
        public class Clones {
            [FancyInfo("If you use clones, objects will show in both portals while crossing. See documentation for details")]
            [Header("Use of clones")]
            public bool useClones = true;

            [Tooltip("If your player has a visible body (or parts of it) then you should check this option. See documentation for details.")]
            public bool clonePlayerToo = true;
        }
        public Clones clones;



        [Serializable]
        public class AfterTeleportOptions {
            
            [FancyInfo("If your player doesn't teleport, try checking these first 4 options. See documentation for more.")]
            [Header("When player teleports...")]
            [Tooltip("Remain checked if your player has a character controller component")]
            public bool tryResetCharacterController = true;
            [Tooltip("Some advanced characters may need this option to be repositioned")]
            public bool tryResetCameraObject = true;
            [Tooltip("Some advanced characters may need this option to be repositioned")]
            public bool tryResetCameraScripts = true;
            [Tooltip("Check this if the camera is not a child of the player")]
            public bool mantainPlayerOffset = false;

        }
        public AfterTeleportOptions afterTeleport;



        [Serializable]
        public class Advanced {

            [FancyInfo(
                "WARNING: Changing these values can produce unwanted results. " +
                "See documentation for details.", FancyInfoAttribute.FancyInfoType.Warning)]
            public int renderOrder = 0;


            [Header("Elastic plane")]
            [Tooltip("This value has to be equal or slightly higher than your player camera's near clipping plane")]
            [Range(.02f, 1f)] public float elasticPlaneOffset = .2f;
            [HideInInspector] public bool useElasticPlane = true;

            [Tooltip("At what distance from the plane the elastic plane should begin moving (see documentation)")]
            [Range(-.5f, .5f)] public float elasticPlaneMinTreshold = -.2f;
            [Tooltip("At what distance from the plane player teleports (see documentation)")]
            [Range(-.5f, .5f)] public float elasticPlaneTeleportWindowFrom = .1f;
            [Range(-.5f, .5f)] public float elasticPlaneTeleportWindowTo = .3f;


            [Header("Behave differently with fast player? (Not recommended)")]
            [Tooltip("Unnecessary. Should an offset based on velocity be applied? (see documentation)")]
            public bool dynamicOffsetBasedOnVelocity = false;
            [Tooltip("Unnecessary. Value of 1 covers the same distance the player is moving (see documentation)")]
            [Range(0f, 3f)] public float elasticPlaneVelocityFactor = 2f;



            [Header("Advanced visual settings")]
            [Tooltip("Always true if you don't want weird effects when portals are skewd or in narrow places")]
            public bool alignNearClippingPlane = true;
            [Tooltip("Add a little if you see a blank gap between portals")]
            [Range(-5f, 5f)] public float clippingOffset = .5f;
            [Tooltip("Always set a little number here, so you can travel without flickering")]
            [Range(.001f, 1f)] public float dontAlignNearerThan = .1f;
            [Tooltip("Unnecesary")]
            [Range(-2f, 2f)] public float dotCalculationOffset = 0;



            [FancyInfo("Additional considerations for very fast players (or very slow PC)")]
            [Header("Try to prevent high speed on trigger enter")]
            [Tooltip("Should use this feature?")]
            public bool tryPreventTooFastFailure = true;
            [Tooltip("See documentation. X axis: current FPS, Y axis: beyond this velocity, will teleport earlier.")]
            public AnimationCurve maxSpeedForElasticPlane;

            [Header("Try to teleport on trigger exit if it didn't")]
            public bool tryEmergencyTeleport = true;
            [Range(1f,5f)] public float maxElasticPlaneValueForEmergency = 3f;

            [Header("Others")]
            public int depthTexture = 16;
            public bool avoidZeroViewport = true;

        }

        public Advanced advanced;




        [Serializable]
        public class NestedPortals {
            [FancyInfo("This is useful when you can see a portal through another set of portals")]
            [Header("Any other portal visible through portal A?")]
            public Transform[] nestedPortalsA = new Transform[0];

            [Header("Any other portal visible through portal B?")]
            public Transform[] nestedPortalsB = new Transform[0];

            [Header("Other options")]
            public NestedPriorityOptions priorityWhenBothVisible = NestedPriorityOptions.ShowDirectly;
            public enum NestedPriorityOptions { ShowDirectly, ShowNested }
        }
        public NestedPortals nestedPortals;




        [Serializable]
        public class SurfacePortals {
            [FancyInfo("Are these portals going to be placed into surfaces (walls, cieling)?")]
            public bool isSurfacePortals = false;
            [Header("Colliders management")]
            [Space(10)]
            public bool disableCollidersNearPortals=false;
            public enum SurfaceCollisions { AllowEverythingExceptForbidden, AllowOnlyExplicit }
            public SurfaceCollisions filterType = SurfaceCollisions.AllowEverythingExceptForbidden;
            public List<string> allowedColliders;
            public List<string> forbiddenColliders;
            public bool getCollidersOnAwake = true;
            [Header("Surface management")]
            [Space(10)]
            public bool hideSurfacesBeforeRendering=false;

        }
        public SurfacePortals surfacePortals;





        [Serializable]
        public class GravityOptions {
            [FancyInfo("These portals are meant to change player's gravity?")]
            public bool applyMultiGravity = false;

        }
        public GravityOptions multiGravity;





        [Serializable]
        public class Optimization {
            [FancyInfo("Having many portals can be expensive. These options help reducing camera rendering as much as possible.")]
            [Tooltip("Check to only render cameras which projects on visible portals")]
            public bool disableUnnecessaryCameras = true;
            [Tooltip("Unnecesary when the previous option is checked")]
            public bool disableDistantCameras = false;
            [Tooltip("At what distance from the player portals should not render")]
            public float fartherThan = 100f;
            [Tooltip("Recommended. If checked, all portls will render the first frame, so 'something' is shown on them")]
            public bool renderCamerasOnFirstFrame = true; //so the portals with disabled cameras can show something

        }
        public Optimization optimization;



        [Serializable]
        public class Recursive {

		    [FancyInfo(
                "Is one portal facing the other? Try this option. See documentation for more info. " +
                "Tip: Rotate a little in the Y axis one of your portals, so the recursion is not infinite. " +
                "Remember, this can be expensive."
            )]
            public bool useRecursiveRendering = false;
			[Tooltip("How many times can you see your portal inside itself?")]
            public int maximumRenderPasses = 5;

        }
        public Recursive recursiveRendering;


        [Serializable]
        public class OnOffSwitch {
            [HideInInspector] public bool isOn;
            public enum OnOff { On, Off }
		    [FancyInfo("Does these portals have a switch or button?")]
			[Tooltip("Is the portal initially on or off?")]
            public OnOff initialState = OnOff.Off;

			[Tooltip("Once turned on, should it automatically turn off in a while?")]
            public bool isTimed = false;
            public float durationSeconds = 10f;
            [HideInInspector] public float momentLastOn = 0;
            [HideInInspector] public float desiredAngle = 0;

            [Header("Turn lights on/off to match state?")]
			[Tooltip("If checked, any light inside the portal will turned on/off to match the portal state")]
            public bool manageLights = true;
            [HideInInspector] public Light[] lights;

        }
        public OnOffSwitch onOffSwitch;


        [Serializable]
        public class DebugOptions {

            [FancyInfo("Useful tools for debugging")]

            [Tooltip("When checked, you'll get a lot of info in the console.")]
            public bool verboseDebug = false;

            [Tooltip("If checked, you'll see the sphere or cubic transition area for 3rd person")]
            public bool draw3rdPersonAreaGizmos = true;

            [Tooltip("If checked, you'll see lines showing the elastic plane points")]
            public bool drawElasticPlaneGizmos = true;

            [Tooltip("If checked, the game will pause when the player teleports")]
            public bool pauseWhenPlayerTeleports = false;

            [Tooltip("If checked, the game will pause when other objects (not the player) teleport")]
            public bool pauseWhenOtherTeleports = false;
        }
        public DebugOptions debugOptions;   

        private bool isFirstFrame = true;
        [HideInInspector] public bool forceActivateCamsInNextFrame = false;
        [HideInInspector] public bool teleportInProgress = false;
        [HideInInspector] public float lastTeleportTime = 0;

        public enum StartWorkflow { Runtime, Deployed }
		[Tooltip("Runtime: setup automatic on awake. Deployed: setup beforehand. See documentation")]
        public StartWorkflow startWorkflow = StartWorkflow.Runtime;


        private static int nextUniqueId = 0;
        private static bool alreadyShownFrustrumIssue = false;
        #endregion


        void Awake() {
            PortalInitialization();
        }


        
        void PortalInitialization() {


            //default group id
            if (groupId == "" || References.portalSets.ContainsKey(groupId)) {
                groupId = $"PortalSet{nextUniqueId++}";
            }

            //auto-deploy, and setup materials
            if (startWorkflow == StartWorkflow.Runtime) {
                DeployAndSetupMaterials();
            } else {
                SetupMaterials();
            }

            lastInstance = this;


          

            Portals.References.DeclareNewPortalSet(this);
            Portals.References.DeclareNewIndividualPortal(refs.scriptPortalA, $"{groupId}.A");
            Portals.References.DeclareNewIndividualPortal(refs.scriptPortalB, $"{groupId}.B");


            if (PortalRenderer.sceneHasPortalRenderer) {
                PortalRenderer.manualOrder.Add(advanced.renderOrder, this);
                if (advanced.renderOrder < PortalRenderer.minId) PortalRenderer.minId = advanced.renderOrder;
                if (advanced.renderOrder > PortalRenderer.maxId) PortalRenderer.maxId = advanced.renderOrder;
            }


            //does this portal has a lever? Let's tell him which portals to manage
            PortalLever[] allLevers = GetComponentsInChildren<PortalLever>();
            foreach (PortalLever lever in allLevers) {
                if (lever != null) {
                    lever.portalSet = this;
                }
            }


            //put a limit on the recursive rendering
            if (recursiveRendering.useRecursiveRendering && recursiveRendering.maximumRenderPasses > 20) {
                recursiveRendering.maximumRenderPasses = 20;
                Debug.LogWarning(
                    "Are you trying to make your PC explode? " +
                    "Recursive rendering has been limited to 20 recursions."
                );
            }

            //check window values
            if (advanced.elasticPlaneTeleportWindowFrom >= advanced.elasticPlaneTeleportWindowTo) { 
                advanced.elasticPlaneTeleportWindowTo = advanced.elasticPlaneTeleportWindowFrom + .2f;
                Debug.LogWarning(
                    "In variable 'ElasticPlaneTeleportWindow', 'to' should be greater than 'from'. " +
                    $"Therefore, ElasticPlaneTeleportWindowTo has been increased to {advanced.elasticPlaneTeleportWindowTo}"
                );
            }


            //does this portal has lights? Collect them
            onOffSwitch.lights = GetComponentsInChildren<Light>();


            //done
            setupComplete = true;
            PortalEvents.setupComplete?.Invoke(groupId, this);
        }

        [ContextMenu("Deploy and setup")]
        public void DeployAndSetupMaterials() {
            //first, quickly detect if it's already deployed
            if (transform.GetComponentInChildren<Camera>() != null) {
                Debug.LogWarning("This portal seems to be already deployed. Operation aborted.");
                return;
            }


            //if not provided, recognize player and its camera
            if (player.playerCameraComp == null) player.playerCameraComp = Camera.main;
            if (player.playerCameraComp == null) {
                Debug.LogError("If you don't specify players camera on PortalSetup, you should have an active camera with 'Main Camera' tag");
            }
            player.playerCameraTr = player.playerCameraComp.transform;

            //first try to get the player by tag
            if (player.playerMainObj == null) {
                GameObject _tempPlayer = GameObject.FindWithTag(player.playerTag);
                if (_tempPlayer != null) player.playerMainObj = _tempPlayer.transform;
            }

            //if fail, try to set player as camera's parent
            if (player.playerMainObj == null) { 
                player.playerMainObj = player.playerCameraTr.parent;
            }
            if (groupId == "") groupId = transform.name;

            
			//get rigidbody or character controller
            if (player.controllerType == Player.ControllerType.CharacterController && player.playerCc == null) {
                player.playerCc = player.playerMainObj.GetComponent<CharacterController>();
            }
            if (player.controllerType == Player.ControllerType.Rigidbody && player.playerRb == null) {
                player.playerRb = player.playerMainObj.GetComponent<Rigidbody>();
            }
            if (player.controllerType == Player.ControllerType.AutoDetect) {
                player.playerCc = player.playerMainObj.GetComponent<CharacterController>();
                player.playerRb = player.playerMainObj.GetComponent<Rigidbody>();
                if (player.playerCc != null) player.controllerType = Player.ControllerType.CharacterController;
                if (player.playerRb != null) player.controllerType = Player.ControllerType.Rigidbody;

                if (debugOptions.verboseDebug) Debug.Log("Controller type autodetected as " + player.controllerType.ToString());
            }
            if (player.controllerType == Player.ControllerType.AutoDetect) {
                Debug.LogWarning("Portal couldn't find neither a character controller nor a rigidbody component. Teleportation may fail.");
            }


            //detect type of camera
            if (player.cameraType == Player.CameraType.AutoDetect) {
                Vector3 offset = player.playerMainObj.InverseTransformPoint(player.playerCameraTr.position);

                player.cameraType =
                    (offset.z < -.2f)
                    ? Player.CameraType.ThirdPerson
                    : Player.CameraType.FirstPerson
                ;

                if (debugOptions.verboseDebug) Debug.Log("Camera type detected as " + player.cameraType.ToString());
            }
            player.adjustOffsetPosition = true; //mandatory

            
            //check frustrum issue
            if (!alreadyShownFrustrumIssue && player.playerCameraComp.nearClipPlane < 0.0105f) {
                Debug.LogWarning(
                    $"Warning: Your player's camera has a 'near clip plane' value of {player.playerCameraComp.nearClipPlane}. " +
                    $"Values this low can produce an Unity's error of 'Screen position out of view frustrum'. " +
                    $"If you see this error, please change this value to at least 0.012. Keep in mind, portal's 'elastic plane offset' " +
                    $"should be slightly greater than this."
                );
                alreadyShownFrustrumIssue = true;
            }


            //check that players camera's clipping plane is compatible with portal's
            if (player.playerCameraComp.nearClipPlane >= advanced.elasticPlaneOffset) {
                float newValue = Mathf.Max(advanced.elasticPlaneOffset / 2f, .011f);
                Debug.LogWarning(
                    $"Portal 'elastic plane offset' ({advanced.elasticPlaneOffset}) should be greater " +
                    $"than player camera's near clipping plane ({player.playerCameraComp.nearClipPlane}). " +
                    $"Camera near plane will be now adjusted to {newValue} to avoid flickering.");

                player.playerCameraComp.nearClipPlane = newValue;

            }

            //reference to each portal of this set
            refs.portalA = transform.GetChild(0);
            refs.portalB = transform.GetChild(1);

            refs.functionalFolderA = refs.portalA.GetChild(0);
            refs.functionalFolderB = refs.portalB.GetChild(0);

            Transform triggerA = refs.functionalFolderA.Find("trigger");
            Transform triggerB = refs.functionalFolderB.Find("trigger");

            refs.planeA = refs.functionalFolderA.Find("plane");
            refs.planeB = refs.functionalFolderB.Find("plane");

            refs.rendererA = refs.planeA.GetComponent<Renderer>();
            refs.rendererB = refs.planeB.GetComponent<Renderer>();


            //generate the empty objects for the cameras
            refs.objCamA = new GameObject("Camera (around A on plane B)");
            refs.objCamB = new GameObject("Camera (around B on plane A)");

            //and put them inside the containers. 
            refs.objCamA.transform.SetParent(refs.functionalFolderA, true);
            refs.objCamB.transform.SetParent(refs.functionalFolderB, true);

            //add camera components to the cameras
            refs.cameraA = refs.objCamA.AddComponent<Camera>();
            refs.cameraB = refs.objCamB.AddComponent<Camera>();


            //and its scripts
            refs.scriptCamA = refs.cameraA.gameObject.AddComponent<PortalCamMovement>();
            refs.scriptCamB = refs.cameraB.gameObject.AddComponent<PortalCamMovement>();

            //give this new cameras same setup than main camera
            refs.cameraA.CopyFrom(player.playerCameraComp);
            refs.cameraB.CopyFrom(player.playerCameraComp);

            refs.objCamA.tag = "Untagged";
            refs.objCamB.tag = "Untagged";

            //but if player camera has a non-standard viewport rect, correct the portal cameras
            refs.cameraA.rect = new Rect(0, 0, 1, 1);
            refs.cameraB.rect = new Rect(0, 0, 1, 1);



            //Setup both camera's scripts
            refs.scriptCamA.playerCamera = player.playerCameraTr;
            refs.scriptCamA.playerCameraComp = player.playerCameraComp;
            refs.scriptCamA.currentViewer = player.playerCameraTr;
            refs.scriptCamA.currentViewerCameraComp = player.playerCameraComp;
            refs.scriptCamA._camera = refs.cameraA;
            refs.scriptCamA._plane = refs.planeA;
            refs.scriptCamA.portal = refs.portalA;
            refs.scriptCamA._renderer = refs.rendererA;
            refs.scriptCamA._filter = refs.planeA.GetComponent<MeshFilter>();
            refs.scriptCamA._collider = triggerA.GetComponent<Collider>();
            refs.scriptCamA.otherScript = refs.scriptCamB;
            refs.scriptCamA.setup = this;
            refs.scriptCamA.cameraId = groupId + ".a";
            refs.scriptCamA.inverted = false;

            refs.scriptCamB.playerCamera = player.playerCameraTr;
            refs.scriptCamB.playerCameraComp = player.playerCameraComp;
            refs.scriptCamB.currentViewer = player.playerCameraTr;
            refs.scriptCamB.currentViewerCameraComp = player.playerCameraComp;
            refs.scriptCamB._camera = refs.cameraB;
            refs.scriptCamB._plane = refs.planeB;
            refs.scriptCamB.portal = refs.portalB;
            refs.scriptCamB._renderer = refs.rendererB;
            refs.scriptCamB._filter = refs.planeB.GetComponent<MeshFilter>();
            refs.scriptCamB._collider = triggerB.GetComponent<Collider>();
            refs.scriptCamB.otherScript = refs.scriptCamA;
            refs.scriptCamB.setup = this;
            refs.scriptCamB.cameraId = groupId + ".b";
            refs.scriptCamB.inverted = true;

            //and setup both portal's script
            refs.scriptPortalA = triggerA.GetComponent<Teleport>();
            refs.scriptPortalB = triggerB.GetComponent<Teleport>();

            refs.scriptPortalA.setup = this;
            refs.scriptPortalA.cameraScript = refs.scriptCamA;
            refs.scriptPortalA.otherScript = refs.scriptPortalB;
            refs.scriptPortalA.portal = refs.portalA;
            refs.scriptPortalA.plane = refs.planeA;
            refs.scriptPortalA._collider = refs.scriptPortalA.GetComponent<BoxCollider>();
            refs.scriptPortalA.planeIsInverted = false;


            refs.scriptPortalB.setup = this;
            refs.scriptPortalB.cameraScript = refs.scriptCamB;
            refs.scriptPortalB.otherScript = refs.scriptPortalA;
            refs.scriptPortalB.portal = refs.portalB;
            refs.scriptPortalB.plane = refs.planeB;
            refs.scriptPortalB._collider = refs.scriptPortalB.GetComponent<BoxCollider>();
            refs.scriptPortalB.planeIsInverted = true;


            refs.scriptCamA.thisSidePortalScript = refs.scriptPortalA;
            refs.scriptCamB.thisSidePortalScript = refs.scriptPortalB;

            //Create materials with the shader
            //and asign those materials to the planes (here is where they cross)
            SetupMaterials();


            //camera objects enabled, but cameras components disabled, we'll use manual rendering, even if is not recursive
            refs.objCamA.SetActive(true);
            refs.objCamB.SetActive(true);
            refs.cameraA.enabled = false;
            refs.cameraB.enabled = false;


            //deploy 3rd person detection trigger
            if (
                player.cameraType == Player.CameraType.ThirdPerson
                && player.transitionAreaDetectionMode != Player.TransitionAreaDetectionMode.None
            ) {
                GameObject objA = null, objB;
                objA = new GameObject("3rd person area trigger");
                objA.transform.SetParent(refs.functionalFolderA);
                objA.transform.rotation = refs.portalA.rotation;

                objB = new GameObject("3rd person area trigger");
                objB.transform.SetParent(refs.functionalFolderB);
                objB.transform.rotation = refs.portalB.rotation;

                if (player.transitionAreaDetectionMode == Player.TransitionAreaDetectionMode.ByRadialDistance) {
                    objA.transform.localPosition = Vector3.zero;
                    SphereCollider tempCollider;
                    tempCollider = objA.AddComponent<SphereCollider>();
                    tempCollider.radius = player.transitionRadius;
                    tempCollider.isTrigger = true;
                    

                    objB.transform.localPosition = Vector3.zero;
                    tempCollider = objB.AddComponent<SphereCollider>();
                    tempCollider.radius = player.transitionRadius;
                    tempCollider.isTrigger = true;
                }

                if (player.transitionAreaDetectionMode == Player.TransitionAreaDetectionMode.ByCustomBox) {
                    objA.transform.localPosition = Vector3.zero;
                    BoxCollider tempCollider;
                    tempCollider = objA.AddComponent<BoxCollider>();
                    tempCollider.center = player.customBoxCenter;
                    tempCollider.size = player.customBoxSize;
                    tempCollider.isTrigger = true;

                    objB.transform.localPosition = Vector3.zero;
                    tempCollider = objB.AddComponent<BoxCollider>();
                    tempCollider.center = player.customBoxCenter;
                    tempCollider.size = player.customBoxSize;
                    tempCollider.isTrigger = true;
                }

                objA.AddComponent<Portal3rdPersonArea>().triggerScript = refs.scriptPortalA; 
                objB.AddComponent<Portal3rdPersonArea>().triggerScript = refs.scriptPortalB;
                objA.layer = 2; //ignore raycast
                objB.layer = 2; //ignore raycast

                //and add a script to the player's camera (if it was allowed and it doesn't have one yet)
                if (player.letAddScriptToPlayerCam) {
                    if (!player.playerCameraTr.TryGetComponent(out ThirdPersonOffsetAndFov _temp)) {
                        player.playerCameraTr.gameObject.AddComponent<ThirdPersonOffsetAndFov>();
                    }
                }


            }



#if UNITY_EDITOR
            if (Application.isEditor) {
                //finally, mark changes as dirty, otherwise they won't be saved
                UnityEditor.EditorUtility.SetDirty(gameObject);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.EditorUtility.SetDirty(refs.scriptCamA);
                UnityEditor.EditorUtility.SetDirty(refs.scriptCamB);
                UnityEditor.EditorUtility.SetDirty(refs.scriptPortalA);
                UnityEditor.EditorUtility.SetDirty(refs.scriptPortalB);

                startWorkflow = StartWorkflow.Deployed;
            }
#endif			

        }

        [ContextMenu("Undeploy (back to 'runtime' mode")]
        public void Undeploy() {

            //restaure all internal references
            refs = new InternalReferences();


            //delete cameras
            foreach (Camera _cam in transform.GetComponentsInChildren<Camera>()) {
                DestroyImmediate(_cam.gameObject);
            }

            startWorkflow = StartWorkflow.Runtime;

        }

        void Start() {

            foreach (Transform tr in nestedPortals.nestedPortalsA) {
                PortalCamMovement pcm = tr.GetComponentInChildren<PortalCamMovement>();
                if (pcm != null && pcm.gameObject.activeInHierarchy) {
                    refs.scriptCamB.nested.Add(pcm);
                }
            }
            foreach (Transform tr in nestedPortals.nestedPortalsB) {
                PortalCamMovement pcm = tr.GetComponentInChildren<PortalCamMovement>();
                if (pcm != null && pcm.gameObject.activeInHierarchy) {
                    refs.scriptCamA.nested.Add(pcm);
                }
            }

            if (surfacePortals.isSurfacePortals && surfacePortals.getCollidersOnAwake) {
                refs.scriptPortalA.GetInitialColliders();
                refs.scriptPortalB.GetInitialColliders();
            }
        }


        List<GameObject> tempDisabledObjects = new List<GameObject>();
        private void Update() {
            //1 second after resize is done, it updates the render textures
            if (timeStartResize == -1 && (refs.screenHeight != Screen.height || refs.screenWidth != Screen.width)) timeStartResize = Time.time;

            if (timeStartResize > 0 && Time.time > timeStartResize + 1f) ResizeScreen();


            if (!PortalRenderer.sceneHasPortalRenderer) {

                refs.scriptCamA.int_renderPasses = 0;
                refs.scriptCamB.int_renderPasses = 0;

                //manually render cameras (only when necessary)
                refs.scriptCamA.ManualRenderIfNecessary();
                refs.scriptCamB.ManualRenderIfNecessary();

            }

            if (forceActivateCamsInNextFrame) forceActivateCamsInNextFrame = false;
            if (optimization.renderCamerasOnFirstFrame && isFirstFrame) forceActivateCamsInNextFrame = true;
            isFirstFrame = false;
           
        }


        void SetupMaterials() {

            int _width = Mathf.Max(Screen.width, 100);
            int _height = Mathf.Max(Screen.height, 100);
            timeStartResize = -1;

            //Create materials with the shader
            Material matA = new Material(refs.planeShader);
            refs.cameraA.targetTexture?.Release();
            refs.cameraA.targetTexture = new RenderTexture(_width, _height, advanced.depthTexture);
            matA.mainTexture = refs.cameraA.targetTexture;
            matA.SetTexture("_MainTex", refs.cameraA.targetTexture);

            

            Material matB = new Material(refs.planeShader);
            refs.cameraB.targetTexture?.Release();
            refs.cameraB.targetTexture = new RenderTexture(_width, _height, advanced.depthTexture);
            matB.mainTexture = refs.cameraB.targetTexture;

            //and asign those materials to the planes (here is where they cross)
            refs.rendererA.material = matB;
            refs.rendererB.material = matA;

			//remember this values to check them later
            refs.screenHeight = _height;
            refs.screenWidth = _width;

        }

        void ResizeScreen() {
            SetupMaterials();
            PortalEvents.gameResized?.Invoke(
                groupId,
                transform,
                new Vector2(refs.screenHeight, refs.screenWidth),
                new Vector2(Screen.height, Screen.width)
            );
        }

        Transform CreateShadowClone(GameObject obj) {
            Transform tr = Instantiate(obj).transform;

            //delete cameras
            foreach (Camera _cam in tr.GetComponentsInChildren<Camera>()) {
                Destroy(_cam);
            }

            //delete scripts
            foreach (MonoBehaviour _scr in tr.GetComponentsInChildren<MonoBehaviour>()) {
                Destroy(_scr);
            }

            //delete colliders
            foreach (Collider _col in tr.GetComponentsInChildren<Collider>()) {
                Destroy(_col);
            }

            //set renderers to only cast shadows
            foreach (MeshRenderer _mr in tr.GetComponentsInChildren<MeshRenderer>()) {
                _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }

            return tr;
        }

        void OnValidate() {
            //when values (like clippingOffset) changes in the editor, it should apply the changes immediatly
            if (!Application.isPlaying) return;
            if (refs.scriptCamA == null) return;

            refs.scriptCamA.currentClippingOffset = advanced.clippingOffset;
            refs.scriptCamB.currentClippingOffset = advanced.clippingOffset;

            refs.scriptCamA.ApplyAdvancedOffset();
            refs.scriptCamB.ApplyAdvancedOffset();
        }


        [ContextMenu("Disable both portals")]
        public void DisableBothPortals() {
            refs.scriptPortalA.DisableThisPortal();
            refs.scriptPortalB.DisableThisPortal();

            PortalEvents.onOffStateChanged?.Invoke(groupId, this, false);

            if (onOffSwitch.manageLights) {
                foreach (Light l in onOffSwitch.lights) l.gameObject.SetActive(false);
            }
        }

        [ContextMenu("Enable both portals")]
        public void EnableBothPortals() {
            refs.scriptPortalA.EnableThisPortal();
            refs.scriptPortalB.EnableThisPortal();

            PortalEvents.onOffStateChanged?.Invoke(groupId, this, true);

            if (onOffSwitch.manageLights) {
                foreach (Light l in onOffSwitch.lights) l.gameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR
        private Transform temp_portalA, temp_portalB;

        private void OnDrawGizmosSelected() {
            if (!debugOptions.draw3rdPersonAreaGizmos && !debugOptions.drawElasticPlaneGizmos) return;

            //prepare to draw
            Gizmos.color = Color.yellow;
            if (temp_portalA == null) {
                temp_portalA = (refs.portalA != null) ? refs.portalA : transform.GetChild(0);
                temp_portalB = (refs.portalB != null) ? refs.portalB : transform.GetChild(1);
            }

            if (debugOptions.draw3rdPersonAreaGizmos && player.cameraType != Player.CameraType.FirstPerson) {
                //draw third person area by distance
                if (player.transitionAreaDetectionMode == Player.TransitionAreaDetectionMode.ByRadialDistance) {
                    Gizmos.DrawWireSphere(temp_portalA.position, player.transitionRadius);
                    Gizmos.DrawWireSphere(temp_portalB.position, player.transitionRadius);
                }

                //draw third person area by custom box trigger
                if (player.transitionAreaDetectionMode == Player.TransitionAreaDetectionMode.ByCustomBox) {
                    Gizmos.matrix = temp_portalA.localToWorldMatrix;
                    Gizmos.DrawWireCube(player.customBoxCenter, player.customBoxSize);
                    Gizmos.matrix = temp_portalB.localToWorldMatrix;
                    Gizmos.DrawWireCube(player.customBoxCenter, player.customBoxSize);
                }
            }

            if (debugOptions.drawElasticPlaneGizmos && advanced.useElasticPlane) {
                void DrawElasticPlaneGizmos(Transform _portal, Vector3 _fwd) {
                    //draw elastic plane lines:
                    //1. center
                    Gizmos.color = Color.white;
                    Vector3 _start = _portal.position;
                    Vector3 _endOffset = Vector3.up * 4f;
                    Gizmos.DrawLine(_start, _start + _endOffset);

                    //2. min treshold
                    Gizmos.color = Color.cyan;
                    _start = _portal.position + _fwd * advanced.elasticPlaneMinTreshold;
                    Gizmos.DrawLine(_start, _start + _endOffset);

                    //3. teleport window
                    Gizmos.color = Color.blue;
                    _start = _portal.position + _fwd * advanced.elasticPlaneTeleportWindowFrom;
                    Gizmos.DrawLine(_start, _start + _endOffset);
                    _start = _portal.position + _fwd * advanced.elasticPlaneTeleportWindowTo;
                    Gizmos.DrawLine(_start, _start + _endOffset);
                }

                DrawElasticPlaneGizmos(temp_portalA, temp_portalA.forward);
                DrawElasticPlaneGizmos(temp_portalB, -temp_portalA.forward);
            }
        }
#endif

        public bool IsSurfaceColliderAllowed(Collider col) {

            //positive filter
            if (surfacePortals.filterType == SurfacePortals.SurfaceCollisions.AllowEverythingExceptForbidden) {
                //allowed, so far
            } else {
                //not listed in positive filter: not allowed
                if (!surfacePortals.allowedColliders.Contains(col.gameObject.tag)) return false;
            }

            //negative filter
            return surfacePortals.forbiddenColliders.Contains(col.gameObject.tag);
        }
    }
}