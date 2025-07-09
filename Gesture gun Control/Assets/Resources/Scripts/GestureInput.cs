using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

public class GestureInput : MonoBehaviour
{
    public GunController gun; // Drag your player here in Inspector

    void Start()
    {
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
            Debug.Log("📦 Raw Response: " + response);  // 🔍 Watch this in console

            var json = JSON.Parse(response);
            if (json == null)
            {
                Debug.LogError("❌ Failed to parse JSON");
                yield break;
            }

            string gesture = json["gesture"];
            float angle = json["angle"].AsFloat;

            Debug.Log("🖐 Gesture: " + gesture + " | 📐 Angle: " + angle);

            if (gesture == "pistol" && gun != null)
            {
                gun.MoveInDirection(angle);
            }
            else if (gun != null)
            {
                gun.StopMoving();
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
