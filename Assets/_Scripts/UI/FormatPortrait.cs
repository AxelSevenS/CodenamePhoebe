using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeleneGame.UI {

    [RequireComponent(typeof(AspectRatioFitter))]
    public class FormatPortrait : MonoBehaviour{
        
        private AspectRatioFitter ARF;
        private Image image;
        private RectTransform rectTransform;
        private RectTransform parentTransform;
        
        void OnEnable(){
            ARF = GetComponent<AspectRatioFitter>();
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            parentTransform = gameObject.transform.parent.GetComponent<RectTransform>();
        }

        void LateUpdate(){

            if (image?.sprite == null) return;

            float width = image.sprite.rect.width;
            float height = image.sprite.rect.height;
            float aspectRatio = width/height;
            
            float maxwidth = parentTransform.sizeDelta.x;
            float maxheight = parentTransform.sizeDelta.y;
            
            if (height > width){ 
                ARF.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
                rectTransform.sizeDelta = new Vector2(maxwidth, rectTransform.sizeDelta.y);
            }else if (height < width){
                ARF.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, maxheight);
            }else{
                ARF.aspectMode = AspectRatioFitter.AspectMode.None;
                aspectRatio = 1f;
            }

            ARF.aspectRatio = aspectRatio;
            
        }
    }
}