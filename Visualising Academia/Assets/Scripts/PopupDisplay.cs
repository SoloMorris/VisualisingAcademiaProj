using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;

public class PopupDisplay : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    public TextMeshProUGUI TitleField;
    public TextMeshProUGUI AuthorField;
    public TextMeshProUGUI DateField;
    public TextMeshProUGUI isPartofField;
    public TextMeshProUGUI PublisherField;
    public TextMeshProUGUI TopicsField;
    public TextMeshProUGUI LanguageField;

    public void SetupField(Document newData)
    {
        TitleField.text = newData.Title.AttributeValue; // TODO: Add a method of dealing with weird characters in titles.
        for (var i = 0; i < newData.Authors.AttributesList.Count; i++)
        {
            AuthorField.text += newData.Authors.AttributesList[i];
            if (newData.Authors.AttributesList.Count > i + 1)
                AuthorField.text += ", ";
        }
        DateField.text = newData.DatePublished.AttributeValue;
        isPartofField.text = newData.IsPartOf.AttributeValue;
        PublisherField.text = newData.Publisher.AttributeValue;
        LanguageField.text = newData.Language.AttributeValue;
        uiPanel.transform.position = newData.transform.position;
        //Camera.main.GetComponent<FreeFlyCamera>()._active = false;
        //Cursor.lockState = CursorLockMode.None;
    }

    public void CloseWindow()
    {
        print("Closing");
        TitleField.text = "";
        AuthorField.text = "";
        DateField.text = "";
        PublisherField.text = "";
        LanguageField.text = "";
        gameObject.SetActive(false);
        //Camera.main.GetComponent<FreeFlyCamera>()._active = true;
    }
}
