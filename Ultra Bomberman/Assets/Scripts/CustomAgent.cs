﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

public class CustomAgent : Agent
{
    public const int NONE = 0;

    public const int FORWARD = 1;
    public const int BACK = 2;
    public const int LEFT = 3;
    public const int RIGHT = 4;

    public const int BOMB = 1;

    private Character character;

    private void Start()
    {
        character = GetComponent<Character>();
        if (character.isPlayer)
            GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.HeuristicOnly;

        if (G.train)
            G.gameController.reset.AddListener(Reset);

        character.takeDamager.AddListener(TakeDamage);
        character.die.AddListener(Die);
    }

    private void Reset()
    {
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(character.health);
        sensor.AddObservation(character.cooldown);

        sensor.AddObservation(transform.position.x);
        sensor.AddObservation(transform.position.z);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = (int)character.GetNextDirection();
        discreteActionsOut[1] = (int)character.GetNextAction();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        character.currentDirection = (Direction)actionBuffers.DiscreteActions[0];
        character.currentAction = (Action)actionBuffers.DiscreteActions[1];

        AddReward(-0.0003f);
    }

    public void CharacterHit()
    {
        AddReward(1);
    }

    public void BombPlaced()
    {
        AddReward(0.05f);
    }

    private void TakeDamage(Character character)
    {
        AddReward(-1);
    }

    private void Die(Character character)
    {
        AddReward(-2);
    }
}
