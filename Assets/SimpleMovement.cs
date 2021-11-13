using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private Camera _head;

    [SerializeField]
    private float _jumpCoeff = 1;

    [SerializeField]
    private float _maxVelocity = 1;

    [Range(0, 1)]
    [SerializeField]
    private float _mouseSens = .5f;

    [TagSelector]
    [SerializeField]
    private string _groundTag = string.Empty;

    private bool _active;

    private int _floorCollisions;
    private Rigidbody _rigidbody;

#if UNITY_EDITOR
    [NonSerialized]
    public Vector3 MaxPosition;
    [NonSerialized]
    public Vector3 MinPosition;

    public void ResetEditorValues()
    {
        MaxPosition = MinPosition = transform.position;
    }
#endif

    private bool IsAirborne => _floorCollisions <= 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _active = false;

#if UNITY_EDITOR
        ResetEditorValues();
#endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == _groundTag)
        {
            ++_floorCollisions;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == _groundTag)
        {
            --_floorCollisions;
        }
    }

    private void Update()
    {
        if (!_active)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                _active = true;
            }

            Input.ResetInputAxes();

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _active = false;

            Input.ResetInputAxes();

            return;
        }

        if (!IsAirborne
            && Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(new Vector3(0, _jumpCoeff * _rigidbody.mass, 0), ForceMode.Impulse);
        }
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        MaxPosition = Vector3.Max(MaxPosition, transform.position);
        MinPosition = Vector3.Min(MinPosition, transform.position);
#endif

        var aX = _mouseSens * Input.GetAxis("Mouse X");
        var aY = _mouseSens * Input.GetAxis("Mouse Y");

        var
        eulerAnglesDst = _head.transform.localEulerAngles;
        var pitch = eulerAnglesDst.x % 360;
        pitch += 360 * Mathf.Min(0, Mathf.Sign(180 - pitch));
        eulerAnglesDst.x = Mathf.Clamp(pitch
            - aY * Screen.height / _head.fieldOfView, -90, 90);

        _head.transform.localEulerAngles = eulerAnglesDst;

        eulerAnglesDst = transform.localEulerAngles;
        var yaw = eulerAnglesDst.y % 360;
        eulerAnglesDst.y = yaw + aX * Screen.width
            / (_head.fieldOfView * _head.aspect);

        transform.localEulerAngles = eulerAnglesDst;

        if (!IsAirborne)
        {
            _rigidbody.AddForce(Input.GetAxis("Horizontal") * transform.right
                + Input.GetAxis("Vertical") * transform.forward, ForceMode.VelocityChange);
        }
        else
        {
        }

        var velocity = _rigidbody.velocity;
        var vh = Vector3.ClampMagnitude(new Vector3(velocity.x, 0, velocity.z), _maxVelocity);
        var vv = new Vector3(0, velocity.y, 0);

        _rigidbody.velocity = vh + vv;
    }
}
