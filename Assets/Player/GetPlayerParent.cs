using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerParent : MonoBehaviour
{
    private GameObject owner;

    public GameObject GetOwner()
    {
        return owner;
    }
    void Start()
    {
        //Cycles up every body part until the parent player system is found
        Transform checkTransform = this.transform;
        Debug.Log(checkTransform.name);
        while (true)
        {
            Transform parent = checkTransform.parent;
            Debug.Log("PARENT " + parent.name);

            if (parent.tag == "Player")
            {
                owner = parent.gameObject;
                break;
            }
            checkTransform = parent;
        }
    }

    void Update()
    {

    }
}
