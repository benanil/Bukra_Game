

using AnilTools.Update;
using System;
using UnityEngine;
using UrFairy;

namespace AnilTools.Lerping
{
    public static class Lerp
    {
        // Animator
        /// <param name="senderId">for debug</param>
        public static UpdateTask LerpAnim(this Animator animator, int hashset, float value, float speed, Action then = null , float stopValue = 0.1f)
        {
            var task = new UpdateTask(
                () => animator.SetFloatLerp(hashset, value, speed), 
                () => animator.Difrance(hashset, value) > stopValue, then, animator.GetInstanceID());
            
            AnilUpdate.Register(task);
            return task;
        }
        
        /// <param name="senderId">for debug</param>
        public static UpdateTask ColorAnim(this Material material, string id,Color target,float speed, Action then = null , int senderId = 0)
        {
            var task = new UpdateTask(
                () => material.SetColor(id,Color.Lerp(material.GetColor(id), target, speed * Time.deltaTime)),
                () => material.GetColor(id).Difrance(target) > 0.01f, then , senderId);

            AnilUpdate.Register(task);
            return task;
        }

        /// <param name="senderId">for debug</param>
        public static UpdateTask ColorAnim(this Material material, int id, Color target, float speed, Action then = null, int senderId = 0)
        {

            var task = new UpdateTask(
                () => material.SetColor(id, Color.Lerp(material.GetColor(id), target, speed * Time.deltaTime)),
                () => material.GetColor(id).Difrance(target) > 0.01f, then, senderId);

            AnilUpdate.Register(task);
            return task;
        }

        /// <param name="senderId">for debug</param>
        public static UpdateTask FloatAnim(this Material material, int id, float target, float speed, Action then = null, int senderId = 0)
        {

            var task = new UpdateTask(
                () => material.SetFloat(id, Mathf.Lerp(material.GetFloat(id), target, speed * Time.deltaTime)),
                () => material.GetFloat(id).Difrance(target) > 0.01f, then, senderId);

            AnilUpdate.Register(task);
            return task;
        }

        /// <param name="senderId">for debug</param>
        public static UpdateTask FloatAnim(this Material material, string id, float target, float speed, Action then = null, int senderId = 0)
        {

            var task = new UpdateTask(
                () => material.SetFloat(id, Mathf.Lerp(material.GetFloat(id), target, speed * Time.deltaTime)),
                () => material.GetFloat(id).Difrance(target) > 0.01f, then, senderId);

            AnilUpdate.Register(task);
            return task;
        }

        public static UpdateTask AlphaFade(this Material material, float value , float speed , Action then = null, int propertyId = 0)
        {
            if (propertyId == 0)
                propertyId = ShaderPool.Color;

            material.shader = ShaderPool.FadeShader;

            var task = RegisterUpdate.UpdateWhile(
                () => material.Color(material.color.A(x => Mathf.Lerp(x, value, speed * Time.deltaTime))),
                () => material.color.a.Difrance(value) > 0.02f , then , material);

            AnilUpdate.Register(task);

            return task;
        }

        public static Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float interpolateAmount)
        {
            Vector3 ab = Vector3.Lerp(a, b, 1);
            Vector3 bc = Vector3.Lerp(b, c, 1);

            return Vector3.Lerp(ab, bc, interpolateAmount);
        }

        /// <summary>
        /// 2 nokta arasında bombeli gidiş
        /// </summary>
        /// <param name="a">gidecek olan</param>
        /// <param name="b">baslangıc noktasi</param>
        /// <param name="c">yukari noktasi</param>
        /// <param name="d">bitis noktasi</param>
        /// <param name="interpolateAmount"></param>
        public static Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float interpolateAmount)
        {
            Vector3 ab_bc = QuadraticLerp(a, b, c, interpolateAmount);
            Vector3 bc_cd = QuadraticLerp(b, c, d, interpolateAmount);
            return Vector3.Lerp(ab_bc, bc_cd, interpolateAmount);
        }


        /// <summary>
        /// 2 nokta arasında bombeli gidiş
        /// </summary>
        /// <param name="transform">gidecek olan</param>
        /// <param name="b">baslangıc noktasi</param>
        /// <param name="c">yukari noktasi</param>
        /// <param name="d">bitis noktasi</param>
        /// <param name="senderId">for debug or multiple times</param>
        /// <param name="interpolateAmount"></param>
        public static UpdateTask CubicLerpAnim(this Transform transform, Vector3 b, Vector3 c , Vector3 d , float speed,AnimationCurve curve = null ,Action then = null , int senderId = 0)
        {
            Tuple1<float> currentAmount = new Tuple1<float>(0);

            if (curve == null)
            {
                curve = AnimationCurve.Linear(0, 0, 1, 1);
            }

            var task = new UpdateTask(
                () =>
                {
                    currentAmount.value = Mathmatic.Max(currentAmount.value + speed * Time.deltaTime , 1);
                    transform.position = CubicLerp(transform.position, b, c, d, curve.Evaluate(currentAmount.value));
                },
                () => transform.Distance(d) > 0.01f, then, senderId);

            AnilUpdate.Register(task);

            return task;
        }

        // ses , canvas , sprite , material , uı

    }
}