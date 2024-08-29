using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodysLeft : MonoBehaviour
{
    public GameObject[] bodys;

    public void changeBodys(int amount)
    {
        foreach (GameObject b in bodys)
        {
            b.SetActive(false);
        }
        StartCoroutine(smoothActive(amount));
    }

    IEnumerator smoothActive(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            bodys[i].SetActive(true);
            yield return new WaitForSeconds(.2f);
        }
    }
}
