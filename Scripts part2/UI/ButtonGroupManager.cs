

using UnityEngine;
using UrFairy;

namespace Assets.Resources.Scripts.UI
{
    public static class ButtonGroupManager{
        private static ButtonGroup[] _AllGroups;
        private static ButtonGroup[] AllGroups
        {
            get{
                if (_AllGroups == null){
                    _AllGroups = GameObject.FindObjectsOfType<ButtonGroup>();
                }
                return AllGroups;
            }
        }

        public static ButtonGroup CurrentGroup;

        public static void SetCurrentGroup(ButtonGroup group)
        {
            CurrentGroup.IfJust(x => x.Desellect());
            CurrentGroup = group;
        }

    }
}
