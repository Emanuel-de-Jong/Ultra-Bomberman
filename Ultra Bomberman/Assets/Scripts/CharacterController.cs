﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    public int health = 3;
    public float cooldownDuration = 3f;
    public float movementSpeed = 7.5f;
    public float bombRange = 2f;

    [SerializeField] protected GameObject bomb;

    protected float cooldown = 0f;
    protected bool spawnBomb = false;
    protected Animator animator;
    protected Dictionary<Direction, bool> input;
    protected Direction lookDir = Direction.Forward;
    protected Direction lastMoveDir = Direction.None;
    protected Direction moveDir = Direction.None;

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
        animator = GetComponent<Animator>();
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
        Quaternion rotation = Quaternion.Euler(bomb.transform.rotation.x, Random.Range(0f, 361f), bomb.transform.rotation.z);
        GameObject bombInstance = Instantiate(bomb, new Vector3(transform.position.x, bomb.transform.position.y, transform.position.z), rotation);
        bombInstance.GetComponent<DynamiteCycle>().range = bombRange;
    }

    void TakeDamage()
    {
        health--;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
