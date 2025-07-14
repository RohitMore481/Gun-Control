using UnityEngine;
using TMPro;


[System.Serializable]
public class GunData
{
    public string gunName;
    public Sprite[] idleFrames;
    public Sprite[] fireFrames;
    public Sprite[] reloadFrames;

    public int roundCapacity = 100;     // Max bullets per round
    public Transform firePoint;         // Where bullets spawn from
    public float bulletSpeed = 10f;     // Speed at which bullets travel
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

    [Header("Ammo System")]
    public int currentBullets;                   // Bullets left in current round
    public TextMeshProUGUI bulletText;           // UI: "10 / 100"
    public GameObject reloadText;                // UI: shows when bullets = 0

    [Header("Bullet System")]
    public GameObject bulletPrefab;              // Bullet prefab to spawn

    void Start()
    {
        if (guns.Length == 0)
        {
            Debug.LogError("❗ No guns assigned!");
            return;
        }

        currentGun = guns[currentGunIndex];
        currentBullets = currentGun.roundCapacity;
        ApplyCurrentGunFrames();
        UpdateBulletUI();
    }

    void Update()
    {
        // For testing: press R to reload manually
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Fire()
{
    if (currentBullets > 0)
    {
        if (animator != null)
        {
            animator.StartFiring();
        }

        Debug.Log("🔫 Firing: " + currentGun.gunName);

        // Spawn the bullet at firePoint position and facing direction
        GameObject bullet = Instantiate(bulletPrefab, currentGun.firePoint.position, currentGun.firePoint.rotation); // ✅ correct


        // Set bullet velocity in the direction firePoint is facing (green arrow)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = currentGun.firePoint.up * currentGun.bulletSpeed;
        }

        currentBullets--;
        UpdateBulletUI();
    }
    else
    {
        Debug.Log("❌ Out of bullets! Press reload.");
    }
}


    public void StopFiring()
    {
        animator?.StopFiring();
    }

    public void Reload()
    {
        if (currentBullets == currentGun.roundCapacity)
        {
            Debug.Log("⛔ Round is already full.");
            return;
        }

        animator?.PlayReload();
        currentBullets = currentGun.roundCapacity;
        UpdateBulletUI();

        Debug.Log("🔁 Reloaded to full: " + currentGun.roundCapacity);
    }

    public void TrySwitchGun(string leftGesture)
    {
        if (leftGesture == "v" && lastLeftGesture != "v")
        {
            currentGunIndex = (currentGunIndex + 1) % guns.Length;
            currentGun = guns[currentGunIndex];

            ApplyCurrentGunFrames();
            currentBullets = currentGun.roundCapacity;
            UpdateBulletUI();

            Debug.Log("🔁 Switched to: " + currentGun.gunName);
        }

        lastLeftGesture = leftGesture;
    }

    private void ApplyCurrentGunFrames()
    {
        animator?.SetFrames(
            currentGun.idleFrames,
            currentGun.fireFrames,
            currentGun.reloadFrames
        );
    }

    private void UpdateBulletUI()
    {
        if (bulletText != null)
            bulletText.text = $"{currentBullets} / {currentGun.roundCapacity}";

        if (reloadText != null)
            reloadText.SetActive(currentBullets == 0);
    }

    public string GetCurrentGunName()
    {
        return currentGun.gunName;
    }
}
