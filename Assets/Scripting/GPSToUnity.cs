using UnityEngine;
using TMPro;
using System.Collections;

public class GpsToUnity : MonoBehaviour
{
    [Header("Target to move")]
    [SerializeField] Transform player;          // Le point/perso à placer sur la carte

    [Header("Optional UI debug")]
    [SerializeField] TMP_Text debugLabel;       // Un Text (TMP) UI pour afficher lat/lon etc. (optionnel)

    [Header("Reference GPS (this point = unityOrigin)")]
    [SerializeField] double lat0 = 48.856614;   // Latitude du point de référence (ex: centre de ta carte)
    [SerializeField] double lon0 = 2.3522219;   // Longitude du point de référence

    [Header("Reference Unity position")]
    [SerializeField] Vector3 unityOrigin = Vector3.zero;  // Position Unity correspondant à (lat0, lon0)

    [Header("Map alignment")]
    [SerializeField] bool useXZPlane = true;    // true: carte au sol (X,Z). false: 2D (X,Y)
    [SerializeField] float unitsPerMeter = 1f;  // échelle: 1 unity unit = 1 m (à ajuster selon ta carte)
    [SerializeField] float rotationDegrees = 0f;// rotation de la carte autour de l’axe vertical (en degrés)

    [Header("GPS settings")]
    [SerializeField] float refreshSeconds = 0.5f;
    [SerializeField] float desiredAccuracyMeters = 5f;
    [SerializeField] float updateDistanceMeters = 1f;

    [Header("Smoothing (recommended)")]
    [SerializeField] bool smooth = true;
    [SerializeField] float smoothSpeed = 3f;    // plus grand = suit plus vite

    const double R = 6378137.0; // rayon terrestre (m), WGS84

    IEnumerator Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (player == null)
        {
            Debug.LogError("GpsToUnity: player not assigned.");
            yield break;
        }

        if (!Input.location.isEnabledByUser)
        {
            if (debugLabel) debugLabel.text = "GPS: désactivé sur l'appareil.";
            yield break;
        }

        Input.location.Start(desiredAccuracyMeters, updateDistanceMeters);

        if (debugLabel) debugLabel.text = "GPS: initialisation...";
        int maxWait = 15;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
            yield return new WaitForSeconds(1);

        if (Input.location.status != LocationServiceStatus.Running)
        {
            if (debugLabel) debugLabel.text = "GPS: échec (" + Input.location.status + ")";
            Debug.LogError("GpsToUnity: LocationService not running: " + Input.location.status);
            yield break;
        }

        if (debugLabel) debugLabel.text = "GPS: OK";

        while (true)
        {
            var d = Input.location.lastData;
            double lat = d.latitude;
            double lon = d.longitude;
            float alt = d.altitude;
            float acc = d.horizontalAccuracy;

            if (acc > 15f)
            {
                if (debugLabel) debugLabel.text = $"GPS bruité (acc {acc:F1}m) -> on ignore";
                yield return new WaitForSeconds(refreshSeconds);
                continue;
            }


            Vector3 targetPos = GpsToUnityPosition(lat, lon);

            if (smooth)
            {
                float t = 1f - Mathf.Exp(-smoothSpeed * refreshSeconds);
                player.position = Vector3.Lerp(player.position, targetPos, t);
            }
            else
            {
                player.position = targetPos;
            }


            if (debugLabel)
            {
                debugLabel.text =
                    $"lat: {lat:F6}  lon: {lon:F6}\n" +
                    $"acc: {acc:F1}m  alt: {alt:F1}m\n" +
                    $"unity: {player.position.x:F2}, {player.position.y:F2}, {player.position.z:F2}";
            }

            yield return new WaitForSeconds(refreshSeconds);
        }
#else
        if (debugLabel) debugLabel.text = "GPS non dispo sur PC (test sur Android).";
        yield break;
#endif

    }

    Vector3 GpsToUnityPosition(double lat, double lon)
    {
        // degrés -> radians
        double latRad = lat * Mathf.Deg2Rad;
        double lonRad = lon * Mathf.Deg2Rad;
        double lat0Rad = lat0 * Mathf.Deg2Rad;
        double lon0Rad = lon0 * Mathf.Deg2Rad;

        // differences
        double dLat = latRad - lat0Rad;
        double dLon = lonRad - lon0Rad;

        // conversion en mètres (plan tangent local)
        double eastMeters  = R * System.Math.Cos(lat0Rad) * dLon;
        double northMeters = R * dLat;

        // mètres -> unity units
        float x = (float)(eastMeters * unitsPerMeter);
        float z = (float)(northMeters * unitsPerMeter);

        // rotation de l’axe (pour aligner ta carte si elle est tournée)
        Quaternion rot = Quaternion.Euler(0f, rotationDegrees, 0f);

        if (useXZPlane)
        {
            Vector3 local = new Vector3(x, 0f, z);
            return unityOrigin + rot * local;
        }
        else
        {
            // 2D XY: east -> X, north -> Y
            Vector3 local = new Vector3(x, z, 0f);
            return unityOrigin + local; // rotation pas utile en 2D XY dans la plupart des cas
        }
    }
}
