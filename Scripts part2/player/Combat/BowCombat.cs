
using AnilTools;
using AnilTools.Move;
using UnityEngine;
using AnilTools.Update;
using UrFairy;
using static AnilTools.Mathmatic;
using static GameConstants;
using System.Linq;

namespace Player
{
    //oku kullanmak için yazılmıştır
    public partial class CombatControl
    {
        // camera olayı eklenicek
        // oku germeye başlayınca sola doğru gider

        private const byte BowGoRange = 80;

        private Transform camera;

        private void OkuGerme() {
            GameManager.instance.audioSource.PlayOneShot(BowReloadSound);
            Geriyor = true;
            ArrowCircale.enabled = true;
            animator.SetTrigger(BowDraw);
            var obj = PoolManager.instance.ReuseObject(Arrow, ArrowSpawnPos.position, ArrowSpawnPos.rotation);

            CurrentArow = obj.transform;// oku spawn et
            CurrentArow.transform.SetParent(rightHand);
            this.Delay(.2f, () => {
                YayıGer();
                bowAnimation.Play("strech",PlayMode.StopAll);
            });
        }

        private void OkuFırlatma(){
            GameManager.instance.audioSource.PlayOneShot(ArrowThrowSound);

            Geriyor = false;
            animator.SetTrigger(BowFire);

            arrow = CurrentArow.gameObject.AddComponent<Arrow>();
            CurrentArow.transform.parent = null;
            CurrentArow.transform.eulerAngles += Vector3.up * 30;

            if (RaycastFromCameraMidle(out hit,arrowHitDistance,PlayerInfo.Damagables))
            {
                if (hit.transform.CompareTag(Tags.Enemy) || hit.transform.CompareTag(Tags.Animal)){
                    var health = hit.transform.GetComponent<HealthBase>();
                    Debug.Log("damage : " + PlayerInfo.AttackDamage);

                    if (health && health.AddDamage(PlayerInfo.AttackDamage)){/* stats kill count arttır */}
                }
                arrow.Throw(hit , ArrowThrowSpeed);
            }
            else
                arrow.Throw(camera.position + camera.forward * BowGoRange);

            var task = bowUp.Move(bowUpStart, .5f, ReadyVeriables.EaseOut);
            var task1 = bowDown.Move(bowDownStart,.5f, ReadyVeriables.EaseOut);
            bowAnimation.Play("fire", PlayMode.StopAll);

            ArrowCircale.enabled = false;
            BowLine.SetPosition(1, BowMid);
            
            ArrowCircale.transform.localScale = Vector3.one;

            this.Delay(.5f, () =>
            {
                task.Dispose();
                task1.Dispose();
                RegisterUpdate.UpdateWhile(
                action: () => CharacterMove.instance.PositionAnchor = Vector3.Lerp(CharacterMove.instance.PositionAnchor, Vector3.up, CameraLerpSpeed * Time.deltaTime),
                endCnd: () => Geriyor == false);
            });
        }

        private void YayıGer(){
            CharacterMove.SetPlayer(false);
            
            RegisterUpdate.UpdateWhile(
            action:delegate
            {
                CharacterMove.instance.PositionAnchor = Vector3.Lerp(CharacterMove.instance.PositionAnchor,LookAnchor,CameraLerpSpeed * Time.deltaTime);
                ArrowCircale.transform.localScale = Vector3.Lerp(ArrowCircale.transform.localScale, ArrowCrosScale, CameraLerpSpeed   * Time.deltaTime);

                var PlayerTransform = NpcController2.Player;
                var targetPos = camera.transform.position + camera.right * BowGoRange;
                var targetAngle = Quaternion.LookRotation((targetPos.Y(0) - PlayerTransform.position.Y(0)).normalized, Vector3.up);

                PlayerTransform.rotation = Quaternion.Lerp(PlayerTransform.rotation, targetAngle,Time.deltaTime * 30);

                //fix it men
                BowLine.SetPosition(1, rightHand.position);
                CurrentArow.transform.position = rightHand.position;
            },
            endCnd: () => Geriyor == true,
            then: () =>
            { 
                CharacterMove.SetPlayer(true);
            }, calledInstance:this, updateType: UpdateType.lateUpdate);
        }

        private void LateUpdate(){
            if (handState == HandState.BOW)
            {
                // line için pozisyonlar
                BowLine.SetPosition(0, bowDown.position);
                BowLine.SetPosition(2, bowUp.position);
                if (!Geriyor)
                    BowLine.SetPosition(1, bowDown.position + (bowDown.position - bowUp.position).normalized * -.5f);
            }
        }
        //
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugObj && handState == HandState.BOW)
            {
                debugObj.transform.position = RayMidleScreen.GetPoint(arrowHitDistance);
                Debug.DrawLine(Mathmatic.camera.transform.position, debugObj.transform.position,Color.cyan);
            }
        }
#endif

    }
}