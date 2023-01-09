using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Dialogue Line", menuName = "Dialogue/Line")]
    public class DialogueLine : ScriptableObject {



        [SerializeField] private string characterId;
        public CharacterCostume.Emotion emotion;

        [TextArea] public string text;

        public List<GameEvent> GameEvents;
        public List<GameEvent> interruptionEvents;

        public DialogueLine nextLine;


        public Entity entity => /* usePlayerEntity ?  */PlayerEntityController.current.entity/*  : _entity */;



        
    }


}