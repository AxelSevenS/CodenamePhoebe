using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    public class Entity : MonoBehaviour, IDamageable, ICostumable<EntityCostume>{

        public enum WalkSpeed {idle, crouch, walk, run, sprint};

        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public CustomPhysicsComponent physicsComponent;

        public EntityData data;
        public EntityCostume costume;

        public GameObject model;
        public CostumeData modelData;

        protected Animator animator => modelData?.animator ?? null;
        public GameObject this[string key]{
            get { try{ return modelData.bones[key]; } catch{ return model; } }
        }

        [Space(10)]

        [SerializeReference] public State state;
        public State defaultState => EntityData.TypeToDefaultState(data.entityType);

        [Space(10)]

        public float currHealth;


        [Header("Jumping")]

        public float jumpCount = 1f;
        public float jumpCooldown;
        public event Action<Vector3> onJump;

        [Header("Movement")]

        const int moveCollisionStep = 1;

        public VectorData moveDirection = new VectorData();
        public QuaternionData rotation = new QuaternionData();

        private Vector3 _absoluteForward;
        public Vector3 absoluteForward { get => _absoluteForward; set { _absoluteForward = value; _relativeForward = Quaternion.Inverse(rotation) * value; } }
        private Vector3 _relativeForward;
        public Vector3 relativeForward { get => _relativeForward; set { _relativeForward = value; _absoluteForward = rotation * value; } }

        public WalkSpeed walkSpeed;
        public float moveSpeed;
        
        public Vector3 gravityDown = Vector3.down;

        public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);
        public Quaternion finalPlayerRotation => rotation * cameraRotation;



        public bool walkingTo;
        public bool turningTo;
        [HideInInspector] public float subState;


        public IInteractable lastInteracted;

        public event Action onDamaged;
        public event Action onDeath;

        public RaycastHit groundHit;

        public BoolData onGround;
        public BoolData sliding;

        [Header("Input")]

        public BoolData lightAttackInput;
        public BoolData heavyAttackInput;
        public BoolData jumpInput;
        public BoolData evadeInput;
        public BoolData walkInput;
        public BoolData crouchInput;
        public BoolData focusInput;
        public BoolData shiftInput;
        public VectorData moveInput;
        public QuaternionData cameraRotation;



        public bool isPlayer => Player.current.entity == this;
        public Vector3 bottom => transform.position - transform.up*data.size.y/2f;
        public float fallVelocity => Vector3.Dot(rb.velocity, -gravityDown);
        public bool inWater => physicsComponent.inWater;
        public bool isOnWaterSurface => inWater && transform.position.y > (physicsComponent.waterHeight - data.size.y);

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
        protected virtual void EntityAnimation(){;}
        protected virtual void EntityLoadModel(){;}
        protected virtual void EntitySetCostume(){;}


        private void OnEnable(){
            Global.entityManager.entityList.Add( this );
            EntityEnable();
        }
        private void OnDisable(){
            Global.entityManager.entityList.Remove( this );
            EntityDisable();
        }

        [ContextMenu("Initialize")]
        private void Awake(){

            // Ensure only One Entity is on a single GameObject
            // Entity[] entities = GetComponents<Entity>();
            // for (int i = 0; i < entities.Length; i++) {
            //     if (entities[i] != this) {
            //         Global.SafeDestroy(entities[i]);
            //     }
            // }
            
            rb = GetComponent<Rigidbody>();
            physicsComponent = GetComponent<CustomPhysicsComponent>();
            
            if (data == null) data = EntityData.GetDefaultEntityData();
            gameObject.name = data.name;
            
            if (costume == null) SetCostume(data.defaultCostume);
            if (model == null) LoadModel();

            if (state == null) SetState(defaultState);

            onGround = new BoolData();
            sliding = new BoolData();

            lightAttackInput = new BoolData();
            heavyAttackInput = new BoolData();
            jumpInput = new BoolData();
            evadeInput = new BoolData();
            walkInput = new BoolData();
            crouchInput = new BoolData();
            focusInput = new BoolData();
            shiftInput = new BoolData();
            moveInput = new VectorData();
            cameraRotation = new QuaternionData();

            EntityAwake();
        }

        private void Start(){
            rotation.SetVal(transform.rotation);
            relativeForward = Vector3.forward;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            EntityStart();
        }

        private void OnDestroy(){
            EntityDestroy();
        }

        public void Reset(){
            DestroyModel();
        }

        private void Update(){
            onGround?.SetVal( ColliderCast( Vector3.zero, gravityDown.normalized * 0.1f, out groundHit, 0.15f, Global.GroundMask ) );
            
            state?.HandleInput();
            state?.StateUpdate();

            if (animator.runtimeAnimatorController != null){
                animator.SetBool("OnGround", onGround);
                animator.SetBool("Falling", fallVelocity <= -20f);
                animator.SetBool("Idle", moveDirection.magnitude == 0f );
                animator.SetInteger("State", state.id);
                animator.SetFloat("SubState", subState);
                animator.SetFloat("WalkSpeed", (float)walkSpeed);
                animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, transform.forward));
                animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-transform.up, transform.forward)));
                state?.StateAnimation();
            }
            
            EntityUpdate();
        }

        private void FixedUpdate(){
            EntityFixedUpdate();
            state?.StateFixedUpdate();
        }

        private void LateUpdate(){
            EntityLateUpdate();
            state?.StateLateUpdate();
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

            rb.AddForce(knockback, ForceMode.Impulse);
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
            cameraRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(rotation) * direction, rotation * Vector3.up ) );
        }

        public void SetRotation(Vector3 newUp) {
            rotation.SetVal(Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation);
        }
        public void RotateTowardsRelative(Vector3 newDirection, Vector3 newUp){

            Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newDirection.x, newDirection.z) * Mathf.Rad2Deg, Vector3.up) ;
            transform.rotation = Quaternion.Slerp(transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
        }
        public void RotateTowardsAbsolute(Vector3 newDirection, Vector3 newUp){

            Vector3 inverse = Quaternion.Inverse(rotation) * newDirection;
            RotateTowardsRelative(inverse, newUp);

            // Quaternion apparentRotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;
            // Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(inverse.x, inverse.z) * Mathf.Rad2Deg, Vector3.up) ;
            // transform.rotation = Quaternion.Slerp(transform.rotation, apparentRotation * turnDirection, 12f*Global.timeDelta);
        }

        public void SetState(State newState){
            state?.OnExit();
            newState.OnEnter(this);
            state = newState;
            Debug.Log($"{name} switched state to {state.name}");
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
            rb.AddForce(force * Global.timeDelta * direction);

            // Inertia that's only active when falling
            if ( onGround ) return;
            
            float floatingMultiplier = slowFall ? 1.75f : 3.25f;
            float fallingMultiplier = 5f;
            float multiplier = fallVelocity >= 0 ? floatingMultiplier : fallingMultiplier;
            rb.velocity += multiplier * force * Global.timeDelta * direction.normalized;
        
        }
        public void Jump(Vector3 jumpDirection){

            Debug.Log(data.jumpHeight * JumpMultiplier());

            Vector3 newVelocity = rb.velocity.NullifyInDirection( -jumpDirection );
            newVelocity += data.jumpHeight * JumpMultiplier() * jumpDirection;
            rb.velocity = newVelocity;

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
            //         Vector3 pos = transform.position + move + stepPos;
            //         bool stepGroundCollision = ColliderCast( pos, -gravityDown * 10f, out RaycastHit stepGroundHit, 0.15f, Global.GroundMask);
            //         // if (stepGroundCollision) {
            //         //     Debug.DrawLine(pos, stepGroundHit.point, Color.red, 0.2f);
            //         //     Debug.Log(stepGroundHit.distance);
            //         // }

            //         // bool stepGroundCollision = Physics.Raycast( transform.position + move + (-gravityDown * data.stepHeight), gravityDown, out RaycastHit stepGroundHit, data.stepHeight, Global.GroundMask);

            //         // float newMagnitude = Mathf.Sqrt(move.magnitude*move.magnitude - data.stepHeight*data.stepHeight);
            //         // move = (move.normalized * Mathf.Max( newMagnitude, -newMagnitude) + gravityDown * data.stepHeight);

            //         // move += -gravityDown * data.stepHeight * Global.timeDelta;
            //     }else if (walkCollision && stepCollision) {
            //         move = move.NullifyInDirection( -walkHit.normal );
            //     }
            // }else if (walkCollision) {
            //     move = move.NullifyInDirection( -walkHit.normal );
            // }
            // rb.MovePosition(rb.position + move);

            Move(move);

        }
        public void Move(Vector3 dir){
            if (dir.magnitude == 0f) return;

            Vector3 move = dir / moveCollisionStep;

            for (int i = 0; i < moveCollisionStep; i++) {
                
                bool walkCollision = ColliderCast( Vector3.zero, move, out RaycastHit walkHit, 0.15f, Global.GroundMask);

                if ( walkCollision ){
                    move = move.NullifyInDirection( -walkHit.normal );
                    i = moveCollisionStep;
                }
                rb.MovePosition(rb.position + move);
            }

        }

        public void EntityInput(Vector3 rawInput, Quaternion camRotation, SafeDictionary<string, bool> inputDictionary){

            lightAttackInput?.SetVal( inputDictionary["LightAttack"] );
            heavyAttackInput?.SetVal( inputDictionary["HeavyAttack"] );
            jumpInput?.SetVal( inputDictionary["Jump"] );
            evadeInput?.SetVal( inputDictionary["Evade"] );
            walkInput?.SetVal( inputDictionary["Walk"] );
            crouchInput?.SetVal( inputDictionary["Crouch"] );
            focusInput?.SetVal( inputDictionary["Focus"] );
            shiftInput?.SetVal( inputDictionary["Shift"] );
            moveInput?.SetVal( rawInput );
            cameraRotation?.SetVal( camRotation );

        }

        public void RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D){
            camRight = finalPlayerRotation * Vector3.right;
            camForward = Vector3.Cross(camRight, transform.up).normalized;
            groundDirection = (moveInput.x * camRight + moveInput.z * camForward).normalized;
            groundDirection3D = groundDirection + (cameraRotation * Vector3.up) * moveInput.y;
        }



        public void SetCostume(EntityCostume newCostume){

            costume = newCostume;
            LoadModel();

            EntitySetCostume();
        }

        [ContextMenu("Load Model")]
        public void LoadModel() {
            DestroyModel();

            model = Instantiate(costume.model, transform.position, transform.rotation, transform);
            model.name = "Model";
            modelData = model.GetComponent<CostumeData>();
            gameObject.SetLayerRecursively(6);

            EntityLoadModel();
        }

        public void DestroyModel(){

            model = Global.SafeDestroy(model);

            foreach (Transform child in transform) {
                if (child.gameObject.name.Contains("Model")) 
                    Global.SafeDestroy(child.gameObject);
                    
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

        public static Entity CreateEntity(System.Type entityType, Vector3 position, Quaternion rotation, EntityData data, EntityCostume costume){
            GameObject entityGO = new GameObject("Entity");
            Entity entity = (Entity)entityGO.AddComponent(entityType);
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            entity.data = data;
            entity.SetCostume(costume);
            return entity;
        }

    }
}