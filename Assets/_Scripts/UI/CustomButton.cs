using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SeleneGame.UI {

    public abstract class CustomButton : CustomUIElement, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
        
        [SerializeField] private Image background;

        [SerializeField] protected Sprite bgUnselected;
        [SerializeField] protected Sprite bgSelected;
        [SerializeField] protected Sprite bgClicked;


        protected void SetBackground( Sprite sprite ) {
            background.sprite = sprite;
        }

        public override void OnSelect(BaseEventData eventData) {
            SetBackground( bgSelected );
        }

        public override void OnDeselect(BaseEventData eventData) {
            SetBackground( bgUnselected );
        }

        public virtual void OnPointerDown(PointerEventData eventData) {
            SetBackground( bgClicked );
        }

        public virtual void OnPointerUp(PointerEventData eventData) {
            SetBackground( bgSelected );
        }

        public virtual void OnPointerClick(PointerEventData eventData) {

        }

    }

}