using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {

    [System.Serializable]
    public class ControlSaveData {

        private Dictionary<System.Guid, string> inputOverrides = new Dictionary<System.Guid, string>();



        public void Save() {
            inputOverrides.Clear();

            // Save Input Overrides
            InputActionMap map = Keybinds.playerMap;
            foreach (var binding in map.bindings){
                if (!string.IsNullOrEmpty(binding.overridePath))
                    inputOverrides[binding.id] = binding.overridePath;
            }
        }

        public void Load() {

            Keybinds.playerMap.Disable();

            // Load Input Overrides
            Keybinds.playerMap.RemoveAllBindingOverrides();

            for (int i = 0; i < Keybinds.playerMap.bindings.Count; ++i){
                InputBinding binding = Keybinds.playerMap.bindings[i];

                if ( string.IsNullOrEmpty(binding.groups) || !binding.groups.Contains("Keyboard&Mouse") ) continue;

                if (inputOverrides.TryGetValue(binding.id, out string overridePath))
                    Keybinds.playerMap.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                
                Keybinds.UpdateKeybind( binding.id );
            }

            Keybinds.playerMap.Enable();
        }

    }
}
