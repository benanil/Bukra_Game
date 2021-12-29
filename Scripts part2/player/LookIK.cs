using Dialog;
using Player;
using UnityEngine;

public class LookIK : MonoBehaviour
{
    private Animator animator;
    private Camera _camera;
    private CombatControl combatControl;

    public bool Look = true;
    private const float PlayerEnemyLookHeight = 1.8f;

    private void Start()
    {
        _camera = Camera.main;
        animator = GetComponent<Animator>();
        combatControl = GetComponentInChildren<CombatControl>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (Look)
        {
            animator.SetLookAtWeight(0.7f, .5f, 1.2f, 0f, .5f);
        
            Ray lookPos;

            if (combatControl.hasEnemy)
            {
                animator.SetLookAtPosition(CombatControl.CurrentEnemy.position + Vector3.up * PlayerEnemyLookHeight);
            }
            else if (DialogControl.OnConversation)
            {
                animator.SetLookAtPosition(DialogControl.npcController.transform.position);
            }
            else
            {
                lookPos = ReturnRay(_camera.transform.forward);
                animator.SetLookAtPosition(lookPos.GetPoint(25));
            }
        }
    }

    private Ray ReturnRay(Vector3 pos)
    {
        return new Ray(transform.position, pos);
    }

}
