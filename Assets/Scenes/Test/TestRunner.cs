using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TestRunner : MonoBehaviour
{
    private MeshFilter _quadDst;
    private MeshFilter _quadSrc;

    public Camera mainCamera;
    public Camera portalCamera;

    public Transform portalA;
    public Transform portalB;

    public RenderTexture receiverTexture;

    public float clippingPlaneOffset = 0.001f;

    private void OnEnable()
    {
        _quadDst = portalA.GetComponentInChildren<MeshFilter>();
        _quadSrc = portalB.GetComponentInChildren<MeshFilter>();

        _quadDst.mesh = Instantiate(_quadSrc.sharedMesh);
    }

    void Update()
    {
        Portals.CalculatePortalView(mainCamera, portalA, portalB, portalCamera);

        var Z = portalCamera.worldToCameraMatrix;
        var n = Z.MultiplyVector(portalB.forward);
        var c = Z.MultiplyPoint3x4(portalB.position + clippingPlaneOffset * portalB.forward);

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
        portalCamera.targetTexture = receiverTexture;

        var P = _quadSrc.transform.localToWorldMatrix;
        _quadDst.sharedMesh.SetUVs(0, _quadDst.sharedMesh.vertices.Select(P.MultiplyPoint3x4)
            .Select(portalCamera.WorldToViewportPoint)
            .Select(v3 => new Vector2(v3.x, v3.y))
            .ToArray());
    }
}
