using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [DisallowMultipleComponent]
    public class EntityController : MonoBehaviour {

        [SerializeField] private Entity _entity;




        public Entity Entity => _entity;

        protected virtual void Reset() {
            _entity = GetComponent<Entity>();
            if (_entity == null) {
                Debug.LogError("EntityController requires an Entity component to function.");
                GameUtility.SafeDestroy(this);
            }
        }
    }
}
