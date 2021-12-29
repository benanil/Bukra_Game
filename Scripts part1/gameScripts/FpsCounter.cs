using UnityEngine;
using UnityEngine.UI;

using Sstring = StringOperationUtil.OptimizedStringOperation;

public class FpsCounter : MonoBehaviour
{
    private const string fps = "FPS: ";
    [SerializeField] private Text _fpsText;

    private void Start()
    {
        InvokeRepeating(nameof(FpsUpdate), 1.5f, 1.11f);
    }

    private void FpsUpdate()
    {
        byte fps = (byte)(1f / Time.unscaledDeltaTime);
        _fpsText.text = Sstring.small + FpsCounter.fps + fps;
    }
}