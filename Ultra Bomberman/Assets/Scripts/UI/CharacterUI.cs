﻿using TMPro;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    [SerializeField]
    private int characterNumber = 1;

    private Character character;
    private TextMeshProUGUI lifes;

    private void Awake()
    {
        if (G.characterCount < characterNumber)
            gameObject.SetActive(false);
    }

    private void Start()
    {
        if (characterNumber > G.playerCount)
            transform.GetChild(2).gameObject.SetActive(false);

        if (G.train)
            G.gameController.reset.AddListener(Reset);

        character = GameObject.Find("Character" + characterNumber).GetComponent<Character>();
        character.takeDamager.AddListener(SetHealth);
        character.die.AddListener(HideCharacterUIs);

        lifes = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        Reset();
    }

    private void Reset()
    {
        lifes.text = character.startHealth.ToString();
    }

    private void SetHealth(Character character)
    {
        lifes.text = character.health.ToString();
    }

    private void HideCharacterUIs(Character character)
    {
        if (G.train)
        {
            Reset();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
