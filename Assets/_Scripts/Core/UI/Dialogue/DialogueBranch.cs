using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Dialogue Branch", menuName = "Dialogue/Branch")]
    public class DialogueBranch : DialogueSource {
            
        public List<ConditionalDialogue> highPriorityDialogues;
        public List<DialogueSource> lowPriorityDialogues;

        public DialogueSource GetHighPrioritySource() {

            foreach (ConditionalDialogue dialogue in highPriorityDialogues) {

                // if (dialogue.dialogueSource.GetFlag(DialogueFlag.Spent)) 
                //     continue; // Skip if DialogueSource has been used already
                if (!dialogue.Evaluate()) 
                    continue; // Skip if conditions are not met

                // If DialogueSource is valid, mark it as used (raise flag)
                return dialogue.dialogueSource;

            }

            return null;
        }

        public DialogueSource GetLowPrioritySource() {

            int hashCode = GetHashCode(); 
            int usedFlags = ScribeFlags.GetFlag($"{hashCode}Low");

            System.Random r = new System.Random(); // Randomize the order of the DialogueSources, for replay variety
            foreach (DialogueSource source in lowPriorityDialogues.OrderBy(x => r.Next())) {

                if (source.GetFlag(DialogueFlag.Spent)) 
                    continue; // Skip if DialogueSource has been used already

                // If DialogueSource is valid, mark it as used (raise flag)
                return source;
                
            }

            return null;
        }
    
        public override DialogueLine GetDialogue() {
            
            // // if all high priority dialogue is spent, remove the flags
            // if (highPriorityDialogues.All(x => x.dialogueSource.GetFlag(DialogueFlag.Spent))) {
            //     foreach (ConditionalDialogue dialogue in highPriorityDialogues) {
            //         dialogue.dialogueSource.ClearFlag(DialogueFlag.Spent);
            //     }
            // }

            // if all low priority dialogue is spent, remove the flags
            if (lowPriorityDialogues.All(x => x.GetFlag(DialogueFlag.Spent))) {
                foreach (DialogueSource dialogue in lowPriorityDialogues) {
                    dialogue.ClearFlag(DialogueFlag.Spent);
                }
            }


            DialogueLine candidate = GetHighPrioritySource()?.GetDialogue() ?? null;
            candidate ??= GetLowPrioritySource()?.GetDialogue();

            return candidate;
        }
    }
}
