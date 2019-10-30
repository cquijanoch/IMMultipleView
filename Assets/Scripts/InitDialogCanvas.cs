using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitDialogCanvas : MonoBehaviour
{
    public GameObject Head;
    public bool Answer = false;
    public bool ModeTask = false;

    void Start()
    {
        Head = GameObject.FindGameObjectWithTag("MainCamera");
        if (transform.parent)
            transform.position = transform.parent.position;
        Answer = false;
    }
    
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
    }

    public void onClickYesButton()
    {
        print("Yes Button Clicked");
        Answer = true;
        if (!ModeTask)
            transform.parent.GetComponent<MacroHand>().Delete(Answer);
        else
        {
            GameObject menu = GameObject.Find("MainMenu");
            if (menu)
                menu.GetComponent<InitMainMenu>().EndTask();
        }
    }

    public void onClickNoButton()
    {
        print("No Button Clicked");
        Answer = false;
        if (!ModeTask)
            transform.parent.GetComponent<MacroHand>().Delete(Answer);
        else
        {
            transform.parent.GetComponent<Hand>().DisableFinishTask(Answer);
        }
        
    }
}