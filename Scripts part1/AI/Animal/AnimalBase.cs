using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Player;
using AnilTools;
using System.Linq;
using NaughtyAttributes;

namespace Animal
{
    enum AnimalState
    { 
        idle, walk, chasing
    }

    [SelectionBase]
    [RequireComponent(typeof(NavMeshAgent),typeof(AnimalHealth))]
    public class AnimalBase : MonoBehaviour
    {
        [SerializeField, Expandable] private AnimalData animalData;

        private NavMeshAgent nav;
        private Animator animator;

        private Vector3 DesiredPosition;
        
        [SerializeField, Min(0)] float minWalkDist = 5, maxWalkDist = 10;

        private const byte StopDistance = 2;

        private int CurrentIddleAnim;
        
        private AnimalState state;

        private float delay;

        private void Start()
        {
            animator = GetComponent<Animator>();
            nav = GetComponent<NavMeshAgent>();
            state = AnimalState.idle;
            DesiredPosition = transform.position;
            nav.speed = animalData.WalkSpeed;
            nav.enabled = true;
        }

        private void ChoseIdleAnim()
        {
            if (++CurrentIddleAnim == 4) CurrentIddleAnim = 0; 
            animator.SetFloat(AnimPool.Idle, CurrentIddleAnim);
        }

        private void Update()
        {
            
            switch (state)
            {
                case AnimalState.idle:

                    if (CheckThreat()) return;
                    if (delay < 0)
                    {
                        state = AnimalState.walk;
                        animator.SetBool(AnimPool.Walk, true);
                        GoRandomPos();
                    }

                    delay -= Time.deltaTime;

                    break;
                case AnimalState.walk:

                    if (CheckThreat()) return;
                    
                    if (Vector3.Distance(transform.position, DesiredPosition) < StopDistance)
                    {
                        state = AnimalState.idle;
                        ChoseIdleAnim();
                        animator.SetBool(AnimPool.Walk, false);
                        delay = animalData.IdleTime;
                        nav.speed = animalData.WalkSpeed;
                    }

                    break;
                case AnimalState.chasing:


                    if (Vector3.Distance(transform.position, DesiredPosition) < StopDistance)
                    { 
                        if (!CheckBoth())
                        {
                            state = AnimalState.idle;
                            animator.SetBool(AnimPool.Walk, false);
                            ChoseIdleAnim();
                            delay = animalData.IdleTime;
                            nav.speed = animalData.WalkSpeed;
                            return;
                        }
                        Vector3 dir = (threatPos - transform.position).normalized;
                        GoPosition(Mathf.Atan2(dir.x, dir.z));
                    }


                    break;
            }
        }

        bool CheckThreat()
        {
            if (CheckBoth())
            {
                animator.SetBool(AnimPool.Walk, true);
                state = AnimalState.chasing;
                Vector3 dir = (threatPos - transform.position).normalized;
                GoPosition(Mathf.Atan2(dir.x, dir.z));
                nav.speed = animalData.ChaseSpeed;
                return true;
            }
            return false;
        }

        private void GoRandomPos()
        {
            GoPosition(Random.Range(0, 6.4f));
        }

        public NavMeshQueryFilter filter;

        private void GoPosition(float angle)
        { 
            Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            int tryCount = 0;

            while (tryCount < 20)
            {
                float randDist = Random.Range(minWalkDist, maxWalkDist);

                if (Physics.Raycast(new Ray(transform.position + (dir * randDist) + (Vector3.up * 10), Vector3.down), out RaycastHit hit, 40, 2048, QueryTriggerInteraction.Ignore))
                {
                    DesiredPosition = hit.point;
                    nav.SetDestination(hit.point);
                }
                angle += 0.4f;
                dir.x = Mathf.Sin(angle);
                dir.z = Mathf.Cos(angle);

                tryCount++;
            }

        }

        Vector3 threatPos;

        private bool CheckBoth() // enemy Close and enemy pos
        {
            threatPos   = NpcController2.Player.position; // buna enemy vs eklenebilir
            return CheckPlayerClose() || CheckMonstersClose();
        }

        private bool CheckPlayerClose()
        {
            if (transform.DistanceSqr(NpcController2.Player) < animalData.DetectDistance){
                return true;
            }
            return false;
        }

        private bool CheckMonstersClose()
        {
            return SpawnController.instance.canavars.Any(x => x.transform.DistanceSqr(transform) < animalData.DetectDistance);
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying && DesiredPosition != default)
            {
                Gizmos.DrawLine(transform.position, DesiredPosition);
            }
        }
    }
}

