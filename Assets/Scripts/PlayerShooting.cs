using UnityEngine.Networking;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour {

    private const string P_TAG = "Player";
    private const string E_TAG = "Enemy";
    public PlayerWeapon wep;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    private WepGraphics wp; 

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShooter: No camera referenced.");
            this.enabled = false;
        }

        wp = FindObjectOfType<WepGraphics>();

    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    // server function for when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoMuzzleFlash();
    }

    // display shoot effect on all clients
    [ClientRpc]
    void RpcDoMuzzleFlash()
    {
        // bug shooting animation appears on remotePlayer when localPlayer shoots
        //wp.muzzleFlash.Play();
    }

    // called on all clients when something is hit
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 norm)
    {
        GameObject hit_ref = (GameObject)Instantiate(wp.impactEffectPrefab, pos, Quaternion.LookRotation(norm));
        Destroy(hit_ref, 2f);
    }

    // called on server when something is hit
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _norm)
    {
        RpcDoHitEffect(_pos, _norm);
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Calling shoot method on server
        CmdOnShoot();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, wep.range, mask))
        {
            if (hit.collider.tag == P_TAG)
            {
                CmdPlayerHit(hit.collider.name, wep.damage, transform.name);
            }
            if (hit.collider.tag == E_TAG)
            {
                CmdAiHit(hit.collider.name, wep.damage, transform.name);
            }
            // hit something, show hit effect
            CmdOnHit(hit.point, hit.normal);
        }
    }

    // [Command] Methods that are only called on server
    [Command]
    void CmdPlayerHit(string player_id, int damage, string player_name)
    {
        Debug.Log(player_id + " has been shot.");
        Player player = GameManager.GetPlayer(player_id);
        player.RpcTakeDamage(damage, player_name);
    }

    [Command]
    void CmdAiHit(string player_id, int damage, string player_name)
    {
        Debug.Log(player_id + " has been shot.");
        // need to register AI the same way we register player
        AI ai_player = GameManager.getAI(player_id);
        ai_player.RpcTakeDamage(damage, player_name);
        
    }
}
