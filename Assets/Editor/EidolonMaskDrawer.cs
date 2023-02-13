using UnityEditor;

using SeleneGame.Core;
using UnityEngine;
using System;

namespace SeleneGame.Content {

    [CustomPropertyDrawer(typeof(EidolonMask), true)]
    public class EidolonMaskDrawer : CostumableDrawer<EidolonMask, EidolonMaskCostume, EidolonMaskModel> {


        public override void SetValue(SerializedProperty property, int typeIndex) {

            ((EidolonMask)property.managedReferenceValue)?.Dispose();


            if (typeIndex <= -1) {
                property.managedReferenceValue = null;
                return;
            }

            
            MaskedEntity entityRef =(targetCostumable as EidolonMask)?.maskedEntity;


            if (entityRef == null) {

                Debug.LogWarning($"EidolonMask {property.managedReferenceValue} has nulled-out Entity Reference. Not good.");

                MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;
                entityRef ??= targetObject.GetComponent<MaskedEntity>();

            }

            Type type = EidolonMask._types[typeIndex];
            if (entityRef == null) {
                Debug.LogError("No MaskedEntity found for EidolonMask");
                
                // property.managedReferenceValue = EidolonMask.CreateInstance(type, entityRef);
            } else {

                entityRef.SetMask( type );
            }

        }
    }
}
