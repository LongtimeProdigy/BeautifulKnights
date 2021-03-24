using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary;

public abstract class UIShow : MonoSingleton<UIShow> {

    public abstract void Show();

    public abstract void Initialize();

    public abstract void UnShow();
}
