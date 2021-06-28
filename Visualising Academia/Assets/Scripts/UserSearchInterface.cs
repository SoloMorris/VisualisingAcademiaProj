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

    [SerializeField] private TMP_InputField FTitle;
    [SerializeField] private TMP_InputField FAuthorName;
    [SerializeField] private TMP_InputField FJournalTitle;
    [SerializeField] private TMP_InputField FKeyWords;
    [SerializeField] private TMP_InputField FYearPublished;

    private const string FieldText = "Type here";

    [SerializeField] private Transform originPoint;
    [SerializeField] private GameObject DocumentObj;
    [SerializeField] private float avgDocDistance; // The distance documents will be separated


    private Document hit;
    public bool searchComplete = false;

    private void Start()
    {
    }
    private void Update()
    {
        if (Input.GetKeyDown("return") && !searchComplete)
            CommenceSearch();
        //TODO: Compare the query document against the dataset and return exact and matching results.
    }

    private void OnGUI()
    {
    }

    public void CommenceSearch()
    {
        var art = DocumentPlotter.Instance.GetArticles();
        var search = GenerateQueryDocument();
        var doc = search.QueryDoc;
        foreach (var item in art)
        {
            if (item.title != null && doc.title == item.title) 
                TrySetClosestMatch(search, item);
            if (item.language != null && doc.language == item.language) TrySetClosestMatch(search, item);
            if (item.publisher != null && doc.publisher == item.publisher) TrySetClosestMatch(search, item);
            if (item.datePublished != null && doc.datePublished == item.datePublished) TrySetClosestMatch(search, item); // Not best option for matching, try another method
            if (item.doi != null && doc.doi == item.doi) TrySetClosestMatch(search, item);
            if (item.Authors != null && doc.Authors == item.Authors) TrySetClosestMatch(search, item);
            if (item.topics != null && doc.topics == item.topics) TrySetClosestMatch(search, item);
            // TODO: match id to read-in unigram in a separate function to get topics!!
        }
        // TODO: Now instantiate the objects in space and make them be separate!
        GameObject primaryMatch = Instantiate(DocumentObj, originPoint);

        var comp = primaryMatch.AddComponent<Document>();
        comp.ApplyNewValues(search.closestMatch);

    }

    private SearchResult GenerateQueryDocument()
    {
        var query = new SearchResult();
        if (CheckFieldInput(FTitle))
        {
            GenerateSearchResults(FTitle.text, ref query.QueryDoc.title );
        }
        if (CheckFieldInput(FAuthorName))
        {
            GenerateSearchResults(FAuthorName.text, ref query.QueryDoc.Authors );
        }
        if (CheckFieldInput(FJournalTitle))
        {
            GenerateSearchResults(FJournalTitle.text, ref query.QueryDoc.publisher );
        }
        if (CheckFieldInput(FKeyWords))
        {
            GenerateSearchResults(FKeyWords.text, ref query.QueryDoc.topics );
        }
        if (CheckFieldInput(FYearPublished))
        {
            GenerateSearchResults(FYearPublished.text, ref query.QueryDoc.datePublished );
        }
        if (query == new SearchResult()) Debug.LogError("Error generating query document - \n no results applied!");
        return query;
    }
    private void TrySetClosestMatch(SearchResult sr, DocumentData doc)
    {
        var test = new DocumentData();
        if (sr.closestMatch.title == "nulltitle")
            sr.SetClosestMatch(doc);
        else
            sr.AddMatch(doc);

            foreach (var VARIABLE in sr.matches)
        {
            print(VARIABLE);
        }
    }
    private bool CheckFieldInput(TMP_InputField input)
    {
        return !String.Equals(input.text, FieldText);
    }

    /// <summary>
    /// Take a search query and apply it to a new location -- i.e. the QueryDoc in a SearchResult.
    /// </summary>
    /// <param name="input">The search query</param>
    /// <param name="location">Where to apply the query</param>
    public void GenerateSearchResults(string input, ref string location)
    {
        location = input;
    }  
    public void GenerateSearchResults(string input, ref List<string> location)
    {
        string term = "";
        
        foreach (var t in input)
        {
            if (t == ',')
            {
                location.Add(term);
                term = "";
                continue;
            }
            term += t;
        }
        location.Add(term);
    }
    
}

public class SearchResult
{
    public SearchResult()
    {
        QueryDoc = new DocumentData();
        closestMatch = new DocumentData();
        matches = new List<DocumentData>();
    }

    public DocumentData QueryDoc;
    public DocumentData closestMatch { get; private set; }

    public void SetClosestMatch(DocumentData match)
    {
        closestMatch = match;
    }
    public List<DocumentData> matches { get; private set; }

    public void AddMatch(DocumentData match)
    {
        matches.Add(match);
    }
    
}
