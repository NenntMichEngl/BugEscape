using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Textbox : MonoBehaviour
{
    public TMP_Text textMeshPro; // Reference to the TextMeshPro component
    public float typingSpeed = 0.1f; // Speed of typing in seconds

    public string fullText; // The complete text to be typed
    

    private void Start()
    {

        textMeshPro.text = string.Empty; // Clear the text
GetComponent<Image>().enabled = false;
    }
    public void displayText(string text)
    {
        if(text.Length > 0)
        GetComponent<Image>().enabled = true;
        StartCoroutine(TypeText(text));

    }
    public void cleartext()
    {
        GetComponent<Image>().enabled = false;
        textMeshPro.text = string.Empty;
    }
    IEnumerator TypeText(string text)
    {

        foreach (char letter in text)
        {
            textMeshPro.text += letter; 
            yield return new WaitForSeconds(typingSpeed); 
        }
    }

}
