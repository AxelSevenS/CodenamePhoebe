using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.UI {

    [System.Serializable]
    public class DialogueLine{
        
        public enum Emotion {neutral, determined, hesitant, shocked, disgusted, sad, happy};
        public Emotion emotion;

        [SerializeField] private bool usePlayerEntity;
        [SerializeField] private Entity _entity;
        public Entity entity => usePlayerEntity ? Player.current.entity : _entity;

        [TextArea] public string text;

        public List<InvokableEvent> dialogueEvents;
    }

    [CreateAssetMenu(fileName = "new Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject {

        public DialogueLine[] lines;

        public static Dialogue GetDialogueByName(string fileName) => Resources.Load<Dialogue>($"Dialogue/{fileName}");
    }
}