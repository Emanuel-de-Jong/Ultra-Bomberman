﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class G
{
    public static bool train = true;
    public static bool record = false;

    public static int characterCount = 3;
    public static int characterWon = 1;

    public static int roundDuration = 90;

    public static GameController gameController;
}

public enum Direction
{
    None,
    Forward,
    Back,
    Left,
    Right
}

public enum Action
{
    None,
    Bomb
}