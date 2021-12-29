
using AnilTools;
using AnilTools.Move;
using AnilTools.Update;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bukra.Sliding
{
    public class SlidingArea : MonoBehaviour
    {
        const TransposeFlags transposeFlags = TransposeFlags.position | TransposeFlags.rotation;

        public string SlideEndTag;

        public float TargetClimbSpeed = 2;
        public float ClimbAcelleration = 10;

        public List<Transform> points;

        public TransformationArrayData slideData;

        public Transform slideStartPos,climbStartPos;

        // climbing de siralaması ile oynanacağı için pointsin bir kopyası alınıyor
        readonly List<Transform> pointsCopy = new List<Transform>(); 
        Coroutine climbTask;

        private Transform Player => NpcController2.Player;

        private float climbSpeed;

        private Vector3 _targetPosition;
        private Vector3 targetPosition
        {
            get{
                return _targetPosition + slideData.anchor;
            }
            set{
                _targetPosition = value;
            }
        }

        private void Reset(){
            slideData.anchor = new Vector3(0,1.70f,0);
        }

        private void Start(){
            // climbing de siralaması ile oynanacağı için pointsin bir kopyası alınıyor
            points.ForEach(x => pointsCopy.Add(x));
        }

        public void SlideStart(){
            PlayerInfo.Sliding = true;
            CharacterMove.SetPlayer(false);

            var rts = new RTS(slideStartPos);
            
            // animasyonun başlayaccağı noktaya gidiş
            NpcController2.Player.TransformationLerp(
            ref rts ,transposeFlags,
            MoveType.towards,null,CharacterMove.PlaySlideJumpAnim);
        }

        // playerinfo üzerinden animation event ile çağır
        public void SlideJumpEnd(){
            Slide();
        }

        private void Slide(){
            slideData.Proceed(NpcController2.Player, transposeFlags, SlideEnd);
        }

        private void SlideEnd(){
            JumpOff();
        }

        public void ClimbStart(){
            PlayerInfo.Sliding = true;
            CharacterMove.SetPlayer(false);

            var rts = new RTS(climbStartPos);

            // animasyonun başlayaccağı noktaya gidiş
            NpcController2.Player.TransformationLerp(
            ref rts, transposeFlags,MoveType.lerp, null, CharacterMove.PlaySlideJumpAnim);
        }

        // playerinfo üzerinden animation event ile çağır
        public void ClimbJumpEnd()
        { 
            // climbTask = this.UpdateWhile(Climb, () => !Input.GetKeyDown(KeyCode.E),JumpOff);
        }
        
        public void Climb()
        {
            pointsCopy.OrderBy(x => x.Distance(NpcController2.Player));
            Tuple<Vector3, Vector3> closestTransforms = new Tuple<Vector3, Vector3>(pointsCopy[0].position, pointsCopy[1].position);
            
            if (Input.GetKey(KeyCode.W)){
                
                targetPosition = Vector3.Distance(closestTransforms.Item1,slideStartPos.position) < Vector3.Distance(closestTransforms.Item2,slideStartPos.position) ? closestTransforms.Item1 : closestTransforms.Item2;
        
                climbSpeed = Mathf.Lerp(climbSpeed, TargetClimbSpeed, Time.deltaTime * ClimbAcelleration);
        
                Player.position = Vector3.Lerp(Player.position, targetPosition, Time.deltaTime * climbSpeed);
        
                CharacterMove.instance.animatior.SetFloat(AnimPool.InputY, climbSpeed);
            }
            else if (Input.GetKey(KeyCode.S)){
        
                targetPosition = Vector3.Distance(closestTransforms.Item1,climbStartPos.position) < Vector3.Distance(closestTransforms.Item2,climbStartPos.position) ? closestTransforms.Item1 : closestTransforms.Item2;
        
                climbSpeed = Mathf.Lerp(climbSpeed, TargetClimbSpeed, Time.deltaTime * ClimbAcelleration);
        
                Player.position = Vector3.Lerp(Player.position, targetPosition, Time.deltaTime * climbSpeed);
        
                CharacterMove.instance.animatior.SetFloat(AnimPool.InputY, -climbSpeed);
            }
            else{
                climbSpeed = Mathf.Lerp(climbSpeed, 0, Time.deltaTime * ClimbAcelleration * 2);
            }
        }

        private void JumpOff()
        {
            CharacterMove.PlaySlideFallAnim();
            StopCoroutine(climbTask);
        }
        
        // playerinfo üzerinden animation event ile çağır
        public void SlideFallEnd()
        {
            PlayerInfo.Sliding = false;
            CharacterMove.SetPlayer(true);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (PlayerInfo.Sliding && other.CompareTag(SlideEndTag))
            {
                JumpOff();        
            }
        }

        private void OnDrawGizmos(){
            Gizmos.DrawWireSphere(targetPosition, 1);
            for (int i = 0; i < points.Count-1; i++){
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
