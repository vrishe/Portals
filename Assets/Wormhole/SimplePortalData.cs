using System;
using UnityEngine;

namespace Wormhole
{
    internal sealed class SimplePortalData : MonoBehaviour
    {
#pragma warning disable 0649
        public Renderer PortalRenderer;
        public Camera PortalCamera;
        public float PortalPlaneOffset = .001f;
#pragma warning restore 0649
    }
}
