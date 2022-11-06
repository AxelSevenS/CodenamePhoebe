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
            InputActionMap map = ControlsManager.playerMap;
            foreach (var binding in map.bindings){
                if (!string.IsNullOrEmpty(binding.overridePath))
                    inputOverrides[binding.id] = binding.overridePath;
            }
        }

        public void Load() {

            ControlsManager.playerMap.Disable();

            // Load Input Overrides
            ControlsManager.playerMap.RemoveAllBindingOverrides();

            for (int i = 0; i < ControlsManager.playerMap.bindings.Count; ++i){
                InputBinding binding = ControlsManager.playerMap.bindings[i];

                if ( string.IsNullOrEmpty(binding.groups) || !binding.groups.Contains("Keyboard&Mouse") ) continue;

                if (inputOverrides.TryGetValue(binding.id, out string overridePath))
                    ControlsManager.playerMap.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                
                ControlsManager.UpdateKeybind( binding.id );
            }

            ControlsManager.playerMap.Enable();
        }

    }
}
