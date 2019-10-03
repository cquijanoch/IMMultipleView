using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTitleCanvas : MonoBehaviour
{
    //public GameObject Head;
    public GameObject objectToFollow;

    void Start()
    {
        
    }
    
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
        if (objectToFollow)
        {
            transform.position = objectToFollow.transform.position + transform.TransformDirection(new Vector3(0, objectToFollow.transform.lossyScale.y/2f + 0.1f, 0));
            transform.rotation = objectToFollow.transform.rotation;
        }
            
    }

}

