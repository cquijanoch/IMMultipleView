using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTitleCanvas : MonoBehaviour
{
    public GameObject Head;

    void Start()
    {
        Head = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
        //if (objectToFollow)
        //    transform.position = objectToFollow.GetComponent<MeshFilter>().mesh.bounds.max;
    }

}

