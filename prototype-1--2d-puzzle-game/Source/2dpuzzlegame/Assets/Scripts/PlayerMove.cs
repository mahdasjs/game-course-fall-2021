﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMove : MonoBehaviour
{
    
    public float factor = 0.01f;
    public float jumpAmount = 0.5f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public GameObject clones;
    public CloneMove[] cloneMoves;

    private bool canJump;

    private bool getKey;
    private bool getKey1;
    public  static bool pause=false;

    private bool haveKey;

    private bool win;
    private bool transport;

    public EventSystemCustom eventSystem;

    public Text counterText;

    public GameObject arrow;
    private Vector3 moveVector;
    void Start()
    {
        cloneMoves = clones.GetComponentsInChildren<CloneMove>();

        canJump = true;
        getKey = false;
        getKey1 = false;
        win = false;
        haveKey = false;
        transport = false;
        moveVector = new Vector3(1 * factor, 0, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveVector;

            MoveClones(moveVector, true);

            spriteRenderer.flipX = false;

        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveVector;

            MoveClones(moveVector, false);

            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            JumpClones(jumpAmount);
        }


        // This was added to answer a question.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Destroy(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.E) )
        {
            if (transport == true)
            {
                transform.position = GameObject.FindGameObjectWithTag(TagNames.destination.ToString()).transform.position;
                transport = false;
            }
            if (win == true)
            {
                SceneManager.LoadScene("win");
            }
            getKey = true;
            getKey1 = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameControl.disabled = true;
        }

        if (pause == true)
        {
            moveVector = new Vector3(0, 0, 0);
        }
        else
        {
            moveVector = new Vector3(1 * factor, 0, 0);
        }
        // This is too dirty. We must decalare/calculate the bounds in another way. 
        /*if (transform.position.x < -0.55f) 
        {
            transform.position = new Vector3(0.51f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 0.53f)
        {
            transform.position = new Vector3(-0.53f, transform.position.y, transform.position.z);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.DeathZone.ToString()))
        {
            SceneManager.LoadScene("gameover");
        }

        if (collision.gameObject.CompareTag(TagNames.CollectableItem.ToString()))
        {
            collision.gameObject.SetActive(false);
            Debug.Log("POTION!");
        }

        if (collision.gameObject.CompareTag(TagNames.KeyItem.ToString()))
        {
            if (getKey == true)
            {
                eventSystem.OnGetKey.Invoke();
                collision.gameObject.SetActive(false);
                Debug.Log("KEY!");
                getKey1 = false;
                getKey = false;
            }
        }

        if (collision.gameObject.CompareTag(TagNames.keySource.ToString()))
        {
            if (getKey1 == true)
            {
                collision.gameObject.SetActive(false);
                Debug.Log("KEY!");
                getKey = false;
                getKey1 = false;
                haveKey = true;
            }
        }

        if (collision.gameObject.CompareTag(TagNames.switchKey.ToString()))
        {
            collision.gameObject.SetActive(false);
            gameControl.disabled = false;
            pause = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky");
            win = false;
            canJump = false;
        }

        if (collision.gameObject.CompareTag(TagNames.exitDoor.ToString()))
        {
            int newTextValue = int.Parse(counterText.text);
            if (newTextValue > 0 )
            {
                win = true;
            }
        }

        if (collision.gameObject.CompareTag(TagNames.SourceDoor.ToString()) && haveKey)
        {
            transport = true;

        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky no more bruh");
            canJump = true;
        }
    }

    public void MoveClones(Vector3 vec, bool isDirRight)
    {
        foreach (var c in cloneMoves)
            c.Move(vec, isDirRight);
    }

    public void JumpClones(float amount)
    {
        foreach (var c in cloneMoves)
            c.Jump(amount);
    }
}
