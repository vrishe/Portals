using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private Camera _head;

    [SerializeField]
    private float _axisSpeed = 0.1f;

    [Range(0, 1)]
    [SerializeField]
    private float _mouseSens = .5f;

    [Min(.1f)]
    [SerializeField]
    private float _jumpDuration = .1f;
    [Min(0)]
    [SerializeField]
    private float _jumpHeight = 0;

    private Vector3 _movementDir;

    private bool _active;
    private bool _isInJump;
    private float _jumpTime;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        _active = true;
    }

    private void Update()
    {
        if (!_active)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;

                _active = true;
            }

            Input.ResetInputAxes();

            return;
        }

        if (!_isInJump
            && Input.GetKeyDown(KeyCode.Space))
        {
            _isInJump = true;
            _jumpTime = 0;

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _active = false;
            Input.ResetInputAxes();
        }
    }

    private void LateUpdate()
    {
        var t = Time.deltaTime;
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

        var
        positionDst = transform.localPosition;

        if (!_isInJump)
        {
            var movementDir = _axisSpeed * (Input.GetAxis("Horizontal") * transform.right
                + Input.GetAxis("Vertical") * transform.forward).normalized;

            _movementDir = Vector3.Lerp(_movementDir, movementDir,
                Mathf.Lerp(9, 3, movementDir.sqrMagnitude) * t);
        }
        else
        {
            _jumpTime = _jumpTime + t;
            _isInJump &= _jumpTime <= _jumpDuration;

            var x = 2 * _jumpTime / _jumpDuration - 1;
            positionDst.y = _jumpHeight * Mathf.Clamp01(1 - x * x);
        }

        positionDst += t * _movementDir;
        transform.localPosition = positionDst;
    }
}
