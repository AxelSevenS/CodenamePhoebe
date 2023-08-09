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
    [RequireComponent(typeof(Health))]
    [DisallowMultipleComponent]
    [SelectionBase]
    public class Entity : MonoBehaviour, IDamageable, IDamageDealer {
        

        public const float LIGHTWEIGHT_THRESHOLD = 20f;
        public const float HEAVYWEIGHT_THRESHOLD = 50f;
        public const float MAX_FALL_INERTIA = 10f;

        
    
        [Tooltip("The entity's current Animator.")]
        [SerializeField][HideInInspector][ReadOnly] private Animator _animator;
        [Tooltip("The entity's current Animator.")]
        [SerializeField][HideInInspector][ReadOnly] private AnimancerComponent _animancer;

        [Tooltip("The entity's current Rigidbody.")]
        [SerializeField][HideInInspector][ReadOnly] private Rigidbody _rigidbody;
        
        [Tooltip("The entity's current Physics Script.")]
        [SerializeField][HideInInspector][ReadOnly] private CustomPhysicsComponent _physicsComponent;

        [Tooltip("The entity's current Health System.")]
        [SerializeField][HideInInspector][ReadOnly] private Health _health;
    

        [Header("Entity Data")]
        
        [Tooltip("The entity's current Character, defines their game Model, portraits, display name and base Stats.")]
        [SerializeReference] private Character _character;

        [Tooltip("The current state of the Entity, can be changed using the SetState method.")]
        [SerializeField] private EntityBehaviour _behaviour;


        [Header("Movement")]

        [Tooltip("The forward direction in absolute space of the Entity.")]
        [SerializeField] private Vector3 _absoluteForward;
        
        [Tooltip("The forward direction in relative space of the Entity.")]
        [SerializeField] private Vector3 _relativeForward;

        [Tooltip("The direction in which the Entity is currently moving.")]
        public Vector3Data moveDirection;

        private Vector3 _totalMovement = Vector3.zero; 



        public event Action<float> OnHeal;
        public event Action<float, DamageType> OnDamage;

        public event Action<float> OnHealed;
        public event Action<DamageData> OnDamaged;
        public event Action OnDeath;

        public event Action<Character> OnSetCharacter;


    
        [Header("Gravity")]

        [Tooltip("The direction in which the Entity is attracted by Gravity.")]
        public Vector3 gravityDown = Vector3.down;

        [SerializeField] private Vector3 _inertia = Vector3.zero;

        [Tooltip("If the Entity is currently on the ground.")] 
        public BoolData onGround;
        public BoolData groundDetected;

        public RaycastHit groundHit;
        private Transform anchorTransform;

        public event Action<Vector3> OnJump;
        public event Action<Vector3> OnEvade;
        public event Action<DamageData> OnParry;
        

        /// <summary>
        /// The entity's health system.
        /// </summary>
        public Health Health => _health;

        /// <summary>
        /// The entity's Animator system.
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// The entity's Animancer component.
        /// </summary>
        public AnimancerComponent Animancer => _animancer;

        /// <summary>
        /// The entity's Rigidbody.
        /// </summary>
        public Rigidbody Rigidbody => _rigidbody;

        /// <summary>
        /// The entity's Physics Script.
        /// </summary>
        public CustomPhysicsComponent PhysicsComponent => _physicsComponent;

        /// <summary>
        /// Returns an Armature Bone by name.
        /// </summary>
        /// <remarks>
        /// If the bone is not found, the main Transform of the Entity's Model is returned instead.
        /// </remarks>
        public GameObject this[string key]{
            get { 
                try { 
                    return Character.Model.costumeData.bones[key]; 
                } catch { 
                    return Character?.Model?.mainTransform?.gameObject ?? gameObject; 
                } 
            }
        }


        /// <summary>
        /// The entity's current Character, defines their Game Model, portraits, display name and base Stats.
        /// </summary>
        /// <remarks>
        /// You can set this value by calling <see cref="SetCharacter"/> and providing CharacterData.
        /// </remarks>
        public Character Character {
            get => _character;
        }
        
        /// <summary>
        /// The main transform of the current Costume's model.
        /// </summary>
        public Transform ModelTransform { 
            get {
                if (Character == null || Character.Model == null)
                    return null;
                return Character.Model.mainTransform;
            }
        }

        public Vector3 CenterOfMass => Character.Model?.costumeData?.centerOfMass.transform.position ?? transform.position;

        /// <summary>
        /// The current state (Behaviour) of the Entity.
        /// </summary>
        /// <remarks>
        /// You can change the State using <see cref="SetBehaviour"/>.
        /// </remarks>
        public EntityBehaviour Behaviour {
            get {
                if ( _behaviour == null )
                    ResetBehaviour();

                return _behaviour;
            }
        }


        /// <summary>
        /// The forward direction in absolute space of the Entity.
        /// </summary>
        /// <remarks>
        /// Editing this value also changes <see cref="RelativeForward"/> to match.
        /// </remarks>
        public Vector3 AbsoluteForward { 
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
        /// Editing this value also changes <see cref="AbsoluteForward"/> to match.
        /// </remarks>
        public Vector3 RelativeForward { 
            get => _relativeForward; 
            set { 
                _relativeForward = value;
                _absoluteForward = transform.rotation * value;
            }
        }

        /// <summary>
        /// Returns true if the Entity is not moving.
        /// </summary>
        public bool IsIdle => moveDirection.sqrMagnitude == 0;

        /// <summary>
        /// The strength of gravity applied to the Entity.
        /// </summary>
        public float GravityMultiplier => Weight * Behaviour?.GravityMultiplier ?? 1f;

        /// <summary>
        /// The strength of gravity applied to the Entity.
        /// </summary>
        public Vector3 GravityForce => GravityMultiplier * gravityDown;

        public float FallVelocity => Vector3.Dot(Inertia, -gravityDown);

        public virtual float JumpMultiplier => 1f;
        

        public Vector3 Inertia {
            get => _inertia;
            set {
                
                _inertia = value;
            }
        }

        public virtual float Weight => Character?.Data?.weight ?? 1f;
        public EntityWeightCategory WeightCategory {
            get {
                return Weight switch {
                    float i when i <= LIGHTWEIGHT_THRESHOLD => EntityWeightCategory.Light,
                    float i when i > LIGHTWEIGHT_THRESHOLD && i < HEAVYWEIGHT_THRESHOLD => EntityWeightCategory.Medium,
                    float i when i >= HEAVYWEIGHT_THRESHOLD => EntityWeightCategory.Heavy,
                    _ => EntityWeightCategory.Medium,
                };
            }
        }

        public bool IsPlayer => Player.Current?.Entity == this;

        public bool InWater => PhysicsComponent.inWater;




        /// <summary>
        /// Create an Entity using the given Parameters
        /// </summary>
        /// <param name="entityType">The type of entity to create</param>
        /// <param name="character">The character of the entity to create</param>
        /// <param name="position">The position of the entity</param>
        /// <param name="rotation">The rotation of the entity</param>
        /// <param name="costume">The costume of the entity, leave empty to use character's default costume</param>
        public static Entity CreateEntity(System.Type entityType, Vector3 position, Quaternion rotation, CharacterData data, CharacterCostume costume = null) {

            GameObject entityGO = new("Entity");
            entityGO.SetActive(false);

            Entity entity = (Entity)entityGO.AddComponent(entityType);
            entity.Initialize();
            entityGO.AddComponent<EntityController>();
            entity.SetCharacter(data, costume);

            entityGO.SetActive(true);

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

            GameObject entityGO = new("Entity");
            entityGO.SetActive(false);

            Entity entity = (Entity)entityGO.AddComponent(entityType);
            entity.Initialize();
            entityGO.AddComponent<Player>();
            entity.SetCharacter(data, costume);

            entityGO.SetActive(true);

            entity.transform.position = position;
            entity.transform.rotation = rotation;

            return entity;
        }


        [ContextMenu("Set As Player Entity")]
        public void SetAsPlayer() {
            GameUtility.SafeDestroy(gameObject.GetComponent<EntityController>());
            gameObject.AddComponent<Player>();
            // Character.SetInstanceWithId("Player", character);
        }


        public void SetBehaviour<T>(EntityBehaviour.Builder<T> stateBuilder) where T : EntityBehaviour {
            if ( stateBuilder == null )
                throw new System.ArgumentNullException( nameof(stateBuilder) );

            if ( _behaviour is T )
                return;

            try {
                EntityBehaviour newBehaviour = stateBuilder.Build(this, _behaviour);
                GameUtility.SafeDestroy( _behaviour );
                _behaviour = newBehaviour;
            } catch ( System.Exception e ) {
                Debug.LogError( e );
            }
        }

        public virtual void ResetBehaviour() => SetBehaviour( GroundedBehaviour.Builder.Default );


        /// <summary>
        /// Set the Entity's current Character.
        /// </summary>
        /// <param name="characterData">The data of the Character</param>
        /// <param name="characterCostume">The costume to give the character, leave null to use CharacterData's base costume</param>
        public virtual void SetCharacter(CharacterData characterData, CharacterCostume costume = null) {

            _character?.Dispose();
            _character = characterData?.GetCharacter(this, costume) ?? null;

            _health.MaxAmount = _character?.Data?.maxHealth ?? 1f;

            _physicsComponent.size = _character?.Data?.size ?? Vector3.one;

            OnSetCharacter?.Invoke(_character);
        }

        /// <summary>
        /// Set the Entity's current Character Costume.
        /// </summary>
        /// <param name="costumeName">The name of the new Character Costume</param>
        public void SetCostume(string costumeName) => _character?.SetCostume(costumeName);

        /// <summary>
        /// Set the Entity's current Character Costume.
        /// </summary>
        /// <param name="costume">The new Character Costume</param>
        public void SetCostume(CharacterCostume costume) => _character?.SetCostume(costume);


        /// <summary>
        /// Set the current "Fighting Style" of the Entity
        /// (e. g. the different equipped weapons, the current stance, etc.)
        /// </summary>
        /// <param name="newStyle">The style to set the Entity to</param>
        public virtual void SetStyle(int newStyle){;}


        public virtual void HandleInput(Player controller) {
            Behaviour?.HandleInput(controller);
        }
        public virtual void HandleAI(AIController controller) {
            Behaviour?.HandleAI(controller);
        }


        public virtual void Move(Vector3 direction) {
            Behaviour?.Move(direction);
        }
        public virtual void Jump() {
            Behaviour?.Jump();
        }
        public virtual void Evade(Vector3 direction) {
            Behaviour?.Evade(direction);
        }
        public virtual void LightAttack() {
            Behaviour?.LightAttack();
        }
        public virtual void HeavyAttack() {
            Behaviour?.HeavyAttack();
        }
        public virtual void SetSpeed(MovementSpeed speed) {
            Behaviour?.SetSpeed(speed);
        }


        public void PlayerHitStop(DamageData damageData) {
            if (!IsPlayer)
                return;

            if (damageData.damageType == DamageType.Critical) {
                Debug.Log("Hard Hit Stop");
                EntityManager.current.HardHitStop();
            } else {
                Debug.Log("Soft Hit Stop");
                EntityManager.current.SoftHitStop();
            }
        }


        /// <summary>
        /// Deal damage to the Entity.
        /// </summary>
        /// <param name="amount">The amount of damage done to the Entity</param>
        /// <param name="knockback">The direction of Knockback applied through the damage</param>
        public void Damage(IDamageDealer owner, DamageData damageData) {

            if (owner == (IDamageDealer)this) return;

            PlayerHitStop(damageData);

            owner?.AwardDamage(damageData);

            _health.Amount -= damageData.amount;

            // TODO: Add actual knockback animations
            

            OnDamaged?.Invoke(damageData);
        }

        /// <summary>
        /// Heal the Entity.
        /// </summary>
        /// <param name="amount">The amount of health the Entity is healed</param>
        public void Heal(float amount) {

            _health.Amount += amount;

            OnHealed?.Invoke(amount);
        }

        private void OnHealthUpdate(float newHealth) {
            if (newHealth == 0f)
                Kill();
        }

        public void AwardDamage(DamageData damageData) {
            PlayerHitStop(damageData);
            OnDamage?.Invoke(damageData.amount, damageData.damageType);
        }

        public void AwardParry(DamageData damageData) {
            OnParry?.Invoke(damageData);
        }

        public bool IsValidTarget(IDamageable target) {
            return target != (IDamageable)this;
        }

        /// <summary>
        /// Initiate the Entity's death sequence.
        /// </summary>
        public virtual void Kill(){
            OnDeath?.Invoke();
        }

        const float DISPLACEMENT_SKIN_THICKNESS = 0.15f;

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
        public void Displace(Vector3 direction, float deltaTime = -1f) {
            if (direction.sqrMagnitude == 0f) return;

            if (deltaTime < 0f) deltaTime = GameUtility.timeDelta;

            _totalMovement += direction * deltaTime;

        }

        private void DisplacePenetration(Collider entityCollider, Collider worldCollider) {

            // move the character out of the object it's clipping into
            bool penetrationHit = Physics.ComputePenetration(entityCollider, entityCollider.transform.position, entityCollider.transform.rotation, worldCollider, worldCollider.transform.position, worldCollider.transform.rotation, out Vector3 penetrationDirection, out float penetrationDistance);

            if (penetrationHit) {
                transform.position += penetrationDirection * penetrationDistance;
            }
        }

        public bool DisplaceStep(Vector3 direction, float deltaTime = -1f) {
            if (direction.sqrMagnitude == 0f) 
                return false;

            // if (!onGround) {
            //     Displace(direction, deltaTime);
            //     return false;

            if (deltaTime < 0f) 
                deltaTime = GameUtility.timeDelta;

            Vector3 displacement = direction * deltaTime;
            Vector3 checkOffset = gravityDown * Character.Data.stepHeight;
            

            // Check for obstacle
            bool obstacle = Character.Model.ColliderCast(Vector3.zero, displacement.normalized * (displacement.magnitude + DISPLACEMENT_SKIN_THICKNESS * 100f), out RaycastHit walkHit, out Collider castOrigin, DISPLACEMENT_SKIN_THICKNESS, CollisionUtils.EntityCollisionMask); 
            if ( !obstacle ) {
                transform.position += displacement;
                // Debug.Log("No obstacle");
                return false;
            }

            // Check for valid step
            bool validStep = Character.Model.ColliderCast(displacement - checkOffset, checkOffset, out RaycastHit stepHit, out _, 0f, CollisionUtils.EntityCollisionMask);
            // bool validStep = Physics.Raycast(castOrigin.transform.position + displacement - checkOffset, checkOffset, out RaycastHit stepHit, character.data.stepHeight, CollisionUtils.EntityCollisionMask);
            if ( !validStep ) {
                transform.position += displacement;
                DisplacePenetration(castOrigin, walkHit.collider);
                // Debug.Log("No valid step");
                return false;
            }

            // Check if the step is low enough
            Vector3 stepDisplacement = Vector3.Project(stepHit.point - transform.position, gravityDown);
            bool stepAtCorrectHeight = stepDisplacement.sqrMagnitude <= Character.Data.stepHeight * Character.Data.stepHeight;
            if ( !stepAtCorrectHeight ) {
                transform.position += displacement;
                DisplacePenetration(castOrigin, walkHit.collider);
                // Debug.Log("Step at incorrect height");
                return false;
            }

            transform.position += displacement + stepDisplacement;
            DisplacePenetration(castOrigin, walkHit.collider);
            // Debug.Log("Step");
            return true;

        }
        
        public void DisplaceImmediate(Vector3 displacement) {
            if (displacement.sqrMagnitude == 0f) return;


            // stop the character from clipping into obstacles
            bool castHit = Character.Model.ColliderCast(Vector3.zero, displacement.normalized * (displacement.magnitude + DISPLACEMENT_SKIN_THICKNESS * 100f), out RaycastHit walkHit, out Collider castOrigin, DISPLACEMENT_SKIN_THICKNESS, CollisionUtils.EntityCollisionMask);

            if ( !castHit ) {
                transform.position += displacement;
                return;
            }

            transform.position += displacement.normalized * Mathf.Min(displacement.magnitude, walkHit.distance);
            DisplacePenetration(castOrigin, walkHit.collider);

            // Physics.SyncTransforms();
        }

        public void DisplaceTo(Vector3 position, float deltaTime = -1f) {
            Displace(position - transform.position, deltaTime);
        }



        /// <summary>
        /// Apply gravity to the Entity.
        /// </summary>
        private void Gravity() {

            if (GravityMultiplier == 0f || Weight == 0f || gravityDown == Vector3.zero) return;

            Vector3 verticalInertia = Vector3.Project( Inertia, gravityDown );
            Vector3 horizontalInertia = Inertia - verticalInertia;

            verticalInertia = Vector3.MoveTowards( verticalInertia, GravityForce, 35f * GameUtility.timeDelta );
            horizontalInertia = Vector3.MoveTowards( horizontalInertia, Vector3.zero, 0.25f * GameUtility.timeDelta );
            
            Inertia = horizontalInertia + verticalInertia;

            if (onGround)
                Inertia = Inertia.NullifyInDirection(gravityDown);

        }

        /// <summary>
        /// Apply all instructed movement to the Entity.
        /// This is where collision is calculated.
        /// </summary>
        private void ExecuteMovement() {

            moveDirection.SetVal( _totalMovement.normalized );

            Vector3 totalDisplacement = _totalMovement + (Inertia * GameUtility.timeDelta);

            DisplaceImmediate(totalDisplacement);

            // Physics.SyncTransforms();
            
            _totalMovement = Vector3.zero;
        }

        private void EntityCollision() {
                
            if (Character.Model == null) return;

            Collider[] overlaps = Character.Model.ColliderOverlap(0f, CollisionUtils.EntityObjectMask);

            foreach (Collider overlap in overlaps) {
                
                Vector3 displacement = Vector3.ProjectOnPlane((transform.position - overlap.transform.position), groundHit.normal);

                _totalMovement += displacement * GameUtility.timeDelta * 2f;
            }
    
        }

        public void Initialize() {
            transform.rotation = Quaternion.identity;
            AbsoluteForward = transform.forward;

            _animator ??= GetComponent<Animator>();
            _animancer ??= GetComponent<AnimancerComponent>();
            _rigidbody ??= GetComponent<Rigidbody>();
            _physicsComponent ??= GetComponent<CustomPhysicsComponent>();
            _health ??= GetComponent<Health>();

            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Rigidbody.interpolation = RigidbodyInterpolation.None;

            GameUtility.SetLayerRecursively(gameObject, CollisionUtils.EntityObjectLayer);
        }

        protected virtual void Awake() {
            Initialize();

            if (Character == null) {
                Debug.LogError($"Entity {name} has no Character assigned!", this);
                enabled = false;
            }
        }

        private void Reset() => ResetEntity();
    
        [ContextMenu("Reset")]
        private void ResetEntity() => EntityReset();
        protected virtual void EntityReset(){
            Initialize();

            _character?.Dispose();
            _character = null;
        }

        protected virtual void OnEnable(){
            EntityManager.current.entityList.Add( this );
            Health.OnUpdate += OnHealthUpdate;
        }
        protected virtual void OnDisable(){
            EntityManager.current.entityList.Remove( this );
            Health.OnUpdate -= OnHealthUpdate;
        }
        
        protected virtual void OnDestroy(){;}

        protected virtual void Update(){

            bool closeToGround = false;
            bool castHit = Character.Model.ColliderCast(Vector3.zero, gravityDown.normalized * 0.35f, out groundHit, out Collider castOrigin, 0.15f, CollisionUtils.EntityCollisionMask);
            if (castHit) {
                closeToGround = castOrigin.ColliderCast(castOrigin.transform.position, gravityDown.normalized * 0.1f, out _, 0.15f, CollisionUtils.EntityCollisionMask);
            }
            groundDetected.SetVal( castHit );
            onGround.SetVal( closeToGround );


            // behaviour?.Update();
            Character?.Update();

        }
        

        protected virtual void LateUpdate(){

            // behaviour?.LateUpdate();
            Character?.LateUpdate();

            Gravity();
            EntityCollision();
            ExecuteMovement();
        }

        protected virtual void FixedUpdate() {

            // behaviour?.FixedUpdate();
            Character?.FixedUpdate();
        }

        // NOTE : this hasn't been tested yet. This is supposed to handle collisions for Root Motion Animations.
        protected virtual void OnAnimatorMove() {
            Vector3 deltaPosition = _animator.deltaPosition;
            Vector3 movement = deltaPosition / GameUtility.timeDelta;
            if (movement.sqrMagnitude > 0f) {
                _totalMovement += movement;
            }
        }

        protected virtual void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.tag != "Anchor")
                return;

            anchorTransform = collider.transform;
            transform.SetParent(anchorTransform);
        }

        protected virtual void OnTriggerStay(Collider collider) {
            if (!anchorTransform)
                OnTriggerEnter(collider);
        }

        protected virtual void OnTriggerExit(Collider collider) {
            if (collider.transform == anchorTransform) {
                transform.SetParent(null);
                anchorTransform = null;
            }
        }

        public enum EntityWeightCategory {
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