using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoClipCamera : MonoBehaviour
{

    [SerializeField] private float Speed = 0.1f;
    [SerializeField] private float LookSpeed = 0.1f;
    Vector2 rotation = new Vector2 (0, 0);
    Vector3 input;

    void Update()
    {
        input += transform.right * Input.GetAxis("Horizontal");
        input += transform.forward * Input.GetAxis("Vertical");
        input += transform.up * Input.GetAxis("UpDown");
        input = Vector3.ClampMagnitude(input, 1) * Speed * Time.deltaTime;
        transform.position += input;
        rotation.y += Input.GetAxis ("Mouse X");
		rotation.x += -Input.GetAxis ("Mouse Y");
		transform.eulerAngles = (Vector2)rotation * LookSpeed;
    }
}
