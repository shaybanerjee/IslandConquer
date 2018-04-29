using UnityEngine;
using UnityEngine.Networking;

// This script will act as an object that is networked
[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    private Camera sceneCamera;
    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;
    [SerializeField]
    GameObject playerCrossHairPrefab;
    private GameObject playerUiInstance;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                // disable main camera when we connect
                sceneCamera.gameObject.SetActive(false);
            }

            // Disable overlapping player model on camera
            SetLayerRecurse(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Create player UI
            playerUiInstance = Instantiate(playerCrossHairPrefab);
            playerUiInstance.name = playerCrossHairPrefab.name;
        }
        GetComponent<Player>().Setup();
    }

    // set layer for gameObject and all children through hiearchy
    void SetLayerRecurse(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecurse(child.gameObject, newLayer);
        }
    }

    // called everytime a client is setup locally
    // we will use this to register our players
    public override void OnStartClient()
    {
        base.OnStartClient();   
        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player playr = GetComponent<Player>();
        GameManager.RegisterPlayer(netId, playr);
    }

    private void DisableComponents()
    {
        // if the object is not the player being controlled, disable
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void AssignRemoteLayer()
    {
        // all layers are assigned numbers, represented in Unity editor as strings
        // gameObject.layer expects a number 
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void OnDisable()
    {
        Destroy(playerUiInstance);

        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.DeRegistPlayer(transform.name);
    }
}
