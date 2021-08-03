using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace NodeNetwork
{
    /// <summary>
    /// Customisable options for the user to set while the application is running
    /// </summary>

    public class NetworkOptions : MonoBehaviour
    {
        [SerializeField] private Visualiser visualiser;

        [SerializeField] private TextMeshProUGUI textField;
        /// <summary>
        /// Whether to display ALL connections from nodes instead of only when clicked
        /// </summary>
        public bool ViewAllConnections { get; private set; }

        /// <summary>
        /// How connections are displayed between nodes inside a network.
        /// </summary>
        public InternalNetworkConnections InternalNetworkView { get; private set; }

        #region Internal network Settings

        public enum InternalNetworkConnections
        {
            /// <summary>
            /// Connections between nodes in the same network cluster are NEVER displayed.
            /// </summary>
            OFF,

            /// <summary>
            /// Connections between nodes in the same network cluster are shown if they are different from the network's
            /// OWN connection type.
            /// </summary>
            UNIQUE,

            /// <summary>
            /// Connections between nodes in the same network cluster are ALL displayed.
            /// </summary>
            ALL
        }

        public void SetNetworkConnectionsAll()
        {
            InternalNetworkView = InternalNetworkConnections.ALL;
        }

        public void SetNetworkConnectionsOff()
        {
            InternalNetworkView = InternalNetworkConnections.OFF;
        }

        public void SetNetworkConnectionsUnique()
        {
            InternalNetworkView = InternalNetworkConnections.UNIQUE;
        }

        #endregion

        private void Start()
        {
            SetNetworkConnectionsOff();
        }

        private void Update()
        {
            if (visualiser.internalNodesSet) return;
            InternalNetworkView = InternalNetworkConnections.OFF;
            textField.text = "Off";
        }

        public void HandleNetworkConnections()
        {
            print("Swapped");
            //  Switch between states
            switch (InternalNetworkView)
            {
                case InternalNetworkConnections.ALL:
                    InternalNetworkView = InternalNetworkConnections.UNIQUE;
                    textField.text = "Unique";
                    break;
                case InternalNetworkConnections.OFF:
                    InternalNetworkView = InternalNetworkConnections.ALL;
                    textField.text = "All";
                    break;
                case InternalNetworkConnections.UNIQUE:
                    InternalNetworkView = InternalNetworkConnections.OFF;
                    textField.text = "Off";
                    break;
            }

            
            // Execute the resulting state
            switch (InternalNetworkView)
            {
                case InternalNetworkConnections.ALL:
                    if (!visualiser.internalNodesSet) visualiser.FindInternalNodeConnections();
                    break;
                case InternalNetworkConnections.UNIQUE:
                    if (!visualiser.internalNodesSet) visualiser.FindInternalNodeConnections();

                    break;
            }
            visualiser.HandleInternalNodes(InternalNetworkView);
        }
    }
}
