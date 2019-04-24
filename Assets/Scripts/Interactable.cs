using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public float m_minPositionDistance = 0.5f;
    public float m_minRotationDistance = 300f;

    [HideInInspector]
    public Hand m_PrimaryHand = null;

    [HideInInspector]
    public Hand m_SecondaryHand = null;

    public int m_numControllersInner = 0;

    //[HideInInspector]
    //public bool m_isTarget = false;

    private void Start()
    {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Plane").GetComponent<Collider>(), GetComponent<Collider>());
        foreach (GameObject gm in GameObject.FindGameObjectsWithTag("Interactable"))
            Physics.IgnoreCollision(gm.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void Update()
    {

    }

    /**
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Controller"))
            return;
        m_numControllersInner++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Controller"))
            return;
        m_numControllersInner++;
    }**/

    public bool DetectSimimilarTransform(Interactable other)
    {
        print(Vector3.Distance(transform.position, other.gameObject.transform.position));
        print(Vector3.Distance(transform.rotation.eulerAngles, other.gameObject.transform.rotation.eulerAngles));
        if (Vector3.Distance(transform.position, other.gameObject.transform.position) <= m_minPositionDistance &&
            Vector3.Distance(transform.rotation.eulerAngles, other.gameObject.transform.rotation.eulerAngles) < m_minRotationDistance)
            return true;
        return false;
    }

    public void SetTransformToObject(Interactable obj)
    {
        transform.position = obj.transform.position;
        transform.rotation = obj.transform.rotation;
    }

}
