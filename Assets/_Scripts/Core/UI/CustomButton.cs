using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SeleneGame.Core.UI {

    public abstract class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler {

        public virtual void OnPointerClick(PointerEventData eventData) {
            OnSubmit(eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData) {
            Debug.Log($"Button {name} submitted");
        }
    }

}