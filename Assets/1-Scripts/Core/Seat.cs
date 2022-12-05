using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class Seat : ObjectFollower, IInteractable{

        protected const string SEAT_INTERACT_DESCRIPTION = "Sit Down";



        [Space(15)]

        public Entity seatEntity;
        public Entity seatOccupant;
        private Transform previousParent;

        [Space(15)]
        
        [SerializeField] private List<SittingPose> _sittingPoses = new List<SittingPose>();
        private int sittingIndex = 0;



        protected virtual string seatedInteractionText => "";

        // protected List<SittingPose> sittingPoses {
        //     get {
        //         if (_sittingPoses == null || _sittingPoses.Count == 0)
        //             _sittingPoses = defaultSittingPoses;

        //         return _sittingPoses;
        //     }
        //     set {
        //         _sittingPoses = value;
        //     }
        // }

        // protected virtual List<SittingPose> defaultSittingPoses {
        //     get {
        //         return new List<SittingPose>() {
        //             new SittingPose(
        //                 new Vector3(0f, 0f, 1f),
        //                 Quaternion.LookRotation(Vector3.back)
        //             )
        //         };
        //     }
        // }

        protected List<SittingPose> sittingPoses;
        
        public SittingPose currentSittingPose => sittingPoses[sittingIndex];

        public Vector3 sitPosition { 
            get {
                Vector3 seatOccupantUp = seatOccupant?.transform.up ?? transform.up;
                float seatOccupantSize = seatOccupant?.character.size.y/2f ?? 1.67f;

                return transform.TransformPoint(currentSittingPose.position) + (seatOccupantUp * seatOccupantSize);
            }
        }

        public Quaternion sitRotation {
            get {
                return transform.rotation * currentSittingPose.rotation;
            }
        }

        public bool isSeated => seatOccupant != null;

        public string InteractDescription => seatOccupant ? seatedInteractionText : SEAT_INTERACT_DESCRIPTION;

        public virtual bool IsInteractable => !isSeated;



        public void Interact(Entity entity){
            if (entity == seatOccupant){
                SeatedInteract(entity);
                return;
            }
            StartSitting(entity);
        }

        protected virtual void SeatedInteract(Entity entity){;}

    
        private /* async */ void StartSitting(Entity entity){
            previousParent = entity.transform.parent;
            
            StopSitting();

            seatOccupant = entity;
            seatOccupant.SetState(seatOccupant.defaultState);
            
            SelectSittingPosition();

            // if (speed < 4){
            //     seatOccupant.walkingTo = true;
            //     await seatOccupant.WalkTo( sitPosition, (Entity.WalkSpeed)(speed+1) );
            //     await seatOccupant.TurnTo( transform.rotation * -sittingDir );
            //     seatOccupant.walkingTo = false;
            // }
            
            Sitting sittingState = new Sitting();
            sittingState.seat = this;
            entity.SetState( sittingState );
            GameUtility.SetLayerRecursively(entity.gameObject, 8);

            seatOccupant.transform.SetParent(this.transform);

            // entity.SetAnimationState("Sitting", 0.2f);
        }

        public void StopSitting(){

            if (!isSeated) return;

            seatOccupant.SetState(seatOccupant.defaultState);
            GameUtility.SetLayerRecursively(seatOccupant.gameObject, 6);

            seatOccupant.transform.SetParent(previousParent);

            seatOccupant = null;
        }

        public void SetSittingPoses( IEnumerable<SittingPose> newPoses ) {
            sittingPoses = new List<SittingPose>(newPoses);
        }

        private void SelectSittingPosition(){
            sittingIndex = 0;

            for (int i = 0; i < sittingPoses.Count; i++){
                float distanceToOldPosition = Vector3.Distance(transform.TransformPoint(currentSittingPose.position), seatOccupant.transform.position);
                float distanceToCurrentPosition = Vector3.Distance(transform.TransformPoint(sittingPoses[i].position), seatOccupant.transform.position);
                if (distanceToCurrentPosition < distanceToOldPosition){
                    sittingIndex = i;
                }
            }
        }



        private void OnDisable(){
            StopSitting();
        }

        private void Reset() {
            seatEntity = GetComponent<Entity>();
            sittingPoses.Clear();
            // sittingPoses = defaultSittingPoses;
        }

        private void Update(){
            FollowObject();
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(sitPosition, 0.3f);
            Gizmos.DrawLine(transform.position, sitPosition);
        }



        [System.Serializable]
        public struct SittingPose {

            public Vector3 position;
            public Quaternion rotation;

            public AnimationClip startAnimation;
            public AnimationClip loopAnimation;
            public AnimationClip endAnimation;

            public SittingPose(Vector3 position, Quaternion rotation){
                this.position = position;
                this.rotation = rotation;
                startAnimation = null;
                loopAnimation = null;
                endAnimation = null;
            }
        }

    }
}