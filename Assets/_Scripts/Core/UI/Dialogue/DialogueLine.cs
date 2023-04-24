using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Dialogue Line", menuName = "Dialogue/Line")]
    public class DialogueLine : DialogueSource {



        [SerializeField] private string characterId;
        public CharacterCostume.Emotion emotion;

        [TextArea] public string text;

        public List<GameEvent> gameEvents;
        public List<GameEvent> interruptionEvents;

        public DialogueSource nextLine;



        public Entity entity => /* usePlayerEntity ?  */Player.current.entity/*  : _entity */;


        
        public override DialogueLine GetDialogue() {
            return this;
        }
    }


}
