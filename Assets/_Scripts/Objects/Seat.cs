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
                else 
                    return transform.position + transform.rotation*relativePos + transform.up*( 1.67f - transform.localScale.y/2f );
            }
        }

        public string interactionDescription => seatOccupant == Player.current.entity ? seatedInteractionText : "Sit Down";

        private void OnDestroy(){
            UnSit();
        }

        private void Awake(){
            seatEntity = GetComponent<Entity>();
        }

        private void Update(){
            FollowObject();
        }

        private void OnDrawGizmosSelected(){
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(sitPosition, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + transform.rotation * relativePos);
        }

        public void UnSit(){
            if (seatOccupant == null) return;
            StopSitting(seatOccupant);
        }

        public void Interact(Entity entity){
            if (entity == seatOccupant){
                SeatedInteraction(entity);
            }else{
                StartSitting(entity);
            }

            entity.lastInteracted = this;
        }

        private async void StartSitting(Entity entity){
            seatOccupant = entity;
            entity.SetState("Walking");
            seatOccupant.transform.SetParent(this.transform);
            
            CalculateClosestDirection();

            if (speed < 4){
                seatOccupant.walkingTo = true;
                await seatOccupant.WalkTo( sitPosition, (Entity.WalkSpeed)(speed+1) );
                await seatOccupant.TurnTo( transform.rotation * -relativePos );
                seatOccupant.walkingTo = false;
            }

            previousAnchor = seatOccupant.transform.parent;
            
            entity.SetState("Sitting");
            ((SittingState)entity.state).seat = this;

            entity.AnimatorTrigger("Sit");
        }

        private void StopSitting(Entity entity){
            seatOccupant = null;

            entity._transform.SetParent(previousAnchor);

            entity.SetState("Walking");
        }

        protected virtual string seatedInteractionText => ""; 

        protected virtual void SeatedInteraction(Entity entity){;}

        private void CalculateClosestDirection(){
            relativePos = (transform.position - seatOccupant.transform.position).normalized;
            foreach (Vector4 currentDirection in Directions){
                Vector3 direction = new Vector3( currentDirection.x, currentDirection.y, currentDirection.z ).normalized;
                Vector3 directionToOccupant = (seatOccupant.transform.position - transform.position).normalized;

                if ( Vector3.Dot( transform.rotation * direction, directionToOccupant) > Vector3.Dot( transform.rotation * relativePos, directionToOccupant ) ){
                    relativePos = direction;
                    seatOccupant.subState = currentDirection.w;
                }
            }
        }
    }
}