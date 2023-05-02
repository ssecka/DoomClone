using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIBehaviour : MonoBehaviour
{
    [Header("References")] 
    public NavMeshAgent agent;
    public Transform player;
    public Animator anim;
    public Transform GunTip;

    [Header("AI")] 
    public LayerMask whatIsPlayer, whatIsGround;
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public GameObject bullet;
    public float health;
    public int damageToAI;
    private bool AIDied = false;
   


    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !AIDied ) AttackPlayer();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        
        anim.SetBool("Shooting", false);
        anim.SetBool("Running", false);
        anim.SetBool("Walking", true);
        
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f )
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        walkPoint = new Vector3(randomX, transform.position.y,randomZ);

        //if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        anim.SetBool("Walking", false);
        anim.SetBool("Shooting", false);
        anim.SetBool("Running", true);
        
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        
        anim.SetBool("Walking", false);
        anim.SetBool("Running", false);
        anim.SetBool("Shooting", true);

        if (!alreadyAttacked)
        {
            Rigidbody rb = Instantiate(bullet, GunTip.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 40f, ForceMode.Impulse);
            rb.AddForce(transform.up * 2f, ForceMode.Impulse);
            

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            TakeDamage(damageToAI);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            anim.Play("Dying");
            AIDied = true;
            Invoke(nameof(DestroyEnemy), 1.95f);
        }
        else 
        {
            Debug.Log("Damaged");
            anim.Play("Hit Reaction");
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}