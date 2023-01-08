using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core {

    public static class Dialogue {
        

        private static Dictionary<string, int> flags = new Dictionary<string, int>();
        private static Dictionary<string, int> tempFlags = new Dictionary<string, int>();


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
                return -1;
            }
        }

    }


}