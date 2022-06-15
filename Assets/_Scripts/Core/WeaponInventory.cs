using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInventory : IEnumerable {
        private GameObject gameObject;
        [SerializeField] private Map<int, Weapon> items = new Map<int, Weapon>();
        
        private const int defaultIndex = 0;
        [SerializeField] private int currentIndex;

        public Weapon currentItem { get {
                if ( !items.Exists(currentIndex) )
                    return items[defaultIndex];
                return items[currentIndex];
            }
        }

        public WeaponInventory(GameObject gameObject){
            this.gameObject = gameObject;
            Set( defaultIndex, Weapon.GetWeaponTypeByName(WeaponData.defaultData) );
        }

        public void Set(int index, System.Type type){

            if (items.Exists(index)) 
                Remove(index);

            items[index] = gameObject.AddComponent(type) as Weapon;
            items[index].enabled = true;
        }

        public void Remove(int index) {
            if (index == defaultIndex) return;

            items[index] = Global.SafeDestroy(items[index]);
        }

        public void Switch(int index){
            if (index == currentIndex) return;

            foreach ( ValuePair<int, Weapon> item in items )
                item.valueTwo.Hide();

            try {
                Weapon newItem = items[index];
                currentIndex = index;
            } catch ( KeyNotFoundException ) {
                currentIndex = defaultIndex;
            }

            currentItem.Display();
        }

        public IEnumerator GetEnumerator() => items.GetEnumerator();
    }
}
