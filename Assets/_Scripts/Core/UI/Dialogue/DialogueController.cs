using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SevenGame.Utility;
using SeleneGame.Core;

namespace SeleneGame.Core.UI {

    public sealed class DialogueController : DialogueReader<DialogueController> {


        [SerializeField] private Image dialogueIndicator;


        protected override void Update(){
            base.Update();

            if (!Enabled) return;

            dialogueIndicator.enabled = !isTyping;


            if (PlayerEntityController.current == null) return;

            // End the Dialogue if the player is too far away
            if (dialogueObject != null && Vector3.SqrMagnitude(PlayerEntityController.current.entity.transform.position - dialogueObject.transform.position) > 100f)
                InterruptDialogue();

            if (
                PlayerEntityController.current.lightAttackInput.started || 
                PlayerEntityController.current.heavyAttackInput.started || 
                PlayerEntityController.current.jumpInput.started
            )
                SkipLine();

        }

    }
}