﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CharacterController : MonoBehaviour
{
    [System.Serializable]
    public class CharacterControllerEvent : UnityEvent<CharacterController> {};
    public CharacterControllerEvent takeDamager;
    public CharacterControllerEvent die;

    public int playerNumber;
    public int health = 3;
    public int bombRange = 2;
    public float cooldownDuration = 3f;
    public float movementSpeed = 7.5f;
    public string model = "MechanicalGolem";

    [SerializeField] protected GameObject bomb;
    [SerializeField] protected GameUI ui;
    [SerializeField] protected GameObject deathExplosion;

    protected AudioSource damageSound;
    protected float cooldown = 0f;
    protected bool spawnBomb = false;
    protected Animator animator;
    protected new Renderer renderer;
    protected Direction lookDir = Direction.Forward;
    protected Direction lastMoveDir = Direction.None;
    protected Direction moveDir = Direction.None;
    protected Vector3 startPos;
    protected Dictionary<Direction, bool> input;

    protected enum Direction
    {
        Forward,
        Back,
        Left,
        Right,
        None
    }

    void Start()
    {
        damageSound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        renderer = gameObject.transform.Find(model).GetComponent<Renderer>();
        startPos = transform.position;
        input = new Dictionary<Direction, bool>() { [Direction.None] = false };
    }

    void FixedUpdate()
    {
        UpdateInput();
        UpdateMovement();
        UpdateAnimation();
        UpdateBomb();
    }

    void LateUpdate()
    {
        lastMoveDir = moveDir;
    }

    protected abstract void UpdateInput();

    void UpdateMovement()
    {
        moveDir = Direction.None;
        if (input[lastMoveDir])
        {
            moveDir = lastMoveDir;
            Move();
        }
        else
        {
            foreach (KeyValuePair<Direction, bool> entry in input)
            {
                if (entry.Value)
                {
                    moveDir = entry.Key;
                    Move();
                    break;
                }
            }
        }
    }

    void Move()
    {
        Vector3 offset = Vector3.zero;
        if (moveDir == Direction.Forward)
        {
            offset = Vector3.forward;
        }
        else if (moveDir == Direction.Back)
        {
            offset = Vector3.back;
        }
        else if (moveDir == Direction.Left)
        {
            offset = Vector3.left;
        }
        else if (moveDir == Direction.Right)
        {
            offset = Vector3.right;
        }

        transform.position += offset * movementSpeed * Time.deltaTime;
    }

    void UpdateAnimation()
    {
        if (lookDir != moveDir)
        {
            if (moveDir == Direction.Forward)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (moveDir == Direction.Back)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else if (moveDir == Direction.Left)
            {
                transform.localRotation = Quaternion.Euler(0, 270, 0);
            }
            else if (moveDir == Direction.Right)
            {
                transform.localRotation = Quaternion.Euler(0, 90, 0);
            }

            lookDir = moveDir;
        }

        if (moveDir != Direction.None && lastMoveDir == Direction.None)
        {
            animator.SetBool("isWalking", true);
        }
        else if (moveDir == Direction.None && lastMoveDir != Direction.None)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void UpdateBomb()
    {
        if (spawnBomb && cooldown <= Time.time)
        {
            PlaceBomb();
            cooldown = Time.time + cooldownDuration;
        }
    }

    void PlaceBomb()
    {
        float x, z;
        if ((x = Mathf.Ceil(transform.position.x)) % 2 == 0)
            x = Mathf.Floor(transform.position.x);

        if ((z = Mathf.Ceil(transform.position.z)) % 2 == 0)
            z = Mathf.Floor(transform.position.z);

        Quaternion rotation = Quaternion.Euler(bomb.transform.rotation.x, Random.Range(0f, 361f), bomb.transform.rotation.z);
        GameObject bombInstance = Instantiate(bomb, new Vector3(x, bomb.transform.position.y, z), rotation);
        bombInstance.GetComponent<BombController>().range = bombRange;
    }

    public void TakeDamage()
    {
        damageSound.Play();

        health--;
        if (health < 1)
        {
            if (G.train)
            {
                Respawn();
            }
            else
            {
                Die();
            }
        }

        StartCoroutine(DamageColor());

        takeDamager.Invoke(this);
    }

    private void Respawn()
    {
        health = 3;
        transform.position = startPos;
    }

    private void Die()
    {
        die.Invoke(this);

        Instantiate(deathExplosion, new Vector3(transform.position.x, deathExplosion.transform.position.y, transform.position.z), deathExplosion.transform.rotation);

        GetComponent<BoxCollider>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        renderer.enabled = false;
        Destroy(gameObject, damageSound.clip.length);
    }

    IEnumerator DamageColor()
    {
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        renderer.material.color = Color.white;
    }
}
