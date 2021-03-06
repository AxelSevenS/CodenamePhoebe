using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    public class EntityController : MonoBehaviour {

        public Entity entity;

        [Header("Input")]

        public KeyInputData lightAttackInput;
        public KeyInputData heavyAttackInput;
        public KeyInputData jumpInput;
        public KeyInputData evadeInput;
        public KeyInputData walkInput;
        public KeyInputData crouchInput;
        public KeyInputData focusInput;
        public KeyInputData shiftInput;
        public Vector2Data moveInput;
        public Vector2Data lookInput;

        public virtual void RawInputToGroundedMovement(out Quaternion camRotation, out Vector3 groundedMovement){
            Vector3 camUp = entity.rotation * Vector3.up;
            Vector3 camForward = Vector3.Cross(Vector3.right, camUp).normalized;
            camRotation = Quaternion.LookRotation(camForward, camUp);
            groundedMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y);
        }
        public virtual void RawInputToCameraRelativeMovement(out Quaternion camRotation, out Vector3 cameraRelativeMovement){
            camRotation = Quaternion.identity;
            cameraRelativeMovement = new Vector3(moveInput.x, 0, moveInput.y);
        }

        protected virtual void OnEnable() => SetController(); 
        protected virtual void Reset() => SetController();

        protected virtual void SetController() {
            EntityController[] controllers = GetComponents<EntityController>();
            foreach (EntityController controller in controllers) {
                if (controller != this) {
                    GameUtility.SafeDestroy(controller);
                }
            }
        } 
    }
}
