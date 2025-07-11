using UnityEngine;

public class PlayerRotationController : MonoBehaviour
{
    public float rotationSpeed = 100f; // Degrees per second

    public void RotateLeft()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    public void RotateRight()
    {
        transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
