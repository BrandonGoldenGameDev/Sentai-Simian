using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SkinnedMeshRenderer rend;
    [SerializeField]
    private float duration = 1f;
    [SerializeField, GradientUsage(true)]
    private Gradient gradient;
    private float timer = 0f;

    private void Awake()
    {
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void Activate(Animator target)
    {
        Animator animator = GetComponent<Animator>();
        AnimatorStateInfo targetStateInfo = target.GetCurrentAnimatorStateInfo(0);
        animator.Play(targetStateInfo.shortNameHash, 0, targetStateInfo.normalizedTime);
        animator.speed = 0f;

        foreach (var param in target.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, target.GetFloat(param.name));
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, target.GetInteger(param.name));
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, target.GetBool(param.name));
                    break;
                case AnimatorControllerParameterType.Trigger:
                default:
                    break;
            }
        }

        Destroy(gameObject, duration);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        Color color = gradient.Evaluate(timer / duration);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetColor("_BaseColor", color);
        rend.SetPropertyBlock(block);
    }
}
