using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public float m_minDistance = 0.5f;
    
    [HideInInspector]
    public Hand m_ActiveHand = null;

    private void Start()
    {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Plane").GetComponent<Collider>(), GetComponent<Collider>());
        foreach(GameObject gm in GameObject.FindGameObjectsWithTag("Interactable"))
            Physics.IgnoreCollision(gm.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void Update()
    {
          
    }

    public bool DetectSimimilarTransform(Interactable other)
    {
        print(Vector3.Distance(transform.position, other.gameObject.transform.position));
        if (Vector3.Distance(transform.position, other.gameObject.transform.position) <= m_minDistance)
            return true;
        return false;
    }

    public void SetTransformToObject(Interactable obj)
    {
        transform.position = obj.transform.position;
    }

}
