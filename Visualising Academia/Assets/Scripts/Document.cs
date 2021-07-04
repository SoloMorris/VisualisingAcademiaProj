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
    public string title = "nulltitle";
    public string Abstract;
    public List<string> Authors = new List<string>();
    public string datePublished;
    public string documentType;
    public string doi;
    public string isPartOf;
    public string publisher;
    public List<string> topics = new List<string>();
    public string language;
    public string id;

    public void ApplyNewValues(DocumentData newDoc)
    {
        title = newDoc.title;
        Abstract = newDoc.Abstract;
        Authors = newDoc.Authors;
        datePublished = newDoc.datePublished;
        documentType = newDoc.documentType;
        doi = newDoc.doi;
        isPartOf = newDoc.isPartOf;
        publisher = newDoc.publisher;
        topics = newDoc.topics;
        language = newDoc.language;
        id = newDoc.id;
    }
    
}
public class DocumentData
{
    public string title = "nulltitle";
    public string Abstract;
    public List<string> Authors = new List<string>();
    public string datePublished;
    public string documentType;
    public string doi;
    public string isPartOf;
    public string publisher;
    public List<string> topics = new List<string>();
    public string language;
    public string id;

    public void ApplyNewValues(DocumentData newDoc)
    {
        title = newDoc.title;
        Abstract = newDoc.Abstract;
        Authors = newDoc.Authors;
        datePublished = newDoc.datePublished;
        documentType = newDoc.documentType;
        doi = newDoc.doi;
        isPartOf = newDoc.isPartOf;
        publisher = newDoc.publisher;
        topics = newDoc.topics;
        language = newDoc.language;
        id = newDoc.id;
    }

}
