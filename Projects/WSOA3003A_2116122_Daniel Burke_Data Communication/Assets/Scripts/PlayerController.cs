using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

  
    private _2DTopdown_PlayerMovement playerInput;
    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;

    SaveLoad playerPosData;

    Vector2 movement;
    private void Awake()
    {
        playerInput = new _2DTopdown_PlayerMovement();
        rb = GetComponent<Rigidbody2D>();

        playerPosData = FindObjectOfType<SaveLoad>();
        playerPosData.PlayerPosLoad();

    }

    

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    private void FixedUpdate()
    {
        Vector2 moveInput = playerInput.Movement.Move.ReadValue<Vector2>();

        rb.velocity = moveInput * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Slime01")
        {
            Debug.Log("Collided with Slime");
            SceneManager.LoadScene("Slime_Battle"); 
            gameObject.SetActive(false);
            playerPosData.PlayerPosSave();
        }
        if(collision.gameObject.tag == "Cultist")
        {
            float randCrit = Random.Range(0, 100);
            if(randCrit > 95)
            {
                playerPosData.PlayerPosSave();
                //SceneManager.LoadScene(3);
            }
            else
            {
                playerPosData.PlayerPosSave();
                //SceneManager.LoadScene(4);
            }
        }
        if(collision.gameObject.tag == "CursedCultist")
        {
            playerPosData.PlayerPosSave();
            //SceneManager.LoadScene(3);
        }
    }

}
