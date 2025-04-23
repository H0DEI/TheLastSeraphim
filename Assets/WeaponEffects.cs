using System.Collections;
using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    public GameObject muzzleFlash;

    public void TriggerMuzzleFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f); // Duración muy corta
        muzzleFlash.SetActive(false);
    }
}