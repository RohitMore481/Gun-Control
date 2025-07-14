using UnityEngine;
using System.Collections;

public class FiringAnimator : MonoBehaviour
{
    [Header("Animation Timing")]
    public float idleFrameRate = 0.2f;
    public float fireFrameRate = 0.1f;
    public float reloadFrameRate = 0.05f;

    private Sprite[] idleFrames;
    private Sprite[] fireFrames;
    private Sprite[] reloadFrames;

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
        if (isReloading || sr == null) return;

        timer += Time.deltaTime;

        if (isFiring && fireFrames != null && fireFrames.Length > 0)
        {
            if (timer >= fireFrameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % fireFrames.Length;
                sr.sprite = fireFrames[currentFrame];
                // Debug.Log("🔥 Firing Frame: " + currentFrame);
            }
        }
        else if (!isFiring && idleFrames != null && idleFrames.Length > 0)
        {
            if (timer >= idleFrameRate)
            {
                timer = 0f;
                currentFrame = (currentFrame + 1) % idleFrames.Length;
                sr.sprite = idleFrames[currentFrame];
                // Debug.Log("🧍 Idle Frame: " + currentFrame);
            }
        }
    }

    public void SetFrames(Sprite[] newIdle, Sprite[] newFire, Sprite[] newReload)
    {
        // Only reset if actual change happens
        if (idleFrames != newIdle || fireFrames != newFire || reloadFrames != newReload)
        {
            idleFrames = newIdle;
            fireFrames = newFire;
            reloadFrames = newReload;

            currentFrame = 0;
            timer = 0f;

            if (idleFrames != null && idleFrames.Length > 0)
                sr.sprite = idleFrames[0];
        }
    }

    public void StartFiring()
    {
        if (isReloading || fireFrames == null || fireFrames.Length == 0) return;
        if (isFiring) return; // Already firing, don't reset

        isFiring = true;
        currentFrame = 0;
        timer = 0f;
    }

    public void StopFiring()
    {
        if (isReloading || !isFiring) return;

        isFiring = false;
        currentFrame = 0;
        timer = 0f;

        if (idleFrames != null && idleFrames.Length > 0)
            sr.sprite = idleFrames[0];
    }

    public void PlayReload()
    {
        if (!isReloading && reloadFrames != null && reloadFrames.Length > 0)
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
        timer = 0f;

        if (idleFrames != null && idleFrames.Length > 0)
            sr.sprite = idleFrames[0];
    }
}
