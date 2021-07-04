using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class SearchHandler : MonoBehaviour
{
   private UserSearchInterface search;
   private FilterSearch filter;
   private Visualiser visualiser;
   [SerializeField] private Transform spawnPoint;
   private void Awake()
   {
      search = GetComponentInChildren<UserSearchInterface>();
      filter = GetComponentInChildren<FilterSearch>();
      visualiser = GetComponent<Visualiser>();
      filter.enabled = false;
      visualiser.enabled = false;
      spawnPoint = search.originPoint;
   }

   private void Update()
   {
      if (search.searchComplete && !filter.isActiveAndEnabled)
         EnableFilterScript();
      if (search.searchComplete && !visualiser.isActiveAndEnabled)
         EnableVisualiserScript();
   }

   public void EnableFilterScript()
   {
      filter.enabled = true;
      print("filter enabled");

      filter.AssignUserQuery(search.CompletedSearch);
   }

   public void EnableVisualiserScript()
   {
      visualiser.enabled = true;
      print("visualiser enabled");
      visualiser.spawnPoint = spawnPoint;
      visualiser.AssignUserQuery(search.CompletedSearch);
   }
   
}
