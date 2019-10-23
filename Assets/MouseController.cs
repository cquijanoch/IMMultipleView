using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{

    private int m_FingerID = -1;
    private Interaction m_interaction;
    private bool m_dragging = false;
    private Subspace subspaceNearest = null;
    private GameObject subspacePivot = null;
    
    private Vector3 cachedPosition;
    private bool m_modeDelete = false;
    private bool m_isAxisInUse = false;

    public int currentHit = 0;
    public float rotationSpeed = 1000f;
    public GameObject dialogConfirmation = null;
    public GameObject pivot = null;

    /**Interactable Components **/
    protected MeshRenderer[] highlightRenderers;
    protected MeshRenderer[] existingRenderers;
    protected GameObject highlightHolder;
    protected SkinnedMeshRenderer[] highlightSkinnedRenderers;
    protected SkinnedMeshRenderer[] existingSkinnedRenderers;
    protected static Material highlightMat;

    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;

    public GameObject interaction = null;

    private void Awake()
    {
        #if !UNITY_EDITOR
             fingerID = 0; 
        #endif
    }

    void Start()
    {
        highlightMat = (Material)Resources.Load("SteamVR_HoverHighlight", typeof(Material));
        if (interaction)
            m_interaction = interaction.GetComponent<Interaction>();
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        yield return new WaitForSeconds(2);
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas)
        {
            for (int i = 0; i < canvas.transform.childCount; i++)
            {
                if (canvas.transform.GetChild(i).name == "DialogConfirmation")
                {
                    dialogConfirmation = canvas.transform.GetChild(i).gameObject;
                    for (int j = 0; j < dialogConfirmation.transform.childCount; j++)
                    {
                        if (dialogConfirmation.transform.GetChild(j).name == "ButtonYes")
                            dialogConfirmation.transform.GetChild(j).GetComponent<Button>().onClick.AddListener(ConfirmationDeleteYes);
                        if (dialogConfirmation.transform.GetChild(j).name == "ButtonNo")
                            dialogConfirmation.transform.GetChild(j).GetComponent<Button>().onClick.AddListener(ConfirmationDeleteNo);
                    }   
                }

                if (canvas.transform.GetChild(i).name == "Text1")
                    text1 = canvas.transform.GetChild(i).GetComponent<Text>();
                if (canvas.transform.GetChild(i).name == "Text2")
                    text2 = canvas.transform.GetChild(i).GetComponent<Text>();
                if (canvas.transform.GetChild(i).name == "Text3")
                    text3 = canvas.transform.GetChild(i).GetComponent<Text>();
                if (canvas.transform.GetChild(i).name == "Text4")
                    text4 = canvas.transform.GetChild(i).GetComponent<Text>();
            }
        }

        if (!interaction)
        {
            GameObject coordenation = GameObject.Find("Coordination");
            if (coordenation)
                m_interaction = coordenation.GetComponent<Interaction>();
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit[] f = Physics.RaycastAll(ray, Mathf.Infinity);

        if (Input.GetMouseButtonUp(1))
        {
            GetComponent<MovementFPS>().stopMovement = false;
            if (subspaceNearest && !m_modeDelete)
            {
                subspaceNearest.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
            }
            cachedPosition = Vector3.zero;
            m_dragging = false;
            DeletePivot();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Collider collision = GetNearestCollider(f, LayerMask.NameToLayer("Subspace"));
            
            if (collision)
            {
                subspaceNearest = collision.GetComponent<Subspace>();
                GetComponent<MovementFPS>().stopMovement = true;
                CreatePivot();
                subspaceNearest.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITH_CONTROLLER;
                subspaceNearest.GetComponent<Subspace>().m_letRotate = false;
                cachedPosition = transform.position;
            }
            return;
        }

        if (subspaceNearest && Input.GetMouseButton(1))//macro
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_dragging = true;
                DeletePivot();
                CreatePivot();
                subspaceNearest.GetComponent<Subspace>().m_letRotate = true;
                return;
            }
            if (m_dragging && (Input.GetMouseButtonUp(0)))
            {
                m_dragging = false;
                DeletePivot();
                subspaceNearest.GetComponent<Subspace>().m_letRotate = false;
                return;
            }
                

            if (m_dragging)
            {
                float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
                float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;
                
                subspaceNearest.transform.parent.Rotate(Vector3.up, -rotX);
                subspaceNearest.transform.parent.Rotate(Vector3.right, rotY);
            }


            if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0
                || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q))
            {
                if (m_isAxisInUse == false)
                {
                    cachedPosition = transform.position;
                    float translation = Input.GetAxis("Vertical") * 10f;
                    float straffe = Input.GetAxis("Horizontal") * 10f;
                    float high = 0f;
                    if (Input.GetKey(KeyCode.E))
                        high = 0.01f;
                    if (Input.GetKey(KeyCode.Q))
                        high = -0.01f;
                    translation *= Time.deltaTime;
                    straffe *= Time.deltaTime;
                    DeletePivot();
                    CreatePivot();
                    Vector3 newPos = subspacePivot.transform.position + subspacePivot.transform.TransformDirection(straffe, high, translation);
                    if (newPos.x < 2.7f && newPos.x > -2.7f
                    && newPos.z < 2.7f && newPos.z > -2.7f
                    && newPos.y < 4f && newPos.y > 1f)
                        subspacePivot.transform.Translate(straffe, high, translation);
                    m_isAxisInUse = true;
                }

                if (Input.GetAxisRaw("Vertical") == 0 || Input.GetAxisRaw("Horizontal") == 0)
                    m_isAxisInUse = false;
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
                subspaceNearest.ChangeScaleScroll(Input.GetAxis("Mouse ScrollWheel"));

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Clone(subspaceNearest);
            }

            if (Input.GetKeyDown(KeyCode.Delete) && !subspaceNearest.isOriginal)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamMouseLook>().mouselooked = true;
                m_modeDelete = true;
                subspaceNearest.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_PREPARE_TO_DELETE;
                dialogConfirmation.SetActive(true);
            }

        }

        bool existData = false;
        Collider dataNearest = GetNearestCollider(f, LayerMask.NameToLayer("Data"));

        if (dataNearest)
        {
            existData = true;
            if (currentHit != dataNearest.gameObject.GetInstanceID())
            {
                Destroy(highlightHolder);
                currentHit = dataNearest.gameObject.GetInstanceID();
                Interactable interactable = dataNearest.gameObject.GetComponent<Interactable>();
                CreateHighlightRenderers(interactable);
                if (text1) text1.text = dataNearest.GetComponent<Data>().Name_1;
                if (text2) text2.text = dataNearest.GetComponent<Data>().Name_2;
                if (text3) text3.text = dataNearest.GetComponent<Data>().Name_3;
                if (text4) text4.text = dataNearest.GetComponent<Data>().Name_4;
            } 
        }

        if (!existData)
        {
            currentHit = 0;
            Destroy(highlightHolder);
            CleanTextCanvas();
        }
        UpdateHighlightRenderers(false);

        if (m_modeDelete)
            return;

        if (!m_dragging && existData && Input.GetMouseButtonUp(0) && m_interaction)// micro
        {
            m_interaction.FilterData(dataNearest.GetComponent<Data>());
            return;
        }
    }

    private void CreatePivot()
    {
        subspacePivot = Instantiate(pivot, subspaceNearest.transform.position, transform.rotation) as GameObject;
        subspaceNearest.transform.SetParent(subspacePivot.transform);
    }

    private void DeletePivot()
    {
        if (subspaceNearest)
        {
            subspaceNearest.transform.SetParent(null);
        }
        Destroy(subspacePivot);
    }

    public void ConfirmationDeleteYes()
    {
        Destroy(subspaceNearest.gameObject);
        dialogConfirmation.SetActive(false);
        m_modeDelete = false;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamMouseLook>().mouselooked = false;
        GetComponent<MovementFPS>().stopMovement = false;
    }

    public void ConfirmationDeleteNo()
    {
        dialogConfirmation.SetActive(false);
        subspaceNearest.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;
        m_modeDelete = false;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamMouseLook>().mouselooked = false;
        GetComponent<MovementFPS>().stopMovement = false;
    }

    private void Clone(Subspace subspace)
    {
        GameObject clone = Instantiate(subspace.gameObject,
            subspacePivot.transform.position + new Vector3(0.2f, 0, 0), subspace.transform.rotation) as GameObject;
        clone.GetComponent<Subspace>().m_numControllersInner = 0;
        clone.GetComponent<Subspace>().m_modePrepareToDelete = false;
        clone.GetComponent<Subspace>().isOriginal = false;
        clone.GetComponent<Subspace>().m_letFilter = false;
        clone.GetComponent<Subspace>().m_letRotate = false;
        clone.GetComponent<Renderer>().material.color = Constants.SPACE_COLOR_WITHOUT_CONTROLLER;

        if (m_interaction)
        {
            if (m_interaction.versionSubspace.ContainsKey(subspace.name))
                m_interaction.versionSubspace[subspace.name]++;
            else
                m_interaction.versionSubspace.Add(subspace.name, 1);
            clone.GetComponent<Subspace>().version = m_interaction.versionSubspace[subspace.name];
        }
    }
    private void CleanTextCanvas()
    {
        if (text1) text1.text = "";
        if (text2) text2.text = "";
        if (text3) text3.text = "";
        if (text4) text4.text = "";
    }

    private Collider GetNearestCollider(RaycastHit[] hits, int layer)
    {
        float distance = float.MaxValue;
        int value = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform != transform && hits[i].collider.gameObject.layer == layer)
            {
                float currentDistance = Vector3.Distance(transform.position, hits[i].collider.transform.position);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    value = i;
                }
            }
        }
        if (value <= 0)
            return null;
        return hits[value].collider;
    }

    private void CreateHighlightRenderers(Interactable interactable)
    {
        existingSkinnedRenderers = interactable.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        highlightHolder = new GameObject("Highlighter");
        highlightSkinnedRenderers = new SkinnedMeshRenderer[existingSkinnedRenderers.Length];

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];
            GameObject newSkinnedHolder = new GameObject("SkinnedHolder");
            newSkinnedHolder.transform.parent = highlightHolder.transform;
            SkinnedMeshRenderer newSkinned = newSkinnedHolder.AddComponent<SkinnedMeshRenderer>();
            Material[] materials = new Material[existingSkinned.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = highlightMat;
            }

            newSkinned.sharedMaterials = materials;
            newSkinned.sharedMesh = existingSkinned.sharedMesh;
            newSkinned.rootBone = existingSkinned.rootBone;
            newSkinned.updateWhenOffscreen = existingSkinned.updateWhenOffscreen;
            newSkinned.bones = existingSkinned.bones;
            highlightSkinnedRenderers[skinnedIndex] = newSkinned;
        }

        MeshFilter[] existingFilters = interactable.GetComponentsInChildren<MeshFilter>(true);
        existingRenderers = new MeshRenderer[existingFilters.Length];
        highlightRenderers = new MeshRenderer[existingFilters.Length];

        for (int filterIndex = 0; filterIndex < existingFilters.Length; filterIndex++)
        {
            MeshFilter existingFilter = existingFilters[filterIndex];
            MeshRenderer existingRenderer = existingFilter.GetComponent<MeshRenderer>();

            if (existingFilter == null || existingRenderer == null)
                continue;

            GameObject newFilterHolder = new GameObject("FilterHolder");
            newFilterHolder.transform.parent = highlightHolder.transform;
            MeshFilter newFilter = newFilterHolder.AddComponent<MeshFilter>();
            newFilter.sharedMesh = existingFilter.sharedMesh;
            MeshRenderer newRenderer = newFilterHolder.AddComponent<MeshRenderer>();

            Material[] materials = new Material[existingRenderer.sharedMaterials.Length];
            for (int materialIndex = 0; materialIndex < materials.Length; materialIndex++)
            {
                materials[materialIndex] = highlightMat;
            }
            newRenderer.sharedMaterials = materials;

            highlightRenderers[filterIndex] = newRenderer;
            existingRenderers[filterIndex] = existingRenderer;
        }
    }

    private void UpdateHighlightRenderers(bool attachedToHand)
    {
        if (highlightHolder == null)
            return;

        for (int skinnedIndex = 0; skinnedIndex < existingSkinnedRenderers.Length; skinnedIndex++)
        {
            SkinnedMeshRenderer existingSkinned = existingSkinnedRenderers[skinnedIndex];
            SkinnedMeshRenderer highlightSkinned = highlightSkinnedRenderers[skinnedIndex];

            if (existingSkinned != null && highlightSkinned != null && attachedToHand == false)
            {
                highlightSkinned.transform.position = existingSkinned.transform.position;
                highlightSkinned.transform.rotation = existingSkinned.transform.rotation;
                highlightSkinned.transform.localScale = existingSkinned.transform.lossyScale;
                highlightSkinned.localBounds = existingSkinned.localBounds;
                highlightSkinned.enabled = existingSkinned.enabled && existingSkinned.gameObject.activeInHierarchy;

                int blendShapeCount = existingSkinned.sharedMesh.blendShapeCount;
                for (int blendShapeIndex = 0; blendShapeIndex < blendShapeCount; blendShapeIndex++)
                {
                    highlightSkinned.SetBlendShapeWeight(blendShapeIndex, existingSkinned.GetBlendShapeWeight(blendShapeIndex));
                }
            }
            else if (highlightSkinned != null)
                highlightSkinned.enabled = false;

        }

        for (int rendererIndex = 0; rendererIndex < highlightRenderers.Length; rendererIndex++)
        {
            MeshRenderer existingRenderer = existingRenderers[rendererIndex];
            MeshRenderer highlightRenderer = highlightRenderers[rendererIndex];

            if (existingRenderer != null && highlightRenderer != null && attachedToHand == false)
            {
                highlightRenderer.transform.position = existingRenderer.transform.position;
                highlightRenderer.transform.rotation = existingRenderer.transform.rotation;
                highlightRenderer.transform.localScale = existingRenderer.transform.lossyScale;
                highlightRenderer.enabled = existingRenderer.enabled && existingRenderer.gameObject.activeInHierarchy;
            }
            else if (highlightRenderer != null)
                highlightRenderer.enabled = false;
        }
    }
}
