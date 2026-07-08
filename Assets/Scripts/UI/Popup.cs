using UnityEngine;

public class Popup : BaseUI {
    public override void Init() {
        Managers.UI.SetPopupCanvas(gameObject);
    }

    public virtual void ClosePopupUI() {

    }
}
