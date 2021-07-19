using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateSetup : MonoBehaviour
{
    protected CompleteSearch CompletedSearch;
    protected DocNode OriginNode;
    public bool setupComplete { get; protected set; }

    protected List<DocumentData> articles;

    private void Start()
    {
        articles = DocumentPlotter.Instance.GetArticles();
    }
    public void AssignCompleteSearch(CompleteSearch newS, DocNode newN)
    {
        CompletedSearch = newS;
        OriginNode = newN;
    }

    protected bool IsSetupComplete()
    {
        return setupComplete;
    }
    
}
