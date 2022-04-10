// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class SeleneData : EntityData {

//     private Vector3[] manipulatedDirections = new Vector3[4]{new Vector3(3.5f, 1.5f, 3f), new Vector3(-3.5f, 1.5f, 3f), new Vector3(2.5f, 2.5f, 3f), new Vector3(-2.5f, 2.5f, 3f)};

//     public override void EntityAwake(){
//         LoadModel();
//         _entity.SetState("Walking");
        
//         _entity.SetWeapon(0, "Hypnos");
//         _entity.SetWeapon(1, "Unarmed");
//         _entity.SetWeapon(2, "Eris");
//         _entity.SwitchWeapon(1);
//     }

//     public override void EntityUpdate(){

//     }

//     public override void EntityFixedUpdate(){
//         for (int i = 0; i < Mathf.Min(4, _entity.manipulatedObject.Count); i++){
//             var manipulated = _entity.manipulatedObject[i];
//             if (manipulated == null) return;

//             var manipulatedMono = _entity.manipulatedObject[i] as MonoBehaviour;

//             manipulatedMono.transform.position = Vector3.Lerp(manipulatedMono.transform.position, _entity.transform.position + _entity.currentFrameLookRotation * manipulatedDirections[i], 10f* Time.deltaTime);
//             manipulated.Grabbed(_entity);
//         }
//     }

//     public override void Shift(){
//         if( _entity.shiftCooldown == 0f){
//             if (_entity.currentState.name == "Walking"){
//                 _entity.shiftCooldown = 0.3f;
//                 if(_entity.onGround) _entity.rigidbody.velocity += -_entity.gravityDown*3f;
                
//                 _entity.SetState("Shifting");

//                 //Shift effects
//                 // var shiftParticle = Instantiate(Global.LoadParticle("ShiftParticles"), transform.position, Quaternion.FromToRotation(Vector3.up, transform.up));
//                 // Destroy(shiftParticle, 2f);

//             }else if (_entity.currentState.name == "Shifting"){
//                 StopShifting(Vector3.down);
//             }
//         }
//     }

//     public override void StopShifting(Vector3 newDown){
//         _entity.gravityDown = newDown;
//         _entity.SetState("Walking");
//     }

//     public override void Power(int level){

//     }

    
//     // public override void QuickFocus(){;}

//     // public override void Manipulate(ManipulatableAbstract obj, int action){
//     //     // switch (action){
//     //     //     case 0:
//     //     //         obj.Click();
//     //     //         break;
//     //     //     case 1:
//     //     //         obj.Hold();
//     //     //         break;
//     //     // }
//     // }
// }
