using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeoFPS.SinglePlayer;
using System;

namespace NeoFPS.Grabber
{
    public class NeoFpsMyGrabberHudPopup : MonoBehaviour
    {
        [SerializeField, Tooltip("The grab state to show this popup for")]
        private NeoFpsMyGrabberInput.GrabState m_GrabState = NeoFpsMyGrabberInput.GrabState.Grabbed;

        void OnDestroy()
        {
            NeoFpsMyGrabberInput.onGrabStateChanged -= OnGrabStateChanged;
        }

        private void Awake()
        {
            NeoFpsMyGrabberInput.onGrabStateChanged += OnGrabStateChanged;
            gameObject.SetActive(false);
        }

        private void OnGrabStateChanged(NeoFpsMyGrabberInput.GrabState grabState)
        {
            gameObject.SetActive(grabState == m_GrabState);
        }
    }
}