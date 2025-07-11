using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

public class GestureInput : MonoBehaviour
{
    public GunController gun;   // Assign your Gun parent in Inspector

    private string lastGesture = "";

    void Start()
    {
        if (gun == null)
        {
            Debug.LogError("❗ GunController not assigned in GestureInput");
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
                        gun.Reload();  // We'll add this method
                    }
                    else
                    {
                        // Optional: stop firing on unknown gestures
                        gun.StopFiring();
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
