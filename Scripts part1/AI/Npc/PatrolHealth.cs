using System.Collections;
using UnityEngine;
using static GameConstants;

public class PatrolHealth : MonoBehaviour
{
    NpcController2 npcController;
    public TextMesh stiuationTxt;
    public short health = 100;
    private readonly WaitForSecondsRealtime DefaultAwakeTime = new WaitForSecondsRealtime(20);
    private const string alive = "alive";
    private const string faint = "faint";

    public void Start()
    {
        npcController = GetComponent<NpcController2>();
    }

    public void AddDamage(short Damage)
    {
        health -= Damage;
        if (health <= 0)
        {
            StartCoroutine(Faint());
            gameObject.tag = Tags.faintPatrol;
            stiuationTxt.text = faint;
        }
    }

    private IEnumerator Faint()
    {
        yield return DefaultAwakeTime;
        npcController.canMove = false;

        gameObject.tag = Tags.Patrols;
        stiuationTxt.text = alive;
        // patrolu ayağa kaldır

    }

}
