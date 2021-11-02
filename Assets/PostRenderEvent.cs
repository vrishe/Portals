using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostRenderEvent : MonoBehaviour
{
    public static PostRenderEvent Instance;

    public event Action<Camera> OnPostRenderEvent;


    private Camera _camera;

    public void Init()
    {
        _camera = GetComponent<Camera>();

        Instance = this;
    }

    private void OnPostRender()
    {
        OnPostRenderEvent?.Invoke(_camera);
    }
}
