using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    private void Start()
    {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Plane").GetComponent<Collider>(), GetComponent<Collider>());
        foreach(GameObject gm in GameObject.FindGameObjectsWithTag("Interactable"))
            Physics.IgnoreCollision(gm.GetComponent<Collider>(), GetComponent<Collider>());
    }
    [HideInInspector]
    public Hand m_ActiveHand = null;

}
