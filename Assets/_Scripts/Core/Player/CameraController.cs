using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class CameraController : MonoBehaviour{

        private Quaternion entityRotation;
        private Vector3 delayedPosition;
        private Vector3 cameraVector;
        [SerializeField]private float distanceToPlayer = 1f; 
        [SerializeField]private float fov = 90f;

        private Vector3 velocity = Vector3.zero;
        private float additionalDistance = 0f;


        private void Update() {

            if (Player.current?.entity == null) return;

            Cursor.visible = Player.current.menu;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;

            UpdateCameraDistance();
        }

        private void FixedUpdate() {
            if (Player.current?.entity == null) return;

            UpdateCameraPosition();
        }
        private void UpdateCameraDistance() {
            Vector3 cameraRelativePosition = Player.current.entity.state.cameraPosition;
            Vector3 cameraTargetVector = new Vector3( cameraRelativePosition.x, cameraRelativePosition.y, cameraRelativePosition.z * distanceToPlayer -additionalDistance);

            cameraVector = Vector3.Slerp(cameraVector, cameraTargetVector, 3f * GameUtility.timeDelta);
        }

        private void UpdateCameraPosition() {
            entityRotation = Quaternion.Slerp( entityRotation, Player.current.entity.rotation, 4f * GameUtility.timeDelta );
            
            transform.rotation = entityRotation * Player.current.entity.cameraRotation;

            delayedPosition = Vector3.SmoothDamp(delayedPosition, Player.current.entity["head"].transform.position, ref velocity, 0.06f);
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


        public Texture replacement;
        public Material material;

        private void OnRenderImage(RenderTexture src, RenderTexture dest) {
            if (material == null) return;
            // To overwrite the entire screen
            // Graphics.Blit(replacement, null);

            // Or to overwrite only what this specific Camera renders
            // Graphics.Blit(src, dest, material);
            Graphics.Blit(replacement, null as RenderTexture);
        }

    }
}