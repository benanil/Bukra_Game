using System.Collections;
using UnityEngine;
using UrFairy;

namespace Assets.Resources.Scripts.UI
{
    public class MenuInput : MonoBehaviour
    {

        private void Update()
        {
            if (!GameMenu.PlayerOnMenu)
                return;
            
            // for windows otherwise change

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                ButtonGroupManager.CurrentGroup.IfJust(x => x.UpPressed());
            
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                ButtonGroupManager.CurrentGroup.IfJust(x => x.DownPressed());
            
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                ButtonGroupManager.CurrentGroup.IfJust(x => x.RightPressed());
            
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                ButtonGroupManager.CurrentGroup.IfJust(x => x.LeftPressed());
            
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                ButtonGroupManager.CurrentGroup.IfJust(x => x.EnterPressed());
            
            //
        }

    }
}