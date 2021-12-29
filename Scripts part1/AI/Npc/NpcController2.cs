
using AnilTools;
using Assets.Resources.Scripts.AI.Enemy;
using Dialog;
using Player;
using Subtegral.DialogueSystem.DataContainers;
using NaughtyAttributes;
using UnityEngine.AI;
using UnityEngine;
using System;

public enum NpcState
{
    none,
    idle,
    walking,
    constant,// like smith  
    random   // todo add patrol
}

[Serializable]
public struct NpcBehaving
{
    public string jobAnimName; // in animator
    /// <summary> at rotadatabase's willage position </summary>
    public int jobWaypointIndex;
    public float waitTime; // in seconds
}


[RequireComponent(typeof(Trigger),typeof(NavMeshAgent))]
[SelectionBase]
public class NpcController2 : MonoBehaviour
{
    public NpcName Name;
    public NpcType npcType;
    public NpcState state;
    public byte HomeNo;

    [Tooltip("okçuysa arrow position")]
    public LayerMask Damageables;

    [ShowIf("npcType", NpcType.Patrol)] public byte Damage; 
    [HideIf("npcType", NpcType.Patrol)] public bool IsArcher;
    [HideIf("npcType", NpcType.Patrol)] public Transform SwordPosition; 
    /// <summary> her dövüş silahı için ayrı animasyonlar </summary>
    [HideIf("npcType", NpcType.Villager), SerializeField] private AnimationClip[] AttackAnims;
    [HideIf("npcType", NpcType.Villager), SerializeField] private AnimationClip ReplaceableAttackAnim;
    [ShowIf("npcState", NpcState.constant), SerializeField] private float animChangeTime;
    [ShowIf("npcState", NpcState.constant), SerializeField] private string[] idleAnims;

    private Animator animator;      private NavMeshAgent nav;
                                    
    // patrol AI
    internal bool isHered = false,canMove = true;
    internal byte patrolId;

    [SerializeField]
    private DialogueContainer[] dialogueContainers;

    public DialogueContainer GetDialogueContainer()
    {
        return dialogueContainers[(short)GameMenu.instance.languages.currentLanguage];
    }

    public int willageIndex; // first or second willage

    private static Transform _Player;
    public static Transform Player{
        get
        {
            if (_Player == null) _Player = CharacterMove.instance.transform;
            return _Player;
        }
    }

    public NpcBehaving[] behavings;
    private NpcBehaving currentBehaving;
    private int behavingIndex;


    private void Start()
    {
        if (state == NpcState.walking)  currentBehaving = behavings[0];
        if (state == NpcState.constant) animator.SetTrigger(idleAnims[0]);
        if (state == NpcState.random)   targetPos = RotaDatabase.ChoseRandPosNearToVillage(0);

        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();

        nav.enabled = true;
        // wait for rota database
        this.Delay(3f, () => { 
            // StartCoroutine(WalkAround());
            targetPos = RotaDatabase.ChoseRandPosNearToVillage(willageIndex);
            DecidePos();
        });
    }
    
    private Vector3 DecidedPos, targetPos;
    private float idleTime;

    private void Update()
    {
        switch (state)
        {
            case NpcState.constant:
                idleTime -= Time.deltaTime;
                if (idleTime < 0)
                {
                    if (++behavingIndex == idleAnims.Length) behavingIndex = 0; // loop behavings
                    animator.SetTrigger(idleAnims[behavingIndex]);
                    idleTime = animChangeTime;
                }
                break;
            case NpcState.walking:
                {
                    animator.SetFloat(AnimPool.Blend, nav.speed);

                    if (Vector3.Distance(transform.position, targetPos) < 2f) // walk state onChange
                    {
                        animator.SetFloat(AnimPool.Blend, 0);
                        // wrap player 
                        Transform waypoint = RotaDatabase.GetWaypoint(willageIndex, currentBehaving.jobWaypointIndex);
                        nav.Warp(waypoint.position);
                        transform.rotation = waypoint.rotation;
                        animator.SetTrigger(currentBehaving.jobAnimName);
                        state = NpcState.idle;
                        idleTime = currentBehaving.waitTime;
                        if (++behavingIndex == behavings.Length) behavingIndex = 0; // loop behavings
                        currentBehaving = behavings[behavingIndex]; // change behaving
                        targetPos = RotaDatabase.GetWaypointPos(willageIndex, currentBehaving.jobWaypointIndex); // reset TargetPos

                    }

                    if (Vector3.Distance(transform.position, DecidedPos) < 1f) { // walk state update
                        DecidePos();
                    }
                    break;
                }
            case NpcState.random:

                animator.SetFloat(AnimPool.Blend, nav.speed);
                
                if (Vector3.Distance(transform.position, targetPos) < 2f) // walk state onChange
                {
                    targetPos = RotaDatabase.ChoseRandPosNearToVillage(0);
                    DecidePos();
                }

                if (Vector3.Distance(transform.position, DecidedPos) < 1f) { // walk state update
                    DecidePos();
                }
                break;
            case NpcState.idle:
                idleTime -= Time.deltaTime; // idle state update
                if (idleTime < 0) // idle state on change
                {
                    // walk again
                    animator.SetTrigger(AnimPool.Locomotion);
                    state = NpcState.walking;
                }
                break;
        }
    }
    
    private void DecidePos()
    {

        int[] closestIndexes = new int[16];
        int closestCount = 0;

        for (int i = 0; i < RotaDatabase.GetWillageLength(willageIndex) && closestCount < 15; i++)
        {
            Vector3 point = RotaDatabase.GetWaypointPos(willageIndex, i);
            float dist = Vector3.Distance(transform.position, point);

            if (dist < 20)
            {
                closestIndexes[closestCount++] = i;
            }
        }

        float closestDist = float.MaxValue;
        int result = 0;

        for (int i = 0; i < closestCount; i++)
        {
            float dist = Vector3.Distance(RotaDatabase.GetWaypointPos(willageIndex, closestIndexes[i]), targetPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                result = i;
            }
        }

        DecidedPos = RotaDatabase.GetWaypointPos(willageIndex, closestIndexes[result]);

        nav.SetDestination(DecidedPos);
    }

    /// <summary>
    /// Saldırı animasyonlarının eventlerine Attack finish eklemen önemli
    /// </summary>
    public void AttackFinish()
    {
        EnemyBase.GenerateOverlapSphere(SwordPosition.position, 1, Damageables, Damage, false,
                    (x) =>
                    {
                    });
    }

    public void HeadActive(bool value)
    {
        transform.GetChild(1).gameObject.SetActive(value);
    }

    public void HerePatrol()
    {
        isHered = true;
        canMove = true;
        patrolId = PlayerInfo.instance.AddPatrol(this);
    }

    public void FirePatrol()
    {
        isHered = false;
        canMove = false;
        PlayerInfo.instance.RemovePatrol(this);
    }

    public void OnDialogAnimation()
    {
        animator.SetBool(AnimPool.DialogStr, !animator.GetBool(AnimPool.DialogStr));
    }

    //patrolu durdur yada yürüt
    public void MoveOrStop()
    {
        if (canMove == true){
            canMove = false;
            nav.isStopped = true;
        }
        else{
            canMove = true;
            nav.isStopped = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || npcType != NpcType.Villager) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(DecidedPos, 1);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPos, 1);
        Gizmos.DrawWireSphere(transform.position, 20);
    }
}
