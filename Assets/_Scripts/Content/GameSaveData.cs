using System.Collections.Generic;
using SeleneGame.Core;
using UnityEngine;

namespace SeleneGame.Content {

    [System.Serializable]
    public class GameSaveData : SaveData {

        public EntitySaveData playerData = new EntitySaveData();

        // Game Progress Save Data



        // General Save Data

        public ControlSaveData controls = new ControlSaveData();
        
        public override void Save() {

            base.Save();

            controls.Save();

            playerData.Save(PlayerEntityController.current.entity);
        }

        public override void Load() {

            base.Load();
                
            controls.Load();

            playerData.Load(PlayerEntityController.current.entity);
        }
    }
    
}