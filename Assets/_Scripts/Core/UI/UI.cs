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
            UIController.UpdateMenuState();
        }

        public virtual void Disable() {
            if (!Enabled) return;
            Enabled = false;
            UIController.UpdateMenuState();
        }

        
        public virtual void Toggle() {
            if (Enabled)
                Disable();
            else
                Enable();
        }



        protected virtual void OnEnable() {
            SetCurrent();
        }

        protected virtual void OnDisable() {
            
        }
    }

}