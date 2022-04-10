using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SeleneGame {

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CustomPhysicsComponent))]
    [RequireComponent(typeof(Animator))]
    public abstract class Entity : MonoBehaviour{
        
        [HideInInspector] public Transform _t;
        [HideInInspector] public Rigidbody _rb;
        [HideInInspector] public CustomPhysicsComponent _physicsComponent;
        [HideInInspector] public Animator _animator;
        [ReadOnly] public Collider _collider;
        [ReadOnly] public Collider[] _colliders;
        [ReadOnly] public Bounds _colliderBounds;
        public enum WalkSpeed {idle, crouch, walk, run, sprint}; 

        [Space(20)]

        [ReadOnly] public GameObject model;
        [ReadOnly] public EntityData data;

        [ReadOnly] public State currentState;

        [ReadOnly] public Weapon[] weapons = new Weapon[1];
        [ReadOnly] public Weapon currentWeapon;

        [SerializeField] private List<ValuePairStringGameObject> bones = new List<ValuePairStringGameObject>();
        private Dictionary<string, GameObject> bonesDict = new Dictionary<string, GameObject>();

        public GameObject this[string key]{
            get { try{ return bonesDict[key]; } catch{ return model; } }
            private set {
                bones.Add( new ValuePairStringGameObject(key, value) );
                bonesDict[key] = value;
            }
        }

        public void GetBonesFromData() {
            bones.Clear();
            foreach ( StringPair pair in data.costume.limbsPaths){
                this[pair.valueOne] = model.transform.Find( pair.valueTwo ).gameObject;
            }
        }
        public void ReloadBones() {
            bonesDict.Clear();
            foreach (ValuePairStringGameObject pair in bones){
                bonesDict[pair.valueOne] = pair.valueTwo;
            }
        }

        [Space(20)]

        [HideInInspector] public float subState;

        public float currHealth, moveSpeed, evadeTimer;

        public bool isPlayer => Player.current.entity == this;
        public Vector3 bottom => transform.position - transform.up*_colliderBounds.extents.y;

        public Vector3 gravityDown = Vector3.down;
        public float fallVelocity => Vector3.Dot(_rb.velocity, -gravityDown);

        public WalkSpeed walkSpeed;

        public Vector3 moveDirection;
        public Vector3 evadeDirection = Vector3.forward;
        public Vector3 inertia { get => inertiaDirection * inertiaMultiplier; set { inertiaDirection = value.normalized; inertiaMultiplier = value.magnitude; } }
        public Vector3 inertiaDirection = Vector3.forward;
        public float inertiaMultiplier = 0f;

        private Vector3 _relativeForward;
        public Vector3 relativeForward { get => _relativeForward; set { _relativeForward = value; _absoluteForward = rotation * value; } }
        private Vector3 _absoluteForward;
        public Vector3 absoluteForward { get => _absoluteForward; set { _absoluteForward = value; _relativeForward = Quaternion.Inverse(rotation) * value; } }
        public Vector3 rotationForward;

        public Quaternion groundOrientation => Quaternion.FromToRotation(-gravityDown, groundHit.normal);

        public Quaternion rotation = Quaternion.identity;
        public Quaternion apparentRotation => Quaternion.FromToRotation(rotation * Vector3.up, currentState.entityUp) * rotation ;

        public float gravityForce => data.weight * (currentWeapon?.weightModifier ?? 1f);

        public float jumpCooldown, shiftCooldown;

        public List<IManipulable> manipulatedObject = new List<IManipulable>();
        public IInteractable lastInteracted;

        public bool canMove => !sliding && !/* (evadeTimer > data.evadeCooldown && evadeTimer < data.evadeCooldown + data.evadeDuration - 0.1f) */evading;
        
        public bool idle => moveDirection.magnitude == 0;
        public bool inWater => _physicsComponent.inWater;
        public bool isOnWaterSurface => ( inWater && transform.position.y > (_physicsComponent.waterHeight - _collider.GetSize().y) );
        public bool focusing;
        public bool evading => evadingData.currentValue;
        public bool sliding => slidingData.currentValue;
        public bool onGround => groundData.currentValue;
        public bool onWall => wallData.currentValue;
        public bool walkingTo;
        public bool turningTo;

        public bool shifting => currentState.shifting;

        public ExtendedBool groundData = new ExtendedBool();
        public ExtendedBool wallData = new ExtendedBool();
        public ExtendedBool slidingData = new ExtendedBool();
        public ExtendedBool evadingData = new ExtendedBool();

        public ExtendedBool lightAttackInputData = new ExtendedBool();
        public ExtendedBool heavyAttackInputData = new ExtendedBool();
        public ExtendedBool jumpInputData = new ExtendedBool();
        public ExtendedBool evadeInputData = new ExtendedBool();
        public ExtendedBool walkInputData = new ExtendedBool();
        public ExtendedBool crouchInputData = new ExtendedBool();
        public ExtendedBool focusInputData = new ExtendedBool();
        public ExtendedBool shiftInputData = new ExtendedBool();

        public Vector3 currentFrameMoveInput = Vector3.zero;
        public Vector3 lastFrameMoveInput = Vector3.zero;
        public Quaternion currentFrameLookRotation = Quaternion.identity;
        public Quaternion lastFrameLookRotation = Quaternion.identity;

        public RaycastHit groundHit;
        public RaycastHit wallHit;





        

        protected virtual void EntityAwake(){;}
        protected virtual void EntityStart(){;}
        protected virtual void EntityEnable(){;}
        protected virtual void EntityDisable(){;}
        protected virtual void EntityDestroy(){;}
        protected virtual void EntityUpdate(){;}
        protected virtual void EntityFixedUpdate(){;}
        protected virtual void EntityCustomBehaviour(){;}


        private void OnEnable(){
            Global.entityManager.entityList.Add( this );
            data.changeCostume += LoadModel;
            EntityEnable();
            ReloadBones();
        }
        private void OnDisable(){
            Global.entityManager.entityList.Remove( this );
            data.changeCostume -= LoadModel;
            EntityDisable();
        }

        private void Awake(){
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

            _t = transform;
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();
            _physicsComponent = GetComponent<CustomPhysicsComponent>();
            _animator = GetComponent<Animator>();

            data = UnitData.GetDataByName<EntityData>( GetType().Name.Replace("Entity","") );
            LoadModel();
            gameObject.name = name;
            gameObject.layer = 6;
        }

        private void Update(){
            // Debug.DrawRay(transform.position, inertiaDirection * inertiaMultiplier, Color.red);
            // Debug.DrawRay(transform.position, absoluteForward, Color.blue);
            // Debug.DrawRay(transform.position, evadeDirection, Color.magenta);
            // Debug.DrawRay(transform.position, _rb.velocity, Color.green);

            jumpCooldown = Mathf.MoveTowards( jumpCooldown, 0f, Time.deltaTime );
            shiftCooldown = Mathf.MoveTowards( shiftCooldown, 0f, Time.deltaTime );
            evadeTimer = Mathf.MoveTowards( evadeTimer, 0f, Time.deltaTime );

            // if the Entity is not Player Controlled, then it uses its custom behaviour.

            if (!isPlayer && Player.current.entity != this){
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, 0.01f);
                EntityCustomBehaviour();
            }

            _colliderBounds = new Bounds (_t.position, Vector3.zero);
            foreach (Collider nextCollider in _colliders){
                _colliderBounds.Encapsulate (nextCollider.bounds);
            }

            evadingData.Update( evadeTimer > data.evadeCooldown );
            slidingData.Update( currentState.sliding );
            groundData.Update( GroundCheck(out groundHit) );
            wallData.Update( WallCheck(out wallHit) );
            
            EntityUpdate();

            if (_animator.runtimeAnimatorController != null){
                _animator.SetBool("OnGround", onGround);
                _animator.SetBool("Falling", fallVelocity <= -20f);
                _animator.SetBool("Idle", idle);
                _animator.SetInteger("State", currentState.id);
                _animator.SetFloat("WeaponType", (float)(currentWeapon?.weaponType ?? Weapon.WeaponType.lightSword) );
                _animator.SetFloat("SubState", subState);
                _animator.SetFloat("WalkSpeed", (float)walkSpeed);
                _animator.SetFloat("DotOfForward", Vector3.Dot(absoluteForward, rotationForward));
                _animator.SetFloat("ForwardRight", Vector3.Dot(absoluteForward, Vector3.Cross(-gravityDown, rotationForward)));
                currentState?.StateAnimation();
            }

        }

        private void FixedUpdate(){

            EntityFixedUpdate();
        }
        
        // --------------------------------------------------------------------------------------------------------------------------
        // Get Values
        // --------------------------------------------------------------------------------------------------------------------------
        
        private bool GroundCheck( out RaycastHit groundHitOut) {
            float minRadius = Mathf.Min(_colliderBounds.extents.x, Mathf.Min(_colliderBounds.extents.y, _colliderBounds.extents.z));
            float maxRadius = Mathf.Max(_colliderBounds.extents.x, Mathf.Max(_colliderBounds.extents.y, _colliderBounds.extents.z));
            Debug.DrawLine(_t.position, _t.position + gravityDown*maxRadius, Color.red);
            return Physics.SphereCast( _t.position, (minRadius - 0.05f), gravityDown, out groundHitOut, maxRadius + 0.1f, Global.GroundMask, QueryTriggerInteraction.Ignore );
            // return _rb.GroundCheck( out groundHit );
            // return _rb.SweepTest( gravityDown, out groundHit, 1f, QueryTriggerInteraction.Ignore );
        }

        private bool DirectionCheck( Vector3 direction, out RaycastHit checkHit ) {

            foreach (Collider col in _colliders){
                bool hasHitWall = col.ColliderCast( _t.position + _t.rotation * col.GetCenter(), direction, data.stepHeight, out RaycastHit tempHit );
                if (!hasHitWall) continue;

                checkHit = tempHit;
                return true;
            }
            checkHit = new RaycastHit();
            return false;
        }

        private bool WallCheck( out RaycastHit wallHitOut){
            for (int i = 0; i < 11; i++){
                float angle = (i%2 == 0 && i != 0) ? (i-1 * -30f) : i * 30f;
                Quaternion angleTurn = Quaternion.AngleAxis( angle, rotation * Vector3.down);
                bool hasHitWall = DirectionCheck( angleTurn * absoluteForward * 0.3f, out RaycastHit tempHit );
                if (hasHitWall){
                    wallHitOut = tempHit;
                    return true;
                }
            }
            wallHitOut = new RaycastHit();
            return false;
        }

        // --------------------------------------------------------------------------------------------------------------------------
        // Entity Actions
        // --------------------------------------------------------------------------------------------------------------------------

        public void EntityInput(Vector3 rawInput, Quaternion camRotation, SafeDictionary<string, bool> inputDictionary){

            lightAttackInputData.Update( inputDictionary["LightAttack"] );
            heavyAttackInputData.Update( inputDictionary["HeavyAttack"] );
            jumpInputData.Update( inputDictionary["Jump"] );
            evadeInputData.Update( inputDictionary["Evade"] );
            walkInputData.Update( inputDictionary["Walk"] );
            crouchInputData.Update( inputDictionary["Crouch"] );
            focusInputData.Update( inputDictionary["Focus"] );
            shiftInputData.Update( inputDictionary["Shift"] );

            lastFrameLookRotation = currentFrameLookRotation;
            currentFrameLookRotation = camRotation;
            
            lastFrameMoveInput = currentFrameMoveInput;
            currentFrameMoveInput = rawInput;

            currentState.HandleInput();


        }

        // --------------------------------------------------------------------------------------------------------------------------
        // Entity Async Instructions
        // --------------------------------------------------------------------------------------------------------------------------

        public async Task WalkTo(Vector3 pos, WalkSpeed speed = WalkSpeed.walk){
            walkingTo = true;
            while ((pos - transform.position).magnitude > 0.3f){
                Vector3 dir = Vector3.ProjectOnPlane( pos - transform.position, -gravityDown );
                if (dir.magnitude > 0.4f && ((pos - transform.position).magnitude - dir.magnitude) < 0.3f){
                    SetWalkSpeed(speed);
                    moveDirection = dir.normalized;
                }else{
                    moveDirection = Vector3.zero;
                    transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
                }
                await Task.Yield();
            }
            walkingTo = false;
            moveDirection = Vector3.zero;
        }

        public async Task TurnTo(Vector3 dir){
            Vector3 direction = Vector3.ProjectOnPlane( dir, -gravityDown ).normalized;
            while (Vector3.Dot(relativeForward.normalized, direction) < 0.99f){
                relativeForward = Vector3.Lerp(relativeForward, Quaternion.Inverse(rotation) * direction, 0.1f);
                evadeDirection = Vector3.Lerp(evadeDirection, absoluteForward, 0.1f);
                rotationForward = Vector3.Lerp(rotationForward, relativeForward, 0.7f).normalized;
                turningTo = true;
                await Task.Yield();
            }
            turningTo = false;
        }

        // --------------------------------------------------------------------------------------------------------------------------
        // Standard Entity Methods
        // --------------------------------------------------------------------------------------------------------------------------

        
        public void GroundedMove(Vector3 dir, bool canStep = false){

            if (dir.magnitude == 0f) return;

            Vector3 move = dir;

            move = Vector3.ProjectOnPlane(move, groundOrientation * -gravityDown);

            // if (canStep){
            //     // Needs a bit of Reworking.
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

            // bool walkCollision = _collider.ColliderCast( transform.position, move, data.stepHeight, out RaycastHit walkHit);
            bool walkCollision = DirectionCheck( move, out RaycastHit walkHit);

            // bool walkCollision = _rb.SweepTest( move, out RaycastHit walkHit, move.magnitude, QueryTriggerInteraction.Ignore);

            if ( walkCollision && walkHit.transform.gameObject.layer == 0) move = move.NullifyInDirection( -walkHit.normal );
            _t.position += move;
        }

        public void SetWalkSpeed(WalkSpeed newSpeed){
            if (walkSpeed != newSpeed){
                if ((int)walkSpeed < (int)newSpeed) StartWalkAnim();
                walkSpeed = newSpeed;
            }
        }

        public void SetRotation(Vector3 newUp) => rotation = Quaternion.FromToRotation(rotation * Vector3.up, newUp) * rotation;

        public void SetState(string stateName){
            currentState = Global.SafeDestroy(currentState);
            currentState = gameObject.AddComponent(Type.GetType($"SeleneGame.{stateName}State")) as State;
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

            // Assign Armature Bones as Limbs
            GetBonesFromData();

            _colliders = model.GetComponentsInChildren<Collider>();

            foreach ( Weapon weapon in weapons){
                weapon?.LoadModel();
            }

            // Log for the sake of it.
            Debug.Log($"{name} Model Loaded");
        }

        public void DestroyModel(){

            var children = new List<GameObject>();
            foreach (Transform child in _t) if (child.gameObject.name.Contains("Model")) children.Add(child.gameObject);
            children.ForEach(child => Global.SafeDestroy(child));

            model = Global.SafeDestroy(model);
        }

        public void SwitchWeapon(int weaponIndex){
            weaponIndex = Mathf.Clamp(weaponIndex, 0, 2);
            currentWeapon = weapons[weaponIndex];

            for ( int i = 0; i<weapons.Length; i++ ){
                if (weapons[i] == null) continue;
                if ( i == weaponIndex ){
                    weapons[i].Display();
                }else{
                    weapons[i].Hide(); 
                }
            }
            Debug.Log($"Switched Weapon to {currentWeapon.name}.");
        }

        public void SetWeapon(int weaponIndex, string weaponName){
            if (weapons == null || weapons.Length < weaponIndex + 1) {
                Weapon[] tempWeapons = weapons;
                weapons = new Weapon[weaponIndex + 1];
                for ( int i = 0; i < weapons.Length; i++ ){
                    if (i < tempWeapons.Length){
                        weapons[i] = tempWeapons[i];
                    }else{
                        weapons[i] = gameObject.AddComponent(Type.GetType("SeleneGame.UnarmedWeapon")) as Weapon;
                    }
                }
            }

            // If Weapon is already equipped in this slot, replace it.
            if (weapons[weaponIndex] is object){
                weapons[weaponIndex].DestroyModel();
                weapons[weaponIndex] = Global.SafeDestroy(weapons[weaponIndex]);
            }

            // Then, equip it in the desired slot.
            weapons[weaponIndex] = gameObject.AddComponent(Type.GetType($"SeleneGame.{weaponName}Weapon")) as Weapon;
            weapons[weaponIndex].enabled = true;

            if (currentWeapon == null) SwitchWeapon(weaponIndex);
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

        public void SetSize(Vector3 size){
            if ( _collider is CapsuleCollider ){
                CapsuleCollider cc = (CapsuleCollider) _collider;
                cc.radius = size.x;
                cc.height = size.y;
                cc.center = new Vector3(0f, (size.y-data.size.y)/2f, 0f);
            }else if ( _collider is BoxCollider ){
                BoxCollider bc = (BoxCollider) _collider;
                bc.size = new Vector3(size.x, size.y, size.z);
                bc.center = new Vector3(0f, (size.y-data.size.y)/2f, 0f);
            }
        }

    }
}