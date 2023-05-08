using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float moveSpeed = 0.05f; 
    float turnSpeed = 2.0f; 
    public float limit = 88f;
    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
	const string yAxis = "Mouse Y";
	Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void moveCamera() {
        float verticalInput = Input.GetAxis("Vertical"); 
        float horizontalInput = Input.GetAxis("Horizontal"); 

        Vector3 forward = transform.forward; 
        Vector3 right = transform.right; 

        forward.y = 0; 
        right.y = 0; 

        Vector3 movement = verticalInput * forward + horizontalInput * right; 
        transform.Translate(movement * moveSpeed); 
    }
    // Update is called once per frame
    void Update()
    {
        rotation.x += Input.GetAxis(xAxis) * turnSpeed; 
        rotation.y += Input.GetAxis(yAxis) * turnSpeed; 
        rotation.y = Mathf.Clamp(rotation.y, -limit, limit); 

        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
		var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
		transform.localRotation = xQuat * yQuat;

        moveCamera();   
    }
}
