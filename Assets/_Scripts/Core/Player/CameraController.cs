using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {
    
    public class CameraController : MonoBehaviour{

        private Quaternion entityRotation;
        private Vector3 camPosition;
        private Vector3 delayedPosition;
        private Vector3 cameraRelativePosition;
        private Vector3 cameraTargetVector;
        private Vector3 cameraVector;
        [SerializeField]private float distanceToPlayer = 1f; 
        [SerializeField]private float fov = 90f;
        private float additionalDistance = 0f;

        private void Start() {
        }

        private void Update() {

            Cursor.visible = Player.current.menu;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;

            cameraRelativePosition = Player.current.entity.currentState.cameraPosition;

            cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer -additionalDistance);
            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 0.05f);
        }

        private void FixedUpdate() {
            Entity playerEntity = Player.current.entity;

            entityRotation = Quaternion.Slerp( entityRotation, playerEntity.rotation, 4f * Time.deltaTime );
            
            transform.rotation = entityRotation * playerEntity.lookRotationData.currentValue;

            Vector3 entityColliderPosition = playerEntity["head"].transform.position;

            delayedPosition = Vector3.Slerp(delayedPosition, entityColliderPosition, 0.7f); 
            
            camPosition = delayedPosition + transform.rotation * cameraVector;
            
            if (Physics.Linecast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, Global.GroundMask ))
                camPosition = cameraCollisionHit.point + (delayedPosition - cameraCollisionHit.point).normalized/2f;
            transform.position = camPosition;

        }

        // public void LookTowards(Vector3 forward){
        //     mousePos = (Quaternion.Inverse(entityRotation) * Quaternion.LookRotation(forward, entityRotation * Vector3.down)).eulerAngles;
        // }
    }
}