// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GravityBikeData : EntityData {

//     public override void EntityAwake(){
//         LoadModel();
//         _entity.SetState("Vehicule");
        
//         _entity.SetWeapon(0, "Unarmed");
//         _entity.SetWeapon(1, "Unarmed");
//         _entity.SetWeapon(2, "Unarmed");
//         _entity.SwitchWeapon(0);
//     }

//     /* public override void Shift(){
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
//     } */

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
