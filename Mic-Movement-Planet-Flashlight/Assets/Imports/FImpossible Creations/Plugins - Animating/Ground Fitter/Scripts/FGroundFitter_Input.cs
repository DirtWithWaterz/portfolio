using UnityEngine;

namespace FIMSpace.GroundFitter
{
    public class FGroundFitter_Input : FGroundFitter_InputBase
    {
        [SerializeField] private bool canBackUp = true;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canSprint = true;

        private PlayerControls controls;

        private KeyCode forward;
        private KeyCode backward;
        private KeyCode left;
        private KeyCode right;
        private KeyCode jump;
        private KeyCode sprint;
        private KeyCode backup;

        GameObject objectToFind;
        GameObject secondObjectToFind;
        string tagName = "MainCamera";
        [HideInInspector] public bool shouldUpdateInputs = false;

        private void Awake()
        {
            objectToFind = GameObject.FindGameObjectWithTag(tagName);
            controls = objectToFind.GetComponent<PlayerControls>();
            secondObjectToFind = GameObject.FindGameObjectWithTag("Player");
            forward = controls.forward;
            left = controls.left;
            backward = controls.backward;
            right = controls.right;
            jump = controls.jump;
            sprint = controls.sprint;
        }


        protected virtual void Update()
        {
            if (shouldUpdateInputs) UpdateInputs();

            if (Input.GetKeyDown(jump) && canJump) TriggerJump();

            Vector3 dir = Vector3.zero;

            if (Input.GetKey(forward) || Input.GetKey(left) || Input.GetKey(backward) || Input.GetKey(right))
            {
                if (Input.GetKey(sprint) && canSprint) Sprint = true; else Sprint = false;

                if (Input.GetKey(forward)) dir.z += 1f;
                if (Input.GetKey(left)) dir.x -= 1f;
                if (Input.GetKey(right)) dir.x += 1f;
                if (Input.GetKey(backward)) dir.z -= 1f;

                dir.Normalize();

                RotationOffset = Quaternion.LookRotation(dir).eulerAngles.y;

                MoveVector = Vector3.forward;
            }
            else
            {
                Sprint = false;
                MoveVector = Vector3.zero;
            }

            if (Input.GetKey(KeyCode.X) && canBackUp) MoveVector -= Vector3.forward;

            MoveVector.Normalize();

            controller.Sprint = Sprint;
            controller.MoveVector = MoveVector;
            controller.RotationOffset = RotationOffset;
        }

        protected virtual void UpdateInputs(){
            forward = controls.forward;
            left = controls.left;
            backward = controls.backward;
            right = controls.right;
            jump = controls.jump;
            sprint = controls.sprint;
            shouldUpdateInputs = false;
            return;
        }
    }
}