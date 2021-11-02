using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    public PostRenderEvent postRenderEvent;

    // Start is called before the first frame update
    void Start()
    {
        postRenderEvent.Init();

        Destroy(gameObject);
    }
}
