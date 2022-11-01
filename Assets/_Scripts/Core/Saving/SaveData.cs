namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class SaveData {
        
        public static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        public static System.DateTime loadedTime = System.DateTime.UtcNow;
        public static System.TimeSpan timeSinceEpochStart => (System.DateTime.UtcNow - epochStart);

        public abstract void Save();

        public abstract void Load();


    }

}
