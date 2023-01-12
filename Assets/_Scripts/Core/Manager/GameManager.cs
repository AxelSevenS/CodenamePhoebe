using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class GameManager : Singleton<GameManager> {

        private void OnEnable() {
            SetCurrent();
        }
        

        internal static Dictionary<string, int> flags = new Dictionary<string, int>();
        internal static Dictionary<string, int> tempFlags = new Dictionary<string, int>();


        public static void SetFlag(string flagName, int flagValue, bool isTemporary = false) {
            (isTemporary ? tempFlags : flags)[flagName] = flagValue;
        }

        public static void RemoveFlag(string flagName, bool isTemporary = false) {
            (isTemporary ? tempFlags : flags).Remove(flagName);
        }

        public static int GetFlag(string flagName, bool isTemporary = false) {
            var selectedFlags = (isTemporary ? tempFlags : flags);
            if (selectedFlags.ContainsKey(flagName)) {
                return selectedFlags[flagName];
            } else {
                return 0;
            }
        }

        

        public enum FlagType {
            GlobalFlag,
            TemporaryFlag
        }
    }
}
