using System;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Character), true)]
    public class CharacterDrawer : CostumableDrawer<Character, CharacterCostume, CharacterModel> {

        public override void SetValue(SerializedProperty property, int typeIndex) {

            ((Character)property.managedReferenceValue)?.Dispose();


            if (typeIndex <= -1) {
                property.managedReferenceValue = null;
                return;
            }

            
            Entity entityRef = (targetCostumable as Character)?.entity;

            if (entityRef == null) {

                Debug.LogWarning($"Character {property.managedReferenceValue} has nulled-out Entity Reference. Not good.");

                MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;
                entityRef ??= targetObject.GetComponent<Entity>();

            }

            Type type = Character._types[typeIndex];
            if (entityRef == null) {
                Debug.LogError($"No Entity found for Character {property.managedReferenceValue}");

                // property.managedReferenceValue = Character.CreateInstance(type, entityRef);
            } else {

                entityRef.SetCharacter( type );
            }

        }
    }
}
