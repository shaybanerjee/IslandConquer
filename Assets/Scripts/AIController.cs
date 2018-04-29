using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;


public class AIController : NetworkBehaviour {

    public float lookRadius = 300f;
    public float moveRadius = 5000f;
    public NavMeshAgent agent;
    Player[] playersInGame;
    public float wanderTimer;
    public float timer;
    private bool moveToPlayer = false;
    string focused_player;
    bool attacking = false;
    bool moving = false;
    AI ai_obj = null;

    private void Start()
    { 
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        ai_obj = transform.GetComponent<AI>();
    }

    private void Update()
    {
        playersInGame = GameManager.getPlayerList();
        for (int i = 0; i < playersInGame.Length; i++)
        {
            float distance = Vector3.Distance(playersInGame[i].transform.position, transform.position);
            if (distance <= lookRadius)
            {
                moveToPlayer = false;
                CmdCheck(playersInGame[i].transform.position, distance);

                // we want to attack if the distance is this close
                if (attacking == false && transform.GetComponent<AI>().currentHealth > 0)
                {
                    StartCoroutine(attackPlayer(playersInGame[i].transform.name));
                }
            }
            else if (moveToPlayer)
            {
                Player player = GameManager.GetPlayer(focused_player);
                agent.SetDestination(player.transform.position);

                // we want to attack since the AI has been attacked
                if (!attacking && transform.GetComponent<AI>().currentHealth > 0)
                {
                    StartCoroutine(attackPlayer(playersInGame[i].transform.name));
                }
            }
            else
            {
                if (!moving)
                {
                    // make AI move randomly 
                    timer += Time.deltaTime;
                    if (timer >= wanderTimer)
                    {
                        Vector3 newPos = RandomNavSphere(transform.position, moveRadius, -1);
                        StartCoroutine(updateMovement(newPos));
                        timer = 0;
                    }
                }
            }
        }
    }

    private IEnumerator updateMovement(Vector3 pos)
    {
        moving = true;
        float val = Random.Range(0, 11);
        yield return new WaitForSeconds(val);
        CmdUpdateMovement(pos);
        moving = false;
    }

    private IEnumerator attackPlayer(string name)
    {
        attacking = true;
        yield return new WaitForSeconds(2f);
        CmdAttackPlayer(name);
        CmdShowMuzzleFlash();
        attacking = false;

    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    public void setMoveToPlayer(string player_name)
    {
        focused_player = player_name;
        moveToPlayer = true;
    }

    public void setMoveToPlayerFalse()
    {
        focused_player = null;
        moveToPlayer = false;
    }

    public void makeAIStop()
    {
        CmdStopMoving();
    }

    [ClientRpc]
    void RpcDoMuzzleEffect()
    {
        if (ai_obj.currentHealth > 0)
        {
            transform.GetComponent<AIGraphic>().muzzleFlash.Play();
        }
    }

    [Command]
    void CmdShowMuzzleFlash()
    {
        RpcDoMuzzleEffect();
    }

    [Command]
    void CmdMoveToPlayer(Vector3 pos)
    {
        RpcAiMoveToPlayer(pos);
    }

    [ClientRpc]
    void RpcAiMoveToPlayer(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    [Command]
    void CmdUpdateMovement(Vector3 pos)
    {
        RpcAiMove(pos);
    }

    [Command]
    void CmdAttackPlayer(string name)
    {
        RpcAiAttackPlayer(name);
    }

    [ClientRpc]
    void RpcAiAttackPlayer(string name)
    {
        Player player = GameManager.GetPlayer(name);
        if (player.currentHealth <= 0)
        {
            focused_player = null;
            moveToPlayer = false;
        }
        else if (ai_obj.currentHealth > 0)
        {
            player.RpcTakeDamage(10, player.name);
        }       
    }

    [ClientRpc]
    void RpcAiMove(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    [Command]
    void CmdCheck(Vector3 vec, float distance)
    {
        RpcAiFollow(vec, distance);
    }

    [ClientRpc]
    private void RpcAiFollow(Vector3 vec, float distance)
    { 
   
        agent.SetDestination(vec);
        if (distance <= agent.stoppingDistance)
        { 
            // face the target
            FaceClient(vec);
        }
    }

    [Command]
    private void CmdStopMoving()
    {
        RpcAiStop();
    }

    [ClientRpc]
    private void RpcAiStop()
    {
        agent.isStopped = true;
    }

    /*

    [Command]
    void CmdStopMovement()
    {
        RpcAiStop();
    }

    */


    private void FaceClient(Vector3 vec)
    {
        Vector3 direction = (vec - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }

}
