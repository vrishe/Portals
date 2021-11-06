using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Wormhole
{
    using UnityEngine;

    public sealed class SimplePortalSpace : IPortalSpace
    {
        private readonly Transform _root;
        private readonly GameObject _portalPrefab;

        private readonly IDictionary<int, Slot> _portals = new Dictionary<int, Slot>();
        private readonly IList<(int, int)> _links = new List<(int, int)>();

        private IList<Slot> _portalsList;

        internal SimplePortalData this[int portal] => _portals[portal].Data;

        public SimplePortalSpace(Transform root, GameObject portalPrefab)
        {
            _root = root;
            _portalPrefab = portalPrefab;
        }

        internal bool TryGetPortalData(int portal, out SimplePortalData portalData)
        {
            if (_portals.TryGetValue(portal, out var s))
            {
                portalData = s.Data;
                return portalData;
            }

            portalData = null;
            return false;
        }

        public void Clear()
        {
            foreach (var (_, s) in _portals)
            {
                SmartDestroy(s.Data);
            }

            _portals.Clear();
            _links.Clear();
        }

        public bool HasPortal(int portal)
        {
            return _portals.TryGetValue(portal, out var s) && s.Data;
        }

        public int AddPortal(string namePrefix, Vector3 n, Vector3 p, object args)
        {
            var go = Object.Instantiate(_portalPrefab, _root);
            var id = go.GetInstanceID();

            go.name = $"{namePrefix}Portal {id}";
            go.hideFlags = HideFlags.DontSave;

            var t = go.transform;
            t.position = p;
            t.forward = n;

            var data = go.GetComponent<SimplePortalData>();
            if (!data)
            {
                data = go.AddComponent<SimplePortalData>();

                Debug.LogWarning($"{_portalPrefab}'s missing a predefined {nameof(SimplePortalData)}.");
            }

            _portals[id] = new Slot { Id = id, Data = data };
            _portalsList = new List<Slot>(_portals.Values);

            return id;
        }

        public bool RemovePortal(int portal)
        {
            if (!_portals.TryGetValue(portal, out var p))
            {
                return false;
            }

            for (int i = _links.Count - 1; i >= 0; --i)
            {
                var (portalIn, portalOut) = _links[i];

                if (portalIn == p.Id)
                {
                    _links.RemoveAt(i);

                    continue;
                }

                if (portalOut == p.Id)
                {
                    if (_portals.TryGetValue(portalIn, out var pp))
                    {
                        pp.Link = null;
                    }

                    _links.RemoveAt(i);
                }
            }

            _portals.Remove(portal);
            _portalsList = new List<Slot>(_portals.Values);

            SmartDestroy(p.Data);
            
            return true;
        }

        public int LinkPortals(int portalIn, int portalOut)
        {
            if (portalIn == portalOut)
            {
                throw new ArgumentException("Cannot link a portal with itself");
            }

            if (!_portals.TryGetValue(portalIn, out var src))
            {
                throw new ArgumentException($"A portal with ID {portalIn} does not exist.", nameof(portalIn));
            }

            if (!_portals.TryGetValue(portalOut, out var dst))
            {
                throw new ArgumentException($"A portal with ID {portalOut} does not exist.", nameof(portalOut));
            }

            if (src.Link != null)
            {
                _links.Remove((src.Id, src.Link.Id));
            }

            src.Link = dst;

            var link = (src.Id, dst.Id);

            _links.Add(link);
            return GetLinkId(link);
        }

        public void UnlinkPortals(int link)
        {
            for (int i = _links.Count - 1; i >= 0; --i)
            {
                if (GetLinkId(_links[i]) == link)
                {
                    _links.RemoveAt(i);
                }
            }
        }

        public void UpdateCameras(Camera eye)
        {
            for (int i = _portalsList.Count - 1; i >= 0 ; --i)
            {
                var p = _portalsList[i];
                
                if (!p.Data)
                {
                    Debug.LogWarning($"Missing {nameof(SimplePortalData)} for portal with ID: {p.Id}.");
                    continue;
                }

                if (!IsPortalVisible(eye, p.Data))
                {
                    p.Data.PortalCamera.enabled = false;
                    continue;
                }

                if (p.Link == null)
                {
                    // TODO: handle an unlinkedf portal here.
                    continue;
                }

                p.Data.PortalCamera.enabled = true;
                UpdatePortalCamera(eye, p.Data.PortalCamera, p.Data.transform, p.Link.Data.transform, p.Link.Data.PortalPlaneOffset);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdatePortalCamera(Camera eye, Camera portalCamera, Transform src, Transform dst, float nearPlaneOffset)
        {
            portalCamera.CopyFrom(eye);

            var m = dst.transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.AngleAxis(180, Vector3.up))
                * src.transform.worldToLocalMatrix * eye.transform.localToWorldMatrix;

            portalCamera.transform.position = m.MultiplyPoint3x4(Vector3.zero);
            portalCamera.transform.rotation = Quaternion.LookRotation(m.MultiplyVector(Vector3.forward), m.MultiplyVector(Vector3.up));
            portalCamera.transform.localScale = eye.transform.localScale;

            // Eric Lengyel method follow
            var Z = portalCamera.worldToCameraMatrix;
            var n = Z.MultiplyVector(dst.forward);
            var c = Z.MultiplyPoint3x4(dst.position + nearPlaneOffset * dst.forward);

            var M = portalCamera.projectionMatrix;
            var M_inv = M.inverse;

            var C = new Vector4(n.x, n.y, n.z, -Vector3.Dot(n, c));
            var C_1 = M_inv.transpose * C;
            var Q_1 = new Vector4(Mathf.Sign(C_1.x), Mathf.Sign(C_1.y), 1, 1);
            var Q = M_inv * Q_1;

            var M_4 = M.GetRow(3);
            M_4 = (2 * Vector4.Dot(M_4, Q) / Vector4.Dot(C, Q)) * C - M_4;

            M.SetRow(2, M_4);

            portalCamera.projectionMatrix = M;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool CheckClipSpace(Vector3 min, Vector3 max)
        {
            return ((-1 <= min.x && min.x <= 1) || (-1 <= max.x && max.x <= 1) || (0 <= max.x && min.x <= 0) || (0 <= min.x && max.x <= 0))
                && ((-1 <= min.y && min.y <= 1) || (-1 <= max.y && max.y <= 1) || (0 <= max.y && min.y <= 0) || (0 <= min.y && max.y <= 0))
                && ((-1 <= min.z && min.z <= 1) || (-1 <= max.z && max.z <= 1) || (0 <= max.z && min.z <= 0) || (0 <= min.z && max.z <= 0));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLinkId((int portalIn, int portalOut) link)
        {
            unchecked
            {
                var result = 6661;

                result = result * 61 + link.portalIn;
                result = result * 61 + link.portalOut;

                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPortalVisible(Camera eye, SimplePortalData portalData)
        {
            var MVP = eye.projectionMatrix * eye.worldToCameraMatrix;
            if (Vector3.Dot(Vector3.forward, MVP.MultiplyVector(portalData.transform.forward)) >= 0.3f)
            {
                return false;
            }

            var bounds = portalData.PortalRenderer.bounds;

            return CheckClipSpace(MVP.MultiplyPoint(bounds.min), MVP.MultiplyPoint(bounds.max));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SmartDestroy(Component c)
        {
            if (c)
            {

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(c.gameObject);

                    return;
                }
#endif

                Object.Destroy(c.gameObject);

            }
        }

        private class Slot
        {
            public int Id;

            public SimplePortalData Data;

            public Slot Link;
        }
    }
}
