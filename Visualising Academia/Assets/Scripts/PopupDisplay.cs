using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class PopupDisplay : MonoBehaviour
{
    public TextMeshProUGUI TitleField;
    public TextMeshProUGUI AuthorField;
    public TextMeshProUGUI DateField;
    public TextMeshProUGUI isPartofField;
    public TextMeshProUGUI PublisherField;
    public TextMeshProUGUI TopicsField;
    public TextMeshProUGUI LanguageField;

    public void SetupField(Document newData)
    {
        TitleField.text = newData.title; // TODO: Add a method of dealing with weird characters in titles.
        for (var i = 0; i < newData.Authors.Count; i++)
        {
            AuthorField.text += newData.Authors[i];
            if (newData.Authors.Count > i + 1)
                AuthorField.text += ", ";
        }
        DateField.text = newData.datePublished;
        isPartofField.text = newData.isPartOf;
        PublisherField.text = newData.publisher;
        LanguageField.text = newData.language;
        Camera.main.GetComponent<FreeFlyCamera>()._active = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseWindow()
    {
        TitleField.text = "";
        AuthorField.text = "";
        DateField.text = "";
        PublisherField.text = "";
        LanguageField.text = "";
        gameObject.SetActive(false);
        Camera.main.GetComponent<FreeFlyCamera>()._active = true;
    }
}
