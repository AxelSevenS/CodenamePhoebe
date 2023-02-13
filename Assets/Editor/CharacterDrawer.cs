using System;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {

    [CustomPropertyDrawer(typeof(Character), true)]
    public class CharacterDrawer : CostumableDrawer<Character, CharacterCostume, CharacterModel> {

        public override void SetValue(SerializedProperty property, int typeIndex) {

            
            Entity entityRef =
                (targetCostumable as Character)?.entity ??
                (Entity)GetParent(property) ?? 
                ((MonoBehaviour)property.serializedObject.targetObject).GetComponent<Entity>();


            ((Character)property.managedReferenceValue)?.Dispose();

            if (typeIndex <= -1) {
                property.managedReferenceValue = null;
                return;
            }

            Type type = Character._types[typeIndex];
            entityRef.SetCharacter( type );
        }
    }
}
