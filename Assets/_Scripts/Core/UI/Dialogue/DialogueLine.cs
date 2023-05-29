using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Localization;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Dialogue Line", menuName = "Dialogue/Line")]
    public class DialogueLine : DialogueSource {


        [SerializeField] private string characterId;
        public CharacterCostume.Emotion emotion;

        public LocalizedString localizedText;

        public DialogueSource nextLine;

        public List<GameEvent> gameEvents = new List<GameEvent>();



        public Entity entity => /* usePlayerEntity ?  */Player.current.entity/*  : _entity */;


        
        public override DialogueLine GetDialogue() {
            SetFlag(DialogueFlag.Spent);
            ClearFlag(DialogueFlag.Interrupted);
            return this;
        }
    }


}
