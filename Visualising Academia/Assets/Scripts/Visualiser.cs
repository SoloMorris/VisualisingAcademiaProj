using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class Visualiser : LateSetup
{
    private List<List<DocumentData>> branches = new List<List<DocumentData>>();
    [SerializeField] private GameObject DocumentObj;
    [HideInInspector] public Transform spawnPoint;
    private List<DocNode> instances = new List<DocNode>();

    [SerializeField] private LineRenderer lr;

    Color PublisherColour = Color.black;
    Color AuthorColour = Color.cyan;
    Color DateColour = Color.blue;
    public Dropdown Pubdropdown;
    public Dropdown Authdropdown;
    public Dropdown Datedropdown;

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
            foreach (var inst in instances)
            {
                foreach (var con in inst.incomingConnections)
                {
                    con.UpdatePositions();
                }
            }
        }

    }
    // go through the user's query and find articles that branch off from the main article
    // I.e, Find articles as similar to the query as possible, and then branch outwards from those ones recursively

    private void CreateMatches()
    {
        foreach (var match in DocumentPlotter.Instance.articles)
        {
            if (!IsRelated(match, OriginNode.Data.docData)) continue;
            
            // Generate random offsets in space for each object
            var offsetA = Random.Range(-5f, 5f);
            var offsetB = Random.Range(-5f, 5f);
            var offsetC = Random.Range(-5f, 5f);
            var spawnOffset = new Vector3(spawnPoint.position.x + offsetA, spawnPoint.position.y + offsetB,
                spawnPoint.position.z + offsetC);
            
            //  Instantiate the object and attach the document data to it
            var newObj = Instantiate(DocumentObj, spawnOffset, spawnPoint.rotation);
            var comp = newObj.AddComponent<DocNode>();
            comp.ApplyDocumentData(match);
            //link.relatedDocuments.Add(CompletedSearch.OriginDocument);
            
            // Connect the new Node to the Origin
            
            
            
            //  Finds this document inside the search query's list of relations, so that it can be displayed
           // var index = CompletedSearch.Query.matches.FindIndex(0, CompletedSearch.Query.matches.Count,
           //     data => data.Title == comp.docData.Title);
           // link.relationTypes.Add(CompletedSearch.Query.matchName[index]);
            
           CompareNodes(comp, OriginNode);
            instances.Add(newObj.GetComponent<DocNode>());

        }

        foreach (var match in instances)
        {
            // foreach (var inst in instances)
            // {
            //     var docA = match.GetComponent<Document>();
            //     var docB = inst.GetComponent<Document>();
            //     if (docA.Publisher.AttributeValue == docB.Publisher.AttributeValue)
            //     {
            //         match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
            //     }
            //
            //     if (AreListsRelated(docA.Authors.AttributesList, docB.Authors.AttributesList))
            //     {
            //         match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
            //     }
            //
            //     if (docA.DatePublished.AttributeValue == docB.DatePublished.AttributeValue)
            //     {
            //         match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
            //     }
            // }
            foreach (var inst in instances)
            {
                CompareNodes(match, inst);
            }
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
        var aData = comparisonA.Data;
        var bData = comparisonB.Data;
        
        if (comparisonA.GetComponent<NodeConnection>() &&
            comparisonA.GetComponent<NodeConnection>().origin == comparisonB
            || comparisonA.GetComponent<NodeConnection>() &&
            comparisonA.GetComponent<NodeConnection>().target == comparisonB) return;
        
        if (aData.Publisher.AttributeValue != null && aData.Publisher.AttributeValue == bData.Publisher.AttributeValue)
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Publisher);
        else if (aData.Authors.AttributesList[0] != null && AreListsRelated(aData.Authors.AttributesList, bData.Authors.AttributesList))
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Authors);
        else if (aData.DatePublished.AttributeValue != null && aData.DatePublished.AttributeValue == bData.DatePublished.AttributeValue)
            CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Authors);
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
            
                //TODO: This.
        }
    }
}
