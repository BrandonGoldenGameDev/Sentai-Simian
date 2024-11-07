using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    private HashSet<Zone> connectedZones;
    private bool isOpen = false;
    private Collider coll;
    private Renderer rend;
    private NavMeshObstacle navMeshObstacle;
    private AudioSource source;

    [SerializeField]
    private AudioClip openClip;

    [SerializeField, Min(0)]
    private float openDuration = 1f;
    [SerializeField]
    private LeanTweenType tweenType = LeanTweenType.easeInQuart;

    public bool IsOpen => isOpen;


    private void Awake()
    {
        rend = GetComponent<Renderer>();
        coll = GetComponent<Collider>();
        source = GetComponent<AudioSource>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        connectedZones = new HashSet<Zone>();
    }

    public void AddConnectedZone(Zone zone)
    {
        connectedZones.Add(zone);
    }

    public void Open()
    {
        coll.enabled = false;
        navMeshObstacle.carving = false;
        isOpen = true;

        if (connectedZones.Count > 0)
        {
            foreach (var zone in connectedZones)
            {
                zone.Enable();
            }
        }

        LeanTween.value(1f, -0.2f, openDuration)
            .setEase(tweenType)
            .setOnUpdate(SetDoorFill);

        source.PlayOneShot(openClip);
    }

    private void SetDoorFill(float fill)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        rend.GetPropertyBlock(block);
        block.SetFloat("_Fade_Value", fill);
        rend.SetPropertyBlock(block);
    }
}
