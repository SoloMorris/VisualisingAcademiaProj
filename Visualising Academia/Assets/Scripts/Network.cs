using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Comprised of a group of nodes that all share the same type of relation to the Origin node.
/// The scene is comprised of multiple networks created in separate locations.
/// </summary>
public class Network : MonoBehaviour
{
    /// <summary>
    /// The list of nodes contained by the network.
    /// </summary>
    public List<DocNode> Nodes = new List<DocNode>();

    /// <summary>
    /// The relation this network has to the Origin node. This should only be set ONCE.
    /// </summary>
    public ConnectionType RelationToOrigin { get; private set; }
    
    /// <summary>
    /// Used to set RelationToOrigin in other scripts.
    /// </summary>
    /// <param name="newType">desired relation enum.</param>
    public void SetRelationToOrigin(ConnectionType newType) => RelationToOrigin = newType;

    public SphereCollider SpawnArea { get; private set; }

    private void Awake()
    {
        SpawnArea = GetComponent<SphereCollider>();
    }

    public void RandomiseNodeLocations()
    {
        // Generate random offsets in space for each object
        foreach (var node in Nodes)
        {
            var offsetA = Random.Range(-5f, 5f);
            var offsetB = Random.Range(-5f, 5f);
            var offsetC = Random.Range(-5f, 5f);
            var spawnOffset = new Vector3(transform.position.x + offsetA, transform.position.y + offsetB,
                transform.position.z + offsetC);
            
                node.transform.position = spawnOffset;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("NetworkCol"))
        {
            transform.position = Random.insideUnitSphere * 50;
            RandomiseNodeLocations();
        }
    }
}
