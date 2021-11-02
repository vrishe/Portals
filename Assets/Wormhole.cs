using UnityEngine;

public class Wormhole : MonoBehaviour
{
    public Portal In;
    public Portal Out;

    public void Teleport(Transform t, PortalMode mode)
    {
        var portalIn = In;
        var portalOut = Out;

        if (mode == PortalMode.Out)
        {
            portalIn = Out;
            portalOut = In;
        }

        var m = portalOut.transform.localToWorldMatrix
            * portalIn.transform.worldToLocalMatrix
            * t.localToWorldMatrix
            * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0));

        t.position = m.GetColumn(3);
        t.rotation = m.rotation;
    }

    private void Start()
    {
        In.MainTexture = Out.Camera.activeTexture;
        Out.MainTexture = In.Camera.activeTexture;
    }

    private void Update()
    {
        var t_cam = Camera.main.transform.localToWorldMatrix;

        In.SetEyesTransform(Out.transform.worldToLocalMatrix * t_cam);
        Out.SetEyesTransform(In.transform.worldToLocalMatrix * t_cam);
    }
}
