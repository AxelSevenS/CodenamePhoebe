using System.Collections.Generic;
using UnityEngine;

using Scribe;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "New Scriptable Weapon", menuName = "Data/Scriptable Weapon", order = 0)]
    public class ScriptableWeaponData : WeaponData {

        public List<WeaponInputEvent> inputEvents = new List<WeaponInputEvent>();
        public List<WeaponEvent> lightAttackEvents = new List<WeaponEvent>();
        public List<WeaponEvent> heavyAttackEvents = new List<WeaponEvent>();



        public override void HandleInput(Weapon weapon, Player playerController) {
            foreach (WeaponInputEvent inputEvent in inputEvents) {
                if (inputEvent.Evaluate(playerController)) {
                    inputEvent.Invoke(weapon);
                }
            }
        }

        public override void LightAttack(Weapon weapon) {
            foreach (WeaponEvent weaponEvent in lightAttackEvents) {
                weaponEvent.Invoke(weapon);
            }
        }

        public override void HeavyAttack(Weapon weapon) {
            foreach (WeaponEvent weaponEvent in heavyAttackEvents) {
                weaponEvent.Invoke(weapon);
            }
        }

        public override Weapon CreateWeaponFor(ArmedEntity entity, WeaponCostume costume = null) {
            return new Weapon(entity, this, costume, entity.Weapons);
        }
    }
}
