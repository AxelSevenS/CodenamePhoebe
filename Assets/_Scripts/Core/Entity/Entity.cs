using System;

using UnityEngine;

using Animancer;

using SevenGame.Utility;
using System.Reflection;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AnimancerComponent))]
    [DisallowMultipleComponent]
    [SelectionBase]
    public class Entity : MonoBehaviour, IDamageable {
        

        public const float LIGHTWEIGHT_THRESHOLD = 20f;
        public const float HEAVYWEIGHT_THRESHOLD = 50f;
        
    
        [Tooltip("The entity's current Animator.")]
        [SerializeReference][HideInInspector][ReadOnly] private Animator _animator;
        [Tooltip("The entity's current Animator.")]
        [SerializeReference][HideInInspector][ReadOnly] private AnimancerComponent _animancer;

        [Tooltip("The entity's current Rigidbody.")]
        [SerializeReference][HideInInspector][ReadOnly] private Rigidbody _rigidbody;
        
        [Tooltip("The entity's current Physics Script.")]
        [SerializeReference][HideInInspector][ReadOnly] private CustomPhysicsComponent _physicsComponent;
    

        [Header("Entity Data")]
        
        [Tooltip("The entity's current Character, defines their game Model, portraits, display name and base Stats.")]
        [SerializeReference] private Character _character;

        [Tooltip("The current state of the Entity, can be changed using the SetState method.")]
        [SerializeReference] private EntityBehaviour _behaviour;


        [Header("Movement")]

        [Tooltip("The forward direction in absolute space of the Entity.")]
        [SerializeField] private Vector3 _absoluteForward;
        
        [Tooltip("The forward direction in relative space of the Entity.")]
        [SerializeField] private Vector3 _relativeForward;

        [Tooltip("The direction in which the Entity is currently moving.")]
        public Vector3Data moveDirection;

        private Vector3 _totalMovement = Vector3.zero; 



        [Header("Health")]

        [Tooltip("The current health of the Entity.")]
        [SerializeField] private float _health;

        [Tooltip("The health of the entity, before it took damage. Slowly moves toward the true health.")]
        [SerializeField] private float _damagedHealth;

        [SerializeField] private TimeInterval _damagedHealthTimer = new TimeInterval();
        [SerializeField] private float _damagedHealthVelocity = 0f;

        public event Action<float> onHeal;
        public event Action<float, DamageType> onDamage;

        public event Action<float> onHealed;
        public event Action<float, DamageType> onDamaged;
        public event Action onDeath;


    
        [Header("Gravity")]

        [Tooltip("The direction in which the Entity is attracted by Gravity.")]
        public Vector3 gravityDown = Vector3.down;

        [SerializeField] private Vector3 _inertia = Vector3.zero;

        [Tooltip("If the Entity is currently on the ground.")] 
        public BoolData onGround;

        public RaycastHit groundHit;
        
        public event Action<Vector3> onJump;
        public event Action<Vector3> onEvade;
        public event Action onParry;





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
        /// The entity's current Animator.
        /// </summary>
        public AnimancerComponent animancer {
            get {
                _animancer ??= GetComponent<AnimancerComponent>();
                return _animancer;
            }
            private set => _animancer = value;
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
        /// Returns an Armature Bone by name.
        /// </summary>
        /// <remarks>
        /// If the bone is not found, the main Transform of the Entity's Model is returned instead.
        /// </remarks>
        public GameObject this[string key]{
            get { 
                try { 
                    return character.model.costumeData.bones[key]; 
                } catch { 
                    return character.model.mainTransform.gameObject; 
                } 
            }
        }


        /// <summary>
        /// The entity's current Character, defines their Game Model, portraits, display name and base Stats.
        /// </summary>
        /// <remarks>
        /// You can set this value by calling <see cref="SetCharacter"/> and providing CharacterData.
        /// </remarks>
        public Character character {
            get => _character;
        }
        
        /// <summary>
        /// The main transform of the current Costume's model.
        /// </summary>
        public Transform modelTransform { 
            get {
                if (character == null || character.model == null)
                    return null;
                return character.model.mainTransform;
            }
        }

        /// <summary>
        /// The current state (Behaviour) of the Entity.
        /// </summary>
        /// <remarks>
        /// You can change the State using <see cref="SetState"/>.
        /// </remarks>
        public EntityBehaviour behaviour {
            get {
                if ( _behaviour == null )
                    ResetState();

                return _behaviour;
            }
        }

        /// <summary>
        /// The default state Type of the entity.
        /// </summary>
        /// <remarks>
        /// The Entity state is set to this when it is first created, or when using <see cref="ResetState"/>.
        /// </remarks>
        public virtual System.Type defaultState => typeof(GroundedBehaviour); 


        /// <summary>
        /// The forward direction in absolute space of the Entity.
        /// </summary>
        /// <remarks>
        /// Editing this value also changes <see cref="relativeForward"/> to match.
        /// </remarks>
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
        /// <remarks>
        /// Editing this value also changes <see cref="absoluteForward"/> to match.
        /// </remarks>
        public Vector3 relativeForward { 
            get => _relativeForward; 
            set { 
                _relativeForward = value;
                _absoluteForward = transform.rotation * value;
            }
        }

        /// <summary>
        /// Returns true if the Entity is not moving.
        /// </summary>
        public bool isIdle => moveDirection.sqrMagnitude == 0;

        public virtual float jumpMultiplier => 1f;


        /// <summary>
        /// The current health of the Entity.
        /// </summary>
        /// <remarks>
        /// You can change the health using <see cref="Heal"/> and <see cref="Damage"/>, or by using this property instead.
        /// </remarks>
        public float health {
            get {
                return _health;
            }
            set {
                if (value == _health)
                    return;
                
                if ( value < _health )
                    Damage( _health - value );
                else if ( value > _health )
                    Heal( value - _health );
            }
        }


        /// <summary>
        /// The strength of gravity applied to the Entity.
        /// </summary>
        public float gravityMultiplier => weight * behaviour?.gravityMultiplier ?? 1f;

        /// <summary>
        /// The strength of gravity applied to the Entity.
        /// </summary>
        public Vector3 gravityForce => gravityMultiplier * gravityDown;

        public Vector3 inertia {
            get => _inertia;
            set {
                
                _inertia = value;
            }
        }

        public float fallVelocity => Vector3.Dot(inertia, -gravityDown);

        public virtual float weight => character?.data?.weight ?? 1f;
        public WeightCategory weightCategory {
            get {
                switch ( weight ) {
                    case float i when (i <= LIGHTWEIGHT_THRESHOLD):
                        return WeightCategory.Light;
                    case float i when (i > LIGHTWEIGHT_THRESHOLD && i < HEAVYWEIGHT_THRESHOLD):
                        return WeightCategory.Medium;
                    case float i when (i >= HEAVYWEIGHT_THRESHOLD):
                        return WeightCategory.Heavy;
                    default:
                        return WeightCategory.Medium;
                }
            }
        }

        public bool isPlayer => PlayerEntityController.current?.entity == this;

        public bool inWater => physicsComponent.inWater;




        /// <summary>
        /// Create an Entity using the given Parameters
        /// </summary>
        /// <param name="entityType">The type of entity to create</param>
        /// <param name="character">The character of the entity to create</param>
        /// <param name="position">The position of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="costume">The costume of the entity, leave empty to use character's default costume</param>
        public static Entity CreateEntity(System.Type entityType, Vector3 position, Quaternion rotation, CharacterData data, CharacterCostume costume = null) {
            GameObject entityGO = new GameObject("Entity");
            Entity entity = (Entity)entityGO.AddComponent(entityType);
            entityGO.AddComponent<EntityController>();

            entity.SetCharacter(data, costume);

            entity.transform.position = position;
            entity.transform.rotation = rotation;
            return entity;
        }

        /// <summary>
        /// Create an Entity with a PlayerEntityController.
        /// </summary>
        /// <param name="entityType">The type of entity to create</param>
        /// <param name="position">The position of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="character">The character of the entity to create</param>
        /// <param name="costume">The costume of the entity, leave empty to use character's default costume</param>
        public static Entity CreatePlayerEntity(System.Type entityType, Vector3 position, Quaternion rotation, CharacterData data, CharacterCostume costume = null) {
            GameObject entityGO = new GameObject("Entity");
            Entity entity = (Entity)entityGO.AddComponent(entityType);
            entityGO.AddComponent<PlayerEntityController>();

            entity.SetCharacter(data, costume);

            entity.transform.position = position;
            entity.transform.rotation = rotation;

            return entity;
        }


        [ContextMenu("Set As Player Entity")]
        public void SetAsPlayer() {
            GameUtility.SafeDestroy(gameObject.GetComponent<EntityController>());
            gameObject.AddComponent<PlayerEntityController>();
            // Character.SetInstanceWithId("Player", character);
        }


        // /// <summary>
        // /// Set the current state of the Entity
        // /// </summary>
        // /// <param name="newState">The state to set the Entity to</param>
        // public void SetState<TState>() where TState : EntityBehaviour => SetState(typeof(TState));

        // /// <summary>
        // /// Set the current state of the Entity
        // /// </summary>
        // /// <param name="newState">The state to set the Entity to</param>
        // public void SetState(Type stateType) {

        //     if ( stateType == null || !typeof(EntityBehaviour).IsAssignableFrom(stateType) )
        //         throw new System.ArgumentException($"Cannot set state to {stateType?.Name ?? "null"}: stateType must be a subclass of State");

        //     if (stateType == _state?.GetType())
        //         return;

        //     // Vector3 transitionDirection = Vector3.zero;
        //     // float transitionSpeed = 0;
        //     // _state?.GetTransitionData(out transitionDirection, out transitionSpeed);

        //     // GameUtility.SafeDestroy(_state);
        //     // _state = (EntityBehaviour)gameObject.AddComponent(stateType);

        //     // _state?.Transition(transitionDirection, transitionSpeed);

        //     // Get Constructor
        //     ConstructorInfo constructor = stateType.GetConstructor(new Type[] { typeof(Entity), typeof(EntityBehaviour) });
        //     if ( constructor == null )
        //         throw new System.Exception($"Cannot set state to {stateType.Name}: stateType must have a constructor with the signature {stateType.Name}(Entity, EntityBehaviour)");

        //     // Create new state
        //     EntityBehaviour newState = (EntityBehaviour)constructor.Invoke(new object[] { this, _state });
        //     _state?.Dispose();
        //     _state = newState;


        //     Debug.Log($"{name} switched state to {stateType.Name}");
        // }

        // /// <summary>
        // /// Set the current state of the Entity
        // /// </summary>
        // /// <param name="stateName">The name of the state to set the Entity to</param>
        // public void SetState(string stateName) {
        //     Type stateType = System.Type.GetType(stateName, false, false);
        //     SetState(stateType);
        // }

        public void SetState<T>(EntityBehaviourBuilder<T> stateBuilder) where T : EntityBehaviour {
            if ( stateBuilder == null )
                throw new System.ArgumentNullException(nameof(stateBuilder));

            if ( typeof(T) == _behaviour?.GetType() )
                return;

            EntityBehaviour newState = stateBuilder.Build(this, _behaviour);
            _behaviour?.Dispose();
            _behaviour = newState;
        }

        public virtual void ResetState() {
            SetState(new GroundedBehaviourBuilder());
        }


        /// <summary>
        /// Set the Entity's current Character.
        /// </summary>
        /// <param name="characterData">The data of the Character</param>
        /// <param name="characterCostume">The costume to give the character, leave null to use CharacterData's base costume</param>
        public virtual void SetCharacter(CharacterData characterData, CharacterCostume costume = null) {

            _character?.Dispose();
            _character = null;

            _character = characterData?.GetCharacter(this, costume) ?? null;

            _health = _character?.data?.maxHealth ?? 1f;
            _damagedHealth = health;
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
        /// <param name="costume">The new Character Costume</param>
        public void SetCostume(CharacterCostume costume) {
            _character?.SetCostume(costume);
        }


        /// <summary>
        /// Set the current "Fighting Style" of the Entity
        /// (e. g. the different equipped weapons, the current stance, etc.)
        /// </summary>
        /// <param name="newStyle">The style to set the Entity to</param>
        public virtual void SetStyle(int newStyle){;}


        public virtual void HandleInput(PlayerEntityController controller) {
            behaviour?.HandleInput(controller);
        }


        public virtual void Move(Vector3 direction) {
            behaviour?.Move(direction);
        }
        public virtual void Jump() {
            behaviour?.Jump();
        }
        public virtual void Evade(Vector3 direction) {
            behaviour?.Evade(direction);
        }
        public virtual void Parry() {
            behaviour?.Parry();
        }
        public virtual void LightAttack() {
            behaviour?.LightAttack();
        }
        public virtual void HeavyAttack() {
            behaviour?.HeavyAttack();
        }
        public virtual void SetSpeed(MovementSpeed speed) {
            behaviour?.SetSpeed(speed);
        }


        /// <summary>
        /// Deal damage to the Entity.
        /// </summary>
        /// <param name="amount">The amount of damage done to the Entity</param>
        /// <param name="knockback">The direction of Knockback applied through the damage</param>
        public void Damage(float amount, DamageType damageType = DamageType.Physical, Vector3 knockback = default, Entity owner = null) {

            if (owner == this) return;

            if (isPlayer) {
                EntityManager.current?.PlayerHitStop(amount);
            } else if (owner.isPlayer) {
                EntityManager.current?.EnemyHitStop(amount);
            }

            owner?.AwardDamage(amount, damageType);

            const float damagedHealthDuration = 1.25f;

            _health = Mathf.Max(_health - amount, 0f);
            _damagedHealthTimer.SetDuration(damagedHealthDuration);

            if (_health == 0f)
                Kill();

            // TODO: Add actual knockback animations
            rigidbody.AddForce(knockback, ForceMode.Impulse);

            onDamaged?.Invoke(amount, damageType);
        }

        /// <summary>
        /// Heal the Entity.
        /// </summary>
        /// <param name="amount">The amount of health the Entity is healed</param>
        public void Heal(float amount) {

            health = Mathf.Max(health + amount, Mathf.Infinity);

            onHealed?.Invoke(amount);
        }

        public void AwardDamage(float amount, DamageType damageType) {
            onDamage?.Invoke(amount, damageType);
        }

        /// <summary>
        /// Initiate the Entity's death sequence.
        /// </summary>
        public virtual void Kill(){
            onDeath?.Invoke();
        }

        /// <summary>
        /// Move in the given direction.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is not supposed to be used to instruct the Entity to "Walk" to a specific position; 
        ///         use <see cref="Move"/> for that.
        ///     </para>
        ///     <para>
        ///         This is just used to change the Entity's position.
        ///     </para>
        /// </remarks>
        /// <param name="direction">The direction to move in</param>
        /// <param name="canStep">If the Entity can move up or down stair steps, like on a slope.</param>
        public void Displace(Vector3 direction, bool canStep = false, float deltaTime = -1f) {
            if (direction.sqrMagnitude == 0f) return;

            if (deltaTime < 0f) deltaTime = GameUtility.timeDelta;

            if (onGround && gravityMultiplier > 0f) {
                Vector3 rightOfDirection = Vector3.Cross(direction, -gravityDown).normalized;
                Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

                direction = directionConstrainedToGround * direction.magnitude;
            }

            _totalMovement += direction * deltaTime;

        }

        public void DisplaceTo(Vector3 position, bool canStep = false, float deltaTime = -1f) {
            Displace(position - transform.position, canStep, deltaTime);
        }


        /// <summary>
        /// Apply gravity to the Entity.
        /// </summary>
        private void Gravity() {

            if (gravityMultiplier == 0f || weight == 0f || gravityDown == Vector3.zero) return;

            if (onGround)
                inertia = inertia.NullifyInDirection(gravityDown);

            Vector3 verticalInertia = Vector3.Project( inertia, gravityDown );
            Vector3 horizontalInertia = inertia - verticalInertia;

            verticalInertia = Vector3.MoveTowards( verticalInertia, onGround ? Vector3.zero : gravityForce, 35f * GameUtility.timeDelta );
            horizontalInertia = Vector3.MoveTowards( horizontalInertia, Vector3.zero, 0.25f * GameUtility.timeDelta );
            
            inertia = horizontalInertia + verticalInertia;
        
        }

        /// <summary>
        /// Apply all instructed movement to the Entity.
        /// This is where collision is calculated.
        /// </summary>
        private void ExecuteMovement() {

            moveDirection.SetVal( _totalMovement.normalized );

            Vector3 totalDisplacement = _totalMovement + (inertia * GameUtility.timeDelta);

            if (totalDisplacement.sqrMagnitude == 0f) return;

            bool castHit = character.model.ColliderCast(Vector3.zero, totalDisplacement.normalized * (totalDisplacement.magnitude + 0.15f), out RaycastHit walkHit, out Collider castOrigin, 0.15f, Global.GroundMask);

            transform.position += totalDisplacement;
                
            if ( castHit && Physics.ComputePenetration(castOrigin, castOrigin.transform.position, castOrigin.transform.rotation, walkHit.collider, walkHit.collider.transform.position, walkHit.collider.transform.rotation, out Vector3 direction, out float distance) ) {
                transform.position += (direction * distance);
            }

            
            _totalMovement = Vector3.zero;
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
            _animancer = GetComponent<AnimancerComponent>();
            _rigidbody = GetComponent<Rigidbody>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();

            _character?.Dispose();
            _character = null;
        }

        protected virtual void OnEnable(){
            EntityManager.current.entityList.Add( this );
        }
        protected virtual void OnDisable(){
            EntityManager.current.entityList.Remove( this );
        }
        
        protected virtual void OnDestroy(){;}

        protected virtual void Update(){
            onGround.SetVal( character?.model?.ColliderCast(Vector3.zero, gravityDown.normalized * 0.1f, out groundHit, out _, 0.15f, Global.GroundMask) ?? false );
            if ( _damagedHealthTimer.isDone )
                _damagedHealth = Mathf.SmoothDamp(_damagedHealth, _health, ref _damagedHealthVelocity, 0.2f);
            else {
                _damagedHealthVelocity = 0f;
            }

            behaviour?.Update();
            character?.Update();

        }
        

        protected virtual void LateUpdate(){

            behaviour?.LateUpdate();
            character?.LateUpdate();

            Gravity();
            ExecuteMovement();
        }

        protected virtual void FixedUpdate() {

            behaviour?.FixedUpdate();
            character?.FixedUpdate();
        }

        

        public enum WeightCategory {
            Light,
            Medium,
            Heavy
        }

        public enum MovementSpeed {
            Idle,
            Slow,
            Normal,
            Fast
        }

    }
}