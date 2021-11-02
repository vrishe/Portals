using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class Portals
{
    public static void CalculatePortalView(Camera eye, Transform portalA, Transform portalB, Camera portalViewReceiver)
    {
        portalViewReceiver.CopyFrom(eye);

        var m = portalB.transform.localToWorldMatrix * Matrix4x4.Rotate(Quaternion.AngleAxis(180, Vector3.up))
            * portalA.transform.worldToLocalMatrix * eye.transform.localToWorldMatrix;

        portalViewReceiver.transform.position = m.MultiplyPoint3x4(Vector3.zero);
        portalViewReceiver.transform.rotation = Quaternion.LookRotation(m.MultiplyVector(Vector3.forward), m.MultiplyVector(Vector3.up));
        portalViewReceiver.transform.localScale = eye.transform.localScale;
    }
}
