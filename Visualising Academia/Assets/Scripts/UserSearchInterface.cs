using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor.IMGUI;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class UserSearchInterface : MonoBehaviour
{

    //  The editor input fields to read in the user's input
    [SerializeField] private TMP_InputField FTitle;
    [SerializeField] private TMP_InputField FAuthorName;
    [SerializeField] private TMP_InputField FPublisher;
    [SerializeField] private TMP_InputField FIsPartOf;
    [SerializeField] private TMP_InputField FKeyWords;
    [SerializeField] private TMP_InputField FYearPublished;

    // Used to check whether a field has been typed into
    private const string FieldText = "Type here";

    // Where the ClosestMatch will be instantiated
    public Transform originPoint;
    // The prefab for all document nodes that will be instantiated.
    [SerializeField] private GameObject DocumentObj;
    // The distance documents will be separated.
    [SerializeField] private float avgDocDistance; 

    //  Used to signal to other scripts that this one is done handling the user's query.
    public bool searchComplete = false; 
    //  The user's query with appropriate matches, is accessed by other scripts once setup is done.
    public CompleteSearch CompletedSearch { get; private set; }
    public DocNode OriginNode;
    public Document MainDocument { get; private set; }
    
    private void Update()
    {
        if (Input.GetKeyDown("return") && !searchComplete)
            CommenceSearch();
    }
    
    public void CommenceSearch()
    {
        var art = DocumentPlotter.Instance.GetArticles();
        var search = GenerateQueryDocument();
        var doc = search.QueryDoc;
        var isMatchFound = false;
        // Go through the articles. If the data is valid, and matches the user's query, add it to the matches.
        foreach (var item in art)
        {
            if (CompareAttributes(doc.Title, item.Title) && !isMatchFound)
            {
                search.SetClosestMatch(item);
                isMatchFound = true;
                continue;
            }

            if (CompareAttributes(doc.Abstract, item.Abstract))
            {
                SetDocMatch(ref isMatchFound, search, item);
                continue;
            }
             
            if (CompareAttributes(doc.Authors, item.Authors, true))
            {
                SetDocMatch(ref isMatchFound, search, item);
                continue;
            }
            if (CompareAttributes(doc.Publisher, item.Publisher))
            {
                SetDocMatch(ref isMatchFound, search, item);
                continue;
            }
            
            if (CompareAttributes(doc.IsPartOf, item.IsPartOf))
            {
                SetDocMatch(ref isMatchFound, search, item);
                continue;
            }
            
            if (CompareAttributes(doc.DatePublished, item.DatePublished))
            {
                SetDocMatch(ref isMatchFound, search, item);
            }
            
            // TODO: match id to read-in unigram in a separate function to get topics!!
        }

        //  Create a GameObject that contains the data of the user's successful query.
        GameObject primaryMatch = Instantiate(DocumentObj, originPoint);
        var comp = primaryMatch.AddComponent<DocNode>();
        comp.ApplyDocumentData(search.closestMatch);
        OriginNode = comp;
        //  Now loop back through articles, and populate the matches list with articles that have matching features.
        var workaround = true;
        
        CompletedSearch = new CompleteSearch(OriginNode.Data);
        CompletedSearch.Query = search;
        var completedSearchQuery = CompletedSearch.Query;
        var closestMatch = completedSearchQuery.closestMatch;

        foreach (var item in art) //  This bit may be redundant
        {
            // TODO: Add each category to a DIFFERENT list to distinguish them and make separation easier.
            if (item.Title == closestMatch.Title) continue;

            if (CompareAttributes(closestMatch.Publisher, item.Publisher))
            {
                completedSearchQuery.AddMatch(item);
                completedSearchQuery.matchName.Add(closestMatch.Publisher.AttributeTitle);
                continue;
            }
            
            if (CompareAttributes(closestMatch.Authors, item.Authors, true))
            {
                completedSearchQuery.AddMatch(item);
                completedSearchQuery.matchName.Add(closestMatch.Authors.AttributeTitle);
                continue;
            }
            if (CompareAttributes(closestMatch.DatePublished, item.DatePublished))
            {
                completedSearchQuery.AddMatch(item);
                completedSearchQuery.matchName.Add(closestMatch.DatePublished.AttributeTitle);
            }
           if (completedSearchQuery.matches.Count != completedSearchQuery.matchName.Count)
               Debug.LogError("Error! Matches for OriginDocument don't add up!");
        }
       
        searchComplete = true; // Flag to other scripts to use my completed searchQuery.
        Camera.main.GetComponent<FreeFlyCamera>()._active = true;
    }

    private static void SetDocMatch(ref bool isMatchFound, SearchResult search, DocumentData item)
    {
        // if (isMatchFound)
        // {
        //     search.AddMatch(item);
        //     return;
        // }
        search.SetClosestMatch(item);
        isMatchFound = true;
    }

    /// <summary>
    /// Checks if the query is null, then if it is the same as source.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private bool CompareAttributes<T>(DocAttribute<T> query, DocAttribute<T> source)
    {
        return query.AttributeValue != null && source.AttributeValue != null && query.AttributeValue.Equals(source.AttributeValue);
    }
    private bool CompareAttributes<T>(DocAttribute<T> query, DocAttribute<T> source, bool isList = false) // TODO: This is just wrong
    {
        return query.AttributesList != null && source.AttributesList != null && query.AttributesList.Equals(source.AttributesList);
    }
    
    /// <summary>
    /// Creates a SearchResult by reading whichever fields the user filled in.
    /// </summary>
    /// <returns></returns>
    private SearchResult GenerateQueryDocument()
    {
        var query = new SearchResult();
        ReadValidInputFields(ref query.QueryDoc);
        if (query == new SearchResult()) Debug.LogError("Error generating query document - \n no results applied!");
        return query;
    }
    
    
    private void ReadValidInputFields(ref DocumentData data)
    {
        if (!String.Equals(FTitle.text, FieldText)) data.Title.SetAttributeValue(FTitle.text);
        if (!String.Equals(FAuthorName.text, FieldText)) ParseTextIntoList(FAuthorName.text, data.Authors);
        //if (!String.Equals(FPublisher.text, FieldText)) data.Publisher.SetAttributeValue(FPublisher.text);
        //if (!String.Equals(FIsPartOf.text, FieldText)) data.IsPartOf.SetAttributeValue(FIsPartOf.text);
        if (!String.Equals(FYearPublished.text, FieldText)) data.DatePublished.SetAttributeValue(FYearPublished.text);
    }
    
    public void ParseTextIntoList(string input, DocAttribute<string> output)
    {
        string term = "";
        
        foreach (var t in input)
        {
            if (t == ',')
            {
                output.AddToAttributesList(term);
                term = "";
                continue;
            }
            term += t;
        }
        output.AddToAttributesList(term);
    }
    
}

public class SearchResult
{
    public SearchResult()
    {
        QueryDoc = new DocumentData();
        closestMatch = new DocumentData();
        matches = new List<DocumentData>();
        matchName = new List<string>();

    }

    public DocumentData QueryDoc;
    public DocumentData closestMatch { get; private set; }

    public void SetClosestMatch(DocumentData match)
    {
        closestMatch = match;
    }
    public List<DocumentData> matches { get; private set; }
    public List<string> matchName; // 

    public void AddMatch(DocumentData match)
    {
        matches.Add(match);
    }
    
}

public class CompleteSearch
{
    public CompleteSearch(Document originDocument)
    {
        Query = new SearchResult();
        MatchTypes = new List<string>();
        OriginDocument = originDocument;
    }

    public SearchResult Query;
    public Document OriginDocument;
    public List<string> MatchTypes;
}