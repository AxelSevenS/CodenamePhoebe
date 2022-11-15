using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public class CharacterCostume : SpeakerCostume<CharacterCostume> {
        
        [SerializeField] private GameObject model;


        /* [ReadOnly] */ public GameObject modelInstance;
        /* [ReadOnly] */ public CostumeData costumeData;



        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, _entity.transform);
                costumeData = modelInstance.GetComponent<CostumeData>();
                _entity.animator.Rebind();
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }
        
    }
}