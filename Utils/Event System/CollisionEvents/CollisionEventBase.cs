using UnityEngine;

namespace AnilTools.Events
{
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionEventBase : MonoBehaviour
    {

#if UNITY_EDITOR
        [SerializeField]
        protected Color Color;

        public void OnDrawGizmos()
        {
            Gizmos.color = Color;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        private void Start()
        {
            if (GetComponent<Collider>() == null)
            {
                Debug.Log("event için collider gerekli");
            }
        }
#endif

    }
}