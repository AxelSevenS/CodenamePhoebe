using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame {
    
    public class Seat : ObjectFollower, IInteractable{

        [Space(15)]

        public Entity seatEntity;
        public Entity seatOccupant;
        private Transform previousAnchor;


        public bool isSeated => seatOccupant != null;

        [Space(15)]
        
        [SerializeField] private int speed = 4;
        [SerializeField] private List<Vector4> sittingDirections;
        public Vector3 sitPosition { get {
                Vector3 seatOccupantUp = isSeated ? seatOccupant.transform.up : transform.up;
                float seatOccupantSize = isSeated ? seatOccupant.data.size.y/2f : 1.67f;

                return transform.position + transform.rotation*(sittingDir) + (seatOccupantUp * seatOccupantSize);
            }
        }
        [HideInInspector] public Vector3 sittingDir;


        private void OnDisable(){
            StopSitting();
        }

        private void Update(){
            FollowObject();
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(sitPosition, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * sittingDir);
        }



        protected virtual string seatedInteractionText => "";
        public string InteractDescription() => seatOccupant ? seatedInteractionText : "Sit Down";

        public virtual bool IsInteractable() => !isSeated;
        public void Interact(Entity entity){
            if (entity == seatOccupant){
                SeatedInteract(entity);
                return;
            }
            StartSitting(entity);
        }
        protected virtual void SeatedInteract(Entity entity){;}

        

        private async void StartSitting(Entity entity){
            previousAnchor = entity.transform.parent;
            
            StopSitting();

            seatOccupant = entity;
            seatOccupant.SetState(seatOccupant.defaultState);
            
            CalculateClosestDirection(out sittingDir, out seatOccupant.subState);

            // if (speed < 4){
            //     seatOccupant.walkingTo = true;
            //     await seatOccupant.WalkTo( sitPosition, (Entity.WalkSpeed)(speed+1) );
            //     await seatOccupant.TurnTo( transform.rotation * -sittingDir );
            //     seatOccupant.walkingTo = false;
            // }
            
            entity.SetState( new SittingState() );
            ( (SittingState)entity.state ).seat = this;
            GameUtility.SetLayerRecursively(entity.gameObject, 8);

            seatOccupant.transform.SetParent(this.transform);

            // entity.SetAnimationState("Sitting", 0.2f);
        }

        public void StopSitting(){

            if (!isSeated) return;

            seatOccupant.SetState(seatOccupant.defaultState);
            GameUtility.SetLayerRecursively(seatOccupant.gameObject, 6);

            seatOccupant.transform.SetParent(previousAnchor);

            seatOccupant = null;
        }

        private void CalculateClosestDirection(out Vector3 finalDirection, out float subState){
            finalDirection = (transform.position - seatOccupant.transform.position).normalized;
            subState = 0f;

            foreach (Vector4 currentDirection in sittingDirections){
                Vector3 direction = new Vector3( currentDirection.x, currentDirection.y, currentDirection.z ).normalized;
                Vector3 directionToOccupant = (seatOccupant.transform.position - transform.position).normalized;

                if ( Vector3.Dot( transform.rotation * direction, directionToOccupant) > Vector3.Dot( transform.rotation * finalDirection, directionToOccupant ) ){
                    finalDirection = direction;
                    subState = currentDirection.w;
                }
            }
        }

        public void SetDirections(List<Vector4> sittingDirections){
            this.sittingDirections = sittingDirections;
        }

    }
}