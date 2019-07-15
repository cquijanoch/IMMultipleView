using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class Subspace : MonoBehaviour
{
    public float m_minPositionDistance = 0.5f;
    public float m_minRotationDistance = 300f;
    public float m_minDiffAngleRotation = 15f;

    public bool m_modeScale = false;

    public float m_distanceInitialForScale = 0f;

    public MacroHand m_PrimaryHand = null;

    public MacroHand m_SecondaryHand = null;

    /** HashSet of MacroHands inner into subspace**/
    public HashSet<MacroHand> m_HandsActivedInner = new HashSet<MacroHand>();

    public int m_numControllersInner = 0;

    public bool m_modePrepareToDelete = false;

    public List<Data> selectedData;

    private void Start()
    {
        Physics.IgnoreCollision(GameObject.FindGameObjectWithTag("Plane").GetComponent<Collider>(), GetComponent<Collider>());
        /**foreach (GameObject gm in GameObject.FindGameObjectsWithTag("Subspace"))
            Physics.IgnoreCollision(gm.GetComponent<Collider>(), GetComponent<Collider>());**/
    }

    private void Update()
    {
        if (m_modeScale)
            ChangeScale();
    }

    public bool DetectSimimilarTransform(Subspace other)
    {
        if (Vector3.Distance(transform.position, other.gameObject.transform.position) <= m_minPositionDistance) //&&                                                                                                 // Vector3.Distance(transform.rotation.eulerAngles, other.gameObject.transform.rotation.eulerAngles) < m_minRotationDistance)
            return true;
        return false;
    }

    public void SetTransformToObject(Subspace obj)
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

    public int CountHandsActivedInner()
    {
        int q = 0;
        foreach(MacroHand mc in m_HandsActivedInner)
        {
            if (mc.enabled)
                q++;
        }
        return q;
    }

    public void SetAutoColor()
    {
        if (m_numControllersInner == 0 || GetNumberUsedHandsInner(true) == 0)
        {
            GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        }
        else
        {
            GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
        }

    }

    public int GetNumberUsedHandsInner(bool isUsing = true)
    {
        int n = 0;
        foreach (MacroHand mc in m_HandsActivedInner)
        {
            if (mc.enabled && mc.GetCurrentSubspace() == this)
                n++;
        }
        return isUsing ? n : m_HandsActivedInner.Count - n;
    }

}
