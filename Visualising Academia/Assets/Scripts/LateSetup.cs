using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateSetup : MonoBehaviour
{
    protected CompleteSearch CompletedSearch;
    public bool setupComplete { get; protected set; }

    protected List<DocumentData> articles;

    private void Start()
    {
        articles = DocumentPlotter.Instance.GetArticles();
    }
    public void AssignCompleteSearch(CompleteSearch newS)
    {
        CompletedSearch = newS;
    }

    protected bool IsSetupComplete()
    {
        return setupComplete;
    }
    
}
