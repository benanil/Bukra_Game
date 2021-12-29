


using UnityEngine;

namespace MiddleGames
{ 
    public class WillageCenter : MonoBehaviour
    {
        public Bounds bounds;

        private void Reset()
        {
            bounds.center = transform.position;
            bounds.extents = new Vector3(10, 5, 10);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}