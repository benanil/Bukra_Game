using Dialog;
using UnityEngine;
using UnityEngine.Events;
using static GameConstants;

[RequireComponent(typeof(MeshRenderer), typeof(BoxCollider))]
public class StageTrigger : MonoBehaviour
{
    public string Name;
    public UnityEvent Event;

    public int id;

    [SerializeField] private MissionHandeller missionHandeller;
    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();
    private BoxCollider _collider => GetComponent<BoxCollider>();

    [SerializeField] private GameObject visual;

    private void Start()
    {
        if (string.IsNullOrEmpty(Name)) Name = name;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player) && id == MissionHandeller.CurrentStage)
        {
            // görev stagesine geldi
            InvokeEvent();
            missionHandeller.OnPlayerNextStage();
            visual.SetActive(false);
            _collider.enabled = false;
        }
    }

    internal void InvokeEvent()
    {
        Event.Invoke();
    }

    public void OnEnabled()
    {
        Debug2.Log("enabled");
        MiniMap.instance.SetTargetPosition(transform.position);
        visual.SetActive(true);
    }
}