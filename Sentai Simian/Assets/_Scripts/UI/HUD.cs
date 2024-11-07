using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    private PlayerHealth player;
    private Repulsor repulsor;
    private Hookshot hookshot;
    private UltimateAbility ultimate;
    [SerializeField]
    private ProgressBar healthBar;
    [SerializeField]
    private ProgressBar repulsorBar;
    [SerializeField]
    private ProgressBar hookshotBar;
    [SerializeField]
    private ProgressBar ultimateBar;
    [SerializeField]
    private GameObject waveInfoParent;
    [SerializeField]
    private TextMeshProUGUI waveCounter;
    [SerializeField]
    private ProgressBar waveProgress;
    private AudioSource abilityRechargedSource;
    private Canvas canvas;

    private WaveManager waveManager;

    private void Awake()
    {
        player = FindObjectOfType<PlayerHealth>();
        waveManager = FindObjectOfType<WaveManager>();
        hookshot = player.GetComponent<Hookshot>();
        repulsor = player.GetComponent<Repulsor>();
        ultimate = player.GetComponent<UltimateAbility>();
        abilityRechargedSource = GetComponent<AudioSource>();
        repulsor.onAbilityRecharged += OnAbilityRecharged;
        canvas = GetComponent<Canvas>();

        if (waveManager != null && waveCounter != null)
        {
            waveManager.onWaveCompleted += UpdateWaveCounter;
        }
        else
        {
            waveInfoParent.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Settings.EnableHUD.Value)
        {
            canvas.enabled = false;
            return;
        }
        else
        {
            canvas.enabled = true;
        }

        if (player != null)
        {
            float fill = player.CurrentHealth / (float)player.MaxHealth;
            healthBar.SetFill(fill);
        }

        if (repulsor != null)
            repulsorBar.SetFill(1f-repulsor.PercentComplete);

        if (hookshot != null)
            hookshotBar.SetFill(1f-hookshot.PercentComplete);

        if (ultimate != null)
            ultimateBar.SetFill(ultimate.ChargePercentage);

        if (waveManager != null)
            waveProgress.SetFill(waveManager.CurrentWave.GetPercentCompleted());
    }

    private void OnAbilityRecharged() => abilityRechargedSource.Play();

    private void UpdateWaveCounter() 
    {
        waveCounter.transform.LeanScale(Vector3.one * 1.5f, 1f).setEasePunch();
        waveCounter.text = (waveManager.CurrentWaveIndex + 1).ToString();
    }
}
