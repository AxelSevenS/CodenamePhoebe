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

            int hashCode = GetHashCode(); 
            int usedFlags = ScribeFlags.GetFlag($"{hashCode}High");

            foreach (ConditionalDialogue dialogue in highPriorityDialogues) {
                int sourceFlag = 1 << highPriorityDialogues.IndexOf(dialogue);
                
                
                if ((usedFlags & sourceFlag) == 1) continue; // Check if DialogueSource has not been used yet (flag was not raised)
                if (!dialogue.Evaluate()) continue; // Skip if conditions are not met

                // If DialogueSource is valid, mark it as used (raise flag)
                ScribeFlags.SetFlag($"{hashCode}High", usedFlags |= sourceFlag);
                return dialogue.dialogueSource;
            }

            return null;
        }

        public DialogueSource GetLowPrioritySource() {

            int hashCode = GetHashCode(); 
            int usedFlags = ScribeFlags.GetFlag($"{hashCode}Low");

            System.Random r = new System.Random(); // Randomize the order of the DialogueSources, for replay variety
            foreach (DialogueSource source in lowPriorityDialogues.OrderBy(x => r.Next())) {
                int sourceFlag = 1 << lowPriorityDialogues.IndexOf(source);

                if ((usedFlags & sourceFlag) == 1) continue; // Check if DialogueSource has not been used yet (flag was not raised)
                
                // If DialogueSource is valid, mark it as used (raise flag)
                ScribeFlags.SetFlag($"{hashCode}Low", usedFlags |= sourceFlag);
                return source;
            }

            return null;
        }
    
        public override DialogueLine GetDialogue() {

            int hashCode = GetHashCode(); 

            // If all dialogues have been played, reset the dialogue Flags
            if (ScribeFlags.GetFlag($"{hashCode}High") >= (1 << highPriorityDialogues.Count) - 1)
                ScribeFlags.SetFlag($"{hashCode}High", 0);
                
            if (ScribeFlags.GetFlag($"{hashCode}Low") >= (1 << lowPriorityDialogues.Count) - 1)
                ScribeFlags.SetFlag($"{hashCode}Low", 0);
                

            DialogueLine candidate = GetHighPrioritySource()?.GetDialogue() ?? null;
            candidate ??= GetLowPrioritySource()?.GetDialogue();

            return candidate;
        }
    }
}
