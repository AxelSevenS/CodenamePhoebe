using System;

using UnityEngine;

using Scribe;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public class WeaponAction : ScribeAction {
        
        [ScribeHideLabel]
        [ScribeOption]
        public WeaponActionType eventType;

        [ScribeField(nameof(eventType), (int)WeaponActionType.EvadeRelative)]
        [ScribeField(nameof(eventType), (int)WeaponActionType.EvadeAbsolute)]
        public Vector3 evadeDirection;

        [ScribeField(nameof(eventType), (int)WeaponActionType.SetState)]
        public string stateName;


        public void Invoke(Weapon weapon) {
            switch (eventType) {
                case WeaponActionType.PlayAnimation:
                    break;
                case WeaponActionType.PlaySound:
                    break;
                case WeaponActionType.ResetState:
                    weapon.armedEntity.ResetState();
                    break;
                case WeaponActionType.SetState:
                    try {
                        Type stateType = Type.GetType(stateName, false, true);
                        weapon.armedEntity.SetState(stateType);
                    } catch (Exception e) {
                        Debug.LogError(e);
                    }
                    break;
                case WeaponActionType.Jump:
                    weapon.armedEntity.Jump();
                    break;
                case WeaponActionType.EvadeRelative:
                    weapon.armedEntity.Evade(weapon.armedEntity.transform.rotation * evadeDirection);
                    break;
                case WeaponActionType.EvadeAbsolute:
                    weapon.armedEntity.Evade(evadeDirection);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }


        public enum WeaponActionType {
            PlayAnimation,
            PlaySound,
            ResetState,
            SetState,
            Jump,
            EvadeRelative,
            EvadeAbsolute
        }
    }
}
