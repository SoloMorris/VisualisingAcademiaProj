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

// public class DocumentData
// {
//     public string title = "nulltitle";
//     public string Abstract;
//     public List<string> Authors = new List<string>();
//     public string datePublished;
//     public string documentType;
//     public string doi;
//     public string isPartOf;
//     public string publisher;
//     public List<string> topics = new List<string>();
//     public string language;
//     public string id;
//
//     public void ApplyNewValues(DocumentData newDoc)
//     {
//         title = newDoc.title;
//         Abstract = newDoc.Abstract;
//         Authors = newDoc.Authors;
//         datePublished = newDoc.datePublished;
//         documentType = newDoc.documentType;
//         doi = newDoc.doi;
//         isPartOf = newDoc.isPartOf;
//         publisher = newDoc.publisher;
//         topics = newDoc.topics;
//         language = newDoc.language;
//         id = newDoc.id;
//     }
//
// }
