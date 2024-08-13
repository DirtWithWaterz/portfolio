using UnityEngine;

//simple first person controller using rigidbody, by Damián González, specially for portals asset.

namespace DamianGonzalez.Portals {
    [DefaultExecutionOrder(110)] //110 Movement > 111 Teleport > 112 PortalCamMovement > 113 PortalSetup (rendering) > 114 PortalRenderer
    public class SimpleFPS_rb : MonoBehaviour {
        public static Transform thePlayer; //easy access
        Rigidbody rb;
        Transform cam;
        public float walkSpeed = 5f;
        public float runSpeed  = 15f;
        public float slowSpeed = 1f;
        public Vector2 mouseSensitivity = new Vector2(1f, 1f);
        public float rotX;
        public float maxVelY = 10f;
        public bool standStraight = true;
        public float maxVelocity = 10f;
        public float jumpImpulse = 10f;

        [Header("Custom Gravity")]
        public MultiGravityOpt isMultiGravity = MultiGravityOpt.AutoDetect;
        public enum MultiGravityOpt { Yes, No, AutoDetect }
        CustomGravity gravityScript;

        void Start() {
            thePlayer = transform;
            rb = GetComponent<Rigidbody>();
            cam = Camera.main.transform;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            rotX = cam.localEulerAngles.x;

            if (rotX > 180) rotX -= 360;
            if (rotX < -180) rotX += 360;

            //auto-detect multi gravity
            if (isMultiGravity == MultiGravityOpt.AutoDetect) {
                isMultiGravity =
                    TryGetComponent<CustomGravity>(out gravityScript)
                    ? MultiGravityOpt.Yes
                    : MultiGravityOpt.No
                ;
            }

            
        }


        void Update() {
            float speed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) speed = runSpeed;
            if (Input.GetKey(KeyCode.LeftControl)) speed = slowSpeed;


            rb.velocity = (
                transform.forward * speed * Input.GetAxis("Vertical")    //move forward
                +
                transform.right * speed * Input.GetAxis("Horizontal")   //slide to sides
                +
                Up() * transform.InverseTransformDirection(rb.velocity).y  //let fall and jump (according to 'gravityScript')
            );


            //look up and down
            rotX += Input.GetAxis("Mouse Y") * mouseSensitivity.y * -1;
            rotX = Mathf.Clamp(rotX, -60f, 60f); //clamp look 
            cam.localRotation = Quaternion.Euler(rotX, 0, 0);


            //rotate player left/right
            //transform.Rotate(Up(), Input.GetAxis("Mouse X") * mouseSensitivity.x);
            transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSensitivity.x, 0);

            //slowly stand straight if tilted, but only in normal gravity (up is up)
            if (standStraight && Up() == Vector3.up) {
                rb.MoveRotation(Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.Euler(0, transform.eulerAngles.y, 0),
                    .1f
                ));
            }


            //jump
            if (Input.GetButtonDown("Jump")) {
                if (isMultiGravity == MultiGravityOpt.Yes && gravityScript != null && gravityScript.manageGravity) {
                    //managed by custom script
                } else {
                    //managed by this script
                    if (Physics.CheckSphere(transform.position - new Vector3(0, 1.5f, 0), .5f)) {
                        rb.AddForce(0, jumpImpulse, 0, ForceMode.Impulse);
                    }
                }
            
            }

        }

        Vector3 Up() {
            if (gravityScript == null) return Vector3.up;
            return -gravityScript.roundedDown;
        }

    }
}