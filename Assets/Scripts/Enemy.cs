using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	// maximum values for movement
	public float maxSpeed;
	public float maxAcceleration;
	
	// vectors for current movement info
	public Vector3 linearVelocity;
	public Vector3 linearAcceleration;
	
	// enemy will pursue mouse cursor if the left or right button is held down
	// so we need position vectors
	public Vector3 mousepos;
	public Vector3 currPos;
	public Vector3 prevPos;
	public Vector3 deltaPos;
	public Vector3 estimatedPos;
	
	// initialization
	void Start()
	{
		
	}
	
	// update
	void Update()
	{
		if(Input.GetMouseButton(0))
		{
			/*mousepos = Input.mousePosition;
			currPos = Input.mousePosition;
			deltaPos = currPos - prevPos;
			prevPos = currPos;
			
			estimatedPos = Camera.main.ScreenToWorldPoint(mousepos);
            float mouseSpeed = deltaPos.magnitude;
            
            if(mouseSpeed != 0)
            {
                estimatedPos = estimatedPos + deltaPos * Time.deltaTime;
            }
            
            Vector3 distance = mousepos - transform.position;
			
			Vector3 direction = mousepos - transform.position;
            direction.z = 0;
			float mySpeed = linearVelocity.magnitude + (linearAcceleration * Time.deltaTime).magnitude;
			// slow down if we need to
			if (distance.magnitude < 10)
			{
				mySpeed *= 10/distance.magnitude;
			}
			linearVelocity = distance.normalized * mySpeed;
			if (linearVelocity.magnitude > maxSpeed)
			{
				linearVelocity = direction.normalized * maxSpeed;
			}
			transform.position = transform.position + linearVelocity * Time.deltaTime;*/
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
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularVelocity.magnitude * Time.deltaTime);
            linearVelocity = direction.normalized * myMoveSpeed;
            transform.position = transform.position + linearVelocity * Time.deltaTime * 0.5f;
		}
	}
}