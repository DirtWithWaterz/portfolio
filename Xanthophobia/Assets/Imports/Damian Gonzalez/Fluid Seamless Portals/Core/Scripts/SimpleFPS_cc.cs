using UnityEngine;

namespace DamianGonzalez.Portals {

    [DefaultExecutionOrder(110)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalSetup (rendering) > 114 PortalRenderer
    public class SimpleFPS_cc : MonoBehaviour {
        public static Transform thePlayer; //easy access
        public static Camera cameraComponent;

        CharacterController cc;
        Transform cameraTr;
        float rotX = 0;
        [SerializeField] float walkSpeed = 10f;
        [SerializeField] float runSpeed = 20f;
        [SerializeField] float slowSpeed = 2f;
        [SerializeField] float mouseSensitivity = 1f;


        bool locked = false;
        [SerializeField] bool pressEscToLock = true;
        
        [Header("Custom Gravity")]
        public MultiGravityOpt isMultiGravity = MultiGravityOpt.AutoDetect;
        public enum MultiGravityOpt { Yes, No, AutoDetect }
        CustomGravity gravityScript;

        

        [SerializeField] bool correctInclination = true;
        [Range(0.01f, .5f)] public float inclinationCorrectionSpeed = .1f;
        private void Awake() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            //auto-detect multi gravity
            if (isMultiGravity == MultiGravityOpt.AutoDetect) {
                isMultiGravity =
                    TryGetComponent<CustomGravity>(out gravityScript)
                    ? MultiGravityOpt.Yes
                    : MultiGravityOpt.No
                ;
            }

        }

        void Start() {
            thePlayer = transform;
            cc = GetComponent<CharacterController>();
            cameraTr = transform.GetChild(0);
            cameraComponent = cameraTr.GetComponent<Camera>();

            rotX = cameraTr.eulerAngles.x;


        }


        void Update() {

            Move();

            RotateAndLook();

            LockControls();

        }
		
        private void FixedUpdate() {
            CorrectInclination();
        }		

        private void LockControls() {
            //ESC to lock/unlock
            if (pressEscToLock && Input.GetKeyDown(KeyCode.Escape)) {
                locked = !locked;
            }
        }

        private void RotateAndLook() {
            if (!locked) {
                //rotate player left/right
                transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);

                //look up and down
                rotX += Input.GetAxis("Mouse Y") * mouseSensitivity * -1;
                rotX = Mathf.Clamp(rotX, -90f, 90f); //clamp look 
                cameraTr.localRotation = Quaternion.Euler(rotX, 0, 0);
            }
        }

		void CorrectInclination() {
			if (
                correctInclination && 
                (isMultiGravity == MultiGravityOpt.No || gravityScript.down == -Vector3.up)
            ) {
				//try to make player stand straight when tilted
				transform.rotation = (Quaternion.Lerp(
					transform.rotation,
					Quaternion.Euler(0, transform.eulerAngles.y, 0),
					inclinationCorrectionSpeed
				));
			}			
		}

        private void Move() {
            float speed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) speed = runSpeed;
            if (Input.GetKey(KeyCode.LeftControl)) speed = slowSpeed;

            if (!locked) {
                cc.Move(
                    transform.forward * speed * Input.GetAxis("Vertical") * Time.deltaTime    //move forward
                    +
                    transform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime    //slide to sides
                );

            }
        }

    }
}