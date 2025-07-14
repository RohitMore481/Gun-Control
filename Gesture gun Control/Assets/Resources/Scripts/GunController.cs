using UnityEngine;

[System.Serializable]
public class GunData
{
    public string gunName;
    public Sprite[] idleFrames;
    public Sprite[] fireFrames;
    public Sprite[] reloadFrames;
}

public class GunController : MonoBehaviour
{
    [Header("Guns")]
    public GunData[] guns;
    private int currentGunIndex = 0;
    private GunData currentGun;
    private string lastLeftGesture = "";

    [Header("Animation")]
    public FiringAnimator animator;

    void Start()
    {
        if (guns.Length == 0)
        {
            Debug.LogError("❗ No guns assigned!");
            return;
        }

        currentGun = guns[currentGunIndex];
        ApplyCurrentGunFrames();
    }

    public void Fire()
    {
        if (animator != null)
        {
            animator.StartFiring();
            Debug.Log("🔫 Firing: " + currentGun.gunName);
        }
    }

    public void StopFiring()
    {
        if (animator != null)
        {
            animator.StopFiring();
        }
    }

    public void Reload()
    {
        if (animator != null)
        {
            animator.PlayReload();
        }
    }

    public void TrySwitchGun(string leftGesture)
    {
        // Switch gun only if gesture changed to "v"
        if (leftGesture == "v" && lastLeftGesture != "v")
        {
            currentGunIndex = (currentGunIndex + 1) % guns.Length;
            currentGun = guns[currentGunIndex];

            ApplyCurrentGunFrames();

            Debug.Log("🔁 Switched to: " + currentGun.gunName);
        }

        lastLeftGesture = leftGesture;
    }

    private void ApplyCurrentGunFrames()
    {
        if (animator != null)
        {
            animator.SetFrames(
                currentGun.idleFrames,
                currentGun.fireFrames,
                currentGun.reloadFrames
            );
        }
    }

    public string GetCurrentGunName()
    {
        return currentGun.gunName;
    }
}
