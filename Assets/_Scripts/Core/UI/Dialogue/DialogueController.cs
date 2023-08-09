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


            if (Player.Current == null) return;

            // End the Dialogue if the player is too far away
            if (dialogueObject != null && Vector3.SqrMagnitude(Player.Current.Entity.transform.position - dialogueObject.transform.position) > 100f)
                InterruptDialogue();

            if (
                Player.Current.lightAttackInput.started || 
                Player.Current.heavyAttackInput.started || 
                Player.Current.jumpInput.started
            )
                SkipLine();

        }

    }
}