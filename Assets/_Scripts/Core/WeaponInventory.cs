using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class WeaponInventory : IEnumerable {
        private GameObject gameObject;
        [SerializeField] private Map<int, Weapon> items = new Map<int, Weapon>();
        
        [SerializeField] private int defaultIndex;
        [SerializeField] private int currentIndex;

        public Weapon defaultItem => items[defaultIndex];
        public Weapon currentItem { get {
                if ( !items.Exists(currentIndex) )
                    return defaultItem;
                return items[currentIndex];
            }
        }

        // public Weapon this[int index] {

        // }

        public WeaponInventory(GameObject gameObject, int newDefaultIndex = 0){
            this.gameObject = gameObject;
            defaultIndex = newDefaultIndex;
            Set( newDefaultIndex, Weapon.GetWeaponTypeByName(WeaponData.defaultData) );
        }

        public void Set(int index, System.Type type){
            // if (index == defaultIndex){
            //     Debug.LogError("Can't change the default value of this Inventory");
            //     return;
            // }

            if (items.Exists(index)) 
                Remove(index);

            items[index] = gameObject.AddComponent(type) as Weapon;
            items[index].enabled = true;
        }

        public void Remove(int index) {
            if (index != defaultIndex){
                items[index] = Global.SafeDestroy(items[index]);
            }
        }

        public void Switch(int index){
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
