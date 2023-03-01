using UnityEditor;

using SeleneGame.Core;
using UnityEngine;
using System;

namespace SeleneGame.Content {

    [CustomPropertyDrawer(typeof(EidolonMask), true)]
    public class EidolonMaskDrawer : CostumableDrawer<EidolonMask, EidolonMaskData, EidolonMaskCostume, EidolonMaskModel> {


        public override void SetValue(SerializedProperty property, EidolonMaskData data) {

            ((EidolonMask)property.managedReferenceValue)?.Dispose();

            
            MaskedEntity entityRef = (targetCostumable as EidolonMask)?.maskedEntity;
            if (entityRef == null) {

                if (targetCostumable != null)
                    Debug.LogWarning($"EidolonMask {targetCostumable} has nulled-out Entity Reference. Not good.");

                MonoBehaviour targetObject = (MonoBehaviour)property.serializedObject.targetObject;
                entityRef ??= targetObject.GetComponent<MaskedEntity>();

            }

            // Type type = EidolonMask._types[typeIndex];
            if (entityRef == null) {
                Debug.LogError($"No MaskedEntity found for EidolonMask {targetCostumable}");
                
                property.managedReferenceValue = data.GetMask( entityRef );
            } else {

                entityRef.SetMask( data );
            }

        }
    }
}
