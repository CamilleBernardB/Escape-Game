using TMPro;
using UnityEngine;

public class ShowPointCoords : MonoBehaviour
{
    [Header("What to read")]
    [SerializeField] Transform point;   // personnage / point déplacé

    [Header("Where to display")]
    [SerializeField] TMP_Text label;    // Text (TMP) UI

    [Header("Formatting")]
    [SerializeField] int decimals = 3;
    [SerializeField] bool showAltitude = true; // altitude = Y ou Z selon ton jeu
    [SerializeField] bool altitudeIsZ = false; // mets true si ton altitude est Z (souvent en 3D)

    void Awake()
    {
        if (point == null) Debug.LogError("ShowPointCoords: 'point' not assigned.");
        if (label == null) Debug.LogError("ShowPointCoords: 'label' not assigned.");
    }

    void Update()
    {
        if (point == null || label == null) return;

        Vector3 p = point.position;

        // Convention: longitude = X, latitude = Y (2D) ou Z (3D)
        float lon = p.x;
        float lat = altitudeIsZ ? p.z : p.y;
        float alt = altitudeIsZ ? p.y : p.z; // l'autre axe en “altitude” si tu veux

        if (!showAltitude)
        {
            label.text = $"lon: {lon.ToString($"F{decimals}")}   lat: {lat.ToString($"F{decimals}")}";
        }
        else
        {
            label.text =
                $"lon: {lon.ToString($"F{decimals}")}   lat: {lat.ToString($"F{decimals}")}   alt: {alt.ToString($"F{decimals}")}";
        }
    }
}
