using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame {
    
    public class CameraController : MonoBehaviour{

        private Vector2 mouseInput;
        private Vector2 mousePos;
        private Quaternion entityRotation;
        private Vector3 camPosition;
        private Vector3 delayedPosition;
        private Vector3 cameraRelativePosition;
        private Vector3 cameraTargetVector;
        private Vector3 cameraVector;
        [SerializeField]private float cameraSpeed = 0.1f; 
        [SerializeField]private float distanceToPlayer = 1f; 
        [SerializeField]private float fov = 90f;
        private float additionalDistance = 0f;

        private void Start(){
            // Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
        }

        private void Update(){
            mouseInput = Player.current.playerControls["Look"].ReadValue<Vector2>()*cameraSpeed;

            float additionalCameraSpeed = Player.current.controllerType == Player.ControllerType.Controller ? Player.current.stickSpeed : Player.current.mouseSpeed;
            mouseInput *= additionalCameraSpeed;

            if (Player.current.canLook){
                mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );
            }

            Cursor.visible = Player.current.menu;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;

            cameraRelativePosition = Player.current.entity.currentState.cameraPosition;

            cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer -additionalDistance);
            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 0.05f);
        }

        private void FixedUpdate(){
            Entity playerEntity = Player.current.entity;

            Quaternion mouseRotation = Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
            entityRotation = Quaternion.Slerp( entityRotation, playerEntity.rotation, 4f * Time.deltaTime );
            
            transform.rotation = entityRotation * mouseRotation;

            Vector3 entityColliderPosition = playerEntity["head"].transform.position;

            delayedPosition = Vector3.Slerp(delayedPosition, entityColliderPosition, 0.7f); 
            
            camPosition = delayedPosition + transform.rotation * cameraVector;
            
            if (Physics.Linecast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, Global.GroundMask ))
                camPosition = cameraCollisionHit.point + (delayedPosition - cameraCollisionHit.point).normalized/2f;
            transform.position = camPosition;

        }

        public void LookTowards(Vector3 forward){
            mousePos = (Quaternion.Inverse(entityRotation) * Quaternion.LookRotation(forward, entityRotation * Vector3.down)).eulerAngles;
        }
    }
}