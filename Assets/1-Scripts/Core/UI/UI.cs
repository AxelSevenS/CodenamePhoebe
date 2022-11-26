using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {

    public abstract class UI<T> : Singleton<T>, IUI where T : UI<T> {

        private bool _enabled = false;


        public bool Enabled { 
            get {
                return _enabled;
            } 
            protected set {
                _enabled = value;
            } 
        }

        public virtual void Enable() {
            if (Enabled) return;
            Enabled = true;
        }

        public virtual void Disable() {
            if (!Enabled) return;
            Enabled = false;
        }



        protected virtual void OnEnable() {
            SetCurrent();
        }

        protected virtual void OnDisable() {
            
        }
    }

}