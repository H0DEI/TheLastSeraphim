using System.Collections;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    public GameObject weapon;
    public GameObject weapon2;
    public GameObject gun;
    public GameObject back;

    public GameObject muzzleFlash;
    public GameObject muzzlePlasma;
    public GameObject gunFlash;

    public GameObject special;
    public GameObject bigExplosion;

    public GameObject grenade;
    public GameObject grenade2;
    public GameObject explosion;
    public GameObject gas;
    public GameObject plasma;
    public GameObject heal;
    public GameObject shield;
    public GameObject bubble;

    public GameObject root;

    // Mantén este para las habilidades antiguas ↓
    public void EventoImpacto() // sin parámetros, habilidades antiguas
    {
        GameManager.EmitirImpactoHabilidad();
    }

    public void EventoImpactoConDuracion(int frames) // NUEVO para animaciones con ventana
    {
        GameManager.EmitirVentanaImpacto(frames);
        GameManager.EmitirImpactoHabilidad();
    }

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

    public void ActivateWeapon2()
    {
        weapon2.SetActive(true);
    }

    public void DeactivateWeapon2()
    {
        weapon2.SetActive(false);
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

    public void ActivateGrenade2()
    {
        grenade2.SetActive(true);
    }

    public void DeactivateGrenade2()
    {
        grenade2.SetActive(false);
    }

    public void TriggerMuzzleFlash()
    {
        EffectRoutine(muzzleFlash);
    }

    public void TriggerMuzzlePlasma()
    {
        EffectRoutine(muzzlePlasma);
    }

    public void TriggerGunFlash()
    {
        EffectRoutine(gunFlash);
    }
    public void TriggerSpecial()
    {
        EffectRoutine(special);
    }
    public void TriggerBigExplosion()
    {
        EffectRoutine(bigExplosion);
    }

    public void TriggerExplosion()
    {
        EffectRoutine(explosion);
    }

    public void TriggerGas()
    {
        EffectRoutine(gas);
    }

    public void TriggerPlasma()
    {
        EffectRoutine(plasma);
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