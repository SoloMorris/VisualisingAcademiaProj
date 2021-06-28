using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchHandler : MonoBehaviour
{
   private UserSearchInterface search;


   private void Awake()
   {
      search = GetComponentInChildren<UserSearchInterface>();
   }
}
