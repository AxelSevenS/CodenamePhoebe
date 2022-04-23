using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame {
    
    public class Seat : ObjectFollower, IInteractable{

        [Space(15)]

        public Entity seatEntity;
        public Entity seatOccupant;
        private Transform previousAnchor;

        [Space(15)]
        
        [SerializeField] private int speed;
        [SerializeField] private Vector3 relativePos;
        [SerializeField] private List<Vector4> Directions;
        public Vector3 sitPosition { get {
                if (seatOccupant != null ) 
                    return transform.position + transform.rotation*relativePos + seatOccupant.transform.up*( seatOccupant.data.size.y/2f - transform.localScale.y/2f );

                return transform.position + transform.rotation*relativePos + transform.up*( 1.67f - transform.localScale.y/2f );
            }
        }

        public string interactionDescription => seatOccupant == Player.current.entity ? seatedInteractionText : "Sit Down";
        public void Interact(Entity entity){
            if (entity == seatOccupant){
                SeatedInteract(entity);
                return;
            }
            StartSitting(entity);
        }

        protected virtual string seatedInteractionText => "";
        protected virtual void SeatedInteract(Entity entity){;}


        private void Awake(){
            seatEntity = GetComponent<Entity>();
        }

        private void OnDestroy(){
            StopSitting();
        }

        private void Update(){
            FollowObject();
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(sitPosition, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * relativePos);
        }

        private async void StartSitting(Entity entity){
            seatOccupant = entity;

            seatOccupant.transform.SetParent(this.transform);

            entity.SetState("Walking");
            
            CalculateClosestDirection(out Vector3 direction, out seatOccupant.subState);

            if (speed < 4){
                seatOccupant.walkingTo = true;
                await seatOccupant.WalkTo( sitPosition, (Entity.WalkSpeed)(speed+1) );
                await seatOccupant.TurnTo( transform.rotation * -relativePos );
                seatOccupant.walkingTo = false;
            }

            previousAnchor = seatOccupant.transform.parent;
            
            entity.SetState("Sitting");
            Global.SetLayerRecursively(entity.gameObject, 8);
            ((SittingState)entity.state).seat = this;


            entity.AnimatorTrigger("Sit");
        }

        public void StopSitting(){

            if (seatOccupant == null) return;

            seatOccupant._transform.SetParent(previousAnchor);

            seatOccupant.SetState("Walking");
            Global.SetLayerRecursively(seatOccupant.gameObject, 6);

            seatOccupant = null;
        }

        private void CalculateClosestDirection(out Vector3 finalDirection, out float subState){
            finalDirection = (transform.position - seatOccupant.transform.position).normalized;
            subState = 0f;

            foreach (Vector4 currentDirection in Directions){
                Vector3 direction = new Vector3( currentDirection.x, currentDirection.y, currentDirection.z ).normalized;
                Vector3 directionToOccupant = (seatOccupant.transform.position - transform.position).normalized;

                if ( Vector3.Dot( transform.rotation * direction, directionToOccupant) > Vector3.Dot( transform.rotation * finalDirection, directionToOccupant ) ){
                    finalDirection = direction;
                    subState = currentDirection.w;
                }
            }
        }
    }
}