using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using System;

namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "new Mask Costume", menuName = "Costume/Mask")]
    public class EidolonMaskCostume : SpeakerCostume<EidolonMaskCostume> {

        [SerializeField] private GameObject model;


        [SerializeField] private Entity attachedEntity;

        [ReadOnly] public GameObject modelInstance;

        [ReadOnly] public CostumeData costumeData;
        [ReadOnly] public Animator animator;



        public void Initialize(MaskedEntity attachedEntity) {
            if (this.attachedEntity != null)
                throw new InvalidOperationException("Mask Costume already initialized");
                
            this.attachedEntity = attachedEntity;
        }

        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, attachedEntity.transform.parent);
                costumeData = modelInstance.GetComponent<CostumeData>();
                animator = modelInstance.GetComponent<Animator>();
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }
    }

}