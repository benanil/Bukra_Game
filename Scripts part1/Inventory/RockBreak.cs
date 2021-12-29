
using AnilTools;
using AnilTools.Lerping;
using Inventory;
using SaveableObjects;
using UnityEngine;

public class RockBreak : HealthBase
{
    [SerializeField]
    private Transform[] rockParts;

    [SerializeField]
    private bool breaked;

    [SerializeField] private ItemSC rock;

    public override bool AddDamage(short damage, bool silent = false, bool fly = false, Vector3 AttackerPosition = default)
    {
        if (breaked) return false;

        GameManager.instance.audioSource.PlayOneShot(GameManager.instance.RockSound);

        Health -= damage;
        if (Health <= 0)
        {
            Break();
            breaked = true;
            return true;
        }
        
        return false;
    }

    private void Break()
    {
        for (int i = 0; i < rockParts.Length; i++)
        {
            var part = rockParts[i];
            part.CubicLerpAnim(part.position , part.position + Vector3.up * 2 + RandomReal.V3RandomizerMin1(3) ,
                               part.position + RandomReal.V3Randomizer(new Vector3(4,0,4)) + Vector3.down * .2f , 1 , ReadyVeriables.Linear);
        }

        CharacterInventory.instance.AddItem(rock);

        this.Delay(15f, () => GetComponentInParent<SaveableObject>().Destroy());
    }
}