using System.Collections.Generic;
using UnityEngine;

public enum PortalMode
{
    Out,
    In
}

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    private BoxCollider _collider;
    private Camera _camera;

    private Matrix4x4 _gizmoMatrix;
    private Mesh _gizmoMesh;

    public PortalMode Mode;

    public void SetEyesTransform(Matrix4x4 m)
    {
        m = Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0)) * m;

        var t = _camera.transform;

        t.localPosition = m.GetColumn(3);

        var m0 = m.GetColumn(0);
        var m1 = m.GetColumn(1);
        var m2 = m.GetColumn(2);

        t.localScale.Set(m0.magnitude, m1.magnitude, m2.magnitude);

        var q = new Quaternion(
            .5f * Mathf.Sqrt(Mathf.Max(1 + m0[0] - m1[1] - m2[2], 0)),
            .5f * Mathf.Sqrt(Mathf.Max(1 - m0[0] + m1[1] - m2[2], 0)),
            .5f * Mathf.Sqrt(Mathf.Max(1 - m0[0] - m1[1] + m2[2], 0)),
            .5f * Mathf.Sqrt(Mathf.Max(1 + m0[0] + m1[1] + m2[2], 0)));

        q.x = Mathf.Abs(q.x) * Mathf.Sign(m1[2] - m2[1]);
        q.y = Mathf.Abs(q.y) * Mathf.Sign(m2[0] - m0[2]);
        q.z = Mathf.Abs(q.z) * Mathf.Sign(m0[1] - m1[0]);

        t.localRotation = q;
    }

    private void Reset()
    {
        if (_camera)
        {
            DestroyImmediate(_camera.gameObject);
        }

        Start();
    }

    private void Start()
    {
        if (!_camera)
        {
            var go = new GameObject("Camera", typeof(Camera));
            go.hideFlags = HideFlags.HideAndDontSave;

            var t = go.transform;
            t.Rotate(0, 180, 0);
            t.SetParent(transform);

            _camera = go.GetComponent<Camera>();
            _camera.enabled = false;
        }

        _collider = GetComponent<BoxCollider>();

        PrepareGizmos();
    }

    private void PrepareGizmos()
    {
        _gizmoMatrix = Matrix4x4.Translate(
            -new Vector3(.5f, .5f));

        _gizmoMesh = new Mesh
        {
            subMeshCount = 4,
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

        // Arrows
        _gizmoMesh.SetIndices(new int[] { 8, 5, 4, 5, 5, 6 }, MeshTopology.Lines, 1);
        _gizmoMesh.SetIndices(new int[] { 5, 8, 7, 8, 8, 9 }, MeshTopology.Lines, 0);
    }

    private void OnDrawGizmos()
    {
        var
        m = transform.localToWorldMatrix
            * Matrix4x4.Translate(_collider.center);

        Gizmos.color = Mode == PortalMode.In ?
            Color.red : Color.blue;

        var m0 = m
            * Matrix4x4.Scale(_collider.size)
            * _gizmoMatrix;

        m0.m22 = 1;

        Gizmos.matrix = m0;
        Gizmos.DrawWireMesh(_gizmoMesh, 2);

        var m1 = m * _gizmoMatrix;

        Gizmos.matrix = m1;
        Gizmos.DrawWireMesh(_gizmoMesh, (int)Mode);

        m = _camera.transform.localToWorldMatrix
            * _gizmoMatrix;

        Gizmos.matrix = m;
        Gizmos.DrawWireMesh(_gizmoMesh, 3);
    }
}
