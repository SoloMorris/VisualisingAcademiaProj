using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToParent : LateSetup
{
    public List<Document> relatedDocuments = new List<Document>();

    private LineRenderer lr;
    private List<LineRenderer> lines = new List<LineRenderer>();

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        foreach (var doc in relatedDocuments)
        {
            var newLine = Instantiate(lr);
            newLine.positionCount = 2;
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
