using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public class CharacterCostume : AddressableAsset<CharacterCostume> {


        const string defaultPath = "Costume/SeleneBase";


        [Tooltip("The Portrait of the Character Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Character Costume, used in menus.")]
        public string displayName = "Default Costume Name";

        [Tooltip("The description of the Character Costume, only appears when it is not the Base Costume of the Selected Character.")]
        [TextArea] 
        public string description = "Default Costume Description";


        public GameObject model;
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