using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorManager : MonoBehaviour
{


    void Start()
    {
        Debug.Log("i am active");
    }
    public void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Opener"))
        {
            col.transform.parent.GetComponent<DoorOPener>().isOpen = true;

        }
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Opener"))
        {
            col.transform.parent.GetComponent<DoorOPener>().isOpen = false;
        }
    }

}
