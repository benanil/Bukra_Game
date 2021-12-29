
using UnityEngine;

using AnilTools;
using Player;

public class CameraEventController : Singleton<CameraEventController>
{
    [SerializeField]
    private Animation animation;

    [SerializeField]
    private AnimationClip[] AnimationNames;

    private void Start()
    {
        animation = Mathmatic.camera.GetComponent<Animation>();
    }

    public void Play(byte animId)
    {
        StopAllMotorFonctions();
        animation.Play(AnimationNames[animId].name, PlayMode.StopAll);
    }

    // kamera animasyonu baslamadan once baslatılmalı
    private void StopAllMotorFonctions()
    { 
        CharacterMove.CanMove = false;
        CharacterMove.canLook = false;
        GameMenu.instance.canvases.BaseCanvas.SetActive(false);
    }

    // animation event ile bitir
    private void Analysis()
    { 
        CharacterMove.CanMove = true;
        CharacterMove.canLook = true;
        GameMenu.instance.canvases.BaseCanvas.SetActive(true);
    }
}
