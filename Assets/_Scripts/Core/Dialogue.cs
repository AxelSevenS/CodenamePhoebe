using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {


    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject {

        public Line[] lines;

        public static Dialogue GetDialogueByName(string fileName) => Resources.Load<Dialogue>($"Dialogue/{fileName}");

        
        [System.Serializable]
        public class Line {
            
            public CharacterCostume.Emotion emotion;

            [SerializeField] private bool usePlayerEntity;
            [SerializeField] private Entity _entity;

            [TextArea] public string text;

            public List<InvokableEvent> dialogueEvents;



            public Entity entity => usePlayerEntity ? PlayerEntityController.current.entity : _entity;

        }
    }
}