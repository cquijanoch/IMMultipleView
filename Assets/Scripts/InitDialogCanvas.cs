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

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Head.transform.position);
    }

    /**private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Controller"))
            return;
        print("entro");
    }**/

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
