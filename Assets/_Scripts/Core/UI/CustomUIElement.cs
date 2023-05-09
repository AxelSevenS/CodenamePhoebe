// using System.Collections;
// using System.Collections.Generic;

// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// namespace SeleneGame.Core.UI {

//     public abstract class CustomUIElement : Selectable, ISelectHandler, IDeselectHandler, IMoveHandler, IPointerEnterHandler, IPointerExitHandler {

//         [SerializeField] public CustomUIElement elementUp;
//         [SerializeField] public CustomUIElement elementDown;
//         [SerializeField] public CustomUIElement elementLeft;
//         [SerializeField] public CustomUIElement elementRight;

//         public override void OnMove(AxisEventData eventData) {
//             base.OnMove(eventData);
//             if (elementUp != null && eventData.moveDir == MoveDirection.Up) {
//                 EventSystem.current.SetSelectedGameObject(elementUp.gameObject);
//             } else if (elementDown != null && eventData.moveDir == MoveDirection.Down) {
//                 EventSystem.current.SetSelectedGameObject(elementDown.gameObject);
//             } else if (elementLeft != null && eventData.moveDir == MoveDirection.Left) {
//                 EventSystem.current.SetSelectedGameObject(elementLeft.gameObject);
//             } else if (elementRight != null && eventData.moveDir == MoveDirection.Right) {
//                 EventSystem.current.SetSelectedGameObject(elementRight.gameObject);
//             }
//         }

//         public override void OnPointerEnter(PointerEventData eventData) {
//             EventSystem.current.SetSelectedGameObject(gameObject);
//         }
//         public override void OnPointerExit(PointerEventData eventData) {
//             EventSystem.current.SetSelectedGameObject(null);
//         }

//     }

// }