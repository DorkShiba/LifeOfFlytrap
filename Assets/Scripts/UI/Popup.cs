using UnityEngine;
using UnityEngine.UI;

public class Popup : BaseUI {
    public override void Init() {
        Managers.UI.SetPopupCanvas(gameObject);
    }

    public virtual void ClosePopupUI() {
        Managers.UI.ClosePopupUI(this);
    }
}
