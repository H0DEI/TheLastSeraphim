using UnityEngine;
using SHG.AnimatorCoder;

public class CharacterAnimator : AnimatorCoder
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        //CheckAttack();
        //DefaultAnimation(0);
        //
        //SetBool(Parameters.TESTPARAM, !Input.GetKey(KeyCode.LeftShift));
        //
        //void CheckAttack()
        //{
        //    if (GetBool(Parameters.TESTPARAM) && Input.GetKeyUp(KeyCode.Q)) Play(new(Animations.SHOOT1, true, new()));
        //}
    }

    public void PlayAnim(AnimationData animationData, int layer = 0)
    {
        Play(animationData, layer);
    }

    public void PlayCanv(AnimationData animationData, int layer = 0)
    {
        PlayCanvas(animationData, layer);
    }

    public override void DefaultAnimation(int layer)
    {
        //Debug.Log($"Llamando a DefaultAnimation en layer {layer}");
        Play(new(Animations.IDLE1));
    }

    public override void DefaultCanvasAnimation(int layer)
    {
        PlayCanvas(new(Animations.NOTHING));
    }
}
