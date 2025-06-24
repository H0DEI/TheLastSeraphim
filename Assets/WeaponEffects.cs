using System.Collections;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    public GameObject weapon;
    public GameObject gun;
    public GameObject back;

    public GameObject muzzleFlash;
    public GameObject gunFlash;

    public GameObject special;

    public GameObject grenade;
    public GameObject explosion;
    public GameObject heal;
    public GameObject shield;
    public GameObject bubble;

    public GameObject root;

    public void ApagarLuzJugador()
    {
        GameManager.instance.Linterna.SetActive(false);
    }

    public void EncenderLuzJugador()
    {
        GameManager.instance.Linterna.SetActive(true);
    }

    public void Girar90Y()
    {
        root.transform.rotation = Quaternion.Euler(
            root.transform.eulerAngles.x,
            root.transform.eulerAngles.y + 90f,
            root.transform.eulerAngles.z
        );
    }

    public void RevertirRotacion()
    {
        root.transform.rotation = Quaternion.Euler(
            root.transform.eulerAngles.x,
            root.transform.eulerAngles.y - 90f,
            root.transform.eulerAngles.z
        );
    }

    public void ActivateWeapon()
    {
        weapon.SetActive(true);
    }

    public void DeactivateWeapon()
    {
        weapon.SetActive(false);
    }

    public void ActivateBack()
    {
        back.SetActive(true);
    }

    public void DeactivateBack()
    {
        back.SetActive(false);
    }

    public void ActivateGun()
    {
        gun.SetActive(true);
    }

    public void DeactivateGun()
    {
        gun.SetActive(false);
    }

    public void ActivateShield()
    {
        shield.SetActive(true);
    }

    public void DeactivateShield()
    {
        shield.SetActive(false);
    }
    public void ActivateBubble()
    {
        bubble.SetActive(true);
    }

    public void DeactivateBubble()
    {
        bubble.SetActive(false);
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

    public void TriggerGunFlash()
    {
        EffectRoutine(gunFlash);
    }
    public void TriggerSpecial()
    {
        EffectRoutine(special);
    }

    public void TriggerExplosion()
    {
        EffectRoutine(explosion);
    }
    public void TriggerHeal()
    {
        EffectRoutine(heal);
    }

    private void EffectRoutine(GameObject effect)
    {
        effect.GetComponent<ParticleSystem>().Play();
    }
}