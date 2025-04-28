using System.Collections;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    public GameObject weapon;
    public GameObject muzzleFlash;

    public GameObject grenade;
    public GameObject explosion;

    public void ActivateWeapon()
    {
        weapon.SetActive(true);
    }

    public void DeactivateWeapon()
    {
        weapon.SetActive(false);
    }
    public void ActivateGrenade()
    {
        grenade.SetActive(true);
    }

    public void DeactivateGrenade()
    {
        grenade.SetActive(false);
    }

    public void TriggerMuzzleFlash()
    {
        EffectRoutine(muzzleFlash);
    }

    public void TriggerExplosion()
    {
        EffectRoutine(explosion);
    }

    private void EffectRoutine(GameObject effect)
    {
        effect.GetComponent<ParticleSystem>().Play();
    }
}