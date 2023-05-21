// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace SeleneGame.Core.UI {

//     public abstract class UIBaseMenu<T> : UIMenu<T> where T : UIMenu<T> {


//         public override void Enable() {
//             UIController.DisableModalTree();
//             UIController.modalLeaf = this;

//             base.Enable();
//         }

//         public override void Disable() {

//             if ( Enabled ) {
//                 UIController.DisableModalTree();
//             }

//             base.Disable();
//         }
        
//     }
// }
