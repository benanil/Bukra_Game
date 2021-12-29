
using AnilTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UrFairy;

public class Etkilesim : Singleton<Etkilesim> , IPointerClickHandler
{
    private static Image image;

    private void Start(){
        //image = GetComponent<Image>();
        /*image.color = Color.white;*/
        //image.enabled = false;
    }
    
    private void Update(){
        if (Input.GetKeyDown(KeyCode.E)){
            OnPointerClick(null);
        }
    }

    public static void Disable(){
    }

    // touch ile button
    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(Etkilesim))]
public class EtkilesimEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}

#endif


