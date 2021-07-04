using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualiser : LateSetup
{
    private List<List<DocumentData>> branches = new List<List<DocumentData>>();
    [SerializeField] private GameObject DocumentObj;
    [HideInInspector] public Transform spawnPoint;
    private void Update()
    {
        if (!IsSetupComplete())
            CreateMatches();

    }
    // go through the user's query and find articles that branch off from the main article
    // I.e, Find articles as similar to the query as possible, and then branch outwards from those ones recursively

    private void CreateMatches()
    {
        foreach (var match in userQuery.matches)
        {
            var offsetA = Random.Range(-5f, 5f);
            var offsetB = Random.Range(-5f, 5f);
            var offsetC = Random.Range(-5f, 5f);
            var spawnOffset = new Vector3(spawnPoint.position.x + offsetA, spawnPoint.position.y + offsetB,
                spawnPoint.position.z + offsetC);
            var newObj = Instantiate(DocumentObj, spawnOffset, spawnPoint.rotation);
            var comp = newObj.AddComponent<Document>();
            comp.ApplyNewValues(match);
            //TODO:: Add a UI popup to show the info of each node INSIDE the game!
        }

        setupComplete = true;
    }
}
