
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Resources.Scripts.UI
{
    // Todo: Add sound
    public class ButtonGroup : MonoBehaviour{
        public enum ButtonLayout{ 
            vertical , horizontal , grid
        }

        public ButtonLayout layout;
        public ButtonNew[] buttons;
        
        [Header("null olabilirler alttaki 4 ü")] 
        public ButtonGroup rightGroup;
        public ButtonGroup leftGroup;
        public ButtonGroup upGroup;
        public ButtonGroup downGroup; 

        [SerializeField]
        private int _currentIndex;
        private int currentIndex{
            get{
                return _currentIndex;
            }
            set{
                _currentIndex = value;
                currentButton.Desellect();
                currentButton = buttons[value];
                currentButton.Sellect();

                Debug2.Log("current button: " + currentButton.name);
            }
        }

        private ButtonNew currentButton;

        private int lastIndex;
        private const byte firstIndex = 0;

        private Slider _slider;
        private Slider slider{
            get{
                return _slider;
            }
            set{
                if (value != null){
                    value.Select();
                    _slider = value;
                }
                else{
                    _slider.OnDeselect(null);
                    _slider = null;
                }
            }
        }

        private void Start(){
            currentButton = buttons[0];
            lastIndex = buttons.Length - 1;
        }

        // button event ile cagir
        // yeni sayfa açtığında grubu değiştirmek için
        // exit buttonu yada her hangi bir arayüzü açarken
        public void Set(){
            ButtonGroupManager.SetCurrentGroup(this);
        }

        public void Desellect(){
            currentButton.Desellect();
        }

        public void RightPressed(){

            if (slider){
                slider.value -= .1f;
                return;
            }

            switch (layout){
                case ButtonLayout.vertical:
                    if (rightGroup){
                        ButtonGroupManager.SetCurrentGroup(rightGroup);
                    }
                    break;
                case ButtonLayout.horizontal:
                    if (rightGroup && currentIndex == lastIndex){
                        ButtonGroupManager.SetCurrentGroup(rightGroup);
                        return;
                    }

                    if (++currentIndex == buttons.Length) currentIndex = 0;

                    break;
                case ButtonLayout.grid:
                    break;
            }
        }

        public void LeftPressed(){

            if (slider) { 
                slider.value -= .1f;
                return;
            }

            switch (layout){
                case ButtonLayout.vertical:
                        
                    if (leftGroup){
                        ButtonGroupManager.SetCurrentGroup(rightGroup);
                        return;
                    }
                    break;
                case ButtonLayout.horizontal:
                    if (leftGroup && currentIndex == firstIndex){
                        ButtonGroupManager.SetCurrentGroup(leftGroup);
                        return;
                    }
                    
                    if (++currentIndex == buttons.Length) currentIndex = 0;
                    
                    break;
                case ButtonLayout.grid:
                    break;
            }
        }

        public void UpPressed(){
            if (slider)
                return;

            switch (layout){
                case ButtonLayout.vertical:
                    if (upGroup && currentIndex == lastIndex){
                        ButtonGroupManager.SetCurrentGroup(upGroup);
                        return;
                    }
                    if (++currentIndex == buttons.Length) currentIndex = 0;
                    break;
                case ButtonLayout.horizontal:
                    if (upGroup){
                        ButtonGroupManager.SetCurrentGroup(upGroup);
                    }
                    break;
                case ButtonLayout.grid:
                    break;
            }
        }

        public void DownPressed(){
            if (slider)
                return;

            switch (layout){
                case ButtonLayout.vertical:
                    if (downGroup && currentIndex == firstIndex){
                        ButtonGroupManager.SetCurrentGroup(downGroup);
                        return;
                    }

                    if (++currentIndex == buttons.Length) currentIndex = 0;

                    break;
                case ButtonLayout.horizontal:
                    if (downGroup) ButtonGroupManager.SetCurrentGroup(downGroup);
                    break;
                case ButtonLayout.grid:
                    break;
            }
        }

        public void EnterPressed(){
            Debug2.Log("enter amano");
            slider = currentButton.GetComponent<Slider>();
            if (slider)
                return;

            currentButton.OnPointerDown(null);
        }
        
    }
}