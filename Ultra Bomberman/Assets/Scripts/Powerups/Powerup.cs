﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    [SerializeField] float minY = 0.9f;
    [SerializeField] float maxY = 1.3f;

    private Direction currentDir = Direction.Down;

    private enum Direction
    {
        Up,
        Down
    }

    void Start()
    {

    }

    void Update()
    {
        if (currentDir == Direction.Down)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
            if (transform.position.y <= minY)
            {
                currentDir = Direction.Up;
            }
        }
        else
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
            if (transform.position.y >= maxY)
            {
                currentDir = Direction.Down;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyPowerup(other.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyPowerup(PlayerController player);
}
