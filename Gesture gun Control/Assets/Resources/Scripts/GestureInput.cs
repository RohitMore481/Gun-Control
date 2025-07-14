using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

public class GestureInput : MonoBehaviour
{
    public GunController gun;   // Assign in Inspector
    public PlayerRotationController rotationController; // Assign in Inspector

    private string lastGesture = "";

    void Start()
    {
        if (gun == null)
        {
            Debug.LogError("❗ GunController not assigned in GestureInput");
        }
        if (rotationController == null)
        {
            Debug.LogError("❗ RotationController not assigned in GestureInput");
        }

        StartCoroutine(CheckGesture());
    }

    IEnumerator CheckGesture()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get("http://127.0.0.1:5000/gesture");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                var json = JSON.Parse(response);

                if (json == null || !json.HasKey("gesture") || !json.HasKey("angle") || !json.HasKey("left_gesture"))
                {
                    Debug.LogWarning("⚠️ Invalid JSON response or missing keys.");
                    continue;
                }

                string gesture = json["gesture"];          // Right hand
                string leftGesture = json["left_gesture"]; // Left hand
                float angle = json["angle"].AsFloat;

                // 🔄 Only log if right-hand gesture changed
                if (gesture != lastGesture)
                {
                    Debug.Log("🖐 Gesture changed: " + gesture);
                    lastGesture = gesture;
                }

                // 🔫 Gun controls
                if (gun != null)
                {
                    if (gesture == "fire")
                        gun.Fire();
                    else if (gesture == "reload")
                        gun.Reload();
                    else
                        gun.StopFiring();

                    gun.TrySwitchGun(leftGesture); // ✌️ V gesture = switch weapon
                }

                // ↪️ Rotation control
                if (rotationController != null)
                {
                    if (Mathf.Abs(angle) > 20f)
                    {
                        if (angle > 0)
                            rotationController.RotateRight();
                        else
                            rotationController.RotateLeft();
                    }
                }
            }
            else
            {
                Debug.LogError("🚫 Request failed: " + www.error);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
