using System.Collections;
using System.Collections.Generic;

namespace SevenGame.Utility {

    public static class ArrayUtil {
        
        public static T[] Add<T>(this T[] array, T value) {
            T[] newArray = new T[array.Length + 1];
            for (int i = 0; i < array.Length; i++) {
                newArray[i] = array[i];
            }
            newArray[array.Length] = value;
            return newArray;
        }

        public static T[] Remove<T>(this T[] array, T value) {
            T[] newArray = new T[array.Length - 1];
            for (int i = 0; i < array.Length; i++) {
                if ( !array[i].Equals(value) ) {
                    newArray[i] = array[i];
                }
            }
            return newArray;
        }

        public static T[] RemoveAtIndex<T>(this T[] array, int index) {
            T[] newArray = new T[array.Length - 1];
            for (int i = 0; i < array.Length; i++) {
                if (i == index) continue;
                newArray[i] = array[i];
            }
            return newArray;
        }

        public static T[] RemoveRange<T>(this T[] array, int startIndex, int count) {
            T[] newArray = new T[array.Length - count];
            for (int i = 0; i < array.Length; i++) {
                if (i < startIndex) {
                    newArray[i] = array[i];
                } else if (i >= startIndex + count) {
                    newArray[i] = array[i];
                }
            }
            return newArray;
        }
    }
}