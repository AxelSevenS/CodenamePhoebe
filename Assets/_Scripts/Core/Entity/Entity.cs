using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    public class Entity : MonoBehaviour, IDamageable {


        #region Constants
        
            private const int moveCollisionStep = 1;
        

        #endregion

        #region Editor Fields

            [SerializeField] private AssetReferenceT<Character> characterToLoad;
        
            [Tooltip("The entity's current Character, defines their game Model, portraits, display name and base Stats.")]
            [SerializeReference]
            private Character.Instance _character;
        
            [Tooltip("The entity's current Animator.")]
            [SerializeReference]
            private Animator _animator;



            [Space(10)]
            [Header("Entity Data")]

            [Tooltip("The current state of the Entity, can be changed using the SetState method.")]
            [SerializeReference] 
            private State _state;


            [Tooltip("The current health of the Entity.")]
            [SerializeField] 
            private float _health;

            [Tooltip("If the Entity is currently on the ground.")]
            [SerializeField] 
            private BoolData _onGround;

            [Tooltip("The current rotation of the Entity.")]
            [SerializeField] 
            private QuaternionData _rotation;



            [Space(10)]
            [Header("Movement")]

            [Tooltip("The forward direction in absolute space of the Entity.")]
            [SerializeField] 
            private Vector3 _absoluteForward;
            

            [Tooltip("The forward direction in relative space of the Entity.")]
            [SerializeField] 
            private Vector3 _relativeForward;


            [Tooltip("The direction in which the Entity is currently moving.")]
            [SerializeField] 
            private Vector3Data _moveDirection;


            [Tooltip("The current movement speed of the Entity.")]
            [SerializeField] 
            private float _moveSpeed;

        
            [Tooltip("The direction in which the Entity is attracted by Gravity.")]
            [SerializeField] 
            private Vector3 _gravityDown = Vector3.down;



            [Space(10)]
            [Header("Jumping")]

            [Tooltip("The current allowed number of Jumps the Entity can make before having to \"Refresh\".")]
            [SerializeField] 
            private int _jumpCount = 1;

            [Tooltip("The amount of time needed to wait for the Entity to be able to jump again.")]
            [SerializeField] 
            private TimeUntil _jumpCooldownTimer;



            [Space(10)]
            [Header("Misc")]

            [Tooltip("The last object the Entity interacted with.")]
            [SerializeReference] 
            private IInteractable lastInteracted;

        #endregion

        #region Fields

            private Rigidbody _rigidbody;
            private CustomPhysicsComponent _physicsComponent;
            private EntityController _controller;

            private Vector3 _totalMovement;

            public RaycastHit groundHit;


        #endregion


        #region Properties


            public Character.Instance character {
                get {
                    // if ( _character == null ){
                    //     LoadCharacter();
                    // }
                    return _character;
                }
                private set => _character = value;
            }
            public Animator animator {
                get {
                    if ( _animator == null )
                        _animator = GetComponent<Animator>();
                    return _animator;
                }
                private set => _animator = value;
            }

            public State state {
                get {
                    if ( _state == null )
                        SetState( defaultState );
                    return _state;
                }
                private set => _state = value;
            }


            /// <summary>
            /// The default state of the entity.
            /// </summary>
            public virtual State defaultState => new WalkingState(); 


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

            public ref BoolData onGround {
                get => ref _onGround;
            }

            public ref QuaternionData rotation {
                get => ref _rotation;
            }


            public Vector3 absoluteForward { 
                get => _absoluteForward; 
                set { 
                    _absoluteForward = value; 
                    _relativeForward = Quaternion.Inverse(rotation) * value; 
                } 
            }
            

            public Vector3 relativeForward { 
                get => _relativeForward; 
                set { 
                    _relativeForward = value;
                    _absoluteForward = rotation * value;
                }
            }


            public ref Vector3Data moveDirection { 
                get => ref _moveDirection;
            }

            public float moveSpeed { 
                get => _moveSpeed; 
                set => _moveSpeed = Mathf.Max( value, 0f );
            }


            public Vector3 gravityDown { 
                get => _gravityDown; 
                set => _gravityDown = value.normalized;
            }

            public int jumpCount { 
                get => _jumpCount; 
                set => _jumpCount = Math.Max(value, 0);
            }

            public ref TimeUntil jumpCooldownTimer { 
                get => ref _jumpCooldownTimer;
            }


            // public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);

            public bool isIdle => moveDirection.sqrMagnitude == 0;

            public float fallVelocity => Vector3.Dot(rigidbody.velocity, -gravityDown);
            public bool inWater => physicsComponent.inWater;
            public bool isOnWaterSurface => inWater && transform.position.y > (physicsComponent.waterHeight - character.size.y);


            public Rigidbody rigidbody {
                get {
                    if ( _rigidbody == null )
                        _rigidbody = GetComponent<Rigidbody>();
                    return _rigidbody;
                }
            }


            public CustomPhysicsComponent physicsComponent {
                get {
                    if ( _physicsComponent == null )
                        _physicsComponent = GetComponent<CustomPhysicsComponent>();
                    return _physicsComponent;
                }
            }


            public virtual EntityController controller { 
                get {
                    if ( _controller == null ) {
                        if ( !TryGetComponent<EntityController>(out _controller) )
                            _controller = gameObject.AddComponent<EntityController>();
                    }

                    _controller.entity = this;
                    return _controller;
                
                } 
                protected set => _controller = value; 
            }


            public GameObject this[string key]{
                get { try{ return character.costumeData.bones[key]; } catch{ return character.model; } }
            }


        #endregion

        #region Events

            public event Action<Vector3> onJump;
            public event Action<float> onHeal;
            public event Action<float> onDamage;
            public event Action onDeath;
            
        #endregion

        [HideInInspector] public float subState;



        public virtual bool CanTurn() => true;
        public virtual bool CanWaterHover() => false;
        public virtual bool CanSink() => false;
        public virtual float GravityMultiplier() => character.weight;
        public virtual float JumpMultiplier() => 1f;

        // public Vector3 ConstrainMovementToGround( Vector3 ) {

        // }
        



        protected virtual void EntityAnimation() {
            animator.SetBool("OnGround", onGround);
            // animator.SetBool("Falling", fallVelocity <= -20f);
            animator.SetBool("Idle", moveDirection.sqrMagnitude == 0f );
            // animator.SetInteger("State", state.id);
            // animator.SetFloat("SubState", subState);
            // animator.SetFloat("WalkSpeed", (float)walkSpeed);
            // animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, transform.forward));
            // animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-transform.up, transform.forward)));
        }


        [ContextMenu("Make Player Entity")]
        public void SetAsPlayer() {
            controller = gameObject.AddComponent<PlayerEntityController>();
        }

        /// <summary>
        /// Set the current state of the Entity
        /// </summary>
        /// <param name="newState">The state to set the Entity to</param>
        public void SetState(State newState) {
            _state?.OnExit();

            newState.OnEnter(this);
            _state = newState;

            animator.SetInteger( "State", (int)_state.stateType );
            animator.SetTrigger( "SetState" );

            Debug.Log($"{name} switched state to {_state.name}");
        }

        /// <summary>
        /// Set the current "Fighting Style" of the Entity
        /// (e. g. the different equipped weapons, the current stance, etc.)
        /// </summary>
        /// <param name="newStyle">The style to set the Entity to</param>
        public virtual void SetStyle(int newStyle){;}


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


        public virtual void Kill(){
            // GameUtility.SafeDestroy(gameObject);
            // animator?.SetTrigger("Death");

            onDeath?.Invoke();
        }
        public void Damage(float amount, Vector3 knockback = default) {

            health = Mathf.Max(health - amount, 0f);

            if (health == 0f)
                Kill();

            rigidbody.AddForce(knockback, ForceMode.Impulse);

            onDamage?.Invoke(amount);
        }

        public void Heal(float amount) {

            health = Mathf.Max(health + amount, Mathf.Infinity);

            onHeal?.Invoke(amount);
        }

        public void SetRotation(Vector3 newUp) {
            rotation.SetVal( Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation );
        }


        /// <summary>
        /// Rotate the Entity towards a given direction in relative space.
        /// </summary>
        /// <param name="newDirection">The direction to rotate towards</param>
        /// <param name="newUp">The direction that is used as the Entity's up direction</param>
        public void RotateTowardsRelative(Vector3 newDirection, Vector3 newUp) {

            Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg, Vector3.up) ;
            transform.rotation = Quaternion.Slerp(transform.rotation, apparentRotation * turnDirection, 12f*GameUtility.timeDelta);
        }


        /// <summary>
        /// Rotate the Entity towards a given direction in absolute space.
        /// </summary>
        /// <param name="newDirection">The direction to rotate towards</param>
        /// <param name="newUp">The direction that is used as the Entity's up direction</param>
        public void RotateTowardsAbsolute(Vector3 newDirection, Vector3 newUp) {

            Vector3 inverse = Quaternion.Inverse(rotation) * newDirection;
            RotateTowardsRelative(inverse, newUp);

            // Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            // Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(inverse.x, inverse.z) * Mathf.Rad2Deg, Vector3.up) ;
            // transform.rotation = Quaternion.Slerp(transform.rotation, apparentRotation * turnDirection, 12f*GameUtility.timeDelta);
        }

        [ContextMenu("Load Character")]
        public void LoadCharacter() {
            Character loadedCharacter;
            if ( characterToLoad == null ){
                loadedCharacter = Character.GetDefault();
            } else {
                if ( characterToLoad.Asset == null ) {
                    AsyncOperationHandle<Character> opHandle = characterToLoad.LoadAssetAsync();
                    loadedCharacter = opHandle.WaitForCompletion();
                } else {
                    loadedCharacter = characterToLoad.Asset as Character;
                }
            }
            _character = new Character.Instance( this, loadedCharacter );
        }

        public virtual void LoadModel() {
            character?.LoadModel();
        }

        public virtual void UnloadModel() {
            character?.UnloadModel();
        }

        public void SetCostume(string costumeName) {
            character.SetCostume(costumeName);
        }
        public void SetCostume(CharacterCostume costume) {
            character.SetCostume(costume);
        }


        /// <summary>
        /// Apply a gravity force to the Entity.
        /// </summary>
        /// <param name="force">The force to apply</param>
        /// <param name="direction">The direction to apply the force in</param>
        public void Gravity(float force, Vector3 direction) {
            JumpGravity(force, direction, false);
        }


        /// <summary>
        /// Apply a gravity force to the Entity. the force is lowered if the slowfall flag is set.
        /// </summary>
        /// <param name="force">The force to apply</param>
        /// <param name="direction">The direction to apply the force in</param>
        /// <param name="slowfall">Whether the force should be reduced by a factor of 0.75</param>
        public void JumpGravity(float force, Vector3 direction, bool slowFall) {        
            rigidbody.AddForce(force * GameUtility.timeDelta * direction);

            // Inertia that's only active when falling
            if ( onGround ) return;

            const float slowFallMultiplier = 2.25f;
            const float regularFallMultiplier = 3f;
            const float fallingMultiplier = 2f;
            
            float floatingMultiplier = slowFall ? slowFallMultiplier : regularFallMultiplier;
            float multiplier = floatingMultiplier * (fallVelocity >= 0 ? 1f : fallingMultiplier);

            rigidbody.velocity += multiplier * force * GameUtility.timeDelta * direction.normalized;
        
        }


        /// <summary>
        /// Make the Entity Jump in the given direction.
        /// </summary>
        public void Jump(Vector3 jumpDirection) {

            Debug.Log(character.jumpHeight * JumpMultiplier());

            Vector3 newVelocity = rigidbody.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += character.jumpHeight * JumpMultiplier() * jumpDirection;
            rigidbody.velocity = newVelocity;

            animator.SetTrigger("Jump");

            jumpCooldownTimer.SetDuration( 0.4f );
            onJump?.Invoke(jumpDirection);
        }


        /// <summary>
        /// Move in the given direction, while conforming to the ground's orientation
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        /// <param name="canStep">Whether or not the Entity can step to higher surfaces</param>
        public void GroundedMove(Vector3 direction, bool canStep = false) {

            if (direction.sqrMagnitude == 0f) return;

            if (onGround) {
                Vector3 rightOfDirection = Vector3.Cross(direction, -gravityDown).normalized;
                Vector3 directionConstrainedToGround = Vector3.Cross(groundHit.normal, rightOfDirection).normalized;

                direction = directionConstrainedToGround * direction.magnitude;
            }

            // Vector3 move = Vector3.ProjectOnPlane(direction, groundOrientation * -gravityDown);

            // bool walkCollision = ColliderCast( Vector3.zero, direction, out RaycastHit walkHit, 0.15f, Global.GroundMask);

            Move(direction);

        }


        /// <summary>
        /// Move in the given direction.
        /// </summary>
        /// <param name="direction">The direction to move in</param>
        public void Move(Vector3 direction) {
            if (direction.sqrMagnitude == 0f) return;

            _totalMovement += direction;

        }


        private void ExecuteMovement() {
            if (_totalMovement.sqrMagnitude == 0f) return;

            Vector3 step = _totalMovement / moveCollisionStep;
            for (int i = 0; i < moveCollisionStep; i++) {

                bool walkCollision = ColliderCast(Vector3.zero, step, out RaycastHit walkHit, 0.15f, Global.GroundMask);

                if (walkCollision) {
                    rigidbody.MovePosition(rigidbody.position + (step.normalized * Mathf.Min(Mathf.Clamp01(walkHit.distance - 0.1f), step.magnitude)));

                    // // Compute Penetration seems to be overkill.

                    // foreach ( ValuePair<string, Collider> pair in costumeData.hurtColliders ) {
                    //     Collider col1 = pair.Value;
                    //     Collider[] cols2 = col1.ColliderOverlap(Vector3.zero, 0f, Global.GroundMask);
                    //     for (int j = 0; j < cols2.Length; j++) {
                    //         if ( Physics.ComputePenetration(col1, col1.transform.position, col1.transform.rotation, cols2[j], cols2[j].transform.position, cols2[j].transform.rotation, out Vector3 direction, out float distance) ) {
                    //             rigidbody.MovePosition(rigidbody.position + (direction * distance));
                    //             Debug.Log("Penetration: " + direction + " " + distance);
                    //         }
                    //     }
                    // }

                    break;
                } else {
                    rigidbody.MovePosition(rigidbody.position + step);
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

            foreach (ValuePair<string, Collider> pair in character?.costumeData.hurtColliders){
                Collider collider = pair.Value;
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
            foreach (ValuePair<string, Collider> pair in character?.costumeData.hurtColliders){
                Collider collider = pair.Value;
                Collider[] hits = collider.ColliderOverlap( collider.transform.position, skinThickness, layerMask );
                if ( hits.Length > 0 ) return hits;
            }
            return new Collider[0];
        }


        public bool WallCheck( out RaycastHit wallHitOut, LayerMask layerMask ) {
            for (int i = 0; i < 11; i++){
                float angle = (i%2 == 0 && i != 0) ? (i-1 * -30f) : i * 30f;
                Quaternion angleTurn = Quaternion.AngleAxis( angle, rotation * Vector3.down);
                bool hasHitWall = ColliderCast( Vector3.zero, angleTurn * absoluteForward * 0.3f, out RaycastHit tempHit, 0.05f, layerMask );
                if (hasHitWall){
                    wallHitOut = tempHit;
                    return true;
                }
            }
            wallHitOut = new RaycastHit();
            return false;
        }


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
            entity.character = new Character.Instance(entity, character, costume);
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
            entity.character = new Character.Instance(entity, character, costume);
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            return entity;
            return entity;
        }

        #region MonoBehaviour

        
            protected virtual void Reset(){
                foreach (Transform child in transform) {
                    if ( child.gameObject.name.Contains("Model") )
                        GameUtility.SafeDestroy(child.gameObject);
                }
                _animator = GetComponent<Animator>();
            }

            protected virtual void OnEnable(){
                EntityManager.current.entityList.Add( this );
            }
            protected virtual void OnDisable(){
                EntityManager.current.entityList.Remove( this );
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
            
            protected virtual void OnDestroy(){;}

            protected virtual void Start(){
                rotation.SetVal(transform.rotation);
                relativeForward = Vector3.forward;
                rigidbody.useGravity = false;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }


            protected virtual void Update(){
                onGround.SetVal(ColliderCast(Vector3.zero, gravityDown.normalized * 0.1f, out groundHit, 0.15f, Global.GroundMask));

                state?.HandleInput();
                state?.StateUpdate();

                if (animator.runtimeAnimatorController != null) {
                    EntityAnimation();
                }
            }

            protected virtual void FixedUpdate(){
                state?.StateFixedUpdate();

                ExecuteMovement();
            }

        #endregion

    }
}