using Dialog;
using UnityEngine;
using static GameConstants;

[RequireComponent(typeof(itemInfo))]
public class CoinTrigger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag(Names.player))
        {
            DialogControl.instance.AddMoney((GetComponent<itemInfo>().pickitem as CoinItem).Coin);
            gameObject.SetActive(false);
        }
    }
}
