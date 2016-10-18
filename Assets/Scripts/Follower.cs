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
    
    
    void Start()
    {
        
    }
    
    void Update()
    {
        // first we need to determine what GameObject we are pursuing
        // at the beginning of each frame
        // leader has public variable so all the followers can see the list
        // of followers
        int my_index = 0;//leader.GetComponent<Leader>().followers.FindIndex(gameObject);
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
        currPos = toPursue.transform.position;
        deltaPos = currPos - prevPos;
        
        direction = currPos - this.transform.position;
        float dist_to_friend = direction.magnitude;
        float friend_speed = deltaPos.magnitude;
        
        estimatedPos = currPos + deltaPos;
        float mySpeed;
        
        if(dist_to_friend <= 0.5f)
        {
            mySpeed = friend_speed;
        }
        else
        {
            // we need to catch up
            mySpeed = maxSpeed;
        }
        
        linearVelocity = mySpeed * direction.normalized;
        transform.position = transform.position + linearVelocity * Time.deltaTime;
    }
}