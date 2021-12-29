
using UnityEngine;
using AnilTools;
using AnilTools.Update;
using AnilTools.Move;

public class Arrow : MonoBehaviour
{ 

    public void Throw(RaycastHit hit, float speed)
    {
        RegisterUpdate.UpdateWhile(
        action: () =>
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
        },
        endCnd: () => transform.Distance(hit.point) > 2,
        then: () => transform.SetParent(hit.transform));
    }

    public void Throw(Vector3 position)
    {
        transform.Move(position,2, ReadyVeriables.Linear );
    }
}

