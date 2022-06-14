using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Weapons;
using SeleneGame.States;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    public class Entity : MonoBehaviour, IDamageable{

        public enum WalkSpeed {idle, crouch, walk, run, sprint};
        
        [HideInInspector] public Transform _transform;
        [HideInInspector] public Rigidbody _rb;
        [HideInInspector] public CustomPhysicsComponent _physicsComponent;

        public EntityData data;
        public GameObject model;
        public CostumeData modelData;
        public KeyValuePair<string, int> hello = new KeyValuePair<string, int>("hello", 0);

        protected Animator animator => modelData?.animator ?? null;
        public GameObject this[string key]{
            get { try{ return modelData.bones[key]; } catch{ return model; } }
        }

        [Space(10)]

        public State state;
        public string defaultState => data.defaultState;

        [Space(10)]

        public float currHealth;

        public WalkSpeed walkSpeed;
        public float moveSpeed;

        public float jumpCount = 1f;
        public float jumpCooldown;
        public event Action<Vector3> onJump;


        public BoolData evading = new BoolData();
        public Vector3 evadeDirection = Vector3.forward;
        public float evadeTimer;
        public float evadeCount = 1f;
        public event Action<Vector3> onEvade;
        


        private Vector3 _absoluteForward;
        private Vector3 _relativeForward;
        public Vector3 absoluteForward { get => _absoluteForward; set { _absoluteForward = value; _relativeForward = Quaternion.Inverse(rotation) * value; } }
        public Vector3 relativeForward { get => _relativeForward; set { _relativeForward = value; _absoluteForward = rotation * value; } }

        public VectorData moveDirection = new VectorData();
        public QuaternionData rotation = new QuaternionData();
        public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);
        public Quaternion cameraRotation => rotation * lookRotation;

        public Vector3 gravityDown = Vector3.down;
        public bool walkingTo;
        public bool turningTo;
        [HideInInspector] public float subState;


        public IInteractable lastInteracted;

        public event Action onDamaged;
        public event Action onDeath;


        public BoolData onGround = new BoolData();
        public BoolData sliding = new BoolData();

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

        public RaycastHit groundHit;


        public bool isPlayer => Player.current.entity == this;
        public Vector3 bottom => _transform.position - _transform.up*data.size.y/2f;
        public float fallVelocity => Vector3.Dot(_rb.velocity, -gravityDown);
        public bool inWater => _physicsComponent.inWater;
        public bool isOnWaterSurface => inWater && transform.position.y > (_physicsComponent.waterHeight - data.size.y);

        public virtual bool CanTurn() => true;
        public virtual bool CanWaterHover() => false;
        public virtual bool CanSink() => false;
        public virtual float GravityMultiplier() => data.weight;
        public virtual float JumpMultiplier() => 1f;
        



        

        protected virtual void EntityAwake(){;}
        protected virtual void EntityStart(){;}
        protected virtual void EntityEnable(){;}
        protected virtual void EntityDisable(){;}
        protected virtual void EntityDestroy(){;}
        protected virtual void EntityUpdate(){;}
        protected virtual void EntityLateUpdate(){;}
        protected virtual void EntityFixedUpdate(){;}
        protected virtual void EntityLoadModel(){;}


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
            if (data == null) data = Resources.Load<EntityData>( "Data/Entity/Selene" );
            if (model == null) LoadModel();
            SetState(defaultState);
            EntityAwake();
        }

        private void Start(){
            rotation.SetVal(transform.rotation);
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

            _transform = transform;
            _rb = GetComponent<Rigidbody>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();

            gameObject.name = name;
        }

        private void Update(){
            evading.SetVal( evadeTimer > data.evadeCooldown );
            onGround.SetVal( ColliderCast( Vector3.zero, gravityDown.normalized * 0.1f, out groundHit, 0.15f, Global.GroundMask ) );

            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, Global.timeDelta );
            
            state.HandleInput();
            
            float newSpeed = state.UpdateMoveSpeed();
            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * data.moveIncrement * Global.timeDelta);

            // Debug.DrawRay(transform.position, inertiaDirection * inertiaMultiplier, Color.red);
            // Debug.DrawRay(transform.position, absoluteForward, Color.blue);
            // Debug.DrawRay(transform.position, evadeDirection, Color.magenta);
            // Debug.DrawRay(transform.position, _rb.velocity, Color.green);
            // Debug.DrawRay(transform.position, moveDirection, Color.blue);
            
            
            EntityUpdate();

            if (animator.runtimeAnimatorController != null){
                animator.SetBool("OnGround", onGround);
                animator.SetBool("Falling", fallVelocity <= -20f);
                animator.SetBool("Idle", moveDirection.magnitude == 0f );
                animator.SetInteger("State", state.id);
                animator.SetFloat("SubState", subState);
                animator.SetFloat("WalkSpeed", (float)walkSpeed);
                animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, _transform.forward));
                animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-_transform.up, _transform.forward)));
                state?.StateAnimation();
            }
        }

        private void FixedUpdate(){
            if ( moveDirection.magnitude > 0f && evadeTimer > data.totalEvadeDuration - 0.15f )
                evadeDirection = moveDirection.normalized;

            EntityFixedUpdate();
        }

        private void LateUpdate(){
            EntityLateUpdate();
        }

        public virtual void Death(){
            // Global.SafeDestroy(gameObject);
            AnimatorTrigger("Death");

            onDeath?.Invoke();
        }

        public void Damage(float amount, Vector3 knockback = default){

            currHealth = Mathf.Max(currHealth - amount, 0f);

            if (currHealth == 0f)
                Death();

            _rb.AddForce(knockback, ForceMode.Impulse);
        }

        public bool ColliderCast( Vector3 position, Vector3 direction, out RaycastHit checkHit, float skinThickness, LayerMask layerMask ) {
            // Debug.Log(modelData.hurtColliders["main"]);

            foreach (ValuePair<string, Collider> col in modelData.hurtColliders){
                bool hasHitWall = col.valueTwo.ColliderCast( col.valueTwo.transform.position + position, direction, out RaycastHit tempHit, skinThickness, layerMask );
                if ( !hasHitWall || tempHit.collider == null ) continue;

                checkHit = tempHit;
                return true;
            }
            checkHit = new RaycastHit();
            return false;
        }
        public Collider[] ColliderOverlap( float skinThickness, LayerMask layerMask ) {
            foreach (ValuePair<string, Collider> col in modelData.hurtColliders){
                Collider collider = col.valueTwo;
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

        public void LookAt( Vector3 direction) {
            lookRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(rotation) * direction, rotation * Vector3.up ) );
        }

        public void SetRotation(Vector3 newUp) {
            rotation.SetVal(Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation);
        }
        public void RotateTowardsRelative(Vector3 newDirection, Vector3 newUp){

            Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg, Vector3.up) ;
            _transform.rotation = Quaternion.Slerp(_transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
        }
        public void RotateTowardsAbsolute(Vector3 newDirection, Vector3 newUp){

            Vector3 inverse = Quaternion.Inverse(rotation) * newDirection;
            RotateTowardsRelative(inverse, newUp);

            // Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            // Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(inverse.x, inverse.z) * Mathf.Rad2Deg, Vector3.up) ;
            // _transform.rotation = Quaternion.Slerp(_transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
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


        public void Gravity(float force, Vector3 direction){
            JumpGravity(force, direction, false);
        }
        public void JumpGravity(float force, Vector3 direction, bool slowFall){        
            _rb.AddForce(force * Global.timeDelta * direction);

            // Inertia that's only active when falling
            if ( onGround ) return;
            
            float floatingMultiplier = slowFall ? 1.75f : 3f;
            float fallingMultiplier = 5f;
            float multiplier = fallVelocity >= 0 ? floatingMultiplier : fallingMultiplier;
            _rb.velocity += multiplier * force * Global.timeDelta * direction.normalized;
        
        }
        public void Jump(Vector3 jumpDirection){

            Debug.Log(data.jumpHeight * JumpMultiplier());

            Vector3 newVelocity = _rb.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += data.jumpHeight * JumpMultiplier() * jumpDirection;
            _rb.velocity = newVelocity;

            AnimatorTrigger("Jump");

            jumpCooldown = 0.4f;
            onJump?.Invoke(jumpDirection);
        }

        
        public void GroundedMove(Vector3 dir, bool canStep = false){

            if (dir.magnitude == 0f) return;

            Vector3 move = Vector3.ProjectOnPlane(dir, groundOrientation * -gravityDown);

            bool walkCollision = ColliderCast( Vector3.zero, move, out RaycastHit walkHit, 0.15f, Global.GroundMask);
            // Debug.DrawRay(bottom, -gravityDown * data.size.y, Color.red);

            // if (canStep){
            //     Vector3 stepPos = -gravityDown * data.stepHeight;
            //     bool stepCollision = ColliderCast( stepPos, move, out RaycastHit stepHit, 0.15f, Global.GroundMask);

            //     if (walkCollision && !stepCollision) {
            //         Vector3 pos = _transform.position + move + stepPos;
            //         bool stepGroundCollision = ColliderCast( pos, -gravityDown * 10f, out RaycastHit stepGroundHit, 0.15f, Global.GroundMask);
            //         // if (stepGroundCollision) {
            //         //     Debug.DrawLine(pos, stepGroundHit.point, Color.red, 0.2f);
            //         //     Debug.Log(stepGroundHit.distance);
            //         // }

            //         // bool stepGroundCollision = Physics.Raycast( _transform.position + move + (-gravityDown * data.stepHeight), gravityDown, out RaycastHit stepGroundHit, data.stepHeight, Global.GroundMask);

            //         // float newMagnitude = Mathf.Sqrt(move.magnitude*move.magnitude - data.stepHeight*data.stepHeight);
            //         // move = (move.normalized * Mathf.Max( newMagnitude, -newMagnitude) + gravityDown * data.stepHeight);

            //         // move += -gravityDown * data.stepHeight * Global.timeDelta;
            //     }else if (walkCollision && stepCollision) {
            //         move = move.NullifyInDirection( -walkHit.normal );
            //     }
            // }else if (walkCollision) {
            //     move = move.NullifyInDirection( -walkHit.normal );
            // }

            _rb.MovePosition(_rb.position + move);

        }
        public void Move(Vector3 dir){
            if (dir.magnitude == 0f) return;

            bool walkCollision = this.ColliderCast( Vector3.zero, dir, out RaycastHit walkHit, 0.15f, Global.GroundMask);

            if ( walkCollision ) 
                dir = dir.NullifyInDirection( -walkHit.normal );

            _rb.MovePosition(_rb.position + dir);
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
            camRight = cameraRotation * Vector3.right;
            camForward = Vector3.Cross(camRight, _transform.up).normalized;
            groundDirection = (moveInput.x * camRight + moveInput.z * camForward).normalized;
            groundDirection3D = groundDirection + (lookRotation * Vector3.up) * moveInput.y;
        }

        [ContextMenu("Load Model")]
        public void LoadModel(){
            DestroyModel();

            if (data == null) return;

            model = Instantiate(data.costume.model, transform.position, transform.rotation, transform);
            model.name = "Model";

            modelData = model.GetComponent<CostumeData>();

            gameObject.SetLayerRecursively(6);

            EntityLoadModel();
        }

        public void DestroyModel(){

            model = Global.SafeDestroy(model);

            foreach (Transform child in _transform) {
                if (child.gameObject.name.Contains("Model")) 
                    Global.SafeDestroy(child);
                    
            }
        }
        

        public void AnimatorTrigger(string triggerName){
            if (animator.runtimeAnimatorController == null) return;
            animator.SetTrigger(triggerName);
        }

        public void StartWalkAnim(){
            if (!onGround && moveDirection.magnitude == 0f) return;
            AnimatorTrigger("Walk");
        }

    }
}