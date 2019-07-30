using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitDialogCanvas : MonoBehaviour
{
    public GameObject Head;
    public bool Answer = false;

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
        transform.parent.GetComponent<MacroHand>().Delete(Answer);
    }

    public void onClickNoButton()
    {
        print("No Button Clicked");
        Answer = false;
        transform.parent.GetComponent<MacroHand>().Delete(Answer);
    }
}