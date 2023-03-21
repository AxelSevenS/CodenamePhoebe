using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class CameraController : Singleton<CameraController>{

        public new Camera camera;
        public UniversalAdditionalCameraData cameraData;


        private Vector3 cameraVector = Vector3.zero;
        private float cameraDistance = 0.1f;
        private float additionalDistance = 0f;
        private float fov = 90f;

        private Vector3 delayedHorizontalPosition = Vector3.zero;
        private Vector3 delayedVerticalPosition = Vector3.zero;

        private Vector3 verticalVelocity = Vector3.zero;
        private Vector3 horizontalVelocity = Vector3.zero;
        private float distanceVelocity = 0f;


        // Options
        [SerializeField] private Vector3 cameraOriginPosition;
        [SerializeField] private float distanceToPlayer = 1f;
        [SerializeField] private float horizontalSmoothTime = 0.02f;
        [SerializeField] private float verticalSmoothTime = 0.04f;



        private CameraType cameraType => PlayerEntityController.current?.entity?.state?.cameraType ?? CameraType.ThirdPerson;


        
        private void UpdateCamera(){
        }



        private void OnEnable(){
            SetCurrent();
        }

        private void Reset() => Awake();
        private void Awake() {
            camera = Camera.main;
            cameraData ??= camera.GetComponent<UniversalAdditionalCameraData>();
        }


        private void LateUpdate() {
            if (PlayerEntityController.current?.entity == null) return;
            
            

            Vector3 cameraRelativePosition = Global.cameraDefaultPosition;
            Vector3 cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer - additionalDistance);

            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 3f * GameUtility.timeUnscaledDelta);
            
            transform.rotation = PlayerEntityController.current.worldCameraRotation;



            float verticalSpeed = verticalSmoothTime;

            if ( cameraType != CameraType.ThirdPersonGrounded )
                verticalSpeed = horizontalSmoothTime;


            Vector3 followPosition = PlayerEntityController.current.entity["head"].transform.position + cameraOriginPosition;

            // Make The Camera Movement slower on the Y axis than on the X axis
            Vector3 camHorizontalPos = Vector3.ProjectOnPlane( followPosition, PlayerEntityController.current.entity.gravityDown );
            if ( delayedHorizontalPosition != camHorizontalPos)
                delayedHorizontalPosition = Vector3.SmoothDamp( delayedHorizontalPosition, camHorizontalPos, ref horizontalVelocity, horizontalSmoothTime );

            Vector3 camVerticalPos = followPosition - camHorizontalPos;
            if ( delayedVerticalPosition != camVerticalPos)
                delayedVerticalPosition = Vector3.SmoothDamp( delayedVerticalPosition, camVerticalPos, ref verticalVelocity, verticalSpeed );

            Vector3 delayedPosition = delayedHorizontalPosition + delayedVerticalPosition;
            // Vector3 delayedPosition = followPosition;



            Vector3 camPosition = transform.rotation * cameraVector;
            if (cameraDistance != camPosition.magnitude)
                cameraDistance = Mathf.SmoothDamp( cameraDistance, camPosition.magnitude, ref distanceVelocity, 0.2f );

            // Check for collision with the camera
            const float distanceToWall = 0.4f;
            if ( Physics.Raycast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, camPosition.magnitude + distanceToWall, Global.GroundMask ) ){

                Vector3 collisionToPlayer = delayedPosition - cameraCollisionHit.point;
                Vector3 collisionTangent = Vector3.up;

                Debug.DrawRay( cameraCollisionHit.point, collisionToPlayer * 3f, Color.red );
                float collisionAngle = 90 - Vector3.Angle( collisionToPlayer.normalized, cameraCollisionHit.normal );

                // Fancy Trigonometry to keep the camera at least distanceToWall away from the wall
                float camMargin = distanceToWall / Mathf.Sin(collisionAngle * Mathf.Deg2Rad);
                
                cameraDistance = collisionToPlayer.magnitude - camMargin;
            }

            Vector3 finalPos = delayedPosition + camPosition.normalized * cameraDistance;
            
            transform.position = finalPos;

        }



        public enum CameraType {
            ThirdPerson,
            ThirdPersonGrounded,
            Fixed
        }

    }
}