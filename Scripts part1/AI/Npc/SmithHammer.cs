using UnityEngine;

public class SmithHammer : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;
    private AudioClip HitClip;
    public AudioSource audioSource;

    void Start()
    {
        HitClip = Resources.Load("Sounds/other/sword_ding") as AudioClip;
    }

    void HammerHit()
    {
        particle.Stop();
        particle.Play();
        audioSource.PlayOneShot(HitClip);
    }
}
