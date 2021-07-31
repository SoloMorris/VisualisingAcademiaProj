using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using NodeNetwork;
public class ClickOnDocuments : MonoBehaviour
{

    [SerializeField] private GameObject displayWindow;
    public LayerMask masks;
    public GameObject TargetNode;
    [SerializeField] private Visualiser visualiser;
    [SerializeField] private NetworkOptions options;
    public bool viewingState { get; private set; } = false;

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
            but.onClick.Invoke();
        
        //  If we hit a document object, display the Popup Window
        else if (hit.transform.gameObject.TryGetComponent(out Document doc))
        {
            //  Ignore the click if the user is clicking the same node.
            if (displayWindow.GetComponent<PopupDisplay>().TitleField.text == doc.Title.AttributeValue) return;
            
            //  If the user clicks onto another node, close this one first.
            if (TargetNode != null && hit.transform != TargetNode.transform) ToggleConnectionDisplay(TargetNode.transform);
            
            TargetNode = hit.transform.gameObject;
            ToggleConnectionDisplay(TargetNode.transform);
            displayWindow.SetActive(true);
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

    public void OnNodeButtonClick()
    {
        ToggleConnectionDisplay(TargetNode.transform);
        TargetNode = null;
    }
    public void ToggleConnectionDisplay(Transform hit)
    {
        if (options.ViewAllConnections) return;
        
        var node = hit.GetComponent<DocNode>();
        foreach (var iCon in node.incomingConnections)
        {
            if (!iCon.visible)
            {
                iCon.connection.enabled = false;
                continue;
            }

            if (options.InternalNetworkView != NetworkOptions.InternalNetworkConnections.UNIQUE)
                iCon.connection.enabled = !iCon.connection.enabled;
            else if (visualiser.FindNetworkContainsNode(node).RelationToOrigin == iCon.connectionType)
                iCon.connection.enabled = !iCon.connection.enabled;
        }

        foreach (var oCon in node.outgoingConnections)
        {
            if (!oCon.visible)
            {
                oCon.connection.enabled = false;
                continue;
            }
            if (options.InternalNetworkView != NetworkOptions.InternalNetworkConnections.UNIQUE)
                oCon.connection.enabled = !oCon.connection.enabled;
            else if (visualiser.FindNetworkContainsNode(node).RelationToOrigin == oCon.connectionType)
                oCon.connection.enabled = !oCon.connection.enabled;
        }

        viewingState = !viewingState;
        if (!viewingState)
        {
            displayWindow.GetComponent<PopupDisplay>().CloseWindow();
            TargetNode = null;
        }
    }
}