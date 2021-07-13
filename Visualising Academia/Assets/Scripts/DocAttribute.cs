using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocAttribute<T>
{

    // New generic type class to comprise documents, will replace the static types i've been using up until this point
    //  Makes for a more robust and expandable system that can support more searches
    public DocAttribute(string newTitle, T baseValue)
    {
        AttributeTitle = newTitle;
        AttributeValue = baseValue;
    }
    public DocAttribute(string newTitle)
    {
        AttributeTitle = newTitle;
    }
    
    /// <summary>
    /// Used to identify the category of this Attribute, i.e "Title" or "Authors"
    /// </summary>
    public string AttributeTitle { get; private set; }

    /// <summary>
    /// The main value of this class. Used for storing title or publish date, for example
    /// </summary>
    public T AttributeValue { get; private set; }

    public void SetAttributeValue(T newValue)
    {
        AttributeValue = newValue;
    }
    /// <summary>
    /// For storing a variable size list of elements, such as authors or keywords.
    /// </summary>
    public List<T> AttributesList { get; private set; } = null;

    public void AddToAttributesList(T newObj)
    {
        AttributesList ??= new List<T>();
        AttributesList.Add(newObj);
    }
    
    
}
