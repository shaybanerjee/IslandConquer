using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AI))]
public class AISetup : NetworkBehaviour
{
    private static NetworkStartPosition[] spawn_points = null;
    private void Start()
    {
        /*
        if  (spawn_points == null)
        {
            Debug.Log("HERE");
            spawn_points = FindObjectsOfType<NetworkStartPosition>();
        }
        */
        // Spawn AI BOT 1
        GetComponent<AI>().Setup();
        //spawn_points = GetComponent<AI>().Spawn(spawn_points);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        AI bot = GetComponent<AI>();
        GameManager.RegisterAi(netId, bot);
    }

    private void OnDisable()
    {
        GameManager.DeRegisterAi(transform.name);
    }








}
