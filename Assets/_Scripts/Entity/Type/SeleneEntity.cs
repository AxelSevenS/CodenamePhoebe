using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class SeleneEntity : Entity {

        private Vector3[] manipulatedDirections = new Vector3[4]{
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
            for (int i = 0; i < Mathf.Min(4, manipulatedObject.Count); i++){
                IManipulable manipulated = manipulatedObject[i];
                if (manipulated == null) return;

                var manipulatedMono = manipulatedObject[i] as MonoBehaviour;

                manipulatedMono.transform.position = Vector3.Lerp(manipulatedMono.transform.position, transform.position + currentFrameLookRotation * manipulatedDirections[i], 10f* Time.deltaTime);
                manipulated.Grabbed(this);
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