using UnityEngine;

public class Avatar : MonoBehaviour
{
    private bool _latch;
    private int _portalLayerId;

    public Wormhole wormhole;

    private void Awake()
    {
        _portalLayerId = LayerMask.NameToLayer("Portal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != _portalLayerId)
        {
            return;
        }

        var portal = other.GetComponent<Portal>();

        //wormhole.Teleport(transform, portal.Mode);
    }
}
