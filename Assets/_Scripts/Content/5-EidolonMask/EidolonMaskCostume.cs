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


        [ReadOnly] public GameObject modelInstance;

        [ReadOnly] public CostumeData costumeData;
        [ReadOnly] public Animator animator;



        public override void LoadModel() {
            if (model != null) {
                modelInstance = Instantiate(model, _entity.transform.parent);
                costumeData = modelInstance.GetComponent<CostumeData>();
                animator = modelInstance.GetComponent<Animator>();
            }
        }

        public override void UnloadModel() {
            modelInstance = GameUtility.SafeDestroy(modelInstance);
        }
    }

}