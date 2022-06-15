using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.States;

namespace SeleneGame.Entities {

    public class GravityShifterEntity : ArmedEntity {

        // public bool masked => state.masked;
        
        public bool focusing;
        public float shiftCooldown;

        public List<Grabbable> grabbedObjects = new List<Grabbable>();
        private Vector3[] grabbedObjectPositions = new Vector3[4]{
            new Vector3(3.5f, 1.5f, 3f), 
            new Vector3(-3.5f, 1.5f, 3f), 
            new Vector3(2.5f, 2.5f, 3f), 
            new Vector3(-2.5f, 2.5f, 3f)
        };

        public override float GravityMultiplier() => data.weight * (currentWeapon?.weightModifier ?? 1f);
        public override float JumpMultiplier() => 2 - (GravityMultiplier() / 15f);

        private SpeedlinesEffect speedlines;


        public float shiftEnergy = 0f;

        public bool isMasked() {
            if (state is ShiftingState || state is SwimmingState)
                return true;

            else if (state is WalkingState walkingState) {
                return Vector3.Dot ( gravityDown, Vector3.down ) < 0.95f /* || currentWeapon.shifting */;
            }

            return false;
        }

        protected override void EntityDestroy(){
            base.EntityDestroy();
            Global.SafeDestroy(speedlines);
        }

        protected override void EntityAwake(){
            base.EntityAwake();

            weapons.Set(1, Weapon.GetWeaponTypeByName("Hypnos"));
            weapons.Set(2, Weapon.GetWeaponTypeByName("Eris"));

            GameObject speedlinesObject = GameObject.Instantiate(Resources.Load("Prefabs/Effects/Speedlines"), Global.effects.transform) as GameObject;
            speedlines = speedlinesObject.GetComponent<SpeedlinesEffect>();
            speedlines.SetFollowedObject(gameObject);
        }

        protected override void EntityUpdate(){
            base.EntityUpdate();

            shiftCooldown = Mathf.MoveTowards( shiftCooldown, 0f, Global.timeDelta );
            
            if (shiftInput.stopped && shiftInput.trueTimer < Player.current.holdDuration){
                
                if (state is WalkingState walking && shiftCooldown == 0f){
                    shiftCooldown = 0.3f;
                    if (onGround) _rb.velocity += -gravityDown*3f;
                    
                    SetState(new ShiftingState());

                    //Shift effects
                        // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
                        // Destroy(shiftParticle, 2f);
                }else if (state is ShiftingState shifting){
                    shifting.StopShifting(Vector3.down);
                }
            }
            
            if (animator.runtimeAnimatorController != null){
                animator.SetFloat("WeaponType", (float)(currentWeapon.data.weaponType) );
            }
        }

        protected override void EntityFixedUpdate() {
            base.EntityFixedUpdate();
            
            for (int i = 0; i < Mathf.Min(4, grabbedObjects.Count); i++){
                Grabbable grabbed = grabbedObjects[i];
                if (grabbed == null) return;

                var grabbedMono = grabbedObjects[i] as MonoBehaviour;

                grabbedMono.transform.position = Vector3.Lerp(grabbedMono.transform.position, transform.position + lookRotation * grabbedObjectPositions[i], 10f* Global.timeDelta);
            }

            if (shiftCooldown > 0f){
                shiftCooldown -= Global.timeDelta;
            }
        }

        public override bool CanWaterHover() {
            return currentWeapon.weightModifier < 0.8f && moveInput.zeroTimer < 0.6f;
        }
        public override bool CanSink() {
            return currentWeapon.weightModifier > 1.3f;
        }

        protected void Shift(float timer){
            if (timer >= Player.current.holdDuration) return;
            if (state is WalkingState walking && shiftCooldown == 0f){
                shiftCooldown = 0.3f;
                if (onGround) _rb.velocity += -gravityDown*3f;
                
                SetState(new ShiftingState());

                //Shift effects
                    // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
                    // Destroy(shiftParticle, 2f);
            }else if (state is ShiftingState shifting){
                shifting.StopShifting(Vector3.down);
            }

        }

    }
}




