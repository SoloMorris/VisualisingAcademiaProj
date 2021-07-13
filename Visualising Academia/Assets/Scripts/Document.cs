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
    public DocAttribute<string> Abstract = new DocAttribute<string>("Abstract");
    public DocAttribute<string> Authors = new DocAttribute<string>("Authors");
    public DocAttribute<string> DatePublished = new DocAttribute<string>("Date Published");
    public DocAttribute<string> DocumentType = new DocAttribute<string>("Document Type");
    public DocAttribute<string> Doi = new DocAttribute<string>("DOI");
    public DocAttribute<string> IsPartOf = new DocAttribute<string>("Is Part Of");
    public DocAttribute<string> Publisher = new DocAttribute<string>("Publisher");
    public DocAttribute<string> Topics = new DocAttribute<string>("Topics");
    public DocAttribute<string> Language = new DocAttribute<string>("Language");
    public DocAttribute<string> Url = new DocAttribute<string>("Web URL");

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
    }
    
}

public class DocumentData
{
    public DocAttribute<string> Title = new DocAttribute<string>("Title", "NullTitle");
    public DocAttribute<string> Abstract = new DocAttribute<string>("Abstract");
    public DocAttribute<string> Authors = new DocAttribute<string>("Authors");
    public DocAttribute<string> DatePublished = new DocAttribute<string>("Date Published");
    public DocAttribute<string> DocumentType = new DocAttribute<string>("Document Type");
    public DocAttribute<string> Doi = new DocAttribute<string>("DOI");
    public DocAttribute<string> IsPartOf = new DocAttribute<string>("Is Part Of");
    public DocAttribute<string> Publisher = new DocAttribute<string>("Publisher");
    public DocAttribute<string> Topics = new DocAttribute<string>("Topics");
    public DocAttribute<string> Language = new DocAttribute<string>("Language");
    public DocAttribute<string> Url = new DocAttribute<string>("Web URL");
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
