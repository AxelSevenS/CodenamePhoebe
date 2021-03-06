using System;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.States;
using SevenGame.Utility;

namespace SeleneGame.Entities {

    public abstract class GravityShifterEntity : ArmedEntity {
        
        protected sealed override void ResetWeapons() {
            _weapons = new WeaponInventory(this, 3);
        }
        
        public override State defaultState => new WalkingState();

        public override float GravityMultiplier() => data.weight * (weapons.current?.weightModifier ?? 1f);
        public override float JumpMultiplier() => 2 - (GravityMultiplier() / 15f);
        
        public override bool CanWaterHover() {
            return weapons.current.weightModifier < 0.8f && controller.moveInput.zeroTimer < 0.6f;
        }
        public override bool CanSink() {
            return weapons.current.weightModifier > 1.3f;
        }     
        
        public bool focusing;
        public float shiftCooldown;

        public List<ValueTuple<Vector3, Grabbable>> grabbedObjects = new List<ValueTuple<Vector3, Grabbable>>();
        private Vector3[] grabbedObjectPositions = new Vector3[4]{
            new Vector3(3.5f, 1.5f, 3f), 
            new Vector3(-3.5f, 1.5f, 3f), 
            new Vector3(2.5f, 2.5f, 3f), 
            new Vector3(-2.5f, 2.5f, 3f)
        };

        private SpeedlinesEffect speedlines;


        public float shiftEnergy = 0f;

        public bool isMasked() {
            if (state is ShiftingState || state is SwimmingState)
                return true;

            else if (state is WalkingState walkingState) {
                return Vector3.Dot ( gravityDown, Vector3.down ) < 0.95f /* || weapons.current.shifting */;
            }

            return false;
        }

        protected override void Awake() {
            base.Awake();
            
            if (speedlines == null) {
                GameObject speedlinesObject = GameObject.Instantiate(Resources.Load("Prefabs/Effects/Speedlines"), EffectManager.current.transform) as GameObject;
                speedlines = speedlinesObject.GetComponent<SpeedlinesEffect>();
                speedlines.SetFollowedObject(gameObject);
            }
        }

        protected override void Reset(){
            base.Reset();
        }

        protected override void Update(){
            base.Update();

            shiftCooldown = Mathf.MoveTowards( shiftCooldown, 0f, GameUtility.timeDelta );
            
            if (controller.shiftInput.tapped){
                
                ToggleShift();
            }
        }

        protected override void FixedUpdate() {
            base.FixedUpdate();
            
            int i = 0;
            foreach (ValueTuple<Vector3, Grabbable> grabbed in grabbedObjects) {
                Vector3 randomSpin = grabbed.Item1;
                Grabbable grabbable = grabbed.Item2;
                Transform grabbedTransform = grabbable.transform;

                grabbable.rb.AddTorque(randomSpin.x, randomSpin.y, randomSpin.z, ForceMode.VelocityChange);

                grabbedTransform.position = Vector3.Lerp(grabbedTransform.position, transform.position + rotation * grabbedObjectPositions[i], 10f* GameUtility.timeDelta);

                i++;
            }

            if (shiftCooldown > 0f){
                shiftCooldown -= GameUtility.timeDelta;
            }
        }

        public override void LoadModel() {
            base.LoadModel();
        }

        public override void DestroyModel(){
            base.DestroyModel();
            
            GameUtility.SafeDestroy(speedlines);
        }   
        
        public override void Grab(Grabbable grabbable){
            if (grabbedObjects.Count >= 4) return;
            
            grabbedObjects.Add( new ValueTuple<Vector3, Grabbable>( UnityEngine.Random.insideUnitSphere.normalized * 0.3f, grabbable) );
            grabbable.grabbed = true;
        }
        public override void Throw(Grabbable grabbable){

            foreach (ValueTuple<Vector3, Grabbable> grabbed in grabbedObjects) {
                if (grabbed.Item2 == grabbable) {
                    grabbedObjects.Remove(grabbed);
                    grabbable.grabbed = false;
                    break;
                }
            }

            /// TODO: Get the direction to Target
            Vector3 targetDirection = absoluteForward;

            grabbable.rb.AddForce(targetDirection * 30f, ForceMode.Impulse);

            var impulseParticle = Instantiate(Global.LoadParticle("ShiftImpulseParticles"), transform.position + targetDirection*2f, Quaternion.LookRotation(targetDirection, rotation * Vector3.up));
            Destroy(impulseParticle, 1.2f);
        }

        
        protected void ToggleShift(){
            if (shiftCooldown > 0f) return;

            if (state is ShiftingState) 
                StopShifting(Vector3.down);
            else {
                Shift();
            }
        }

        public void StopShifting(Vector3 newDown){
            gravityDown = newDown;
            SetState( defaultState );
        }

        public void Shift(){
            shiftCooldown = 0.3f;
            if (onGround) rigidbody.velocity += -gravityDown*3f;
            
            SetState( new ShiftingState() );
        }

    }
}




