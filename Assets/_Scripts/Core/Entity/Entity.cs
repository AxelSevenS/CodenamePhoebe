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
        public float shiftCooldown;

        public float jumpCount = 1f;
        public float evadeCount = 1f;
        public float jumpCooldown;

        public WalkSpeed walkSpeed;
        public float moveSpeed;

        public Vector3 moveDirection;
        public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);

        public Vector3 evadeDirection = Vector3.forward;

        private Vector3 _absoluteForward;
        public Vector3 absoluteForward { get => _absoluteForward; set { _absoluteForward = value; _relativeForward = Quaternion.Inverse(rotation) * value; } }
        private Vector3 _relativeForward;
        public Vector3 relativeForward { get => _relativeForward; set { _relativeForward = value; _absoluteForward = rotation * value; } }
        public Vector3 rotationForward;

        public Quaternion rotation = Quaternion.identity;
        
        public Quaternion cameraRotation => rotation * lookRotation;

        public Vector3 gravityDown = Vector3.down;
        
        public bool focusing;
        public bool walkingTo;
        public bool turningTo;
        [HideInInspector] public float subState;


        public List<Grabbable> grabbedObjects = new List<Grabbable>();
        public IInteractable lastInteracted;


        public BoolData onGround = new BoolData();
        public BoolData sliding = new BoolData();
        public BoolData evading = new BoolData();

        // [HideInInspector]
        public BoolData lightAttackInput = new BoolData();
        public BoolData heavyAttackInput = new BoolData();
        public BoolData jumpInput = new BoolData();
        public BoolData evadeInput = new BoolData();
        public BoolData walkInput = new BoolData();
        public BoolData crouchInput = new BoolData();
        public BoolData focusInput = new BoolData();
        public BoolData shiftInput = new BoolData();

        public VectorData moveInput = new VectorData();
        public QuaternionData lookRotation = new QuaternionData();

        public event Action<Vector3> onJump;
        public event Action<Vector3> onEvade;
        public event Action onParry;

        public RaycastHit groundHit;


        public bool isPlayer => Player.current.entity == this;
        public Vector3 bottom => transform.position - transform.up*bounds.extents.y;
        public float fallVelocity => Vector3.Dot(_rb.velocity, -gravityDown);
        public float gravityForce => data.weight * (currentWeapon?.weightModifier ?? 1f);

        public bool masked => state.masked;
        public bool inWater => _physicsComponent.inWater;
        public bool isOnWaterSurface => inWater && transform.position.y > (_physicsComponent.waterHeight - data.size.y);



        

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

            evading.SetVal( evadeTimer > data.evadeCooldown );
            onGround.SetVal( ColliderCast( Vector3.zero, gravityDown.normalized * 0.1f, out groundHit, 0.05f, Global.GroundMask ) );
            
            state.HandleInput();

            moveInput.SetVal(Vector3.zero);

            // Debug.DrawRay(transform.position, inertiaDirection * inertiaMultiplier, Color.red);
            // Debug.DrawRay(transform.position, absoluteForward, Color.blue);
            // Debug.DrawRay(transform.position, evadeDirection, Color.magenta);
            // Debug.DrawRay(transform.position, _rb.velocity, Color.green);
            // Debug.DrawRay(transform.position, moveDirection, Color.blue);

            shiftCooldown = Mathf.MoveTowards( shiftCooldown, 0f, Global.timeDelta );
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, Global.timeDelta );
            parryTimer = Mathf.MoveTowards( parryTimer, 0f, Global.timeDelta );
            
            
            EntityUpdate();

            if (_animator.runtimeAnimatorController != null){
                _animator.SetBool("OnGround", onGround);
                _animator.SetBool("Falling", fallVelocity <= -20f);
                _animator.SetBool("Idle", moveDirection.magnitude == 0f );
                _animator.SetInteger("State", state.id);
                _animator.SetFloat("WeaponType", (float)(currentWeapon.data.weaponType) );
                _animator.SetFloat("SubState", subState);
                _animator.SetFloat("WalkSpeed", (float)walkSpeed);
                _animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, _transform.forward));
                _animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-_transform.up, _transform.forward)));
                state?.StateAnimation();
            }
        }

        private void FixedUpdate(){
            EntityFixedUpdate();
        }

        private void LateUpdate(){
        }

        public bool ColliderCast( Vector3 position, Vector3 direction, out RaycastHit checkHit, float skinThickness, LayerMask layerMask ) {
            foreach (Collider col in _colliders){
                bool hasHitWall = col.ColliderCast( col.transform.position + _transform.TransformDirection(position), direction, out RaycastHit tempHit, skinThickness, layerMask );
                if ( !hasHitWall || tempHit.collider == null ) continue;

                checkHit = tempHit;
                return true;
            }
            checkHit = new RaycastHit();
            return false;
        }
        public Collider[] ColliderOverlap( float skinThickness, LayerMask layerMask ) {
            foreach (Collider col in _colliders){
                Collider[] hits = col.ColliderOverlap( col.transform.position, skinThickness, layerMask );
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

        public void LookAt( Vector3 direction) {
            lookRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(rotation) * direction, rotation * Vector3.up ) );
        }

        public void SetRotation(Vector3 newUp) => rotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
        public void RotateTowardsRelative(Vector3 newDirection, Vector3 newUp){

            Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg, Vector3.up) ;
            _transform.rotation = Quaternion.Slerp(_transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
        }
        public void RotateTowardsAbsolute(Vector3 newDirection, Vector3 newUp){

            Quaternion inverse = Quaternion.Inverse(rotation);
            RotateTowardsRelative(inverse * newDirection, newUp);
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

            Vector3 move = Vector3.ProjectOnPlane(dir, groundOrientation * -gravityDown);

            // if (canStep){
            //     bool stepCollision = _collider.ColliderCast( Vector3.zero, transform.position + move.normalized * 0.5f, gravityDown.normalized * data.stepHeight * 0.98f, data.stepHeight, out RaycastHit stepHit, 0.45f );

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

            bool walkCollision = this.ColliderCast( Vector3.zero, dir, out RaycastHit walkHit, 0.15f, Global.GroundMask);

            if ( walkCollision /* && walkHit.transform.gameObject.layer == 0 */) 
                dir = dir.NullifyInDirection( -walkHit.normal );

            _rb.MovePosition(_rb.position + dir);
            // _transform.position += dir;
        }

        public void Jump(Vector3 jumpDirection){

            float gravityJumpMultiplier = (2 - (gravityForce/15f));

            Vector3 newVelocity = _rb.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += data.jumpHeight * gravityJumpMultiplier * jumpDirection;
            _rb.velocity = newVelocity;

            AnimatorTrigger("Jump");

            jumpCooldown = 0.4f;
            onJump?.Invoke(jumpDirection);
        }

        public bool EvadeUpdate(out float evadeTime, out float evadeCurve){
            if ( !evading ) {
                evadeTime = 0f;
                evadeCurve = 0f;
                return false;
            }

            evadeTime = Mathf.Clamp01( 1 - ( (evadeTimer - data.evadeCooldown) / data.evadeDuration ) );
            evadeCurve = Mathf.Clamp01( data.evadeCurve.Evaluate( evadeTime ) );
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

            lightAttackInput.SetVal( inputDictionary["LightAttack"] );
            heavyAttackInput.SetVal( inputDictionary["HeavyAttack"] );
            jumpInput.SetVal( inputDictionary["Jump"] );
            evadeInput.SetVal( inputDictionary["Evade"] );
            walkInput.SetVal( inputDictionary["Walk"] );
            crouchInput.SetVal( inputDictionary["Crouch"] );
            focusInput.SetVal( inputDictionary["Focus"] );
            shiftInput.SetVal( inputDictionary["Shift"] );
            moveInput.SetVal( rawInput );
            lookRotation.SetVal( camRotation );

        }

        public void RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D){
            camRight = rotation * lookRotation.currentValue * Vector3.right;
            camForward = Vector3.Cross(camRight, _transform.up).normalized;
            groundDirection = (moveInput.currentValue.x * camRight + moveInput.currentValue.z * camForward).normalized;
            groundDirection3D = groundDirection + (lookRotation.currentValue * Vector3.up) * moveInput.currentValue.y;
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
            if (onGround && moveDirection.magnitude > 0f)
                AnimatorTrigger("Walk");
        }

    }
}