using System;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Character), true)]
    public class CharacterDrawer : CostumableDrawer<Character, CharacterData, CharacterCostume, CharacterModel> {

        protected override bool nullSensitive => true;

        

        public override void SetValue(SerializedProperty property, CharacterData data) {

            ((Character)property.managedReferenceValue)?.Dispose();

            
            Entity entityRef = (targetCostumable as Character)?.entity;
            if (entityRef == null) {

                if (targetCostumable != null)
                    Debug.LogWarning($"Character {targetCostumable} has nulled-out Entity Reference. Not good.");

                MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;
                entityRef ??= targetObject.GetComponent<Entity>();

            }
            
            if (entityRef == null) {
                Debug.LogError($"No Entity found for Character {targetCostumable}");

                property.managedReferenceValue = data.GetCharacter( entityRef );
            } else {

                entityRef.SetCharacter( data );
            }

        }
    }
}
