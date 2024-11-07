using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    private PlayerMovement movement;
    private AudioSource source;
    [SerializeField]
    private AudioClip[] footstepClips;
    [SerializeField]
    private Vector2 volumeRange;
    private int lastPlayedStepIndex;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
        source = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        if (movement.IsGrounded)
        {
            int stepIndex;
            do
            {
                stepIndex = Random.Range(0, footstepClips.Length);
            } while (stepIndex == lastPlayedStepIndex);

            lastPlayedStepIndex = stepIndex;
            source.PlayOneShot(footstepClips[stepIndex], Random.Range(volumeRange.x, volumeRange.y));
        }
    }
}
