using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOnDocuments : MonoBehaviour
{

    [SerializeField] private GameObject displayWindow;
    private GameObject targetDoc;

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
        if (!Physics.Raycast(ray, out hit, 100f, 3)) return;
        if (hit.transform.gameObject.TryGetComponent(out Document doc))
        {
            targetDoc = doc.gameObject;
            displayWindow.GetComponent<PopupDisplay>().CloseWindow();
            displayWindow.SetActive(true);
            displayWindow.GetComponent<PopupDisplay>().SetupField(doc);
            Camera.main.GetComponent<FreeFlyCamera>()._active = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}