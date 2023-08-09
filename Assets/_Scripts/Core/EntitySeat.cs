using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class EntitySeat : ObjectFollower, IInteractable {

        protected const string SEAT_INTERACT_DESCRIPTION = "Sit Down";



        [Space(15)]

        public Entity seatOccupant;
        public Entity seatEntity;

        [Space(15)]
        
        [SerializeField] private List<SittingPose> _sittingPoses = new();


        protected virtual string seatedInteractionText => "";

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


        public void SetSittingPoses( IEnumerable<SittingPose> newPoses ) {
            _sittingPoses = new List<SittingPose>(newPoses);
        }

        private SittingPose GetClosestPose(Entity entity) {

            if (_sittingPoses.Count == 0)
                return new SittingPose();

            SittingPose currentSittingPose = _sittingPoses[0];
            foreach (SittingPose pose in _sittingPoses) {

                if (currentSittingPose == pose)
                    continue;

                Transform referencePoint = seatEntity?.ModelTransform ?? transform;

                float currentDistance = Vector3.Distance(referencePoint.TransformPoint(currentSittingPose.position), entity.transform.position);
                float newDistance = Vector3.Distance(referencePoint.TransformPoint(pose.position), entity.transform.position);
                if (newDistance < currentDistance) {
                    currentSittingPose = pose;
                }
            }

            return currentSittingPose;
        }
    

        private void StartSitting(Entity entity) {

            StopSitting();

            seatOccupant = entity;

            SittingPose pose = GetClosestPose(entity);
            entity.SetBehaviour(new SittingBehaviour.Builder(this, pose));
        }

        public void StopSitting(){

            if (!isSeated) return;

            seatOccupant.ResetBehaviour();
        }


        private void Awake() {
            seatEntity = GetComponent<Entity>();
        }

        private void Update() {
            FollowObject();
        }


        [System.Serializable]
        public struct SittingPose {

            public Vector3 position;
            public Quaternion rotation;
            public bool affectCamera;

            public AnimationClip startAnimation;
            public AnimationClip loopAnimation;
            public AnimationClip endAnimation;

            public SittingPose(Vector3 position, Quaternion rotation, bool affectCamera, AnimationClip startAnimation, AnimationClip loopAnimation, AnimationClip endAnimation){
                this.position = position;
                this.rotation = rotation;
                this.affectCamera = affectCamera;
                this.startAnimation = startAnimation;
                this.loopAnimation = loopAnimation;
                this.endAnimation = endAnimation;
            }

            public static bool operator==(SittingPose pose1, SittingPose pose2){
                return pose1.position == pose2.position && pose1.rotation == pose2.rotation && pose1.affectCamera == pose2.affectCamera;
            }
            public static bool operator!=(SittingPose pose1, SittingPose pose2){
                return !(pose1 == pose2);
            }
            public override bool Equals(object obj){
                if (obj == null || GetType() != obj.GetType())
                    return false;

                SittingPose pose = (SittingPose)obj;
                return this == pose;
            }
            public override int GetHashCode(){
                return base.GetHashCode();
            }
        }

    }
}