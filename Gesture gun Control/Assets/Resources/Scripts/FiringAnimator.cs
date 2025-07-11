using UnityEngine;
using System.Collections;

public class FiringAnimator : MonoBehaviour
{
    public Sprite[] fireFrames;         // 🔥 Fire animation frames
    public Sprite[] reloadFrames;       // 🔄 Reload animation frames

    public float fireFrameRate = 0.1f;      // Time per fire frame (seconds)
    public float reloadFrameRate = 0.05f;   // Time per reload frame (seconds)

    private SpriteRenderer sr;
    private bool isFiring = false;
    private bool isReloading = false;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFiring && !isReloading)
        {
            timer += Time.deltaTime;
            if (timer >= fireFrameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % fireFrames.Length;
                sr.sprite = fireFrames[currentFrame];
            }
        }
    }

    public void StartFiring()
    {
        if (isReloading) return;

        if (!isFiring)
        {
            isFiring = true;
            currentFrame = 0;
            timer = 0f;
        }
    }

    public void StopFiring()
    {
        if (isReloading) return;

        isFiring = false;
        currentFrame = 0;

        if (fireFrames.Length > 0)
            sr.sprite = fireFrames[0]; // Idle sprite
    }

    public void PlayReload()
    {
        if (!isReloading)
        {
            isFiring = false;
            StartCoroutine(ReloadSequence());
        }
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;

        for (int i = 0; i < reloadFrames.Length; i++)
        {
            sr.sprite = reloadFrames[i];
            yield return new WaitForSeconds(reloadFrameRate);
        }

        isReloading = false;
        currentFrame = 0;

        if (fireFrames.Length > 0)
            sr.sprite = fireFrames[0]; // Return to idle
    }
}
