using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

public class GestureInput : MonoBehaviour
{
    public GunController gun;   // Assign your Gun parent in Inspector
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

            if (json == null)
            {
                Debug.LogWarning("⚠️ Failed to parse JSON response");
                continue;
            }

            string gesture = json["gesture"];
            float angle = json["angle"].AsFloat;

            if (gesture != lastGesture)
            {
                Debug.Log("🖐 Gesture changed: " + gesture);
                lastGesture = gesture;
            }

            if (gun != null)
            {
                if (gesture == "fire")
                {
                    gun.Fire();
                }
                else if (gesture == "idle")
                {
                    gun.StopFiring();
                }
                else if (gesture == "reload")
                {
                    gun.Reload();
                }
                else
                {
                    gun.StopFiring();
                }
            }

            // ✅ Rotation: apply only when angle is strong
            if (rotationController != null)
            {
                if (Mathf.Abs(angle) > 20f)
                {
                    if (angle > 0)
                        rotationController.RotateRight();
                    else
                        rotationController.RotateLeft();
                }
                // else: do nothing, no rotation
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
