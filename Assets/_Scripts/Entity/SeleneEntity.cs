using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.States;

namespace SeleneGame.Entities {

    public class SeleneEntity : Entity {

        private Vector3[] grabbedPositions = new Vector3[4]{
            new Vector3(3.5f, 1.5f, 3f), 
            new Vector3(-3.5f, 1.5f, 3f), 
            new Vector3(2.5f, 2.5f, 3f), 
            new Vector3(-2.5f, 2.5f, 3f)
        };

        private SpeedlinesEffect speedlines;

        protected override void EntityEnable(){
        }
        protected override void EntityDisable(){
        }

        protected override void EntityDestroy(){
            Global.SafeDestroy(speedlines);
        }

        protected override void EntityAwake(){

            SetState("Walking");
            weapons.Set(1, Weapon.GetWeaponTypeByName("Hypnos"));
            weapons.Set(2, Weapon.GetWeaponTypeByName("Eris"));

            GameObject speedlinesObject = GameObject.Instantiate(Resources.Load("Prefabs/Effects/Speedlines"), Global.effects.transform) as GameObject;
            speedlines = speedlinesObject.GetComponent<SpeedlinesEffect>();
            speedlines.SetFollowedObject(gameObject);
        }

        protected override void EntityUpdate(){
            if (shiftInputData.stopped && shiftInputData.trueTimer < Player.current.holdDuration){
                
                if (state is WalkingState walking && shiftCooldown == 0f){
                    shiftCooldown = 0.3f;
                    if (onGround) _rb.velocity += -gravityDown*3f;
                    
                    SetState("Shifting");

                    //Shift effects
                        // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
                        // Destroy(shiftParticle, 2f);
                }else if (state is ShiftingState shifting){
                    shifting.StopShifting(Vector3.down);
                }
            }
        }

        protected override void EntityFixedUpdate(){
            for (int i = 0; i < Mathf.Min(4, grabbedObjects.Count); i++){
                Grabbable grabbed = grabbedObjects[i];
                if (grabbed == null) return;

                var grabbedMono = grabbedObjects[i] as MonoBehaviour;

                grabbedMono.transform.position = Vector3.Lerp(grabbedMono.transform.position, transform.position + lookRotationData.currentValue * grabbedPositions[i], 10f* Global.timeDelta);
            }
        }

        private void Shift(float timer){
            if (timer >= Player.current.holdDuration) return;
            if (state is WalkingState walking && shiftCooldown == 0f){
                shiftCooldown = 0.3f;
                if (onGround) _rb.velocity += -gravityDown*3f;
                
                SetState("Shifting");

                //Shift effects
                    // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
                    // Destroy(shiftParticle, 2f);
            }else if (state is ShiftingState shifting){
                shifting.StopShifting(Vector3.down);
            }

        }
    }
}