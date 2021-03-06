using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeNetwork;
using Cursor = UnityEngine.Cursor;

public class SearchHandler : MonoBehaviour
{
   private UserSearchInterface search;
   private FilterSearch filter;
   private Visualiser visualiser;
   [SerializeField] private Transform spawnPoint;
   [SerializeField] private GameObject searchMenuCanvas;
   [SerializeField] private GameObject background;
   private void Awake()
   {
      search = GetComponentInChildren<UserSearchInterface>();
      visualiser = GetComponent<Visualiser>();
      visualiser.enabled = false;
      spawnPoint = search.originPoint;
      Camera.main.GetComponent<FreeFlyCamera>()._active = false;

   }

   private void Update()
   {
      if (search.searchComplete)
      {
         searchMenuCanvas.SetActive(false);
      }

      if (search.searchComplete && !visualiser.isActiveAndEnabled)
      {
         EnableVisualiserScript();
         Cursor.lockState = CursorLockMode.Locked;
         background.SetActive(false);
      }
   }

   public void EnableFilterScript()
   {
      filter.enabled = true;
      print("filter enabled");

      filter.AssignCompleteSearch(search.CompletedSearch, search.OriginNode);
   }
   public void EnableVisualiserScript()
   {
      visualiser.enabled = true;
      print("visualiser enabled");
      visualiser.spawnPoint = spawnPoint;
      visualiser.AssignCompleteSearch(search.CompletedSearch, search.OriginNode);
   }
   
}
