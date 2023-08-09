using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    // [System.Serializable]
    public abstract class CompositeBehaviour : MonoBehaviour {


        [SerializeField] protected Entity _entity;
        [SerializeField] protected EntityBehaviour _behaviour;

        public Entity Entity => _entity;
        public EntityBehaviour Behaviour => _behaviour;

        protected internal abstract void HandleInput(Player contoller);


        public abstract class Builder<TBehaviour, TBaseBehaviour> where TBehaviour : TBaseBehaviour where TBaseBehaviour : CompositeBehaviour {
            
            public virtual TBehaviour Build(Entity entity, EntityBehaviour parentBehaviour, TBaseBehaviour previousBehaviour = null) {
                using Context process = new(entity, parentBehaviour, previousBehaviour);
                ConfigureBehaviour(process);
                return process.Behaviour;
            }

            protected virtual void ConfigureBehaviour(Context context) {;}


            protected readonly struct Context : IDisposable {

                private readonly bool wasEnabled;
                private readonly Entity _entity;
                private readonly TBehaviour _behaviour;
                private readonly EntityBehaviour _parentBehaviour;
                private readonly TBaseBehaviour _previousBehaviour;

                public Entity Entity => _entity;
                public TBehaviour Behaviour => _behaviour;
                public EntityBehaviour ParentBehaviour => _parentBehaviour;
                public TBaseBehaviour PreviousBehaviour => _previousBehaviour;


                public Context(Entity entity, EntityBehaviour parentBehaviour, TBaseBehaviour previousBehaviour = null){
                    _entity = entity;
                    _parentBehaviour = parentBehaviour;
                    _previousBehaviour = previousBehaviour;

                    wasEnabled = entity.gameObject.activeSelf;
                    entity.gameObject.SetActive(false);
                    _behaviour = entity.gameObject.AddComponent<TBehaviour>();
                    _behaviour._entity = entity;
                }

                public void Dispose() {
                    _entity.gameObject.SetActive(wasEnabled);
                }
            }
        }
    }
}
