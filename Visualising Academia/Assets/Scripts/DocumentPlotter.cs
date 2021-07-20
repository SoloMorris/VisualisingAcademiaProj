using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class DocumentPlotter : MonoBehaviour
{
    public static DocumentPlotter Instance;
    public string inputFile; // Name of CSV file to be read in

    private List<Dictionary<string, object>> dataList; // List for holding data from CSVReader

    public List<DocumentData> articles = new List<DocumentData>(); // Objects that receive data from the CSV file
    public bool setupComplete; // Used globally in case a system needs file reading to be complete first.


    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != this) Destroy(this);
    } // Create singleton of this script
    private void Start() // Fill Articles with data to be handled elsewhere
    {
        ReadInData();
    }

    public List<DocumentData> GetArticles()
    {
        return articles;
    }
    private void ReadInData()
    { 
        dataList = CSVReader.Read(inputFile);

        //print(dataList.Count.ToString());
        
         List<string> columnList = new List<string>(dataList[1].Keys);
         DocumentData plotter = new DocumentData(); // Map out the keys of the csv
         plotter.Url.SetAttributeValue(columnList[0]);
         plotter.Title.SetAttributeValue(columnList[1]);
         plotter.IsPartOf.SetAttributeValue(columnList[2]);
         plotter.Doi.SetAttributeValue(columnList[4]);
         plotter.DocumentType.SetAttributeValue(columnList[5]);
         plotter.DatePublished.SetAttributeValue(columnList[7]);
         string authours = columnList[11];
         plotter.Publisher.SetAttributeValue(columnList[12]);
         plotter.Language.SetAttributeValue(columnList[13]);

         for (int i = 0; i < dataList.Count - 1; i++)
        {
            DocumentData newDoc = new DocumentData();
            
            newDoc.Title.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.Title.AttributeValue]));
            newDoc.Language.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.Language.AttributeValue]));
            newDoc.IsPartOf.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.IsPartOf.AttributeValue]));
            newDoc.Doi.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.Doi.AttributeValue]));
            newDoc.DocumentType.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.DocumentType.AttributeValue]));
            newDoc.DatePublished.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.DatePublished.AttributeValue]));
            newDoc.IsPartOf.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.IsPartOf.AttributeValue]));
            newDoc.Publisher.SetAttributeValue(System.Convert.ToString(dataList[i][plotter.Publisher.AttributeValue]));

            
            //  Build the list of authors through a for-loop and separate them with ";"
            var authorList = System.Convert.ToString(dataList[i][authours]);
            string stringVar = "";
            for (int j = 0; j < authorList.Length; j++)
            {
                if (authorList[j] == ';')
                {
                    newDoc.Authors.AddToAttributesList(stringVar);
                    stringVar = "";
                    j++;
                    continue;
                }
                stringVar += authorList[j];
            }
            newDoc.Authors.AddToAttributesList(stringVar);
            articles.Add(newDoc);
        }

         //Print out some docs to see if everything works!
         // for (var i = 0; i < 50; i++)
         // {
         //     var art = articles[i];
         //     print(art.Title.AttributeValue +", " +art.Publisher.AttributeValue);
         // }

        setupComplete = true;

    }
}
