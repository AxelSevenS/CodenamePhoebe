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
        
        [SerializeField] private List<SittingPosition> _sittingPositions = new List<SittingPosition>();
        private int sittingIndex = 0;



        protected virtual string seatedInteractionText => "";

        protected List<SittingPosition> sittingPositions {
            get {
                if (_sittingPositions == null || _sittingPositions.Count == 0)
                    _sittingPositions = defaultSittingPositions;

                return _sittingPositions;
            }
            set {
                _sittingPositions = value;
            }
        }

        protected virtual List<SittingPosition> defaultSittingPositions {
            get {
                return new List<SittingPosition>() {
                    new SittingPosition(
                        new Vector3(0f, 0f, 1f),
                        Quaternion.LookRotation(Vector3.back),
                        0
                    )
                };
            }
        }
        
        public SittingPosition sittingPosition => sittingPositions[sittingIndex];

        public Vector3 sitPosition { 
            get {
                Vector3 seatOccupantUp = seatOccupant?.transform.up ?? transform.up;
                float seatOccupantSize = seatOccupant?.character.size.y/2f ?? 1.67f;

                return transform.TransformPoint(sittingPosition.position) + (seatOccupantUp * seatOccupantSize);
            }
        }

        public Quaternion sitRotation {
            get {
                return transform.rotation * sittingPosition.rotation;
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
            
            SittingState sittingState = new SittingState();
            entity.SetState( sittingState );
            sittingState.seat = this;
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

        private void SelectSittingPosition(){
            sittingIndex = 0;

            for (int i = 0; i < sittingPositions.Count; i++){
                float distanceToOldPosition = Vector3.Distance(transform.TransformPoint(sittingPosition.position), seatOccupant.transform.position);
                float distanceToCurrentPosition = Vector3.Distance(transform.TransformPoint(sittingPositions[i].position), seatOccupant.transform.position);
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
            sittingPositions = defaultSittingPositions;
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
        public struct SittingPosition {
            public Vector3 position;
            public Quaternion rotation;
            public int subState;

            public SittingPosition(Vector3 position, Quaternion rotation, int subState){
                this.position = position;
                this.rotation = rotation;
                this.subState = subState;
            }
        }

    }
}