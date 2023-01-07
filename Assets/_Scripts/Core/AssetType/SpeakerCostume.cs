using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public abstract class SpeakerCostume<T> : Costume<T> where T : SpeakerCostume<T> {

        public Sprite neutralPortrait;
        public Sprite determinedPortrait;
        public Sprite hesitantPortrait;
        public Sprite shockedPortrait;
        public Sprite disgustedPortrait;
        public Sprite sadPortrait;
        public Sprite happyPortrait;


        
        public Sprite GetPortrait(Emotion emotion) {
            switch (emotion) {
                default: return neutralPortrait;
                case Emotion.determined: return determinedPortrait;
                case Emotion.hesitant: return hesitantPortrait;
                case Emotion.shocked: return shockedPortrait;
                case Emotion.disgusted: return disgustedPortrait;
                case Emotion.sad: return sadPortrait;
                case Emotion.happy: return happyPortrait;
            }
        }



        public enum Emotion {neutral, determined, hesitant, shocked, disgusted, sad, happy};

    }
}
