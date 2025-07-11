using UnityEngine;


public class GunController : MonoBehaviour
{
    public FiringAnimator animator;

    public void Fire()
    {
        animator.StartFiring();
    }

    public void StopFiring()
    {
        animator.StopFiring();
    }

    public void Reload()
    {
        animator.PlayReload();
    }
}
