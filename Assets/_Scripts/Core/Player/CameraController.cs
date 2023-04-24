using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(50)]
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


        [SerializeField] private float verticalTime = 0f;



        private CameraType cameraType => Player.current?.entity?.behaviour?.cameraType ?? CameraType.ThirdPerson;



        private void OnEnable(){
            SetCurrent();
        }

        private void Reset() => Awake();
        private void Awake() {
            camera = Camera.main;
            cameraData ??= camera.GetComponent<UniversalAdditionalCameraData>();
        }
        private void Start() {
            verticalTime = horizontalSmoothTime;
        }


        private void LateUpdate() {
            if (Player.current?.entity == null) return;
            
            

            Vector3 cameraRelativePosition = new Vector3(1f, 0f, -3.5f);
            Vector3 cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer - additionalDistance);

            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 3f * GameUtility.timeUnscaledDelta);
            
            transform.rotation = Player.current.worldCameraRotation;



            // The camera's vertical movement gets faster as the player keeps moving vertically

            // The camera's new vertical speed is based on the camera's current vertical velocity
            float targetTime = Mathf.Lerp( verticalSmoothTime, horizontalSmoothTime, Mathf.Clamp01(verticalVelocity.sqrMagnitude) );
            // Accelerate faster than decelerate
            float transitionSpeed = targetTime > verticalTime ? 1.5f : 0.5f;
            verticalTime = Mathf.Lerp( verticalTime, targetTime, transitionSpeed * GameUtility.timeDelta );



            Vector3 followPosition = Player.current.entity["head"].transform.position + cameraOriginPosition;

            // Make The Camera Movement slower on the Y axis than on the X axis
            Vector3 camHorizontalPos = Vector3.ProjectOnPlane( followPosition, Player.current.entity.gravityDown );
            // delayedHorizontalPosition = camHorizontalPos;
            if ( delayedHorizontalPosition != camHorizontalPos)
                delayedHorizontalPosition = Vector3.SmoothDamp( delayedHorizontalPosition, camHorizontalPos, ref horizontalVelocity, horizontalSmoothTime, Mathf.Infinity, Time.deltaTime );

            Vector3 camVerticalPos = followPosition - camHorizontalPos;
            // delayedVerticalPosition = camVerticalPos;
            if ( delayedVerticalPosition != camVerticalPos)
                delayedVerticalPosition = Vector3.SmoothDamp( delayedVerticalPosition, camVerticalPos, ref verticalVelocity, cameraType != CameraType.ThirdPersonGrounded ? horizontalSmoothTime : verticalTime );

            Vector3 delayedPosition = delayedHorizontalPosition + delayedVerticalPosition;



            Vector3 camPosition = transform.rotation * cameraVector;
            if (cameraDistance != camPosition.magnitude)
                cameraDistance = Mathf.SmoothDamp( cameraDistance, camPosition.magnitude, ref distanceVelocity, 0.2f );

            // Check for collision with the camera
            const float distanceToWall = 0.4f;
            if ( Physics.Raycast( delayedPosition, camPosition, out RaycastHit cameraCollisionHit, camPosition.magnitude + distanceToWall, Collision.GroundMask ) ){

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