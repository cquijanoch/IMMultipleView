using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class Subspace : MonoBehaviour
{
    private GameObject m_currentTitle;

    public float m_minPositionDistance = 0.5f;
    public float m_minRotationDistance = 300f;
    public float m_minDiffAngleRotation = 15f;
    public float m_distanceInitialForScale = 0f;
    public bool m_modeScale = false;
    public List<Data> selectedData;
    public List<string> subspacesChild;
    public bool isOriginal = false;
    public MacroHand m_PrimaryHand = null;
    public MacroHand m_SecondaryHand = null;
    public GameObject titleSubspace = null;
    public bool showTitle = false;
    public string name;

    public bool m_letFilter = true;
    public bool m_letRotate = true;
    /** HashSet of MacroHands inner into subspace**/
    [HideInInspector]
    public HashSet<MacroHand> m_HandsActivedInner = new HashSet<MacroHand>();
    [HideInInspector]
    public int m_numControllersInner = 0;
    [HideInInspector]
    public bool m_modePrepareToDelete = false;
    [HideInInspector]
    public int version = 0;

    private void Start()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Scenario"))
            Physics.IgnoreCollision(obj.GetComponent<Collider>(), GetComponent<Collider>());
        if (showTitle)
            ShowTitle();

    }

    private void Update()
    {
        if (m_modeScale)
            ChangeScale();

        if ((subspacesChild.Count > 0 && m_PrimaryHand) || (!XRSettings.enabled && m_letRotate))
            foreach (string s in subspacesChild)
                GameObject.Find(s).transform.rotation = transform.rotation;
    }

    private void OnDestroy()
    {
        if (m_currentTitle)
            Destroy(m_currentTitle);
    }

    public bool DetectSimimilarTransform(Subspace other)
    {
        if (Vector3.Distance(transform.position, other.gameObject.transform.position) <= m_minPositionDistance)                                                                                                 // Vector3.Distance(transform.rotation.eulerAngles, other.gameObject.transform.rotation.eulerAngles) < m_minRotationDistance)
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

    public void ChangeScaleScroll(float scale)
    {
        transform.localScale += new Vector3(scale, scale, scale);
    }

    public void ResetDistanceInitialForScale()
    {
        m_distanceInitialForScale = 0f;
        m_modeScale = false;
    }

    public int CountHandsActivedInner()
    {
        int q = 0;
        foreach (MacroHand mc in m_HandsActivedInner)
        {
            if (mc.enabled)
                q++;
        }
        return q;
    }

    public void SetAutoColor()
    {
        if (m_numControllersInner == 0 || GetNumberUsedHandsInner(true) == 0)
            GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        else
            GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
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

    public void ShowTitle()
    {
        if (titleSubspace)
        {
            m_currentTitle = Instantiate(titleSubspace) as GameObject;
            if (version > 0)
                m_currentTitle.GetComponentInChildren<Text>().text = name + " " + version;
            else
                m_currentTitle.GetComponentInChildren<Text>().text = name;
            m_currentTitle.GetComponent<InitTitleCanvas>().objectToFollow = gameObject;
        }
    }
}
