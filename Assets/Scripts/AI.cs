using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// figure out death and other stuff

public class AI : NetworkBehaviour {

    [SyncVar]
    private bool isAiDead = false;
    public bool isAiNoMore
    {
        get { return isAiDead; }
        protected set { isAiDead = value; }
    }

    [SerializeField]
    private int maxHealth = 50;

    [SyncVar]
    public int currentHealth;
    public Image healthBar;

    public void Setup()
    {
        setDefaults();
        healthBar = transform.Find("Graphics").Find("Canvas").Find("HealthBar").Find("Health").GetComponent<Image>();
    }

    public void setDefaults()
    {
        isAiDead = false;
        currentHealth = maxHealth;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, string player_name)
    {
        if (isAiDead)
        {
            return;
        }

        currentHealth -= damage;
        healthBar.fillAmount =((float)currentHealth / (float)maxHealth);
        
        AIController ai_control = transform.GetComponent<AIController>();
        ai_control.setMoveToPlayer(player_name);
        Debug.Log(transform.name + " now has " + currentHealth + " health");
        if (currentHealth <= 0)
        {
            Dead();
        }
    }
  
    private void Dead()
    {
        isAiDead = true;
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        

        Debug.Log(transform.name + " is DEAD!");
        AIController ai_control = transform.GetComponent<AIController>();
        ai_control.makeAIStop();
        ai_control.setMoveToPlayerFalse();

        StartCoroutine(RespawnAi());
    }

    public NetworkStartPosition[] Spawn(NetworkStartPosition[] spawn_points)
    {
        NetworkStartPosition[] startPositions = null; 
        Vector3 spawnPoint = Vector3.zero;
        if (spawn_points != null && spawn_points.Length > 0)
        {
            var wPos = spawn_points[Random.Range(0, spawn_points.Length)];
            startPositions = removeSpawnPoint(wPos, spawn_points);
            spawnPoint = wPos.transform.position;
        }
        transform.position = spawnPoint;
        return startPositions;
    }

    private NetworkStartPosition[] removeSpawnPoint(NetworkStartPosition start_pos, NetworkStartPosition[] spawn_points)
    {
        List<NetworkStartPosition> startArray = new List<NetworkStartPosition>();
        foreach (var item in spawn_points)
        {
            if (item != start_pos)
            {
                startArray.Add(item);
            }
        }
        return startArray.ToArray();
    }

    private IEnumerator RespawnAi()
    {
        yield return new WaitForSeconds(GameManager.instance.matchConfig.respawnDelay);
        setDefaults();
        Transform spawLocation = NetworkManager.singleton.GetStartPosition();
        transform.position = spawLocation.position;
        transform.rotation = spawLocation.rotation;

        Debug.Log("AI RESPAWNED");
    }
}
