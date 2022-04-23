using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    public abstract class Entity : MonoBehaviour{

        public enum WalkSpeed {idle, crouch, walk, run, sprint};
        
        [HideInInspector] public Transform _transform;
        [HideInInspector] public Rigidbody _rb;
        [HideInInspector] public CustomPhysicsComponent _physicsComponent;
        [HideInInspector] public Animator _animator;

        public GameObject model;
        public EntityData data;
        public Collider[] _colliders;

        public Renderer _renderer;
        public Bounds bounds => _renderer.bounds;


        public Map<string, GameObject> bones = new Map<string, GameObject>();
        public GameObject this[string key]{
            get { try{ return bones[key]; } catch{ return model; } }
            private set { bones[key] = value; }
        }

        [Space(10)]

        public State state;

        public WeaponInventory weapons;
        public Weapon currentWeapon => weapons.currentItem;

        [Space(10)]

        public float currHealth;
        public float evadeTimer, parryTimer;
        public float jumpCooldown, shiftCooldown;


        public WalkSpeed walkSpeed;
        public float moveSpeed;

        public Vector3 moveDirection;
        public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);

        public Vector3 evadeDirection = Vector3.forward;

        public Vector3 inertia { get => inertiaDirection * inertiaMultiplier; set { inertiaDirection = value.normalized; inertiaMultiplier = value.magnitude; } }
        public Vector3 inertiaDirection = Vector3.forward;
        public float inertiaMultiplier = 0f;

        private Vector3 _absoluteForward;
        public Vector3 absoluteForward { get => _absoluteForward; set { _absoluteForward = value; _relativeForward = Quaternion.Inverse(rotation) * value; } }
        private Vector3 _relativeForward;
        public Vector3 relativeForward { get => _relativeForward; set { _relativeForward = value; _absoluteForward = rotation * value; } }
        public Vector3 rotationForward;

        public Quaternion rotation = Quaternion.identity;
        
        public Vector3 lookDirection => rotation * lookRotationData.currentValue * Vector3.forward;

        public Vector3 gravityDown = Vector3.down;
        
        public bool focusing;
        public bool walkingTo;
        public bool turningTo;
        [HideInInspector] public float subState;


        public List<Grabbable> grabbedObjects = new List<Grabbable>();
        public IInteractable lastInteracted;


        public BoolData groundData,
        wallData,
        slidingData,
        evadingData = new BoolData();

        // [HideInInspector]
        public BoolData lightAttackInputData,
        heavyAttackInputData,
        jumpInputData,
        evadeInputData,
        walkInputData,
        crouchInputData,
        focusInputData,
        shiftInputData = new BoolData();

        public VectorData moveInputData = new VectorData();
        public QuaternionData lookRotationData = new QuaternionData();

        public event Action<Vector3> onJump;
        public event Action<Vector3> onEvade;
        public event Action onParry;

        public RaycastHit groundHit;
        public RaycastHit wallHit;


        public bool isPlayer => Player.current.entity == this;
        public Vector3 bottom => transform.position - transform.up*bounds.extents.y;
        public float fallVelocity => Vector3.Dot(_rb.velocity, -gravityDown);
        public float gravityForce => data.weight * (currentWeapon?.weightModifier ?? 1f);

        public bool masked => state.masked;
        public bool inWater => _physicsComponent.inWater;
        public bool isOnWaterSurface => inWater && transform.position.y > (_physicsComponent.waterHeight - data.size.y);
        public bool evading => evadingData.currentValue;
        public bool sliding => slidingData.currentValue;
        public bool onGround => groundData.currentValue;
        public bool onWall => wallData.currentValue;



        

        protected virtual void EntityAwake(){;}
        protected virtual void EntityStart(){;}
        protected virtual void EntityEnable(){;}
        protected virtual void EntityDisable(){;}
        protected virtual void EntityDestroy(){;}
        protected virtual void EntityUpdate(){;}
        protected virtual void EntityFixedUpdate(){;}


        private void OnEnable(){
            Global.entityManager.entityList.Add( this );
            data.onChangeCostume += LoadModel;
            EntityEnable();
        }
        private void OnDisable(){
            Global.entityManager.entityList.Remove( this );
            data.onChangeCostume -= LoadModel;
            EntityDisable();
        }

        private void Awake(){
            weapons = new WeaponInventory(gameObject);
            EntityAwake();
        }

        private void Start(){
            rotation = transform.rotation;
            relativeForward = Vector3.forward;
            _rb.useGravity = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            EntityStart();
        }

        private void OnDestroy(){
            EntityDestroy();
        }

        public void Reset(){
            // Ensure only One Entity is on a single GameObject
            if (GetComponents<Entity>().Length > 1){
                Global.SafeDestroy(this);
                return;
            }

            string dataName = GetType().Name.Replace("Entity","");
            data = DataGetter.GetData<EntityData>( dataName );

            _transform = transform;
            _rb = GetComponent<Rigidbody>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();
            _animator = GetComponent<Animator>();

            LoadModel();
            gameObject.name = name;
        }

        private void Update(){
            // Debug.DrawRay(transform.position, inertiaDirection * inertiaMultiplier, Color.red);
            // Debug.DrawRay(transform.position, absoluteForward, Color.blue);
            // Debug.DrawRay(transform.position, evadeDirection, Color.magenta);
            // Debug.DrawRay(transform.position, _rb.velocity, Color.green);
            Debug.DrawRay(transform.position, moveDirection, Color.blue);

            jumpCooldown = Mathf.MoveTowards( jumpCooldown, 0f, Global.timeDelta );
            shiftCooldown = Mathf.MoveTowards( shiftCooldown, 0f, Global.timeDelta );
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, Global.timeDelta );
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, Global.timeDelta );

            evadingData.SetVal( evadeTimer > data.evadeCooldown );
            groundData.SetVal( this.DirectionCheck( gravityDown.normalized * 0.3f, out groundHit) );
            wallData.SetVal( this.WallCheck(out wallHit) );
            
            EntityUpdate();

            if (_animator.runtimeAnimatorController != null){
                _animator.SetBool("OnGround", onGround);
                _animator.SetBool("Falling", fallVelocity <= -20f);
                _animator.SetBool("Idle", moveDirection.magnitude == 0f );
                _animator.SetInteger("State", state.id);
                _animator.SetFloat("WeaponType", (float)(currentWeapon.data.weaponType) );
                _animator.SetFloat("SubState", subState);
                _animator.SetFloat("WalkSpeed", (float)walkSpeed);
                _animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, rotationForward));
                _animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-gravityDown, rotationForward)));
                state?.StateAnimation();
            }
        }

        private void FixedUpdate(){
            EntityFixedUpdate();
        }

        private void LateUpdate(){

            evadingData.Update();
            slidingData.Update();
            groundData.Update();
            wallData.Update();

            lightAttackInputData.Update();
            heavyAttackInputData.Update();
            jumpInputData.Update();
            evadeInputData.Update();
            walkInputData.Update();
            crouchInputData.Update();
            focusInputData.Update();
            shiftInputData.Update();
            moveInputData.Update();
            lookRotationData.Update();

            state.HandleInput();

            moveInputData.SetVal(Vector3.zero);
        }

        public void SetRotation(Vector3 newUp) => rotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
        public void RotateTowardsRelative(Vector3 newDirection, Vector3 newUp){

            Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg, Vector3.up) ;
            _transform.rotation = Quaternion.Slerp(_transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
        }
        public void RotateTowardsAbsolute(Vector3 newDirection, Vector3 newUp){

            Quaternion inverse = Quaternion.Inverse(rotation);
            RotateTowardsRelative(inverse * newDirection, inverse * newUp);
        }

        public void SetState(string stateName){
            state = Global.SafeDestroy(state);
            state = gameObject.AddComponent( State.GetStateTypeByName(stateName) ) as State;
        }

        public void SetWalkSpeed(WalkSpeed newSpeed){
            if (walkSpeed != newSpeed){
                if ((int)walkSpeed < (int)newSpeed) StartWalkAnim();
                walkSpeed = newSpeed;
            }
        }

        public void Gravity(float force, Vector3 direction){
            JumpGravity(force, direction, false);
        }
        public void JumpGravity(float force, Vector3 direction, bool slowFall){        
            _rb.AddForce(force * Global.timeDelta * direction);

            // Inertia that's only active when falling
            if( onGround ) return;
            
            float floatingMultiplier = slowFall ? 1.75f : 3f;
            float fallingMultiplier = 5f;
            float multiplier = fallVelocity >= 0 ? floatingMultiplier : fallingMultiplier;
            _rb.velocity += multiplier * force * Global.timeDelta * direction.normalized;
                
        }
        
        public void GroundedMove(Vector3 dir, bool canStep = false){

            if (dir.magnitude == 0f) return;

            Vector3 move = dir;

            move = Vector3.ProjectOnPlane(move, groundOrientation * -gravityDown);

            // Needs a bit of Reworking.
            // if (canStep){
            //     bool stepCollision = _collider.ColliderCast( transform.position + move.normalized * 0.5f, gravityDown.normalized * data.stepHeight * 0.98f, data.stepHeight, out RaycastHit stepHit, 0.45f );

            //     if (!walkCollision && stepCollision) {
            //         Vector3 stepElevation = Vector3.ProjectOnPlane( stepHit.point - (bottom + move), -move );
            //         float newMagnitude = Mathf.Sqrt(move.magnitude*move.magnitude - stepElevation.magnitude*stepElevation.magnitude);
            //         move = (move.normalized * Mathf.Max( newMagnitude, -newMagnitude) + stepElevation);
            //     }
            // }
            
            Move(move);
        }
        public void Move(Vector3 dir){
            if (dir.magnitude == 0f) return;

            Vector3 move = dir;

            bool walkCollision = this.DirectionCheck( move, out RaycastHit walkHit);

            if ( walkCollision && walkHit.transform.gameObject.layer == 0) 
                move = move.NullifyInDirection( -walkHit.normal );

            _transform.position += move;
        }

        public void Jump(Vector3 jumpDirection){
            if (jumpCooldown > 0f ) return;

            float gravityJumpMultiplier = (2 - (gravityForce/15f));

            Vector3 newVelocity = _rb.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += data.jumpHeight * gravityJumpMultiplier * jumpDirection;
            _rb.velocity = newVelocity;

            AnimatorTrigger("Jump");
            jumpCooldown = 0.4f;

            onJump?.Invoke(jumpDirection);
        }

        public bool EvadeUpdate(out float evadeSpeed){
            if ( !evading ) {
                evadeSpeed = 0f;
                return false;
            }
            if (evadeTimer > data.totalEvadeDuration - 0.15f){
                evadeDirection = absoluteForward;
            }
            inertiaDirection = evadeDirection;

            evadeSpeed = 24f * data.evadeCurve.Evaluate( 1 - ( (evadeTimer - data.evadeCooldown) / data.evadeDuration ) );
            return true;
        }

        public void GroundedEvade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;
            
            Vector3 newVelocity = _rb.velocity.NullifyInDirection( gravityDown );
            if (!onGround){
                newVelocity += -gravityDown.normalized * 5f;
            }
            _rb.velocity = newVelocity;

            Evade(evadeDirection);
        }
        public void Evade(Vector3 evadeDirection){
            if (evadeTimer > 0f) return;

            this.evadeDirection = evadeDirection;

            inertiaMultiplier = Mathf.Max( 12.5f, inertiaMultiplier);
            
            AnimatorTrigger("Evade");
            evadeTimer = data.totalEvadeDuration;

            onEvade?.Invoke(evadeDirection);
        }

        public void Parry(){
            Debug.Log("Parry");
            parryTimer = 0.15f;

            onParry?.Invoke();
        }

        public void EntityInput(Vector3 rawInput, Quaternion camRotation, SafeDictionary<string, bool> inputDictionary){

            lightAttackInputData.SetVal( inputDictionary["LightAttack"] );
            heavyAttackInputData.SetVal( inputDictionary["HeavyAttack"] );
            jumpInputData.SetVal( inputDictionary["Jump"] );
            evadeInputData.SetVal( inputDictionary["Evade"] );
            walkInputData.SetVal( inputDictionary["Walk"] );
            crouchInputData.SetVal( inputDictionary["Crouch"] );
            focusInputData.SetVal( inputDictionary["Focus"] );
            shiftInputData.SetVal( inputDictionary["Shift"] );
            moveInputData.SetVal( rawInput );
            lookRotationData.SetVal( camRotation );

        }

        public void RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D){
            camRight = rotation * lookRotationData.currentValue * Vector3.right;
            camForward = Vector3.Cross(camRight, _transform.up).normalized;
            groundDirection = (moveInputData.currentValue.x * camRight + moveInputData.currentValue.z * camForward).normalized;
            groundDirection3D = groundDirection + (lookRotationData.currentValue * Vector3.up) * moveInputData.currentValue.y;
        }

        public void LoadModel(){
            DestroyModel();

            if (data.costume.model != null) {
                // Load Model into Scene
                model = Instantiate(data.costume.model, transform.position, transform.rotation, transform);
                model.name = "Model";

                // Reset Animations
                _animator.runtimeAnimatorController = Global.entityManager.entityAnimatorController;
                _animator.Rebind();
            }

            // Assign Armature Bones and colliders in Script
            bones.Clear();
            foreach ( ValuePair<string, string> pair in data.costume.limbsPaths){
                this[pair.valueOne] = model.transform.Find( pair.valueTwo ).gameObject;
            }
            _colliders = model.GetComponentsInChildren<Collider>();
            _renderer = model.GetComponentInChildren<Renderer>();
            
            foreach ( Weapon weapon in weapons){
                weapon.LoadModel();
            }

            gameObject.SetLayerRecursively(6);
        }

        public void DestroyModel(){
            var children = new List<GameObject>();
            foreach (Transform child in _transform) if (child.gameObject.name.Contains("Model")) children.Add(child.gameObject);
            children.ForEach(child => Global.SafeDestroy(child));

            model = Global.SafeDestroy(model);
        }
        

        public void AnimatorTrigger(string triggerName){
            if (_animator.runtimeAnimatorController == null) return;
            _animator.SetTrigger(triggerName);
        }

        public void StartWalkAnim(){
            if (onGround && moveDirection.magnitude > 0f){
                AnimatorTrigger("Walk");
            }
        }

    }
}