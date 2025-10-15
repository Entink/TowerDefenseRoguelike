using UnityEngine;

public class SFXUnit : MonoBehaviour
{
    [SerializeField] private Transform sfxAnchor;

    private Transform Anchor => sfxAnchor != null ? sfxAnchor : transform;

    public void PlayAttack()
    {
        if (SfxManager.I != null)
            SfxManager.I.PlayWorldOneShot(SfxManager.I.unitAttack, Anchor.position, SfxManager.I.unitVolume);
    }

    public void PlayHit()
    {
        if (SfxManager.I != null)
            SfxManager.I.PlayWorldOneShot(SfxManager.I.unitHit, Anchor.position, SfxManager.I.unitVolume);
    }

    public void PlayDeath()
    {
        if (SfxManager.I != null)
            SfxManager.I.PlayWorldOneShot(SfxManager.I.unitDeath, Anchor.position, SfxManager.I.unitVolume);
    }
}
