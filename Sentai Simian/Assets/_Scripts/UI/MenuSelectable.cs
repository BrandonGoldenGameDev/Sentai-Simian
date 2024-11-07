using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(EventTrigger), typeof(AudioSource))]
public class MenuSelectable : MonoBehaviour
{
    [SerializeField]
    private AudioClip onClickClip;
    [SerializeField]
    private AudioClip onSelectClip;
    private AudioSource source;
    private EventTrigger trigger;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.ignoreListenerPause = true;

        trigger = GetComponent<EventTrigger>();
        SubscribeToEventTriggerEvent(EventTriggerType.PointerEnter, _ => PlaySelectedAudio());
        SubscribeToEventTriggerEvent(EventTriggerType.Select, _ => PlaySelectedAudio());
        SubscribeToEventTriggerEvent(EventTriggerType.Submit, _ => PlayClickedAudio());
    }

    private void SubscribeToEventTriggerEvent(EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    public void PlaySelectedAudio() => source.PlayOneShot(onSelectClip);
    public void PlayClickedAudio() => source.PlayOneShot(onClickClip);
}
