
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomAnimation : MonoBehaviour
{
    public Vector2 random = new Vector2(10, 20);
    private int clipCount;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        clipCount = animator.runtimeAnimatorController.animationClips.Length;

        StartCoroutine(coroutine());
    }

    IEnumerator coroutine()
    {
        while (true)
        {
            int index = Random.Range(0, clipCount);
            Debug.Log("AnimChanging: " + index);
            animator.SetTrigger(index.ToString());
            yield return new WaitForSecondsRealtime(Random.Range(random.x, random.y));
        }
    }

}
