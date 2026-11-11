using TMPro;
using UnityEngine;

public class HelloBuildUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    void Awake()
    {
        Debug.Log("HelloBuildUI Awake");
        if (label == null) Debug.LogError("Label TMP is NULL (assign it in Inspector)");
    }

    void Update()
    {
        if (label != null)
            label.text = "HELLO BUILD  frame=" + Time.frameCount;
    }
}
