using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class Visualiser : LateSetup
{

    [SerializeField] private GameObject DocumentObj;
    [HideInInspector] public Transform spawnPoint;
    //private List<DocNode> instances = new List<DocNode>(); // Is being transitioned over to nodeNetworks

    [SerializeField] private LineRenderer lr;
    
    // Networks to contain nodes
    private List<Network> nodeNetworks = new List<Network>(); // New way to store nodes.
    [SerializeField] private GameObject NetworkPrefab;

    //  Relation Colours
    Color PublisherColour = Color.black;
    Color AuthorColour = Color.cyan;
    Color DateColour = Color.blue;
    
    // Optimisation
    [SerializeField] private float updateTimer;
    private float tick = 0f;
    
    public Dropdown Pubdropdown;
    public Dropdown Authdropdown;
    public Dropdown Datedropdown;
    
        /* //---------------------------------------------------------------------------------------------------------//
        Split all nodes into groups called networks                                                                   |
        Each network instantiates separately based on the type of relation to the Origin Node.                        |
        This way, relations to the origin are kept separate, and would allow for implementation of a feature for      |
        the user to change the Origin Node and generate new networks                                                  |
        Potentially only visualise connections if they differ from the type the node already shares with the origin   |
        */ //--------------------------------------------------------------------------------------------------------| 

    public void SelectColor()
    {
        Debug.Log("Working");
        switch (Pubdropdown.value)
        {
            case 1:
                PublisherColour = Color.red;
                break;
            case 2:
                PublisherColour = Color.blue;
                break;
            case 3:
                PublisherColour = Color.green;
                break;
        }
        switch (Authdropdown.value)
        {
            case 1:
                AuthorColour = Color.red;
                break;
            case 2:
                AuthorColour = Color.blue;
                break;
            case 3:
                AuthorColour = Color.green;
                break;
        }
        switch (Datedropdown.value)
        {
            case 1:
                DateColour = Color.red;
                break;
            case 2:
                DateColour = Color.blue;
                break;
            case 3:
                DateColour = Color.green;
                break;
        }
    }

    private void Update()
    {
        if (!IsSetupComplete())
            CreateMatches();
        else
        {
            tick++;
            if (tick < updateTimer) return;

            var networkChoice = Random.Range(0, nodeNetworks.Count);
            foreach (DocNode node in nodeNetworks[networkChoice].Nodes)
                {
                    foreach (var con in node.incomingConnections)
                    {
                        con.UpdatePositions();
                    }
                    foreach (var con in node.outgoingConnections)
                    {
                        con.UpdatePositions();
                    }
                }
            tick = 0f;
        }

    }
    // go through the user's query and find articles that branch off from the main article
    // I.e, Find articles as similar to the query as possible, and then branch outwards from those ones recursively

    private void CreateMatches()
    {
        foreach (var match in DocumentPlotter.Instance.articles)
        {
            if (!IsRelated(match, OriginNode.Data.docData)) continue;

            //  Instantiate the object and attach the document data to it
            var newObj = Instantiate(DocumentObj, Vector3.zero, transform.rotation);
            var comp = newObj.AddComponent<DocNode>();
            comp.ApplyDocumentData(match);

            //  Finds the relation between the new node and the Origin.
            CompareNodes(comp, OriginNode);
            TryAddToNodeNetwork();
            void TryAddToNodeNetwork()
            {
                foreach (var network in nodeNetworks)
                {
                    if (network.RelationToOrigin == comp.incomingConnections[0].connectionType)
                    {
                        foreach (var nw in nodeNetworks)
                        {
                            if (nw.Nodes.Contains(newObj.GetComponent<DocNode>()) && nw != network) return;
                        }
                        network.Nodes.Add(newObj.GetComponent<DocNode>());
                        return;
                    }
                }
                var newNetwork = Instantiate(NetworkPrefab, Random.insideUnitSphere * 40, transform.rotation);
                newNetwork.GetComponent<Network>().SetRelationToOrigin(newObj.GetComponent<DocNode>().incomingConnections[0].connectionType);
                nodeNetworks.Add(newNetwork.GetComponent<Network>());
                nodeNetworks[nodeNetworks.Count-1].Nodes.Add(newObj.GetComponent<DocNode>());
            }

        }

        // Go through the completed networks and find relations between everything else
        for (int i = 0; i < nodeNetworks.Count; i++)
        {
            foreach (DocNode node in nodeNetworks[i].Nodes)
            {
                for (int j = 0; j < nodeNetworks.Count; j++)
                {
                    foreach (DocNode otherNode in nodeNetworks[j].Nodes)
                    {
                        CompareNodes(node, otherNode);
                    }
                }
                node.transform.parent = nodeNetworks[i].transform;
            }
            nodeNetworks[i].RandomiseNodeLocations();
        }
        setupComplete = true;
    }

    private bool AreListsRelated<T>(List<T> a, List<T> b)
    {
        foreach (var entryA in a)
        {
            if (b.Contains(entryA)) return true;
        }

        return false;
    }

    private void CompareNodes(DocNode comparisonA, DocNode comparisonB)
    {
        // If these two nodes are the same, exit
        if (comparisonA.Equals(comparisonB)) return;

        // If these two nodes already have a connection, then exit
        if (comparisonA.GetComponent<NodeConnection>() &&
            comparisonA.GetComponent<NodeConnection>().origin == comparisonB
            || comparisonA.GetComponent<NodeConnection>() &&
            comparisonA.GetComponent<NodeConnection>().target == comparisonB) return;
        
        var aData = comparisonA.Data;
        var bData = comparisonB.Data;
        
        if (aData.Publisher.AttributeValue != null && aData.Publisher.AttributeValue == bData.Publisher.AttributeValue)
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Publisher);
        else if (aData.Authors.AttributesList[0] != null && AreListsRelated(aData.Authors.AttributesList, bData.Authors.AttributesList))
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Authors);
        else if (aData.DatePublished.AttributeValue != null && aData.DatePublished.AttributeValue == bData.DatePublished.AttributeValue)
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Date);
    }

    private void CreateNodeConnection(DocNode comparisonA, DocNode comparisonB, ConnectionType desiredConType)
    {
        var originConnection = comparisonA.gameObject.AddComponent<NodeConnection>();
        originConnection.Setup(lr, comparisonB, comparisonA);
        comparisonA.incomingConnections.Add(originConnection);
        comparisonB.outgoingConnections.Add(originConnection);
        originConnection.SetConnectionType(desiredConType);
        ApplyLineSettings(ref originConnection);
        originConnection.connection.enabled = false;
        
    }

    private bool IsRelated(DocumentData source, DocumentData comparison)
    {
        //  Return if the Title, Publisher or Date are similar.
        if (source.Title.AttributeValue != null &&
            source.Title.AttributeValue == comparison.Title.AttributeValue) return true;
        if (source.Publisher.AttributeValue != null &&
            source.Publisher.AttributeValue == comparison.Publisher.AttributeValue) return true;
        if (AreListsRelated(source.Authors.AttributesList, comparison.Authors.AttributesList)) return true;
        if (source.DatePublished.AttributeValue != null &&
            source.DatePublished.AttributeValue == comparison.DatePublished.AttributeValue) return true;
        return false;
    }

    private void ApplyLineSettings(ref NodeConnection connection)
    {
        //Debug.Log("working1");
        switch (connection.connectionType)
        {
            case ConnectionType.Authors:
                connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                connection.connection.startColor = AuthorColour;
                connection.connection.endColor = AuthorColour;
                connection.Create();
                break;
            case ConnectionType.Date:
                connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                connection.connection.startColor = DateColour;
                connection.connection.endColor = DateColour;
                connection.Create();
                break;
            case ConnectionType.Publisher:
                connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                connection.connection.startColor = PublisherColour;
                connection.connection.endColor = PublisherColour;
                connection.Create();
                break;
            case ConnectionType.Title:
                break;
            
        }
    }
}
