using AnilTools;
using Player;
using UnityEngine;

public class Map2dCamera : MonoBehaviour
{
    private Transform player;

    private const sbyte zOffset = 0;

    private void Start(){
        player = CharacterMove.instance.transform;
    }

    private void LateUpdate(){
        transform.position = transform.position.WithoutY(player.position.x , player.position.z + zOffset);
    }
}
