using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class InitMainMenu : MonoBehaviour
{
    private int m_currentScene = 0;
    public int m_trainNumber = -1;
    public int m_scenarioCase = -1;
    public float m_totalTime = 0f;
    public float m_totalTrainTime = 0f;
    public float m_totalScenarioTime = 0f;

    public int m_versionDataset = 0;

    public GameObject m_textUserId;
    public GameObject m_textTaskId;
    public GameObject m_textVersionId;

    public GameObject m_buttonWait;

    public GameObject m_buttonTrain1;
    public GameObject m_buttonTrain2;

    public GameObject m_buttonScene1;
    public GameObject m_buttonScene2;

    public GameObject m_buttonLowScene1;
    public GameObject m_buttonLowScene2;

    public GameObject m_textTotalTime;
    public GameObject m_textTrainTime;
    public GameObject m_textTaskTime;

    public GameObject m_toogle;

    public GameObject m_StartPosition;
    public GameObject m_PlayerVR;
    public GameObject m_PlayerFPS;
    private GameObject m_Player;

    public Record logHandler;
    public bool VR;
    public string CSVResults;
    public string CSVFilename;
    public string PositionFilename;
    public string PostionResults;

    public int interval = 1;
    public float nextTime = 0f;

    public bool training = true;

    private List<string> m_positions = new List<string>();

    private void Awake()
    {  
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(m_StartPosition);
        
    }
    void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        logHandler = new Record();
    }

    void Update()
    {
        if (m_trainNumber > 0 || m_scenarioCase > 0)
            m_totalTime += Time.deltaTime;

        if (m_trainNumber > 0)
            m_totalTrainTime += Time.deltaTime;

        if (m_scenarioCase > 0)
            m_totalScenarioTime += Time.deltaTime;

        m_textTotalTime.GetComponent<Text>().text = m_totalTime.ToString();
        m_textTrainTime.GetComponent<Text>().text = m_totalTrainTime.ToString();
        m_textTaskTime.GetComponent<Text>().text = m_totalScenarioTime.ToString();

        if (!training && m_Player && Time.time >= nextTime)
        {
            float postionX = m_Player.transform.position.x;
            float postionZ = m_Player.transform.position.z;

            m_positions.Add(postionX.ToString("0.00000") + "," + postionZ.ToString("0.00000") + "," + m_Player.transform.forward + "," + Time.time.ToString(new CultureInfo("en-US")));
            nextTime += interval;
        }
    }

    public void StartTraining()
    {
        Train_1();
    }

    public void WaitScene()
    {
        m_scenarioCase = -1;
        CleanColorButton();
        m_buttonWait.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 7;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_trainNumber = -1;
        training = true;
        StartCoroutine(ShowConsoleWait());
    }

    public void Train_1()
    {
        m_scenarioCase = -1;
        CleanColorButton();
        m_buttonTrain1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 1;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_trainNumber = 1;
        training = true;
    }

    public void Train_2()
    {
        m_scenarioCase = - 1;
        CleanColorButton();
        m_buttonTrain2.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 2;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_trainNumber = 2;
        training = true;
    }

    public void StartTask()
    {
        Scenario_1();
    }

    public void Scenario_1()
    {
        m_trainNumber = -1;
        CleanColorButton();
        m_buttonScene1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 3;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_scenarioCase = 1;
        training = false;
        StartCoroutine(ShowConsoleTask());
    }

    public void Scenario_2()
    {
        m_trainNumber = -1;
        CleanColorButton();
        m_buttonScene2.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 5;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_scenarioCase = 3;
        training = false;
        StartCoroutine(ShowConsoleTask());
    }

    public void LowScenario_1()
    {
        m_trainNumber = -1;
        CleanColorButton();
        m_buttonLowScene1.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 4;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_scenarioCase = 2;
        training = false;
        StartCoroutine(ShowConsoleTask());
    }

    public void LowScenario_2()
    {
        m_trainNumber = -1;
        CleanColorButton();
        m_buttonLowScene2.GetComponent<Image>().color = Constants.BUTTON_COLOR_ACTIVATE;
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 6;
        SceneManager.LoadScene(m_currentScene);
        CreatePlayer();
        m_scenarioCase = 4;
        training = false;
        StartCoroutine(ShowConsoleTask());
    }

    private void CleanColorButton()
    {
        m_buttonWait.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTrain1.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonTrain2.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonScene1.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonScene2.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonLowScene1.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
        m_buttonLowScene2.GetComponent<Image>().color = Constants.BUTTON_COLOR_DESACTIVATE;
    }

    public void StopTrain()
    {
        m_trainNumber = -1;
    }

    public void StopTask()
    {
        m_scenarioCase = -1;
    }

    public void ResetValues()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        m_currentScene = 0;
        //SceneManager.LoadScene(0);
        m_trainNumber = -1;
        m_scenarioCase = -1;
        m_totalTime = 0f;
        m_totalTrainTime = 0f;
        m_totalScenarioTime = 0f;
        //m_textUserId.GetComponent<InputField>().text = "";
        CleanColorButton();
        m_positions.Clear();
    }

    public void CreatePlayer()
    {
        VR = m_toogle.GetComponent<Toggle>().isOn;
        if (VR)
            m_Player = Instantiate(m_PlayerVR, m_StartPosition.transform.position, m_StartPosition.transform.rotation);
        else
            m_Player =Instantiate(m_PlayerFPS, new Vector3(m_StartPosition.transform.position.x, 1.75f, m_StartPosition.transform.position.z), m_StartPosition.transform.rotation);
        m_positions.Clear();
    }

    public void EndTask()
    {
        if (training)
        {
            ResetValues();
            if (m_trainNumber == 1) Train_2();
            else if (m_trainNumber == 0) Train_1();
            WaitScene();
            return;
        }
        int task = Convert.ToInt32(m_textTaskId.GetComponent<InputField>().text);
        int version = Convert.ToInt32(m_textVersionId.GetComponent<InputField>().text);
        CSVFilename =   "User-" + m_textUserId.GetComponent<InputField>().text + "-" +
                        "Task-" + task + "-" +
                        "Version-" + version + "-" +
                        "Scenario-" + m_scenarioCase + "-" + "VR" + VR;

        PositionFilename = "Position_" + CSVFilename;

        CSVResults = m_textUserId.GetComponent<InputField>().text + "," +
            task + "," + version  + "," + m_scenarioCase + "," + VR + "," + m_totalScenarioTime.ToString(new CultureInfo("en-US"));

        if (!VR)
        {
            CSVResults +=
                "," + m_Player.GetComponent<MouseController>().m_clones + 
                "," + m_Player.GetComponent<MouseController>().m_deletedAcepted +
                "," + m_Player.GetComponent<MouseController>().m_deletedCanceled +
                "," + m_Player.GetComponent<MouseController>().m_selectSubspaces +
                "," + m_Player.GetComponent<MouseController>().interaction.GetComponent<Interaction>().m_selectSingle +
                "," + m_Player.GetComponent<MouseController>().interaction.GetComponent<Interaction>().m_brushing +
                "," + m_Player.GetComponent<MouseController>().m_pickupTime.ToString(new CultureInfo("en-US")) +
                "," + m_Player.GetComponent<MouseController>().m_rotationTime.ToString(new CultureInfo("en-US")) +
                "," + m_Player.GetComponent<MouseController>().m_traslationTime.ToString(new CultureInfo("en-US")) +
                "," + m_Player.GetComponent<MouseController>().m_scaleTime.ToString(new CultureInfo("en-US")) +
                "," + m_Player.GetComponent<MouseController>().m_navSlaving.ToString(new CultureInfo("en-US")) +
                ", 0 , 0 , 0 , 0, 0" +
                "," + GetDataSelected();
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            MacroHand leftMacro = m_Player.GetComponent<MovementVR>().LeftHand.GetComponent<MacroHand>();
            MacroHand rightMacro = m_Player.GetComponent<MovementVR>().RightHand.GetComponent<MacroHand>();
            CSVResults +=
                "," + leftMacro.m_clones + rightMacro.m_clones +
                "," + leftMacro.m_deletedAcepted + rightMacro.m_deletedAcepted +
                "," + leftMacro.m_deletedCanceled + rightMacro.m_deletedCanceled +
                "," + leftMacro.m_selectSubspaces + rightMacro.m_selectSubspaces +
                "," + m_Player.GetComponent<MovementVR>().Coordination.GetComponent<Interaction>().m_selectSingle +
                "," + m_Player.GetComponent<MovementVR>().Coordination.GetComponent<Interaction>().m_brushing +
                "," + leftMacro.m_pickupTime.ToString(new CultureInfo("en-US")) +
                "," + 0 +
                "," + 0 +
                "," + leftMacro.m_scaleTime.ToString(new CultureInfo("en-US")) +
                "," + leftMacro.m_navSlaving.ToString(new CultureInfo("en-US")) +
                "," + rightMacro.m_pickupTime.ToString(new CultureInfo("en-US")) +
                "," + 0 +
                "," + 0 +
                "," + rightMacro.m_scaleTime.ToString(new CultureInfo("en-US")) +
                "," + rightMacro.m_navSlaving.ToString(new CultureInfo("en-US")) +
                "," + GetDataSelected();
        }

        switch (task)
        {
            //"UserID, TaskID, Scenario, VR, Time(sec), Clones, Delete accepted, Delete canceled, Select Subspaces(clicks),
            //" Rotation(sec), Traslation(sec), Scale(sec), NavSlaving(sec), Select Single, Brushing(clicks), " +
            //" UserAnswer, CorrectAnswer";
            case 1:
                CSVResults += "," + "Regulations";
                break;
            case 2:
                CSVResults += "," + "Mr Dibbs";
                break;
            case 3:
                CSVResults += "," + "Clasic Rock and pop";
                break;
            case 4:
                CSVResults += "," + "Questionario";
                break;
            default:
                UnityEngine.Debug.Log("Defina uma tarefa");
                break;
        }
        UnityEngine.Debug.Log(CSVResults);
        logHandler.Log(CSVResults, CSVFilename);
        logHandler.LogPosition(m_positions, PositionFilename);
        ResetValues();
        WaitScene();
    }

    IEnumerator ShowConsoleWait()
    {
        yield return new WaitForSeconds(3);
        string task = m_textTaskId.GetComponent<InputField>().text;
        if (task.Length > 0)
        {
            GameObject console = GameObject.Find("ConsoleTV");
            if (console)
            {
                Console current_console = console.GetComponent<Console>();
                if (VR)
                    current_console.AddText("Tire os oculus e responda a perguntas. Aguarde instruções...");
                else
                    current_console.AddText("Responda a perguntas. Aguarde instruções...");
            }
        }
    }

    IEnumerator ShowConsoleTask()
    {
        yield return new WaitForSeconds(3);
        int task = Convert.ToInt32(m_textTaskId.GetComponent<InputField>().text);
        GameObject console = GameObject.Find("ConsoleTV");
        if (console)
        {
            Console current_console = console.GetComponent<Console>();
            current_console.Clear();
            switch (task)
            {
                case 1:
                    current_console.AddText("Selecione o artista com mais músicas do gênero Punk entre 2005 e 2010.");
                    break;
                case 2:
                    current_console.AddText("1 - Selecione a música mais diferente da maioria das músicas do gênero Folk entre 2005 e 2010.");
                    current_console.AddText("2 - Selecione o artista mais similar da música selecionada.");
                    break;
                case 3:
                    current_console.AddText("1 - Filtre a música entre 1990 e 2000.");
                    current_console.AddText("2 - Filtre a música entre 2001 e 2005.");
                    current_console.AddText("3 - Selecione o gênero com o menor número de músicas comuns dos dois períodos.");
                    break;
            }
        }
    }

    private string GetDataSelected()
    {
        string dataSelected = "";
        foreach (GameObject subspace in GameObject.FindGameObjectsWithTag("Subspace"))
        {
            Subspace sub = subspace.GetComponent<Subspace>();
            dataSelected += "#" + sub.name + " " + sub.version + ": ";

            foreach (Data data in sub.selectedData)
                dataSelected += data.Id + "-";
        }
        return dataSelected;
    }
}
