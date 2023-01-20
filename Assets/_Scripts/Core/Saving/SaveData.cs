using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class SaveData {
        
        public static readonly System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        public static System.TimeSpan timeSinceEpochStart => (System.DateTime.UtcNow - epochStart);

        public static System.DateTime loadedTime = System.DateTime.UtcNow;

        public Dictionary<string, int> flags = new Dictionary<string, int>();


        public uint saveTimeSeconds = 0;
        public uint playTimeSeconds = 0;

        


        public System.DateTime GetTimeOfLastSave(){
            return SaveData.epochStart.AddSeconds(saveTimeSeconds);
        }
        public System.TimeSpan GetTotalPlaytime(){
            return System.TimeSpan.FromSeconds(playTimeSeconds);
        }

        public virtual void Save() {
            
            // Saving the time of the last save
            saveTimeSeconds = (uint)timeSinceEpochStart.TotalSeconds;

            // Calculating play time of this session.
            uint sessionPlayTime = (uint)(System.DateTime.UtcNow - loadedTime).TotalSeconds;
            playTimeSeconds += sessionPlayTime;

            // Update the Time of loading this save file so "session playtime" gets reset and isn't added multiple times.
            loadedTime = System.DateTime.UtcNow;

            flags = ScribeFlags.flags;
        }

        public virtual void Load() {

            // Time Since last Played on this save
            uint timeSinceLastPlayed = (uint)(System.DateTime.UtcNow - GetTimeOfLastSave()).TotalSeconds;
            Debug.Log( $"Time since last played: {timeSinceLastPlayed} seconds." );
            Debug.Log( $"The date of the last save was {GetTimeOfLastSave()}." );
            Debug.Log( $"Total Playtime is {GetTotalPlaytime()}." );


            // Time of loading this save file is stored temporarily to calculate play time of this session.
            loadedTime = System.DateTime.UtcNow;
            
            ScribeFlags.flags = flags;
        }


    }

}
