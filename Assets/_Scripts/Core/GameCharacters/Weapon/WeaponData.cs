using System;
using System.Collections;
using System.Collections.Generic;
using Scribe;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Data/Weapon", order = 0)]
    public class WeaponData : CostumableData<WeaponCostume> {


        public Weapon.WeaponType weaponType = Weapon.WeaponType.Sparring;
        public float weight = 1f;


        public List<WeaponInputEvent> inputEvents = new List<WeaponInputEvent>();


        public virtual Weapon GetWeapon(ArmedEntity entity, WeaponCostume costume = null) {
            return new Weapon(entity, this, costume);
        }


        [System.Serializable]
        public class WeaponInputEvent : ScribeEvent<InputCondition> {
            
            [ScribeHideLabel]
            [ScribeOption]
            public WeaponEventType eventType;

            [ScribeField(nameof(eventType), (int)WeaponEventType.EvadeRelative)]
            [ScribeField(nameof(eventType), (int)WeaponEventType.EvadeAbsolute)]
            public Vector3 evadeDirection;

            [ScribeField(nameof(eventType), (int)WeaponEventType.SetState)]
            public string stateName;


            public void Execute(Weapon weapon) {
                switch (eventType) {
                    case WeaponEventType.PlayAnimation:
                        break;
                    case WeaponEventType.PlaySound:
                        break;
                    case WeaponEventType.ResetState:
                        weapon.armedEntity.ResetState();
                        break;
                    case WeaponEventType.SetState:
                        try {
                            Type stateType = Type.GetType(stateName, false, true);
                            weapon.armedEntity.SetState(stateType);
                        } catch (Exception e) {
                            Debug.LogError(e);
                        }
                        break;
                    case WeaponEventType.Jump:
                        weapon.armedEntity.Jump();
                        break;
                    case WeaponEventType.EvadeRelative:
                        weapon.armedEntity.Evade(weapon.armedEntity.transform.rotation * evadeDirection);
                        break;
                    case WeaponEventType.EvadeAbsolute:
                        weapon.armedEntity.Evade(evadeDirection);
                        break;
                    default:
                        throw new System.NotImplementedException();
                }
            }


            public enum WeaponEventType {
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
}
