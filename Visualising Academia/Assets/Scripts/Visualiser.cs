using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class Visualiser : LateSetup
{
    private List<List<DocumentData>> branches = new List<List<DocumentData>>();
    [SerializeField] private GameObject DocumentObj;
    [HideInInspector] public Transform spawnPoint;
    private List<GameObject> instances = new List<GameObject>();

    [SerializeField] private LineRenderer lr;
    private void Update()
    {
        if (!IsSetupComplete())
            CreateMatches();

    }
    // go through the user's query and find articles that branch off from the main article
    // I.e, Find articles as similar to the query as possible, and then branch outwards from those ones recursively

    private void CreateMatches()
    {
        foreach (var match in CompletedSearch.Query.matches)
        {
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
            
            instances.Add(newObj);

        }

        foreach (var match in instances)
        {
            foreach (var inst in instances)
            {
                var docA = match.GetComponent<Document>();
                var docB = inst.GetComponent<Document>();
                if (docA.Publisher.AttributeValue == docB.Publisher.AttributeValue)
                {
                    match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
                }

                if (AreListsRelated(docA.Authors.AttributesList, docB.Authors.AttributesList))
                {
                    match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
                }

                if (docA.DatePublished.AttributeValue == docB.DatePublished.AttributeValue)
                {
                    match.GetComponent<LinkToParent>().relatedDocuments.Add(docB);
                }
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
        if (aData.Publisher.Equals(bData.Publisher))
        {
            var originConnection = comparisonA.gameObject.AddComponent<NodeConnection>();
            originConnection.Setup(lr, OriginNode, comparisonA);
            comparisonA.incomingConnections.Add(originConnection);
        }
    }

    private void GenerateConnection(ConnectionType connectionType, DocNode origin, DocNode target)
    {
        var newCon = origin.gameObject.AddComponent<NodeConnection>();
        newCon.Setup(lr, origin, target);
        newCon.SetConnectionType(connectionType);
    }

    private void ApplyLineSettings(ref NodeConnection connection)
    {
        switch (connection.connectionType)
        {
            case ConnectionType.Authors:
                break;
            case ConnectionType.Date:
                break;
            case ConnectionType.Publisher:
                break;
            case ConnectionType.Title:
                break;
                //TODO: This.
        }
    }
}
