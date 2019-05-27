using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public float m_minPositionDistance = 0.5f;
    public float m_minRotationDistance = 300f;
    public float m_minDiffAngleRotation = 15f;

    [HideInInspector]
    public bool m_modeScale = false;

    [HideInInspector]
    public float m_distanceInitialForScale = 0f;

    [HideInInspector]
    public MacroHand m_PrimaryHand = null;

    [HideInInspector]
    public MacroHand m_SecondaryHand = null;

    [HideInInspector]
    public HashSet<MacroHand> m_HandsActivedInner = new HashSet<MacroHand>();

    public int m_numControllersInner = 0;

    private void Start()
    {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Plane").GetComponent<Collider>(), GetComponent<Collider>());
        foreach (GameObject gm in GameObject.FindGameObjectsWithTag("Interactable"))
            Physics.IgnoreCollision(gm.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void Update()
    {
        if (m_modeScale)
            ChangeScale();
    }

    public bool DetectSimimilarTransform(Interactable other)
    {
        if (Vector3.Distance(transform.position, other.gameObject.transform.position) <= m_minPositionDistance) //&&                                                                                                 // Vector3.Distance(transform.rotation.eulerAngles, other.gameObject.transform.rotation.eulerAngles) < m_minRotationDistance)
            return true;
        return false;
    }

    public void SetTransformToObject(Interactable obj)
    {
        transform.position = obj.transform.position;
        transform.rotation *= GetShortestQuaternion(obj.transform);
    }

    private Quaternion GetShortestQuaternion(Transform obj)
    {
        Quaternion result = Quaternion.identity;

        float angle = float.MaxValue;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    Quaternion qd = obj.transform.rotation * Quaternion.Euler(0 + 90f * i, 0 + 90f * j, 0 + 90f * k);
                    Quaternion diff = Quaternion.Inverse(transform.rotation) * qd;
                    float anglebtw = Mathf.Min(angle, 2 * Mathf.Atan2(Vector3.Magnitude(new Vector3(diff.x, diff.y, diff.z)), diff.w));
                    if (anglebtw != angle)
                    {
                        angle = anglebtw;
                        result = diff;
                    }
                }
            }
        }
        return result;
    }

    public void ChangeScale()
    {
        if (!m_PrimaryHand)
        {
            ResetDistanceInitialForScale();
            return;
        }

        if (!m_SecondaryHand)
        {
            ResetDistanceInitialForScale();
            return;
        }

        Vector3 pos1 = m_PrimaryHand.transform.position;
        Vector3 pos2 = m_SecondaryHand.transform.position;

        float dist = Vector3.Distance(pos1, pos2);
        transform.localScale += new Vector3(dist - m_distanceInitialForScale, dist - m_distanceInitialForScale, dist - m_distanceInitialForScale) * 1f;
        m_distanceInitialForScale = dist;
    }

    public void ResetDistanceInitialForScale()
    {
        m_distanceInitialForScale = 0f;
        m_modeScale = false;
    }

}
