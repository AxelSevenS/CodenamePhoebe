using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame {

    public class MaskMovement : MonoBehaviour{
        
        public Entity entity;
        private RaycastHit MaskPosHit;
        private Animator animator;
        
        [SerializeField] private float maskPosT = 1f;
        private Vector3 rightPosition => entity._transform.rotation * new Vector3(1.2f, 1.3f, -0.8f);
        private Vector3 leftPosition => entity._transform.rotation * new Vector3(-1.2f, 1.3f, -0.8f);
        private Vector3 relativePos => onRight ? rightPosition : leftPosition;
        private bool positionBlocked(Vector3 position) => Physics.SphereCast(entity._transform.position, 0.35f, position, out MaskPosHit, position.magnitude, Global.GroundMask);
        private Vector3 flyingPosition;


        private bool onRight;

        void Awake(){
            animator = GetComponent<Animator>();
        }

        void Start(){
            onRight = true;
        }

        void FixedUpdate(){
            if (entity == null || !(entity is GravityShifterEntity gravityShifter)) return;

            bool onFace = gravityShifter.isMasked() || (positionBlocked(leftPosition) && positionBlocked(rightPosition));

            if (!onFace && (positionBlocked(relativePos)))
                onRight = !onRight;

            flyingPosition = Vector3.Lerp(flyingPosition, gravityShifter._transform.position + relativePos, 15f * Global.timeDelta);
            maskPosT = Mathf.MoveTowards(maskPosT, System.Convert.ToSingle(onFace), 4f * Global.timeDelta);

            animator.SetBool("OnFace", onFace);
            animator.SetFloat("OnRight", onRight ? 1f : 0f);
        }
        void LateUpdate(){
            if (entity == null) return;
            BezierCurve currentCurve = new BezierCurve(
                flyingPosition, 
                entity["head"].transform.position,
                flyingPosition + (entity["head"].transform.position - flyingPosition)/2f,
                entity["head"].transform.position + entity["head"].transform.forward
            );
            transform.position = currentCurve.GetPoint(maskPosT).position;
            transform.rotation = Quaternion.Slerp(entity._transform.rotation,entity["head"].transform.rotation,maskPosT);
        }
    }
}