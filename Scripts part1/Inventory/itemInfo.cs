using UnityEngine;
using Inventory;
using Dialog;
using SaveableObjects;
using UnityEngine.Events;
using static GameConstants;
using System.Collections.Generic;

[SelectionBase]
public class itemInfo : HealthBase
{
    public bool pick = true;
    public UnityEvent pickEvent;

    [Tooltip("hiçbirşey koymassan bulunduğu objeyi sayar")]
    [SerializeField]
    private GameObject Object;

    public Object pickitem;

    [SerializeField]
    private Breakable dropableInfo;
    private bool canBreak = true;

    //depolanan veriler
    [System.NonSerialized] public byte id;

    public Transform TreeCutPoint;
    public Vector3 dropRotation;
    [Space(3)]

    private static readonly int Drop = Animator.StringToHash("Drop");

    private void Start()
    {
        if (pick == false && dropableInfo != null)
        {
            if (!dropableInfo)
            {
                Debug2.LogOnce("bazı objelerde Pick item eksik " + name);
                return;
            }
            id = dropableInfo.item.Id;
        }
        if (pick)
        {
            if (pickitem is null)
            {
                Debug2.LogOnce("bazı objelerde Pick item eksik" + name);
                return;
            }
            if (pickitem is ItemSC sc)
            {
                id = sc.Id;
            }
            gameObject.tag = Tags.log;
        }

        if (Object == null) {Object = gameObject;}
    }


    public void Break()
    {
        canBreak = false;
        GetComponent<Animator>().SetTrigger(Drop);
    }

    public void Pick()
    {
        if (pickitem is ItemSC itemSC)
        {
            CharacterInventory.instance.AddItem(itemSC.Id);
        }
        pickEvent.Invoke();
        Object?.SetActive(false);
    }

    public void AddCoin(short amount)
    {
        DialogControl.instance.AddMoney(amount);
        Object.SetActive(false);
    }

    public override bool AddDamage(short damage, bool silent = false, bool fly = false, Vector3 AttackerPosition = default)
    {
        if (!canBreak) return false;
        // kesme efekti
        TreeCutPoint.eulerAngles -= Vector3.forward * 10;

        GameManager.instance.audioSource.PlayOneShot(GameManager.instance.WoodSound);

        Health -= damage;

        if (Health <= 0) {
            Break();
            return true;
        }

        return false;
    }

    private void WaitAndGiveLogs()
    {
        for (byte i = 0; i < 12; i += 4)
        {
            PoolManager.instance.ReuseObject(dropableInfo.Name, transform.position + Vector3.up + transform.right * i , Quaternion.Euler(dropRotation));
        }
        // ağacı kayıt defterinden sil
        GetComponentInChildren<SaveableObject>().Destroy();
    }

}
