using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugOrientation : MonoBehaviour
{
    private Matrix4x4 _gizmoMatrix;
    private Mesh _gizmoMesh;

    public Color gizmoColor;
    public float normalLength = 1;

    private void PrepareGizmos()
    {
        if (_gizmoMesh != null)
        {
            return;
        }

        _gizmoMatrix = Matrix4x4.Translate(
            -new Vector3(.5f, .5f, .5f));

        _gizmoMesh = new Mesh
        {
            subMeshCount = 2,
            vertices = new[] {
                // Cube
                new Vector3(0,0,0),
                new Vector3(0,1,0),
                new Vector3(1,0,0),
                new Vector3(1,1,0),
                new Vector3(0,0,1),
                new Vector3(0,1,1),
                new Vector3(1,0,1),
                new Vector3(1,1,1),

                // Normal direction
                new Vector3(.5f,.5f,.5f),
                new Vector3(.5f,.5f,1.5f),
            },
        };

        _gizmoMesh.SetNormals(new List<Vector3>(System.Linq.Enumerable
            .Repeat(Vector3.forward, _gizmoMesh.vertexCount)));

        _gizmoMesh.SetIndices(new int[] { 0,1, 2,3, 4,5, 6,7, 0,2, 1,3, 4,6, 5,7, 0,4, 1,5, 2,6, 3,7 }, MeshTopology.Lines, 0);
        _gizmoMesh.SetIndices(new int[] { 8,9 }, MeshTopology.Lines, 1);
    }

    private void OnDrawGizmos()
    {
        PrepareGizmos();

        Gizmos.color = gizmoColor;

        var
        m = transform.localToWorldMatrix;

        Gizmos.matrix = m * _gizmoMatrix;
        Gizmos.DrawWireMesh(_gizmoMesh, 0);

        Gizmos.matrix = m * Matrix4x4.Scale(new Vector3(1,1,normalLength)) * _gizmoMatrix;
        Gizmos.DrawWireMesh(_gizmoMesh, 1);
    }
}
