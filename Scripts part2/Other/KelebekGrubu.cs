using AnilTools;
using UnityEngine;

public class KelebekGrubu : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.4f;

    Vector3 startArea;
    Vector3 rota;
    float distance;

    private void Start()
    {
        startArea = transform.position;
        rota = SetRota();
    }

    private void Update()
    {
        Dolan();
    }

    private void Dolan()
    {   
        distance = (transform.position - rota).sqrMagnitude.HalfPower();

        if (distance < 1)  // hata payı 1 metre rotaya ulaşınca
        {
            rota = SetRota();
        }
        
        transform.position = Vector3.MoveTowards(transform.position , rota , speed * Time.deltaTime);
    }

    float x,y,z;

    private Vector3 SetRota()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        x = Random.Range(startArea.x - 6, startArea.x + 6);
        y = Random.Range(startArea.y - 1 , startArea.y + 2);
        z = Random.Range(startArea.z - 6, startArea.z + 6);
        Vector3 rota = new Vector3(x, y, z);

        return rota;
    }

}
