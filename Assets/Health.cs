using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float playerHealth;
    public int damageToPlayer;
    public TextMeshProUGUI healthDisplay;
    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthDisplay.SetText("HP : " + playerHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            DamagePlayer(damageToPlayer);
        }
    }

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            Invoke(nameof(DestroyPlayer), 0f);
            gameOverScreen.SetActive(true);
            //GameOverScreen with Killcount, time
        }
    }

    private void DestroyPlayer()
    {
        Destroy(this.gameObject);
    }
}