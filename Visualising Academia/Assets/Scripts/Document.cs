using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DocumentType
{
    Article,
    Chapter,
    Book,
    Other
}

public struct Topic
{
    public string keyword;
    public int usages;
}

[Serializable]
public class Document  : MonoBehaviour
{
    public DocAttribute<string> Title = new DocAttribute<string>("Title", "NullTitle");
    public DocAttribute<string> Abstract = new DocAttribute<string>("Abstract", null);
    public DocAttribute<string> Authors = new DocAttribute<string>("Authors", null);
    public DocAttribute<string> DatePublished = new DocAttribute<string>("Date Published", null);
    public DocAttribute<string> DocumentType = new DocAttribute<string>("Document Type", null);
    public DocAttribute<string> Doi = new DocAttribute<string>("DOI", null);
    public DocAttribute<string> IsPartOf = new DocAttribute<string>("Is Part Of", null);
    public DocAttribute<string> Publisher = new DocAttribute<string>("Publisher", null);
    public DocAttribute<string> Topics = new DocAttribute<string>("Topics", null);
    public DocAttribute<string> Language = new DocAttribute<string>("Language", null);
    public DocAttribute<string> Url = new DocAttribute<string>("Web URL", null); 
    
    public DocumentData docData { get; private set; } //TODO: Just do this instead

    public void ApplyNewValues(DocumentData newDoc)
    {
        Title = newDoc.Title;
        Abstract = newDoc.Abstract;
        Authors = newDoc.Authors;
        DatePublished = newDoc.DatePublished;
        DocumentType = newDoc.DocumentType;
        Doi = newDoc.Doi;
        IsPartOf = newDoc.IsPartOf;
        Publisher = newDoc.Publisher;
        Topics = newDoc.Topics;
        Language = newDoc.Language;
        Url = newDoc.Url;
        docData = newDoc;
    }
    
}

public class DocumentData
{
    public DocAttribute<string> Title = new DocAttribute<string>("Title", "NullTitle");
    public DocAttribute<string> Abstract = new DocAttribute<string>("Abstract", null);
    public DocAttribute<string> Authors = new DocAttribute<string>("Authors", null);
    public DocAttribute<string> DatePublished = new DocAttribute<string>("Date Published", null);
    public DocAttribute<string> DocumentType = new DocAttribute<string>("Document Type", null);
    public DocAttribute<string> Doi = new DocAttribute<string>("DOI", null);
    public DocAttribute<string> IsPartOf = new DocAttribute<string>("Is Part Of", null);
    public DocAttribute<string> Publisher = new DocAttribute<string>("Publisher", null);
    public DocAttribute<string> Topics = new DocAttribute<string>("Topics", null);
    public DocAttribute<string> Language = new DocAttribute<string>("Language", null);
    public DocAttribute<string> Url = new DocAttribute<string>("Web URL", null);

    public string matchType = "";
}

public class DocNode : MonoBehaviour
{
    /// <summary>
    /// The data held by the node such as title, authors etc.
    /// </summary>
    private Document data;
    
    /// <summary>
    /// The Node that spawned this node. leave null if this is the original search node.
    /// </summary>
    private DocNode parent;

    /// <summary>
    /// Any nodes spawned by this node.
    /// </summary>
    private List<DocNode> children = new List<DocNode>();

    /// <summary>
    /// Any Connections that have this Node set as target. Will always have parent at 0.
    /// </summary>
    public List<NodeConnection> incomingConnections = new List<NodeConnection>();


    /// <summary>
    /// Connections going to children or other related nodes.
    /// </summary>
    public List<NodeConnection> outgoingConnections = new List<NodeConnection>();
    

    public void ApplyDocumentData(DocumentData nData)
    {
        if (nData == null)
        {
            Debug.LogError("Data not valid");
            return;
        }
        Data = gameObject.AddComponent<Document>();
        data.ApplyNewValues(nData);
    }

    /// <summary>
    /// Any nodes spawned by this node.
    /// </summary>
    public List<DocNode> Children
    {
        get => children;
        set => children = value;
    }

    /// <summary>
    /// The Node that spawned this node. leave null if this is the original search node.
    /// </summary>
    public DocNode Parent
    {
        get => parent;
        set => parent = value;
    }

    /// <summary>
    /// The data held by the node such as title, authors etc.
    /// </summary>
    public Document Data
    {
        get => data;
        set { data = value; }
    }
}

public enum ConnectionType
{
    Title,
    Authors,
    Publisher,
    Date
}
public class NodeConnection : MonoBehaviour
{
    public void Setup(LineRenderer lr, DocNode nOrigin, DocNode nTarget)
    {
        connection = lr;
        origin = nOrigin;
        target = nTarget;
    }

    public ConnectionType connectionType;
    public void SetConnectionType(ConnectionType desiredType)
    {
        connectionType = desiredType;
    }

    public void Create()
    {
        connection = Instantiate(connection, transform);
        connection.SetPosition(0, origin.gameObject.transform.position);
        connection.SetPosition(1, target.gameObject.transform.position);
    }

    public void UpdatePositions()
    {
        connection.SetPosition(0, origin.transform.position);
        connection.SetPosition(1, target.transform.position);
    }
    public bool IsConnectionOfType(ConnectionType desiredType)
    {
        return connectionType == desiredType;
    }

    public LineRenderer connection;
    public DocNode origin;
    public DocNode target;
    public bool visible = true;
}