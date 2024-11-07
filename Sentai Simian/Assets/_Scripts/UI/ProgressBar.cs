using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private Image fill;
    [SerializeField]
    private Image mask;

    public void SetFill(float fillAmount)
    {
        mask.fillAmount = fillAmount;
    }
}
