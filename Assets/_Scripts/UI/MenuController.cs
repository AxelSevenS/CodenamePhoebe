using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

public abstract class MenuController<T> : Singleton<T> where T : MenuController<T> {

    private bool _enabled = false;


    public bool Enabled { 
        get {
            return _enabled;
        } 
        protected set {
            _enabled = value;
        } 
    }

    
    public virtual void Toggle() {
        if (Enabled)
            Disable();
        else
            Enable();
    }

    public virtual void Enable() {
        if (Enabled) return;
        Enabled = true;
    }

    public virtual void Disable() {
        if (!Enabled) return;
        Enabled = false;
    }

    public virtual void OnCancel() {
        Disable();
    }

    public virtual void OnControllerTypeChange(ControlsManager.ControllerType type) {
        SetSelectedObject();
    }

    public abstract void SetSelectedObject();

    private void OnEnable() {
        SetCurrent();
        ControlsManager.onCancelEvent += OnCancel;
        ControlsManager.onControllerTypeChange += OnControllerTypeChange;
    }

    private void OnDisable() {
        ControlsManager.onCancelEvent -= OnCancel;
        ControlsManager.onControllerTypeChange -= OnControllerTypeChange;
    }
}
