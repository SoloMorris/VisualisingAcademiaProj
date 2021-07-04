using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateSetup : MonoBehaviour
{
    protected SearchResult userQuery { get; private set; }
    protected Document MainDocument { get; private set; }
    public bool setupComplete { get; protected set; }

    protected List<DocumentData> articles;

    private void Start()
    {
        articles = DocumentPlotter.Instance.GetArticles();
    }
    public void AssignUserQuery(SearchResult newQuery, Document newDocument)
    {
        userQuery = newQuery;
        MainDocument = newDocument;
    }

    protected bool IsSetupComplete()
    {
        return setupComplete;
    }
    
}
