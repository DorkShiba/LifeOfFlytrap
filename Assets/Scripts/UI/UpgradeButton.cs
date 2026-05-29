using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeButton : Popup
{
    List<Button> upgradeButtons;
    void Start()
    {
        upgradeButtons = new List<Button>()
        {
            Get<Button>(0),
            Get<Button>(1),
            Get<Button>(2),
            Get<Button>(3),
            Get<Button>(4)
        };
    }
}
