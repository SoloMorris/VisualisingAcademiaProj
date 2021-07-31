using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

namespace NodeNetwork
{

    public class Visualiser : LateSetup
    {

        [SerializeField] private GameObject DocumentObj;

        [HideInInspector] public Transform spawnPoint;

        [SerializeField] private LineRenderer lr;

        // Networks to contain nodes
        private List<Network> nodeNetworks = new List<Network>(); // New way to store nodes.
        [SerializeField] private GameObject NetworkPrefab;

        [SerializeField] private ClickOnDocuments clickSettings;

        private List<GameObject>
            generatedNodes = new List<GameObject>(); // Used to keep track of generated nodes for multiple generations

        //  Relation Colours
        Color PublisherColour = Color.black;
        Color AuthorColour = Color.cyan;
        Color DateColour = Color.blue;

        // Optimisation
        [SerializeField] private float updateTimer;
        private float tick = 0f;

        //  Options
        public bool internalNodesSet { get; private set; } = false;

        public Dropdown Pubdropdown;
        public Dropdown Authdropdown;
        public Dropdown Datedropdown;

        /* //---------------------------------------------------------------------------------------------------------//
        Split all nodes into groups called networks                                                                   |
        Each network instantiates separately based on the type of relation to the Origin Node.                        |
        This way, relations to the origin are kept separate, and would allow for implementation of a feature for      |
        the user to change the Origin Node and generate new networks                                                  |
        Potentially only visualise connections if they differ from the type the node already shares with the origin   |
        */ //--------------------------------------------------------------------------------------------------------| 

        public void SelectColor()
        {
            Debug.Log("Working");
            switch (Pubdropdown.value)
            {
                case 1:
                    PublisherColour = Color.red;
                    break;
                case 2:
                    PublisherColour = Color.blue;
                    break;
                case 3:
                    PublisherColour = Color.green;
                    break;
            }

            switch (Authdropdown.value)
            {
                case 1:
                    AuthorColour = Color.red;
                    break;
                case 2:
                    AuthorColour = Color.blue;
                    break;
                case 3:
                    AuthorColour = Color.green;
                    break;
            }

            switch (Datedropdown.value)
            {
                case 1:
                    DateColour = Color.red;
                    break;
                case 2:
                    DateColour = Color.blue;
                    break;
                case 3:
                    DateColour = Color.green;
                    break;
            }
        }

        private void Update()
        {
            if (!IsSetupComplete())
                CreateMatches();
            else
            {
                tick++;
                if (tick < updateTimer) return;

                var networkChoice = Random.Range(0, nodeNetworks.Count);
                foreach (DocNode node in nodeNetworks[networkChoice].Nodes)
                {
                    foreach (var con in node.incomingConnections)
                    {
                        con.UpdatePositions();
                    }

                    foreach (var con in node.outgoingConnections)
                    {
                        con.UpdatePositions();
                    }
                }

                tick = 0f;
            }

        }


        #region Network Generation

        private void CreateMatches()
        {
            OriginNode.GetComponent<SphereCollider>().enabled = false;
            foreach (var match in DocumentPlotter.Instance.articles)
            {
                if (!IsRelated(match, OriginNode.Data.docData)) continue;

                CheckIfNodeExists(match);
            }

            foreach (var obj in generatedNodes)
            {
                var node = obj.GetComponent<DocNode>();
                //  If the node isn't related, leave it disabled
                if (!IsRelated(node.Data.docData, OriginNode.Data.docData))
                {
                    obj.SetActive(false);
                    continue;
                }

                //  Finds the relation between the new node and the Origin.
                CompareNodes(node, OriginNode);
                TryAddToNodeNetwork();

                void TryAddToNodeNetwork()
                {
                    foreach (var network in nodeNetworks)
                    {
                        if (network.RelationToOrigin == node.incomingConnections[0].connectionType)
                        {
                            foreach (var nw in nodeNetworks)
                            {
                                if (nw.Nodes.Contains(obj.GetComponent<DocNode>()) && nw != network) return;
                            }

                            network.Nodes.Add(obj.GetComponent<DocNode>());
                            return;
                        }
                    }

                    var newNetwork = Instantiate(NetworkPrefab, Random.insideUnitSphere * 50, transform.rotation);
                    newNetwork.GetComponent<Network>()
                        .SetRelationToOrigin(obj.GetComponent<DocNode>().incomingConnections[0].connectionType);
                    nodeNetworks.Add(newNetwork.GetComponent<Network>());
                    nodeNetworks[nodeNetworks.Count - 1].Nodes.Add(obj.GetComponent<DocNode>());
                }

            }

            // Go through the completed networks and find relations between everything else
            for (int i = 0; i < nodeNetworks.Count; i++)
            {
                foreach (DocNode node in nodeNetworks[i].Nodes)
                {
                    for (int j = 0; j < nodeNetworks.Count; j++)
                    {
                        foreach (DocNode otherNode in nodeNetworks[j].Nodes)
                        {
                            if (nodeNetworks[j] == nodeNetworks[i]) continue;
                            CompareNodes(node, otherNode);
                        }
                    }

                    node.transform.parent = nodeNetworks[i].transform;
                }

                nodeNetworks[i].RandomiseNodeLocations();
            }

            setupComplete = true;
        }

        private void CheckIfNodeExists(DocumentData match)
        {
            if (generatedNodes.Count > 0)
            {
                foreach (var entry in generatedNodes)
                {
                    if (entry.GetComponent<DocNode>().Data.docData == match)
                        return;
                }

                InstantiateNewNode(match);
            }
            else
            {
                InstantiateNewNode(match);
            }
        }

        private void InstantiateNewNode(DocumentData match)
        {
            //  Instantiate the object and attach the document data to it
            var newObj = Instantiate(DocumentObj, Vector3.zero, transform.rotation);
            var comp = newObj.AddComponent<DocNode>();
            comp.ApplyDocumentData(match);
            generatedNodes.Add(newObj);
        }

        private bool AreListsRelated<T>(List<T> a, List<T> b)
        {
            foreach (var entryA in a)
            {
                if (b.Contains(entryA)) return true;
            }

            return false;
        }

        private void CompareNodes(DocNode comparisonA, DocNode comparisonB)
        {
            // If these two nodes are the same, exit
            if (comparisonA.Equals(comparisonB)) return;

            // If these two nodes already have a connection, then exit
            if (comparisonA.GetComponent<NodeConnection>() &&
                comparisonA.GetComponent<NodeConnection>().origin == comparisonB
                || comparisonA.GetComponent<NodeConnection>() &&
                comparisonA.GetComponent<NodeConnection>().target == comparisonB) return;

            var aData = comparisonA.Data;
            var bData = comparisonB.Data;

            if (aData.Publisher.AttributeValue != null &&
                aData.Publisher.AttributeValue == bData.Publisher.AttributeValue)
                CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Publisher);
            if (aData.Authors.AttributesList[0] != null &&
                     AreListsRelated(aData.Authors.AttributesList, bData.Authors.AttributesList))
                CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Authors);
            if (aData.DatePublished.AttributeValue != null &&
                     aData.DatePublished.AttributeValue == bData.DatePublished.AttributeValue)
                CreateNodeConnection(comparisonA, comparisonB, ConnectionType.Date);
        }

        private void CreateNodeConnection(DocNode comparisonA, DocNode comparisonB, ConnectionType desiredConType)
        {
            var originConnection = comparisonA.gameObject.AddComponent<NodeConnection>();
            originConnection.Setup(lr, comparisonB, comparisonA);
            comparisonA.incomingConnections.Add(originConnection);
            comparisonB.outgoingConnections.Add(originConnection);
            originConnection.SetConnectionType(desiredConType);
            ApplyLineSettings(ref originConnection);
            originConnection.enabled = false;

        }

        private bool IsRelated(DocumentData source, DocumentData comparison)
        {
            //  Return if the Title, Publisher or Date are similar.
            if (source.Title.AttributeValue != null &&
                source.Title.AttributeValue == comparison.Title.AttributeValue) return true;
            if (source.Publisher.AttributeValue != null &&
                source.Publisher.AttributeValue == comparison.Publisher.AttributeValue) return true;
            if (AreListsRelated(source.Authors.AttributesList, comparison.Authors.AttributesList)) return true;
            if (source.DatePublished.AttributeValue != null &&
                source.DatePublished.AttributeValue == comparison.DatePublished.AttributeValue) return true;
            return false;
        }

        private void ApplyLineSettings(ref NodeConnection connection)
        {
            //Debug.Log("working1");
            switch (connection.connectionType)
            {
                case ConnectionType.Authors:
                    connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                    connection.connection.startColor = AuthorColour;
                    connection.connection.endColor = AuthorColour;
                    connection.Create();
                    break;
                case ConnectionType.Date:
                    connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                    connection.connection.startColor = DateColour;
                    connection.connection.endColor = DateColour;
                    connection.Create();
                    break;
                case ConnectionType.Publisher:
                    connection.connection.material = new Material(Shader.Find("Particles/Standard Unlit"));
                    connection.connection.startColor = PublisherColour;
                    connection.connection.endColor = PublisherColour;
                    connection.Create();
                    break;
                case ConnectionType.Title:
                    break;

            }
        }

        public void GenerateNewNetwork(DocNode newOrigin)
        {
            if (newOrigin == OriginNode) return;

            //  Make the old OriginNode into a normal node
            CheckIfNodeExists(OriginNode.Data.docData);
            Destroy(OriginNode.gameObject);

            //  Make the normal node the new OriginNode
            generatedNodes.Remove(newOrigin.gameObject);
            OriginNode = newOrigin;
            OriginNode.transform.parent = spawnPoint;
            OriginNode.transform.position = spawnPoint.position;
            OriginNode.incomingConnections.Clear();
            OriginNode.outgoingConnections.Clear();

            //  Clear relevant lists to be refilled
            foreach (var network in nodeNetworks)
            {
                network.Nodes.Clear();
            }

            for (int i = 0; i < generatedNodes.Count; i++)
            {
                generatedNodes[i].GetComponent<DocNode>().incomingConnections.Clear();
                generatedNodes[i].GetComponent<DocNode>().outgoingConnections.Clear();
            }

            internalNodesSet = false;
            CreateMatches();
        }

        #endregion

        #region Options

        /// <summary>
        /// Go back and find all relations between nodes in the same network.
        /// </summary>
        public void FindInternalNodeConnections()
        {
            if (clickSettings.viewingState) clickSettings.ToggleConnectionDisplay(clickSettings.TargetNode.transform);
            foreach (var network in nodeNetworks)
            {
                foreach (var nodeA in network.Nodes)
                {
                    foreach (var nodeB in network.Nodes)
                    {
                        if (nodeA.Equals(nodeB)) continue;
                        CompareNodes(nodeA, nodeB);
                    }
                }
            }

            internalNodesSet = true;
        }

        /// <summary>
        /// Enable or disable connections between nodes inside the same network.
        /// Should be called form the NetworkOptions script.
        /// </summary>
        /// <param name="currentSetting">Current user-chosen setting.</param>
        public void HandleInternalNodes(NetworkOptions.InternalNetworkConnections currentSetting)
        {
            switch (currentSetting)
            {
                case NetworkOptions.InternalNetworkConnections.ALL:
                   SetAllNodesInNetwork(true);
                    break;
                case NetworkOptions.InternalNetworkConnections.OFF:
                  SetAllNodesInNetwork(false);
                    break;
                case NetworkOptions.InternalNetworkConnections.UNIQUE:
                    foreach (var network in nodeNetworks)
                    {
                        foreach (var node in network.Nodes)
                        {
                            foreach (var con in node.incomingConnections)
                            {
                                //  If the node isn't inside this network, ignore it.
                                if (!network.Nodes.Contains(con.origin)) continue;
                                
                                //  If the node is inside this network and doesn't share connection types
                                //  WITH the network, make it visible
                                if (network.Nodes.Contains(con.origin) && !con.IsConnectionOfType(network.RelationToOrigin))
                                    con.visible = true;
                                else
                                    con.visible = false;
                            }

                            foreach (var con in node.outgoingConnections)
                            {
                                if (!network.Nodes.Contains(con.origin)) continue;
                                if (network.Nodes.Contains(con.origin) && !con.IsConnectionOfType(network.RelationToOrigin))
                                    con.visible = true;
                                else
                                    con.visible = false;
                            }
                        }
                    }
                    break;
            }

            void SetAllNodesInNetwork(bool active)
            {
                foreach (var network in nodeNetworks)
                {
                    foreach (var node in network.Nodes)
                    {
                        foreach (var con in node.incomingConnections)
                        {
                            if (network.Nodes.Contains(con.origin))
                                con.visible = active;
                        }

                        foreach (var con in node.outgoingConnections)
                        {
                            if (network.Nodes.Contains(con.target))
                                con.visible = active;
                        }
                    }
                }
            }
        }

        public Network FindNetworkContainsNode(DocNode desiredNode)
        {
            foreach (var network in nodeNetworks)
            {
                foreach (var node in network.Nodes)
                {
                    if (node.Equals(desiredNode)) return network;
                }
            }
            Debug.LogError("Could not find desired Node in existing networks.");
            return null;
        }

        #endregion

    }
}