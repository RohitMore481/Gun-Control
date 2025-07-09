using UnityEngine;

public class GunController : MonoBehaviour
{
    public float rotationAngle = 45f; // Max angle left/right
    public float rotationSpeed = 2f;  // Speed of sweeping

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Shoot()
    {
        Debug.Log("🔫 Pew! Shot at angle: " + transform.rotation.eulerAngles.z);
    }
}
