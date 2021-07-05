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

        // print(dataList);
        //
         List<string> columnList = new List<string>(dataList[1].Keys);
         DocumentData plotter = new DocumentData(); // Map out the keys of the csv
         plotter.id = columnList[0];
         plotter.title = columnList[1];
         plotter.isPartOf = columnList[2];
         plotter.doi = columnList[4];
         plotter.documentType = columnList[5];
         plotter.datePublished = columnList[7];
         string authours = columnList[11];
         plotter.publisher = columnList[12];
         plotter.language = columnList[13];

         for (int i = 0; i < dataList.Count - 1; i++)
        {
            DocumentData newDoc = new DocumentData();
            
            print(dataList.Count);
            newDoc.title = System.Convert.ToString(dataList[i][plotter.title]);
            newDoc.language = System.Convert.ToString(dataList[i][plotter.language]);
            newDoc.isPartOf = System.Convert.ToString(dataList[i][plotter.isPartOf]);
            newDoc.doi = System.Convert.ToString(dataList[i][plotter.doi]);
            newDoc.documentType = System.Convert.ToString(dataList[i][plotter.documentType]);
            newDoc.datePublished = System.Convert.ToString(dataList[i][plotter.datePublished]);
            newDoc.isPartOf = System.Convert.ToString(dataList[i][plotter.isPartOf]);
            newDoc.publisher = System.Convert.ToString(dataList[i][plotter.publisher]);

            
            //  Build the list of authors through a for-loop and separate them with ";"
            var authorList = System.Convert.ToString(dataList[i][authours]);
            string stringVar = "";
            for (int j = 0; j < authorList.Length; j++)
            {
                if (authorList[j] == ';')
                {
                    newDoc.Authors.Add(stringVar);
                    stringVar = "";
                    j++;
                    continue;
                }
                stringVar += authorList[j];
            }
            newDoc.Authors.Add(stringVar);
            articles.Add(newDoc);
        }

         //Print out some docs to see if everything works!
        // for (var i = 0; i < 5; i++)
        // {
        //     var art = articles[i];
        //     print(art.title +", " +art.publisher);
        // }

        setupComplete = true;

    }
}
