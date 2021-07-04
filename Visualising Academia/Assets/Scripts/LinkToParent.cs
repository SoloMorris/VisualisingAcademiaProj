using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToParent : LateSetup
{
    public List<Document> relatedDocuments = new List<Document>();

    public LineRenderer lr;
    [SerializeField] private List<LineRenderer> lines = new List<LineRenderer>();
    void Start()
    {
        foreach (var doc in relatedDocuments)
        {
            var newLine = Instantiate(lr);
            lines.Add(newLine);
        }
    }

    void Update()
    {
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].SetPosition(0, transform.position);
            lines[i].SetPosition(1, relatedDocuments[i].transform.position);
        }
        
        
    }
    
}
