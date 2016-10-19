using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Follower : MonoBehaviour
{
    // maximum values for movement
    public float maxSpeed;
    public float maxAcceleration;
    
    // movement
    public Vector3 linearVelocity;
    public Vector3 linearAcceleration;
    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;
    
    // other characters
    public GameObject leader;
    public GameObject toPursue; // either the leader or the follower in front of self
    public GameObject enemy;
    
    // variables for pursuing
    public Vector3 currPos;
    public Vector3 prevPos;
    public Vector3 deltaPos;
    public Vector3 estimatedPos;
    public Vector3 direction;
    public Rigidbody my_body;
    public Vector3 follow_force;
    public Vector3 follow_torque;
    
    
    void Start()
    {
        
    }
    
    void Update()
    {
        // first we need to determine what GameObject we are pursuing
        // at the beginning of each frame
        // leader has public variable so all the followers can see the list
        // of followers
        //int my_index = leader.GetComponent<Leader>().followers.FindIndex(myself);
        int my_index = 0;
        if(my_index == 0)
        {
            toPursue = leader;
        }
        else
        {
            toPursue = leader.GetComponent<Leader>().followers[my_index - 1];
        }
        
        // next we check if we have been hit by black bird
        Vector3 dist_to_enemy = enemy.transform.position - this.transform.position;
        if (dist_to_enemy.magnitude < 0.5f)
        {
            // delete this
            leader.GetComponent<Leader>().followers.RemoveAt(my_index);
            Destroy(gameObject);
        }
        
        // finally we pursue the appropiate GameObject
        /*currPos = toPursue.transform.position;
        deltaPos = currPos - prevPos;
        
        direction = currPos - this.transform.position;
        float dist_to_friend = direction.magnitude;
        float friend_speed = deltaPos.magnitude;
        
        estimatedPos = currPos + deltaPos;
        float mySpeed;
        
        if(dist_to_friend <= 2f)
        {
            mySpeed = toPursue.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else
        {
            // we need to catch up
            mySpeed = maxSpeed;
        }
        
        linearVelocity = mySpeed * direction.normalized;
        transform.position = transform.position + linearVelocity * Time.deltaTime;*/
        this.Pursue(toPursue);
        
        my_body.AddForce(follow_force);
        CheckSpeed();
        //my_body.AddTorque(follow_torque);
        transform.rotation = toPursue.transform.rotation;
    }
    
    void Pursue(GameObject target)
    {
        Vector3 behind_target = target.GetComponent<Rigidbody>().velocity.normalized * -1;
        Vector3 dist_to_target = (target.transform.position - this.transform.position) + behind_target * 0.5f;
        Vector3 direction = dist_to_target.normalized;
          
        Vector3 heading = Vector3.Cross(transform.up, direction);
        Vector3 torque = Vector3.Cross(transform.up, direction);
        
        torque = torque.normalized * maxAcceleration;
        
        torque = torque * Mathf.Lerp (0.7f, 1.0f, heading.magnitude - my_body.angularVelocity.magnitude);
        
        Debug.DrawRay (transform.position, direction);
        
        follow_torque = torque * Time.deltaTime * 100.0f;
        follow_force = Vector3.ClampMagnitude(direction * maxAcceleration, Mathf.Abs(maxAcceleration));
        
        if(dist_to_target.magnitude < 1f)
        {
            follow_force = follow_force * dist_to_target.magnitude;
        }
        else if(dist_to_target.magnitude < 2f)
        {
            follow_force = follow_force / dist_to_target.magnitude;
        }
        
    }
    
    protected void CheckSpeed()
    {
        if(my_body.velocity.magnitude > maxSpeed)
        {
            my_body.velocity = my_body.velocity.normalized * maxSpeed;
        }
    }
}