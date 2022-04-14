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
            shiftInputData.stopAction += Shift;
        }
        protected override void EntityDisable(){
            shiftInputData.stopAction -= Shift;
        }

        protected override void EntityDestroy(){
            Global.SafeDestroy(speedlines);
        }

        protected override void EntityAwake(){

            SetState("Walking");
            SetWeapon(0, "Hypnos");
            SetWeapon(2, "Eris");

            GameObject speedlinesObject = GameObject.Instantiate(Resources.Load("Prefabs/Effects/Speedlines"), Global.effects.transform) as GameObject;
            speedlines = speedlinesObject.GetComponent<SpeedlinesEffect>();
            speedlines.SetFollowedObject(gameObject);
        }

        protected override void EntityUpdate(){
        }

        protected override void EntityFixedUpdate(){
            for (int i = 0; i < Mathf.Min(4, grabbedObjects.Count); i++){
                Grabbable grabbed = grabbedObjects[i];
                if (grabbed == null) return;

                var grabbedMono = grabbedObjects[i] as MonoBehaviour;

                grabbedMono.transform.position = Vector3.Lerp(grabbedMono.transform.position, transform.position + lookRotationData.currentValue * grabbedPositions[i], 10f* Time.deltaTime);
            }
        }

        private void Shift(float timer){
            if (timer >= Player.current.holdDuration) return;
            if (currentState is WalkingState){
                if ( shiftCooldown == 0f){
                    shiftCooldown = 0.3f;
                    if (onGround) _rb.velocity += -gravityDown*3f;
                    
                    SetState("Shifting");

                    //Shift effects
                        // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
                        // Destroy(shiftParticle, 2f);
                }
            }else if (currentState is ShiftingState){
                ShiftingState shiftingState = currentState as ShiftingState;
                shiftingState.StopShifting(Vector3.down);
            }

        }
    }
}