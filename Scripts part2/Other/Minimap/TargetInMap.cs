using AnilTools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UrFairy;

public class TargetInMap : MonoBehaviour
{
    private const float radian = 50; // meal 2x radius

    public Vector3 targetPosition => MiniMap.instance.TargetPosition;
    private const byte FarDist = 20;

    private float distance;

    private Transform PlayerTransform;
    private RectTransform rectTransform;
    private List<Image> images;
    private RectTransform minimap;

    private bool _canUpdate;
    private bool CanUpdate
    {
        get => _canUpdate;
        set
        {
            if (value == true && _canUpdate == false)
                images.ForEach(x => x.enabled = true);
            else if (value == false && _canUpdate == true)
                images.ForEach(x => x.enabled = false);
            
            _canUpdate = value;
        }
    }

    private void Start()
    {
        PlayerTransform = NpcController2.Player;
        rectTransform = GetComponent<RectTransform>();
        images = GetComponentsInChildren<Image>().ToList();
        images.Add(GetComponent<Image>());
        minimap = MiniMap.instance.transform.parent.parent.GetComponent<RectTransform>();
        this.UpdateCoroutine(1, () =>
        {
            distance = Vector3.Distance(targetPosition,PlayerTransform.position);
            CanUpdate = distance > FarDist;
        }, () => false);
    }

    private void Update()
    {
        if (!CanUpdate) return;
        minimap.localEulerAngles = minimap.localEulerAngles.Z(Mathmatic.camera.transform.localEulerAngles.y);
        var position = (targetPosition - PlayerTransform.position).normalized * radian;
        rectTransform.anchoredPosition = new Vector3(position.x, position.z, 0);
    }
}
