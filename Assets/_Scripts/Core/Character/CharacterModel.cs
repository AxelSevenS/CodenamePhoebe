using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    

    public abstract class CharacterModel : CostumeModel<CharacterCostume> {

        [SerializeField] [ReadOnly] protected Entity _entity;

        public Entity entity => _entity;

        public CharacterModel(Entity entity, CharacterCostume costume) : base(costume) {
            _entity = entity;
        }

        

        /// <summary>
        /// Cast all of the Entity's colliders in the given direction and return the first hit.
        /// </summary>
        /// <param name="position">The position of the start of the Cast, relative to the position of the entity.</param>
        /// <param name="direction">The direction to cast in.</param>
        /// <param name="castHit">The first hit that was found.</param>
        /// <param name="skinThickness">The thickness of the skin of the cast, set to a low number to keep the cast accurate but not zero as to not overlap with the terrain</param>
        /// <param name="layerMask">The layer mask to use for the cast.</param>
        public bool ColliderCast( Vector3 position, Vector3 direction, out RaycastHit castHit, out Collider castOrigin, float skinThickness, LayerMask layerMask ) {

            foreach (Collider collider in costumeData?.colliders){
                bool hasHitWall = collider.ColliderCast( collider.transform.position + position, direction, out RaycastHit tempHit, skinThickness, layerMask );
                if ( !hasHitWall ) continue;

                castHit = tempHit;
                castOrigin = collider;
                return true;
            }

            castHit = new RaycastHit();
            castOrigin = null;
            return false;
        }

        /// <summary>
        /// Check if there are any colliders overlap with the entity's colliders.
        /// </summary>
        /// <param name="skinThickness">The thickness of the skin of the overlap, set to a low number to keep the overlap accurate but not zero as to not overlap with the terrain</param>
        /// <param name="layerMask">The layer mask to use for the overlap.</param>
        public Collider[] ColliderOverlap( float skinThickness, LayerMask layerMask ) {
            foreach (Collider collider in costumeData.colliders){
                Collider[] hits = collider.ColliderOverlap( collider.transform.position, skinThickness, layerMask );
                if ( hits.Length > 0 ) return hits;
            }
            return new Collider[0];
        }


        /// <summary>
        /// Rotate the Entity's model towards the given direction, with the given up direction.
        /// </summary>
        /// <param name="newForward">The direction to rotate towards</param>
        /// <param name="newUp">The up direction to use</param>
        public void RotateTowards(Vector3 newForward, Vector3 newUp, float speed = 12f) => RotateTowards( Quaternion.LookRotation(newForward, newUp), speed );

        /// <summary>
        /// Rotate the Entity's model using the given rotation.
        /// </summary>
        /// <param name="newRotation">The rotation to use</param>
        public abstract void RotateTowards(Quaternion newRotation, float speed = 12f);



    }

}
