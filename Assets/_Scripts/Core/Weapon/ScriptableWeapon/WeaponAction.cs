using System;

using UnityEngine;

using Scribe;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public class WeaponAction : ScribeAction {
        
        [ScribeHideLabel]
        [ScribeOption]
        public WeaponActionType eventType;

        [ScribeField(nameof(eventType), (int)WeaponActionType.CreateDamageZone)]
        public string attackName;
        [ScribeField(nameof(eventType), (int)WeaponActionType.CreateDamageZone)]
        public Vector3 damageZoneOffset;
        [ScribeField(nameof(eventType), (int)WeaponActionType.CreateDamageZone)]
        public Quaternion damageZoneRotation;
        [ScribeField(nameof(eventType), (int)WeaponActionType.CreateDamageZone)]
        public Vector3 damageZoneScale = Vector3.one;
        [ScribeField(nameof(eventType), (int)WeaponActionType.CreateDamageZone)]
        public bool parentToEntity;

        [ScribeField(nameof(eventType), (int)WeaponActionType.EvadeRelative)]
        [ScribeField(nameof(eventType), (int)WeaponActionType.EvadeAbsolute)]
        public Vector3 evadeDirection;

        // [ScribeField(nameof(eventType), (int)WeaponActionType.SetState)]
        // public string stateName;


        public void Invoke(Weapon weapon) {
            switch (eventType) {
                case WeaponActionType.CreateDamageZone:
                    Vector3 position = weapon.armedEntity.ModelTransform.position + (weapon.armedEntity.ModelTransform.rotation * damageZoneOffset);
                    Quaternion rotation = weapon.armedEntity.ModelTransform.rotation * damageZoneRotation;
                    Vector3 scale = damageZoneScale;
                    Transform parent = parentToEntity ? weapon.armedEntity.transform : null;
                    Action<DamageZone> modifier = (zone) => {
                        zone.transform.SetParent(parent);
                        zone.transform.position = position;
                        zone.transform.rotation = rotation;
                        zone.transform.localScale = Vector3.Scale(zone.transform.localScale, scale);
                    };
                    DamageZone.Create(weapon, attackName, modifier);
                    break;
                case WeaponActionType.PlayAnimation:
                    break;
                case WeaponActionType.PlaySound:
                    break;
                case WeaponActionType.ResetState:
                    weapon.armedEntity.ResetBehaviour();
                    break;
                // case WeaponActionType.SetState:
                //     try {
                //         Type stateType = Type.GetType(stateName, false, true);
                //         weapon.armedEntity.SetState(stateType);
                //     } catch (Exception e) {
                //         Debug.LogError(e);
                //     }
                //     break;
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
            CreateDamageZone,
            PlayAnimation,
            PlaySound,
            ResetState,
            // SetState,
            Jump,
            EvadeRelative,
            EvadeAbsolute
        }
    }
}
