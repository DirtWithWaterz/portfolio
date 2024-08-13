using UnityEngine;

namespace DamianGonzalez.Portals {
    public class CustomGravity : MonoBehaviour {

        //this script:
        //
        // - recieves and provides information on "what" is up and down.

        // - if "manageGravity" is enabled and a rigidbody is attached:
        //      1) disables auto gravity in the rigidbody
        //      2) applies custom gravity on FixedUpdate
        //      3) to make this object jump, call the Jump() method.
        

        // note: if your player uses character controller, this script WON'T apply gravity, since it would be too intrusive.
        // in that case, you should use a normal character controller script, but check this script's "down" instead of Vector3.Down


        public bool manageGravity = true;
        public float gravity = 9.81f; //always positive
        
        public Vector3 down;
        public Vector3 roundedDown;

        public Rigidbody rb;
        public CharacterController cc;

        [Header("Only for jumping")]
        public bool canJump = true;
        public bool isGrounded;
        public float jumpImpulse = 10f;
        public float verticalVelocity;
        public float groundDistance = -1.5f;
        public float groundCheckRadius = .1f;
        public LayerMask floorLayerMask = ~0;


        void Start() {
            ChangeGravity(down);

            //try to get rb
            if (rb == null) {
                TryGetComponent(out rb);
            }
            if (rb != null && manageGravity) rb.useGravity = false;
            

            //try to get cc
            if (cc == null) TryGetComponent(out cc);

            if (rb == null && cc == null) {
                Debug.LogWarning("CustomGravity couldn't get a rigidbody or a character controller.");
            }

            if (down == Vector3.zero) down = -Vector3.up;
        }



        
        private void Update() {
            if (!manageGravity) return;

            if (cc != null) ManageCcInUpdate();

            if (isGrounded && canJump && Input.GetButtonDown("Jump")) Jump();

        }

        void FixedUpdate() {
            if (!manageGravity) return;
            if (rb != null) ManageRbInFixedUpdate();

        }

        void ManageCcInUpdate() {
            isGrounded = CheckIfGrounded();
            if (!isGrounded) {
                //if not grounded, make the player fall
                verticalVelocity -= gravity * Time.deltaTime;
            } else {
                if (verticalVelocity < .1f) verticalVelocity = -.1f;
            }

            cc.Move(-down * verticalVelocity * Time.deltaTime);
        }

        void ManageRbInFixedUpdate() {
            isGrounded = CheckIfGrounded();
            rb.AddForce(down * gravity, ForceMode.Acceleration);
        }


        public void Jump() {
            if (!manageGravity) return;
            if (rb != null) {
                rb.AddForce(-down * jumpImpulse, ForceMode.Impulse);
            }
            if (cc != null) {
                verticalVelocity = jumpImpulse;
            }
        }


        public bool CheckIfGrounded() {
            if (!manageGravity) return false;
            if (rb != null && rb.velocity.y > .1f) return false;
            return (Physics.OverlapSphere(transform.position + (down * groundDistance), groundCheckRadius, floorLayerMask).Length > 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (!canJump) return;
            Gizmos.color = Color.yellow;
            if (down == Vector3.zero) down = -Vector3.up;
            Gizmos.DrawWireSphere(transform.position + (down * groundDistance), groundCheckRadius);
        }
#endif
               
        public void ChangeGravity(Vector3 _newDown) {
            down = _newDown;
            roundedDown = new Vector3(
                Mathf.RoundToInt(_newDown.x),
                Mathf.RoundToInt(_newDown.y),
                Mathf.RoundToInt(_newDown.z)
            );
        }
    } 
}