
namespace SeleneGame.Core {

    [System.Serializable]
    public sealed class DialogueEvent : GameEvent {
        
        public override bool DisplayEventType(System.Enum enumValue) {
            switch ((EventType)enumValue) {
                default:
                    return false;
                case EventType.StartDialogue:
                case EventType.StartAlert:
                    return true;
            }
        }

    }
}
