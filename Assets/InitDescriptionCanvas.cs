using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitDescriptionCanvas : MonoBehaviour
{
    public GameObject Head;
    public Transform objectToFollow = null;

    void Start()
    {
        Head = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
        if (objectToFollow)
            transform.position = objectToFollow.position;
    }

    public void SetGameObjectToFollow(Transform objToFollow)
    {
        this.objectToFollow = objToFollow;
    }

}

