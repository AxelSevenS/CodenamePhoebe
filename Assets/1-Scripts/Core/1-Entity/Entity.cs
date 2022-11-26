using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;
using System.Collections.Generic;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    [SelectionBase]
    public partial class Entity : MonoBehaviour, IDamageable {

        
        private const int MOVE_COLLISION_STEP_COUNT = 1;
        
        

    
        [Tooltip("The entity's current Character, defines their game Model, portraits, display name and base Stats.")]
        [SerializeReference][ReadOnly] private Character _character;
    
        [Tooltip("The entity's current Animator.")]
        [SerializeReference][HideInInspector][ReadOnly] private Animator _animator;

        [Tooltip("The entity's current Rigidbody.")]
        [SerializeReference][HideInInspector][ReadOnly] private Rigidbody _rigidbody;
        
        [Tooltip("The entity's current Physics Script.")]
        [SerializeReference][HideInInspector][ReadOnly] private CustomPhysicsComponent _physicsComponent;
    

        [Header("Entity Data")]

        [Tooltip("The current state of the Entity, can be changed using the SetState method.")]
        [SerializeReference][ReadOnly] private State _state;

        // [Tooltip("The current rotation of the Entity.")]
        // public QuaternionData rotation;

        [Tooltip("If the Entity is currently on the ground.")] 
        public BoolData onGround;

        [Tooltip("The current health of the Entity.")]
        private float _health;


        [Header("Movement")]

        [Tooltip("The forward direction in absolute space of the Entity.")]
        [SerializeField] private Vector3 _absoluteForward;
        
        [Tooltip("The forward direction in relative space of the Entity.")]
        [SerializeField] private Vector3 _relativeForward;

        [Tooltip("The direction in which the Entity is currently moving.")]
        public Vector3Data moveDirection;
    
        [Tooltip("The direction in which the Entity is attracted by Gravity.")]
        public Vector3 gravityDown = Vector3.down;



        [Header("Misc")]

        private Vector3 _totalMovement = Vector3.zero; 

        public RaycastHit groundHit;

        [HideInInspector] public float subState;



        public event Action<float> onHeal;
        public event Action<float> onDamage;
        public event Action onDeath;

        public event Action<Vector3> onJump;
        public event Action<Vector3> onEvade;
        public event Action onParry;


        public Transform modelTransform => character?.model?.transform;


        /// <summary>
        /// The entity's current Character, defines their game Model, portraits, display name and base Stats.
        /// </summary>
        public Character character {
            get => _character;
            private set => _character = value;
        }

        /// <summary>
        /// The entity's current Animator.
        /// </summary>
        public Animator animator {
            get {
                _animator ??= GetComponent<Animator>();
                return _animator;
            }
            private set => _animator = value;
        }

        /// <summary>
        /// The entity's current Rigidbody.
        /// </summary>
        public new Rigidbody rigidbody {
            get
            {
                _rigidbody ??= GetComponent<Rigidbody>();
                return _rigidbody;
            }
        }

        /// <summary>
        /// The entity's current Physics Script.
        /// </summary>
        public CustomPhysicsComponent physicsComponent {
            get {
                _physicsComponent ??= GetComponent<CustomPhysicsComponent>();
                return _physicsComponent;
            }
        }

        /// <summary>
        /// The current state of the Entity, can be changed using the SetState method.
        /// </summary>
        public State state {
            get {
                if ( _state == null )
                    SetState( defaultState );

                return _state;
            }
            set {
                SetState( value );
            }
        }

        /// <summary>
        /// The default state of the entity.
        /// </summary>
        public virtual State defaultState => new HumanoidGroundedState(); 

        /// <summary>
        /// The current health of the Entity.
        /// </summary>
        public float health {
            get {
                return _health;
            }
            set {
                if ( value <= _health )
                    Damage( _health - value );
                else if ( value > _health )
                    Heal( value - _health );
            }
        }

        /// <summary>
        /// The forward direction in absolute space of the Entity.
        /// </summary>
        public Vector3 absoluteForward { 
            get => _absoluteForward; 
            set { 
                _absoluteForward = value; 
                _relativeForward = Quaternion.Inverse(transform.rotation) * value; 
            } 
        }
        
        /// <summary>
        /// The forward direction in relative space of the Entity.
        /// </summary>
        public Vector3 relativeForward { 
            get => _relativeForward; 
            set { 
                _relativeForward = value;
                _absoluteForward = transform.rotation * value;
            }
        }

        /// <summary>
        /// The force of gravity applied to the Entity.
        /// </summary>
        public float gravityMultiplier => state.gravityMultiplier;

        public bool isIdle => moveDirection.sqrMagnitude == 0;

        public float fallVelocity => Vector3.Dot(rigidbody.velocity, -gravityDown);
        public bool inWater => physicsComponent.inWater;
        public bool isOnWaterSurface => inWater && Mathf.Abs(physicsComponent.waterHeight - transform.position.y) < 0.5f;


        public GameObject this[string key]{
            get { 
                try { 
                    return character.costumeData.bones[key]; 
                } catch { 
                    return character.model; 
                } 
            }
        }

        public virtual float weight => character.weight;
        public virtual float jumpMultiplier => 1f;




        /// <summary>
        /// Create an Entity using the given Parameters
        /// </summary>
        /// <param name="entityType">The type of entity to create</param>
        /// <param name="character">The character of the entity to create</param>
        /// <param name="position">The position of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="costume">The costume of the entity, leave empty to use character's default costume</param>
        public static Entity CreateEntity(System.Type entityType, Character character, Vector3 position, Quaternion rotation, CharacterCostume costume = null){
            GameObject entityGO = new GameObject("Entity");
            Entity entity = (Entity)entityGO.AddComponent(entityType);

            entity.SetCharacter(character, costume);

            entity.transform.position = position;
            entity.transform.rotation = rotation;
            return entity;
        }


        /// <summary>
        /// Create an Entity with a PlayerEntityController.
        /// </summary>
        /// <param name="entityType">The type of entity to create</param>
        /// <param name="character">The character of the entity to create</param>
        /// <param name="position">The position of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="costume">The costume of the entity, leave empty to use character's default costume</param>
        public static Entity CreatePlayerEntity(System.Type entityType, Character character, Vector3 position, Quaternion rotation, CharacterCostume costume = null){
            GameObject entityGO = new GameObject("Entity");
            entityGO.AddComponent<PlayerEntityController>();
            Entity entity = (Entity)entityGO.AddComponent(entityType);
            
            entity.character = character;
            entity.character.Initialize(entity, costume);

            entity.transform.position = position;
            entity.transform.rotation = rotation;
            return entity;
        }


        [ContextMenu("Set As Player Entity")]
        public void SetAsPlayer() {
            gameObject.AddComponent<PlayerEntityController>(); 
        }

        /// <summary>
        /// Set the current state of the Entity
        /// </summary>
        /// <param name="newState">The state to set the Entity to</param>
        public void SetState(State newState) {

            try {
                newState.OnEnter(this);
            } catch (Exception e) {
                Debug.LogError($"Error while entering state {newState.GetType().Name} : {e.Message}");
                return;
            }

            _state?.OnExit();
            _state = newState;

            //  TODO : add a way to set the corresponding animator for the current Entity State.
            // animator.SetInteger( "State", (int)_state.stateType );
            animator.SetTrigger( "SetState" );

            Debug.Log($"{name} switched state to {_state.name}");
        }

        public void SetState() => SetState( defaultState );

        /// <summary>
        /// Set the Entity's current Character.
        /// </summary>
        /// <param name="character">The new Character</param>
        public void SetCharacter(Character character, CharacterCostume costume = null) {
            
            try {
                character?.Initialize(this, costume);
            } catch (Exception e) {
                Debug.LogError($"Error while Setting Character {character.name} : {e.Message}");
                return;
            }

            _character?.Dispose();
            _character = character;
        }

        /// <summary>
        /// Set the Entity's current Character Costume.
        /// </summary>
        /// <param name="costumeName">The name of the new Character Costume</param>
        public void SetCostume(string costumeName) {
            _character?.SetCostume(costumeName);
        }

        /// <summary>
        /// Set the Entity's current Character Costume.
        /// </summary>
        /// <param name="costumeName">The new Character Costume</param>
        public void SetCostume(CharacterCostume costume) {
            _character?.SetCostume(costume);
        }

        /// <summary>
        /// Set the current "Fighting Style" of the Entity
        /// (e. g. the different equipped weapons, the current stance, etc.)
        /// </summary>
        /// <param name="newStyle">The style to set the Entity to</param>
        public virtual void SetStyle(int newStyle){;}

        /// <summary>
        /// Load the Character Model.
        /// </summary>
        protected internal virtual void LoadModel() {
            character?.LoadModel();
        }

        /// <summary>
        /// Unload the Character Model.
        /// </summary>
        protected internal virtual void UnloadModel() {
            character?.UnloadModel();
        }

        public void RotateModelTowards(Quaternion nowRotation) {
            
            if (modelTransform == null) return;

            Quaternion inverseTransformRotation = Quaternion.Inverse(transform.rotation);

            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, nowRotation, 12f * GameUtility.timeDelta);
        }

        public void RotateModelTowards(Vector3 newForward, Vector3 newUp) => RotateModelTowards( Quaternion.LookRotation(newForward, newUp) );


        protected virtual void EntityAnimation() {
            animator.SetBool("OnGround", onGround);
            // animator.SetBool("Falling", fallVelocity <= -20f);
            animator.SetBool("Idle", isIdle );
            // animator.SetInteger("State", state.id);
            // animator.SetFloat("SubState", subState);
            // animator.SetFloat("WalkSpeed", (float)walkSpeed);
            // animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, transform.forward));
            // animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-transform.up, transform.forward)));
        }
        

        /// <summary>
        /// Pickup a grabbable item.
        /// </summary>
        /// <param name="grabbable">The item to pick up</param>
        public virtual void Grab(Grabbable grabbable){;}


        /// <summary>
        /// Throw a grabbable item.
        /// </summary>
        /// <param name="grabbable">The item to throw</param>
        public virtual void Throw(Grabbable grabbable){;}


        /// <summary>
        /// Initiate the Entity's death sequence.
        /// </summary>
        public virtual void Death(){
            onDeath?.Invoke();
        }

        /// <summary>
        /// Deal damage to the Entity.
        /// </summary>
        /// <param name="amount">The amount of damage done to the Entity</param>
        /// <param name="knockback">The direction of Knockback applied through the damage</param>
        public void Damage(float amount, Vector3 knockback = default) {

            health = Mathf.Max(health - amount, 0f);

            if (health == 0f)
                Death();

            rigidbody.AddForce(knockback, ForceMode.Impulse);

            onDamage?.Invoke(amount);
        }

        /// <summary>
        /// Heal the Entity.
        /// </summary>
        /// <param name="amount">The amount of health the Entity is healed</param>
        public void Heal(float amount) {

            health = Mathf.Max(health + amount, Mathf.Infinity);

            onHeal?.Invoke(amount);
        }


        /// <summary>
        /// Move in the given direction.
        /// Not supposed to be used to instruct the Entity to "Walk" to a specific position; 
        /// This is just used to change the Entity's position.
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="canStep">If the Entity can move up or down stair steps, like on a slope.</param>
        public void Displace(Vector3 direction, bool canStep = false) {
            if (direction.sqrMagnitude == 0f) return;

            if (onGround && gravityMultiplier > 0f) {
                Vector3 rightOfDirection = Vector3.Cross(direction, -gravityDown).normalized;
                Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

                direction = directionConstrainedToGround * direction.magnitude;
            }

            _totalMovement += direction * GameUtility.timeDelta;

        }


        /// <summary>
        /// Apply gravity to the Entity.
        /// </summary>
        private void Gravity() {

            if (gravityMultiplier == 0f || weight == 0f || gravityDown == Vector3.zero) return;

            Vector3 gravityForce = weight * gravityMultiplier * gravityDown * GameUtility.timeDelta;

            rigidbody.velocity += gravityForce;

            // if ( onGround ) return;

            const float regularFallMultiplier = 3.25f;
            const float fallingMultiplier = 2f;
            
            float floatingMultiplier = regularFallMultiplier * gravityMultiplier;
            float multiplier = floatingMultiplier * (fallVelocity >= 0 ? 1f : fallingMultiplier);

            rigidbody.velocity += multiplier * gravityForce;
        
        }

        /// <summary>
        /// Apply all instructed movement to the Entity.
        /// This is where the collision is calculated.
        /// </summary>
        private void ExecuteMovement() {

            moveDirection.SetVal( _totalMovement.normalized ) ;

            if (_totalMovement.sqrMagnitude == 0f) return;

            // rigidbody.MovePosition(rigidbody.position + _totalMovement);

            Vector3 step = _totalMovement / MOVE_COLLISION_STEP_COUNT;
            for (int i = 0; i < MOVE_COLLISION_STEP_COUNT; i++) {

                bool walkCollision = ColliderCast(Vector3.zero, step, out RaycastHit walkHit, 0.15f, Global.GroundMask);

                if (!walkCollision)
                    i = MOVE_COLLISION_STEP_COUNT;

                Vector3 executedMovement = walkCollision ? step.normalized * walkHit.distance : step;
                rigidbody.MovePosition(rigidbody.position + executedMovement);

            }

            // Check for penetration and adjust accordingly
            foreach ( Collider entityCollider in character.costumeData.hurtColliders ) {
                foreach ( Collider worldCollider in entityCollider.ColliderOverlap(Vector3.zero, 0f, Global.GroundMask) ) {
                    if ( Physics.ComputePenetration(entityCollider, entityCollider.transform.position, entityCollider.transform.rotation, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out Vector3 direction, out float distance) ) {
                        rigidbody.MovePosition(rigidbody.position + (direction * distance));
                    }
                }
            }
            
            _totalMovement = Vector3.zero;
        }



        /// <summary>
        /// Cast all of the Entity's colliders in the given direction and return the first hit.
        /// </summary>
        /// <param name="position">The position of the start of the Cast, relative to the position of the entity.</param>
        /// <param name="direction">The direction to cast in.</param>
        /// <param name="castHit">The first hit that was found.</param>
        /// <param name="skinThickness">The thickness of the skin of the cast, set to a low number to keep the cast accurate but not zero as to not overlap with the terrain</param>
        /// <param name="layerMask">The layer mask to use for the cast.</param>
        public bool ColliderCast( Vector3 position, Vector3 direction, out RaycastHit castHit, float skinThickness, LayerMask layerMask ) {
            // Debug.Log(costumeData.hurtColliders["main"]);

            foreach (Collider collider in character?.costumeData.hurtColliders){
                bool hasHitWall = collider.ColliderCast( collider.transform.position + position, direction, out RaycastHit tempHit, skinThickness, layerMask );
                if ( !hasHitWall ) continue;

                castHit = tempHit;
                return true;
            }

            castHit = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Check if there are any colliders overlap with the entity's colliders.
        /// </summary>
        /// <param name="skinThickness">The thickness of the skin of the overlap, set to a low number to keep the overlap accurate but not zero as to not overlap with the terrain</param>
        /// <param name="layerMask">The layer mask to use for the overlap.</param>
        public Collider[] ColliderOverlap( float skinThickness, LayerMask layerMask ) {
            foreach (Collider collider in character?.costumeData.hurtColliders){
                Collider[] hits = collider.ColliderOverlap( collider.transform.position, skinThickness, layerMask );
                if ( hits.Length > 0 ) return hits;
            }
            return new Collider[0];
        }




        // [ContextMenu("Initialize")]
        protected virtual void Awake(){

            // Ensure only One Entity is on a single GameObject
            // Entity[] entities = GetComponents<Entity>();
            // for (int i = 0; i < entities.Length; i++) {
            //     if (entities[i] != this) {
            //         GameUtility.SafeDestroy(entities[i]);
            //     }
            // }
            
        }

        protected virtual void Start(){
            transform.rotation = Quaternion.identity;
            absoluteForward = transform.forward;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            if (character == null) {
                Debug.LogError($"Entity {name} has no Character assigned!", this);
                enabled = false;
            }
        }

        private void Reset() => ResetEntity();
    
        [ContextMenu("Reset")]
        private void ResetEntity() => EntityReset();
        protected virtual void EntityReset(){
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();
            SetCharacter(null);
        }

        protected virtual void OnEnable(){
            EntityManager.current.entityList.Add( this );
        }
        protected virtual void OnDisable(){
            EntityManager.current.entityList.Remove( this );
        }
        
        protected virtual void OnDestroy(){;}

        protected virtual void Update(){
            onGround.SetVal( ColliderCast(Vector3.zero, gravityDown.normalized * 0.2f, out groundHit, 0.15f, Global.GroundMask) );

            state?.StateUpdate();

            if (animator.runtimeAnimatorController != null) {
                EntityAnimation();
            }
        }

        protected virtual void LateUpdate(){
            // state?.StateLateUpdate();
        }

        protected virtual void FixedUpdate(){
            ExecuteMovement();
            
            state?.StateFixedUpdate();

            Gravity();
        }

    }
}