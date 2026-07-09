using UnityEngine;

public class Popup : BaseUI {
    public override void Init() {
        Managers.UI.SetPopupCanvas(gameObject);
        Open();
    }

    public override void Close() {
        base.Close();
    }

    public virtual void ClosePopupUI() {
        Managers.UI.ClosePopupUI(this);
    }
}
