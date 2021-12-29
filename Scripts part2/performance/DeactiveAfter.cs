

using UnityEngine;
using AnilTools;

public class DeactiveAfter : MonoBehaviour
{
    private void Start()
    {
        this.Delay(1.2f, () => gameObject.SetActive(false));
    }
}
