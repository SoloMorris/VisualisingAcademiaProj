using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkToParent : LateSetup
{
    public List<Document> relatedDocuments = new List<Document>();
    public List<string> relationTypes = new List<string>();

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
            if (lines[i].GetPosition(0).Equals(transform.position)) continue;
            lines[i].SetPosition(0, transform.position);
            lines[i].SetPosition(1, relatedDocuments[i].transform.position);
        }
    }
    
}
