using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [DisallowMultipleComponent]
    public class EntityController : MonoBehaviour {

        [SerializeField] private Entity _entity;




        public Entity entity {
            get {
                if ( !_entity ) {
                    Reset();
                }
                return _entity;
            }
        }

        protected virtual void Reset() {
            _entity = GetComponent<Entity>();
        }
    }
}
