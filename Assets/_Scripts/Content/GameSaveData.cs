using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.Content {

    [System.Serializable]
    public class GameSaveData : SaveData {

        public EntitySaveData playerData = new EntitySaveData();

        // Game Progress Save Data


        // General Save Data

        public ControlSaveData controls = new ControlSaveData();
        public uint timeOfLastSave = 0;
        public uint totalTimePlayed = 0;


        public System.DateTime GetTimeOfLastSave(){
            return SaveData.epochStart.AddSeconds(timeOfLastSave);
        }
        public System.TimeSpan GetTotalPlaytime(){
            return System.TimeSpan.FromSeconds(totalTimePlayed);
        }
        
        public override void Save() {

            controls.Save();

            playerData.Save(PlayerEntityController.current.entity);

            // Saving the time of the last save
            timeOfLastSave = (uint)timeSinceEpochStart.TotalSeconds;

            // Calculating play time of this session.
            uint sessionPlayTime = (uint)timeSinceEpochStart.TotalSeconds - (uint)(loadedTime - epochStart).TotalSeconds;
            totalTimePlayed += sessionPlayTime;
            Debug.Log( $"{sessionPlayTime} Seconds were added to the play time." );

            // Update the Time of loading this save file so "session playtime" gets reset and isn't added multiple times.
            loadedTime = System.DateTime.UtcNow;
        }

        public override void Load() {
                
                controls.Load();

                playerData.Load(PlayerEntityController.current.entity);

                // Calculating the time since the last save
                uint timeSinceLastSave = (uint)timeSinceEpochStart.TotalSeconds - timeOfLastSave;
                Debug.Log( $"{timeSinceLastSave} Seconds have elapsed since the file was saved." );
                Debug.Log( $"The date of the last save was {GetTimeOfLastSave()}." );
                Debug.Log( $"Total Playtime is {GetTotalPlaytime()}." );

                // Time of loading this save file is stored temporarily to calculate play time of this session.
                loadedTime = System.DateTime.UtcNow;
            }
    }
    
}
