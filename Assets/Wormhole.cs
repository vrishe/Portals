using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Wormhole : MonoBehaviour
{
    public static Wormhole Instance { get; private set; }

    private Portal _in, _out;
    private Updater _updater;

    public Portal In
    {
        get => _in;
        set
        {
            _in = value;
            if (_in != null)
            {
                _in.Mode = PortalMode.In;
                if (_in == _out)
                {
                    _out = null;
                }
            }

            OnPortalsChanged();
        }
    }

    public Portal Out
    {
        get => _out;
        set
        {
            _out = value;
            if (_out != null)
            {
                _out.Mode = PortalMode.Out;
                if (_out == _in)
                {
                    _in = null;
                }
            }

            OnPortalsChanged();
        }
    }
      
    private void Awake()
    {
        Instance = this;
    }

    private void Reset()
    {
        if (_updater)
        {
            DestroyImmediate(_updater.gameObject);
        }

        Start();
    }

    private void Start()
    {
        var go = new GameObject(nameof(Wormhole) + " Updater", typeof(Updater));
        go.hideFlags = HideFlags.HideAndDontSave;

        _updater = go.GetComponent<Updater>();
        _updater.enabled = false;
        _updater.Owner = this;

        OnPortalsChanged();
    }

    private void OnDestroy()
    {
        DestroyImmediate(_updater.gameObject);
    }

    private void OnPortalsChanged()
    {
        _updater.enabled = _in && _out;
    }

    [ExecuteAlways]
    private class Updater : MonoBehaviour
    {
        public Wormhole Owner;

        private Camera _eyes;

        private void OnEnable()
        {
            _eyes = Camera.main;
        }

        private void Update()
        {
            Owner.Out.SetEyesTransform(Owner.In.transform.worldToLocalMatrix
                * _eyes.transform.localToWorldMatrix);

            Owner.In.SetEyesTransform(Owner.Out.transform.worldToLocalMatrix
                * _eyes.transform.localToWorldMatrix);
        }
    }
}
