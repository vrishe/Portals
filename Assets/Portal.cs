using System;
using System.Collections.Generic;
using UnityEngine;

public enum PortalMode
{
    Out,
    In
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Portal : MonoBehaviour
{
    private Material _mat;
    private Matrix4x4 _gizmoMatrix;
    private Mesh _gizmoMesh;
    private GameObject _occluder;

    public PortalMode Mode;

    public int Id => 64 << (int)Mode;

    public Camera Camera { get; private set; }

    public Texture MainTexture
    {
        get => _mat.mainTexture;
        set => _mat.mainTexture = value;
    }

    public void SetEyesTransform(Matrix4x4 m)
    {
        m = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0)) * m;

        var t = Camera.transform;
        t.localPosition = m.GetColumn(3);
        t.localRotation = m.rotation;
    }

    private static Action<UnityEngine.Object> GetUniversalDestroyRoutine()
    {
        if (Application.isEditor)
        {
            return DestroyImmediate;
        }

        return Destroy;
    }

    private void OnDestroy()
    {
        var destroyer = GetUniversalDestroyRoutine();

        if (Camera)
        {
            Camera.targetTexture.Release();

            destroyer(Camera.gameObject);
            destroyer(_mat);
        }

        if (_occluder)
        {
            destroyer(_occluder);
        }
    }

    private void Reset()
    {
        OnDestroy();
        Start();
    }

    private void Start()
    {
        const HideFlags objHideFlags = HideFlags.DontSave;

        var
        go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.hideFlags = objHideFlags;
        {
            if (go.TryGetComponent<MeshCollider>(out var c))
            {
                Destroy(c);
            }

            var t = go.transform;
            t.SetParent(transform, false);
            t.localScale = Matrix4x4.Scale(transform.localScale).inverse * t.localScale;

            var matOccluder = new Material(Shader.Find("Unlit/Occluder"));

            var mr = go.GetComponent<MeshRenderer>();
            mr.material = matOccluder;

            go.layer = LayerMask.NameToLayer("PortalOccluder");
        }

        _occluder = go;

        go = new GameObject(nameof(Camera));
        go.hideFlags = objHideFlags;
        {
            var t = go.transform;
            t.Rotate(0, 180, 0);
            t.SetParent(transform);

            _mat = new Material(Shader.Find("Unlit/PortalScreenSpaceShader"));

            var mr = GetComponent<MeshRenderer>();
            mr.material = _mat;
        }

        Camera = go.AddComponent<Camera>();
        Camera.depth = float.MinValue;
        Camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.cullingMask &= ~(1 << LayerMask.NameToLayer("Portal"));
    }

    private void PrepareGizmos()
    {
        if (_gizmoMesh != null)
        {
            return;
        }

        _gizmoMatrix = Matrix4x4.Translate(
            -new Vector3(.5f, .5f));

        _gizmoMesh = new Mesh
        {
            subMeshCount = 5,
            vertices = new[] {
                new Vector3(0,0),
                new Vector3(0,1),
                new Vector3(1,1),
                new Vector3(1,0),

                new Vector3(.5f, .375f, .125f),
                new Vector3(.5f, .5f, 0),
                new Vector3(.5f, .625f, .125f),
                new Vector3(.5f, .375f, .375f),
                new Vector3(.5f, .5f, .5f),
                new Vector3(.5f, .625f, .375f),

                new Vector3(.375f, .5f, 0),
                new Vector3(.5f, .625f, 0),
                new Vector3(.625f, .5f, 0),
            },
        };

        _gizmoMesh.SetNormals(new List<Vector3>(System.Linq.Enumerable
            .Repeat(Vector3.forward, _gizmoMesh.vertexCount)));

        // Marker
        _gizmoMesh.SetIndices(new int[] { 5, 8, 7, 8, 8, 9, 10, 11, 11, 12 }, MeshTopology.Lines, 3);

        // Bounds
        _gizmoMesh.SetIndices(new int[] { 0, 1, 1, 2, 2, 3, 3, 0 }, MeshTopology.Lines, 2);
        _gizmoMesh.SetIndices(new int[] { 2, 1, 0, 0, 3, 2 }, MeshTopology.Triangles, 4);

        // Arrows
        _gizmoMesh.SetIndices(new int[] { 8, 5, 4, 5, 5, 6 }, MeshTopology.Lines, 1);
        _gizmoMesh.SetIndices(new int[] { 5, 8, 7, 8, 8, 9 }, MeshTopology.Lines, 0);
    }

    private void OnDrawGizmos()
    {
        PrepareGizmos();

        var
        m = transform.localToWorldMatrix;

        Gizmos.color = Mode == PortalMode.In ?
            Color.red : Color.blue;

        var m0 = m * _gizmoMatrix;

        m0.m22 = 1;

        Gizmos.matrix = m0;
        Gizmos.DrawWireMesh(_gizmoMesh, 2);

        var scale = transform.localScale;

        scale.z *= -1;

        var m1 = m
            * Matrix4x4.Scale(scale).inverse
            * _gizmoMatrix;

        Gizmos.matrix = m1;
        Gizmos.DrawWireMesh(_gizmoMesh, (int)Mode);

        if (!Camera)
        {
            return;
        }

        m = Camera.transform.localToWorldMatrix
            * _gizmoMatrix;

        Gizmos.matrix = m;
        Gizmos.DrawWireMesh(_gizmoMesh, 3);
    }
}
