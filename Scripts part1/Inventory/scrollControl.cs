using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class scrollControl : MonoBehaviour
    {
        [SerializeField]
        private Text fiyat, alınan;
        private short _alınan;
        [SerializeField]
        private DialogControl money;
        private string satınAlınan = "Satın Alınan: ";

        private void Start()
        {
            alınan.text = string.Empty;
        }

        public void AddItem()
        {
            if (money.Money >= 0)
            {
                _alınan += 1;
                alınan.text = satınAlınan + _alınan.ToString();
            }
        }
        
        public void RemoveItem()
        {
            if (_alınan <= 0)
                return;
            _alınan -= 1;
            alınan.text = satınAlınan + _alınan.ToString();
        }

        public void ChangeLanguage(string given , string price)
        {
            satınAlınan = given;
            fiyat.text = price;
        }

    }
}