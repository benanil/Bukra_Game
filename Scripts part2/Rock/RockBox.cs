using AnilTools;
using Inventory;
using UnityEngine;

namespace Rock
{
    public class RockBox : AnilButton
    {
        private static RockBox _instance;
        public static RockBox instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RockBox>();
                }
                return _instance;
            }       
        }

        public static object SellectedItem;
        internal static short InventoryId;

        private bool Draging;
        private const byte anchor = 40;

        public void Update()
        {
            if (Draging)
            {
                transform.position = Input.mousePosition + SVector3.Left * anchor + SVector3.up * anchor;
                if (Input.GetMouseButtonUp(0))
                {
                    Desellect();
                }
            }
        }

        public override void Sellect()
        {
            base.Sellect();
            Draging = true;
            if (SellectedItem is ItemSC)
            {
                Image.sprite = ((ItemSC)SellectedItem).icon;
            }
            else
            {
                Image.sprite = ((ItemNR)SellectedItem).icon;
            }
            Image.color = Color.white;
        }

        public override void Desellect()
        {
            Image.sprite = null;
            Draging = false;
            base.Desellect();
            Image.color = Color.clear;
            SellectedItem = null;
        }
    }
}