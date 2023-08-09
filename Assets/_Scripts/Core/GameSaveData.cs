using System.Collections.Generic;
using SeleneGame.Core;

using SevenGame.SavingSystem;
using Scribe;

namespace SeleneGame.Core {

    [System.Serializable]
    public class GameSaveData : SaveData {

        public EntitySaveData playerData = new EntitySaveData();

        // Game Progress Save Data

        public Dictionary<string, int> flags = new Dictionary<string, int>();


        // General Save Data

        public ControlSaveData controls = new ControlSaveData();
        
        public override void Save() {

            base.Save();

            controls.Save();

            playerData.Save(Player.Current.Entity);

            flags = ScribeFlags.flags;
        }

        public override void Load() {

            base.Load();
                
            controls.Load();

            playerData.Load(Player.Current.Entity);

            ScribeFlags.flags = flags;
        }
    }
    
}
