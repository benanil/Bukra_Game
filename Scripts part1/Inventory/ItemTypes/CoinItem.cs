using UnityEngine;

[CreateAssetMenu(fileName = "Coin", menuName = "Items/Coin")]
public class CoinItem : ScriptableObject
{
    public short Coin => (short)Random.Range(50,byte.MaxValue);
}
