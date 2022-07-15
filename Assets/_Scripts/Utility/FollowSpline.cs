using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    public class FollowSpline : MonoBehaviour{

        [SerializeField] private float speed, moveSpeed, slowDown;
        [SerializeField] private bool onGoing;
        [SerializeField] private bool goingForward = true;
        [SerializeField] private Spline spline;
        private OrientedPoint splinePosition;
        [Range(0, 1f)] [SerializeField] private float weight = 0.9995f;
        [Range(0, 1f)] [SerializeField] private float t;
        private float prevT;
        private float increment;

        void FixedUpdate(){
            prevT = t;

            moveSpeed = Mathf.MoveTowards(moveSpeed, speed * System.Convert.ToSingle(onGoing) * (System.Convert.ToSingle(goingForward)*2f - 1f), 1f-weight);
            if ((goingForward && spline.nextSegment == null) || (!goingForward && spline.prevSegment == null))
                slowDown = Mathf.Min(1f, Mathf.Abs(System.Convert.ToSingle(goingForward)-t)+0.2f);
            else
                slowDown = 1f;

            increment = (moveSpeed * slowDown / 10f * GameUtility.timeDelta);
            
            t += increment;

            SwitchSegment();

            splinePosition = spline.GetBezier(t);
            transform.position = Vector3.Lerp(transform.position, splinePosition.position, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, splinePosition.rotation, 0.1f);
        }

        private void SwitchSegment(){
            if (goingForward && t >= 1f){
                if (spline.nextSegment != null){
                    spline = spline.nextSegment;
                    t -= 1f;
                }else{
                    goingForward = false;
                }
            }else if (!goingForward && t <= 0f){
                if (spline.prevSegment != null){
                    spline = spline.prevSegment;
                    t += 1f;
                }else{
                    goingForward = true;
                }
            }
        }

    }
}