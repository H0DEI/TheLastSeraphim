using System.Collections.Generic;
using UnityEngine;
using SHG.AnimatorCoder;

public class AnimationManager : MonoBehaviour
{
    // Diccionario para manejar múltiples personajes
    private Dictionary<string, CharacterAnimator> characters = new Dictionary<string, CharacterAnimator>();

    public void ClearCharacters()
    {
        characters.Clear();
    }

    /// <summary> Registra un nuevo personaje en el sistema. </summary>
    public void RegisterCharacter(string id, CharacterAnimator characterAnimator)
    {
        if (!characters.ContainsKey(id))
        {
            characters.Add(id, characterAnimator);
        }
        else
        {
            Debug.LogWarning($"El personaje con ID {id} ya está registrado.");
        }
    }

    /// <summary> Ejecuta una animación personalizada para un personaje específico. </summary>
    public void PlayAnimation(string id, AnimationData animationData, int layer = 0)
    {
        if (characters.TryGetValue(id, out CharacterAnimator characterAnimator))
        {
            characterAnimator.PlayAnim(animationData, layer);
        }
    }

    public void PlayCanvas(string id, AnimationData animationData, int layer = 0)
    {
        if (characters.TryGetValue(id, out CharacterAnimator characterAnimator))
        {
            characterAnimator.PlayCanv(animationData, layer);
        }
    }

    /// <summary> Bloquea o desbloquea un layer específico de un personaje. </summary>
    public void SetLayerLock(string id, bool lockLayer, int layer)
    {
        if (characters.TryGetValue(id, out CharacterAnimator characterAnimator))
        {
            characterAnimator.SetLocked(lockLayer, layer);
        }
        else
        {
            Debug.LogError($"Personaje con ID {id} no encontrado.");
        }
    }

    /// <summary> Reproduce una animación global en todos los personajes registrados. </summary>
    public void PlayGlobalAnimation(AnimationData animationData, int layer = 0)
    {
        foreach (var entry in characters)
        {
            PlayAnimation(entry.Key, animationData, layer);
        }
    }

    /// <summary> Resetea las animaciones de todos los personajes. </summary>
    public void ResetAllAnimations()
    {
        AnimationData resetData = new AnimationData(Animations.RESET);
        PlayGlobalAnimation(resetData);
    }
}

