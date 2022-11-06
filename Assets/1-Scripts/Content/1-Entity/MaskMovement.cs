// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using SeleneGame.Core;
// using SeleneGame.Entities;
// using SevenGame.Utility;

// namespace SeleneGame {

//     [RequireComponent(typeof(Animator))]
//     public class MaskMovement : MonoBehaviour {
        
//         public MaskedEntity entity;
        
//         private Animator _animator;
        
//         [SerializeField] private float maskPosT = 1f;
//         private bool onRight = true;
//         private Vector3 hoveringPosition;
//         private Vector3 relativePos;


//         private Vector3 rightPosition => entity.transform.rotation * new Vector3(1.2f, 1.3f, -0.8f);
//         private Vector3 leftPosition => entity.transform.rotation * new Vector3(-1.2f, 1.3f, -0.8f);

//         private Animator animator {
//             get {
//                 if ( _animator == null ) {
//                     _animator = GetComponent<Animator>();
//                 }
//                 return _animator;
//             }
//         }


//         public void SetEntity(MaskedEntity entity) {
//             this.entity = entity;
//             hoveringPosition = entity.transform.position + relativePos;
//             relativePos = rightPosition;
//         }


//         private void FixedUpdate(){
//             if (entity == null) return;

//             bool onFace = /* entity.isMasked() */true || (positionBlocked(leftPosition) && positionBlocked(rightPosition));

//             relativePos = Vector3.Lerp(relativePos, onRight ? rightPosition : leftPosition, 3f * GameUtility.timeDelta);

//             if (!onFace && (positionBlocked(relativePos)))
//                 onRight = !onRight;

//             hoveringPosition = Vector3.Lerp(hoveringPosition, entity.transform.position + relativePos, 15f * GameUtility.timeDelta);
//             maskPosT = Mathf.MoveTowards(maskPosT, System.Convert.ToSingle(onFace), 4f * GameUtility.timeDelta);

//             animator.SetBool("OnFace", onFace);
//             animator.SetFloat("OnRight", onRight ? 1f : 0f);

//             bool positionBlocked(Vector3 position) {
//                 return Physics.SphereCast(entity.transform.position, 0.35f, position, out _, position.magnitude, Global.GroundMask);
//             }
//         }

//         private void LateUpdate(){
//             if (entity == null) return;
            
//             BezierQuadratic currentCurve = new BezierQuadratic(
//                 hoveringPosition, 
//                 entity["head"].transform.position,
//                 entity["head"].transform.position + entity["head"].transform.forward
//             );
//             transform.position = currentCurve.GetPoint(maskPosT).position;
//             transform.rotation = Quaternion.Slerp(entity.transform.rotation,entity["head"].transform.rotation,maskPosT);
//         }
//     }
// }