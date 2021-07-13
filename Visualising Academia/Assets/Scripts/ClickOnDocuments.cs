using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickOnDocuments : MonoBehaviour
{

    [SerializeField] private GameObject displayWindow;
    private GameObject targetDoc;
    public LayerMask masks;

    void Update()
    {
        CheckMouseClick();
    }
    /// <summary>
    /// When the user clicks, retrieve the document data from the object
    /// </summary>
    private void CheckMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 100f, masks)) return;
        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 100f);
        //  If we hit a button, execute whatever function it's connected to
        if (hit.transform.gameObject.TryGetComponent(out Button but))
        {
            print("invoking");
            but.onClick.Invoke();
        }
        //  If we hit a document object, display the Popup Window
        else if (hit.transform.gameObject.TryGetComponent(out Document doc))
        {
            if (displayWindow.GetComponent<PopupDisplay>().TitleField.text == doc.Title.AttributeValue) return;
            targetDoc = doc.gameObject;
            displayWindow.GetComponent<PopupDisplay>().CloseWindow();
            displayWindow.SetActive(true);
            displayWindow.GetComponent<PopupDisplay>().SetupField(doc);
        }
        

    }
}