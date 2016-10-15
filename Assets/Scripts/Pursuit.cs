using UnityEngine;
using System.Collections;

// a class that pursues the mouse when either button is held

public class Pursuit : MonoBehaviour {


    private Vector3 mousepos;
    public Vector3 linearVelocity;
    public Vector3 linearAcceleration;
    public Vector3 prevPos;
    public Vector3 deltaPos;
    public Vector3 currPos;
    public Vector3 angularVelocity;
    public float maxSpeed = 6f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            //pursue the mouse position
            mousepos = Input.mousePosition;
            currPos = Input.mousePosition;
            deltaPos = currPos - prevPos;
            prevPos = currPos;

            float mouseVelocity = deltaPos.magnitude;

            // estimates future mouse position
            mousepos = Camera.main.ScreenToWorldPoint(mousepos);
            Vector3 estimatedPos = mousepos;
            // if it's moving we estimate where it's going to be
            // if not moving estimated position is exactly the same as the current Position
            if(mouseVelocity != 0)
            {
                estimatedPos = mousepos * mouseVelocity;
            }

            Vector3 distance = mousepos - transform.position;
            float myMoveSpeed = linearVelocity.magnitude * distance.magnitude / 10;
            if (distance.magnitude < 10)
            {
                myMoveSpeed = linearVelocity.magnitude * 10/ distance.magnitude;
            }
            if (myMoveSpeed > maxSpeed)
            {
                myMoveSpeed = maxSpeed;
            }
            Vector3 direction = estimatedPos - transform.position;
            direction.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularVelocity.magnitude * Time.deltaTime);
            linearVelocity = direction.normalized * myMoveSpeed;
            transform.position = transform.position + direction.normalized * myMoveSpeed * Time.deltaTime * 0.5f;
        }
	}
}
