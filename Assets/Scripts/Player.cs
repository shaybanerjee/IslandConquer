using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool isDead = false;
    public bool isNoMore
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private Behaviour[] disOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private int maxHealth = 300;
    // not everytime the value changes it will be changed on all clients
    [SyncVar]
    public int currentHealth;
    public Image healthBar;

    public void Setup()
    {
        wasEnabled = new bool[disOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disOnDeath[i].enabled;
        }

        SetDefaults();
        healthBar = transform.Find("graphics").Find("Canvas").Find("HealthBar").Find("Health").GetComponent<Image>();
    }

    /*
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(99999);
        }
    }
    */

    // ClientRpc will make sure to do this on all computers connected to the network
    [ClientRpc]
    public void RpcTakeDamage(int damage, string player_name)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        healthBar.fillAmount = (float)currentHealth / (float)maxHealth;
        Debug.Log(transform.name + " now has " + currentHealth + " health.");
        if (currentHealth <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        isDead = true;

        // we have to disable components here
        for (int i = 0; i < disOnDeath.Length; i++)
        {
            disOnDeath[i].enabled = false;
        }
        Collider col = GetComponent<Collider>();
        if  (col != null)
        {
            col.enabled = false;
        }

        Debug.Log(transform.name + " is dead HAHA");

        // Respawns
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchConfig.respawnDelay);
        SetDefaults();
        Transform spawn = NetworkManager.singleton.GetStartPosition();
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;
        Debug.Log(transform.name + " Respawned!");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; i < disOnDeath.Length; i++)
        {
            disOnDeath[i].enabled = wasEnabled[i];
        }
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();
        GUILayout.Label(transform.name + " - HEALTH: " + currentHealth);
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
