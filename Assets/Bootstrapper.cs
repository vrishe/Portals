using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var w = Wormhole.Instance;

        w.In  = GameObject.Find("Portal_I").GetComponent<Portal>();
        w.Out = GameObject.Find("Portal_O").GetComponent<Portal>();
    }
}
