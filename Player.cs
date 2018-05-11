using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pelaajan liikkumista ja toimintaa säätelevä skripti
/// Copyright (c) Radikaalit Runettajat
/// </summary>
public class Player : MonoBehaviour {

    public float speed;

    public AudioClip [] stepSounds;

    private AudioSource audioSource;

    private Vector2 direction;

    private Rigidbody2D playerRigidbody;

    private static bool playerExists;

    public bool canMove;

    private Animator anim;

    private SpriteRenderer sprite;

    private bool facingRight;

	/// <summary>
    /// Pelaajan alustus
    /// </summary>
	void Start () {

        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        playerRigidbody = GetComponent<Rigidbody2D>();
        if (!playerExists)
        {
            playerExists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }
	
	/// <summary>
    /// Pelaajan tilan päivitys
    /// </summary>
	void Update () {

        if(facingRight == true)
        {
            sprite.flipX = true;
        }

        if (facingRight == false)
        {
            sprite.flipX = false;
        }

        if (playerRigidbody.velocity.x <= 0 || playerRigidbody.velocity.x >= 0 || playerRigidbody.velocity.y <= 0 || playerRigidbody.velocity.y >= 0)
        {
            anim.SetBool("IsMoving", true);
        }

        if (playerRigidbody.velocity.x == 0 && playerRigidbody.velocity.y == 0)
        {
            anim.SetBool("IsMoving", false);
        }

        if (!canMove) // Katsotaan, saako pelaaja liikkua. Muutetaan falseksi dialogin ajaksi vastaavasta controllerista. 
        {
            playerRigidbody.velocity = new Vector2(0, 0);
            return;
        }
        GetInput();
        Move();
	}

    /// <summary>
    /// Liikuttaa hahmoa
    /// </summary>
    public void Move()
    {
        //transform.Translate(direction * speed * Time.deltaTime);
        playerRigidbody.velocity = direction * speed;
    }

    /// <summary>
    /// Vaihtaa liikkeen suuntaa inputin mukaan
    /// </summary>
    private void GetInput()
    {
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction += Vector2.left;
            facingRight = false;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction += Vector2.right;
            facingRight = true;
        }
    }
    /// <summary>
    /// Soittaa ääniefektit pelaajan kävelylle
    /// </summary>
    public void PlayStepSound()
    {
        int i = Random.Range(1, stepSounds.Length);
        audioSource.clip = stepSounds[i];
        audioSource.PlayOneShot(audioSource.clip);
        stepSounds[i] = stepSounds[0];
        stepSounds[0] = audioSource.clip;
    }
}
