using UnityEngine;

public class GunController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    private float targetAngle = 0f;
    private bool shouldRotate = false;

    void Update()
    {
        if (shouldRotate)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void MoveInDirection(float angle)
    {
        targetAngle = angle;
        shouldRotate = true;
        Debug.Log("🧭 Rotating to " + angle + "°");
    }

    public void StopMoving()
    {
        shouldRotate = false;
        Debug.Log("⛔ Stop rotation");
    }
}
