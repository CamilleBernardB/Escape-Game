using UnityEngine;
using TMPro;

public class ShowPointCoords2 : MonoBehaviour
{
    [Header("What to read")]
    [SerializeField] Transform point;   // le point / perso que tu déplaces

    [Header("Where to display")]
    [SerializeField] TMP_Text label;    // le texte UI

    [Header("Settings")]
    [SerializeField] int decimals = 3;
    [SerializeField] bool useXZ = true; // true si ton perso bouge sur XZ (3D)

    void Awake()
    {
        if (point == null)
            Debug.LogError("ShowPointCoords2: point not assigned");

        if (label == null)
            Debug.LogError("ShowPointCoords2: label not assigned");
    }

    void Update()
    {
        if (point == null || label == null) return;

        Vector3 p = point.position;

        float lon = p.x;
        float lat = useXZ ? p.z : p.y;
        float alt = useXZ ? p.y : p.z;

        
        label.text =
    "lon: " + lon.ToString("F" + decimals) +
    "  lat: " + lat.ToString("F" + decimals) +
    "  alt: " + alt.ToString("F" + decimals);
    }
}
