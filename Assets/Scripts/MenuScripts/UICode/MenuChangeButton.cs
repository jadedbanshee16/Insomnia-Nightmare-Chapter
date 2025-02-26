using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuChangeButton : ButtonClass
{
    [SerializeField]
    string menu_;

    [SerializeField]
    MenuPage menuPageOverride_;

    public override void useButton()
    {
        if(menu_ != "")
        {
            if(!menuPageOverride_ && menuManager_)
            {
                //Adjust the given page to default.
                if (GetComponentInParent<MenuPage>())
                {
                    GetComponentInParent<MenuPage>().setToMenuGroup(GetComponentInParent<MenuPage>().getDefaultPage());
                }

                menuManager_.setToMenuGroup(menu_);
            } else if (menuPageOverride_)
            {
                menuPageOverride_.setToMenuGroup(menu_);
            }
        }

        base.useButton();
    }
}
