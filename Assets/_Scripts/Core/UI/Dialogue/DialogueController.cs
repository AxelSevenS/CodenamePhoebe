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


            if (Player.current == null) return;

            // End the Dialogue if the player is too far away
            if (dialogueObject != null && Vector3.SqrMagnitude(Player.current.entity.transform.position - dialogueObject.transform.position) > 100f)
                InterruptDialogue();

            if (
                Player.current.lightAttackInput.started || 
                Player.current.heavyAttackInput.started || 
                Player.current.jumpInput.started
            )
                SkipLine();

        }

    }
}