using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Leader : MonoBehaviour {
	// maximum values, will be set to a lower value than followers
	// in order to allow followers to catch up if popped by blackbird
	private float maxSpeed = 3f;
    private float maxAcceleration = 1.5f;
	
	// vectors to represent movement
	public Vector3 angAcceleration;
	public Vector3 angVelocity;
	public Vector3 linAcceleration;
	public Vector3 linVelocity;
    
    // list of obstacles
    public List<GameObject> obstacle_course;
    
    // path to follow
    public GameObject[] path;
    public int curr_point;
    public GameObject target_point;
    
    // followers
    // need to keep track in order to slow down for catch up
    // followers will just pursue the follower in front of them
    // first follower will pursue leader
    public List<GameObject> followers;
    
    // cone checking
    public float cone_threshold;
    private float avoid_ratio = .8f;
    private float path_ratio = .2f;
    public Vector3 avoid_torque;
    public Vector3 avoid_force;
    public Vector3 follow_torque;
    public Vector3 follow_force;
    public Rigidbody my_body;
	
    // initialization
    void Start() {
        StartCoroutine(master_move());
        StartCoroutine(path_follow());
        StartCoroutine(avoid_obstacles());
    }
    
    void Update() {
        
    }
    
    protected void CheckSpeed()
    {
        if(my_body.velocity.magnitude > maxSpeed)
        {
            my_body.velocity = my_body.velocity.normalized * maxSpeed;
        }
    }
    
    // routine to control movement
    protected IEnumerator master_move()
    {
        while(true){
            Vector3 torque = new Vector3(0,0,0);
            Vector3 force = new Vector3(0,0,0);
            
            if(avoid_torque.magnitude > .1f)
            {
                torque = (follow_torque * path_ratio + avoid_torque * avoid_ratio) * 100.0f * Time.deltaTime;
            }
            else
            {
                torque = follow_torque * Time.deltaTime * 100.0f;
            }
            if(torque.magnitude > maxAcceleration)
            {
                torque = torque.normalized * maxAcceleration;
            }
            my_body.AddForce(follow_force);
            CheckSpeed();
            my_body.AddTorque(torque);
            yield return new WaitForEndOfFrame();
        }
    }
    
    // follow the path 
	protected IEnumerator path_follow()
    {
        while(curr_point < path.Length)
        {
            if(target_point == null)
            {
                target_point = path[curr_point];
            }
            
            this.Pursue (target_point);
            
            Vector3 dist_to_target = target_point.transform.position - this.transform.position;
            if(dist_to_target.magnitude < 1f)
            {
                curr_point++;
                target_point = path[curr_point];
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
    
    void Pursue(GameObject target)
    {
        Vector3 direction = (target.transform.position - this.transform.position).normalized;
          
        Vector3 heading = Vector3.Cross(transform.up, direction);
        Vector3 torque = Vector3.Cross(transform.up, direction);
        
        torque = torque.normalized * maxAcceleration;
        
        torque = torque * Mathf.Lerp (0.7f, 1.0f, heading.magnitude - my_body.angularVelocity.magnitude);
        
        Debug.DrawRay (transform.position, direction);
        
        follow_torque = torque * Time.deltaTime * 100.0f;
        follow_force = Vector3.ClampMagnitude(transform.up * maxAcceleration, Mathf.Abs(maxAcceleration));
    }
    // avoid the obstacles
    protected IEnumerator avoid_obstacles()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            
            Vector3 center_of_mass = new Vector3(0,0,0);
            int evading = 0;
            
            foreach (GameObject g in obstacle_course)
            {
                if(Vector3.Dot(transform.up, g.transform.position - transform.position) < cone_threshold)
                {
                    center_of_mass = center_of_mass + g.transform.position;
                }
            }
            
            if (evading == 0)
            {
                avoid_torque = new Vector3(0,0,0);
                continue;
            }
            
            center_of_mass = center_of_mass / evading;
            
            Vector3 direction = (center_of_mass - this.transform.position).normalized * -1;
            
            Vector3 heading = Vector3.Cross(transform.up, direction);
            
            Vector3 torque = Vector3.Cross(transform.up, direction);
            
            torque = torque.normalized * maxAcceleration;
            
            torque = torque * Mathf.Lerp(0.0f, 1.0f, heading.magnitude);
            
            avoid_torque = torque;
        }
    }
}