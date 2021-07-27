using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ClickOnDocuments : MonoBehaviour
{

    [SerializeField] private GameObject displayWindow;
    public LayerMask masks;
    private GameObject TargetNode;
    [SerializeField] private Visualiser visualiser;

    void Update()
    {
        CheckMouseClick();
        CheckInput();
    }
    /// <summary>
    /// When the user clicks, retrieve the document data from the object
    /// </summary>
    private void CheckMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, 300f, masks, QueryTriggerInteraction.Collide)) return;
        
        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue, 300f);
        //  If we hit a button, execute whatever function it's connected to
        if (hit.transform.gameObject.TryGetComponent(out Button but))
        {
            ToggleConnectionDisplay(TargetNode.transform);
            TargetNode = null;
            but.onClick.Invoke();
        }
        //  If we hit a document object, display the Popup Window
        else if (hit.transform.gameObject.TryGetComponent(out Document doc))
        {
            if (displayWindow.GetComponent<PopupDisplay>().TitleField.text == doc.Title.AttributeValue) return;
            displayWindow.GetComponent<PopupDisplay>().CloseWindow();
            displayWindow.SetActive(true);
            ToggleConnectionDisplay(hit.transform);
            TargetNode = hit.transform.gameObject;
            displayWindow.GetComponent<PopupDisplay>().SetupField(doc);
        }
        
    }

    /// <summary>
    /// If the user presses Space, generate a new network
    /// </summary>
    private void CheckInput()
    {
        if (TargetNode == null) return;
        if (Input.GetButtonDown("Jump"))
        {
            ToggleConnectionDisplay(TargetNode.transform);
            displayWindow.GetComponent<PopupDisplay>().CloseWindow();
            visualiser.GenerateNewNetwork(TargetNode.GetComponent<DocNode>());
        }
    }

    public void ToggleConnectionDisplay(Transform hit)
    {
        var node = hit.GetComponent<DocNode>();
        foreach (var iCon in node.incomingConnections)
        {
            iCon.connection.enabled = !iCon.connection.enabled;
        }

        foreach (var oCon in node.outgoingConnections)
        {
            oCon.connection.enabled = !oCon.connection.enabled;
        }
    }
}