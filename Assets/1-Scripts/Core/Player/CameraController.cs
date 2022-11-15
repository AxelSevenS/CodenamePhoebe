using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class CameraController : Singleton<CameraController>{

        public Camera camera;
        public UniversalAdditionalCameraData cameraData;

        private Quaternion entityRotation;
        private Vector3 delayedPosition;
        private Vector3 cameraVector;
        [SerializeField]private float distanceToPlayer = 1f; 
        [SerializeField]private float fov = 90f;

        private Vector3 velocity = Vector3.zero;
        private float additionalDistance = 0f;


        private void OnEnable(){
            SetCurrent();
        }

        private void Reset() => Awake();
        private void Awake(){
            camera = Camera.main;
            cameraData = camera.GetComponent<UniversalAdditionalCameraData>();
        }


        private void Update(){

            if (PlayerEntityController.current?.entity == null) return;

            UpdateCameraDistance();
        }

        private void FixedUpdate(){
            if (PlayerEntityController.current?.entity == null) return;

            UpdateCameraPosition();
        }
        private void UpdateCameraDistance(){
            Vector3 cameraRelativePosition = PlayerEntityController.current.entity.state.cameraPosition;
            Vector3 cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer -additionalDistance);

            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 3f * GameUtility.timeDelta);
        }

        private void UpdateCameraPosition(){
            entityRotation = Quaternion.Slerp( entityRotation, PlayerEntityController.current.entity.transform.rotation, 4f * GameUtility.timeDelta );
            
            transform.rotation = PlayerEntityController.current.worldCameraRotation;

            delayedPosition = Vector3.SmoothDamp(delayedPosition, PlayerEntityController.current.entity["head"].transform.position, ref velocity, 0.06f);
            Vector3 camPosition = transform.rotation * cameraVector;

            float camDistance = camPosition.magnitude;
            float distanceToWall = 0.4f;

            if ( Physics.Raycast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, camPosition.magnitude + distanceToWall, Global.GroundMask ) ){

                Vector3 collisionToPlayer = delayedPosition - cameraCollisionHit.point;
                Vector3 collisionTangent = Vector3.up;

                Debug.DrawRay( cameraCollisionHit.point, collisionToPlayer * 3f, Color.red );
                float collisionAngle = 90 - Vector3.Angle( collisionToPlayer.normalized, cameraCollisionHit.normal );

                float camMargin = distanceToWall / Mathf.Sin(collisionAngle * Mathf.Deg2Rad);
                
                camDistance = collisionToPlayer.magnitude - camMargin;
            }

            Vector3 finalPos = delayedPosition + camPosition.normalized * camDistance;
            
            transform.position = finalPos;
        }

    }
}