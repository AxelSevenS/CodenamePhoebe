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

        
        [SerializeField] [ReadOnly] private Entity attachedEntity;

        [ReadOnly] public GameObject modelInstance;
        [ReadOnly] public CostumeData costumeData;



        public void Initialize(Entity attachedEntity) {
            if (this.attachedEntity != null)
                throw new InvalidOperationException("Character Costume already initialized");

            this.attachedEntity = attachedEntity;
        }

        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, attachedEntity.transform);
                costumeData = modelInstance.GetComponent<CostumeData>();
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }
        
    }
}