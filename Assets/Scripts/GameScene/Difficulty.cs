using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty
{
    private static Difficulty Beginner = new Difficulty(9, 10, 0, "Beginner");
    private static Difficulty Intermediate = new Difficulty(15, 40, 1, "Intermediate");
    private static Difficulty Expert = new Difficulty(25, 80, 2, "Expert");
    private static Difficulty[] difficulties = new Difficulty[3]{Beginner, Intermediate, Expert};

    public static Difficulty currentDifficulty;

    public String name;
    public byte boardSize;
    public byte bombAmount;
    private int id;

    private Difficulty(byte boardSize, byte bombAmount, byte id, String name)
    {
        this.bombAmount = bombAmount;
        this.boardSize = boardSize;
        this.id = id;
        this.name = name;
    }

    public static void ChangeDifficultyByName(String name)
    {
        foreach (Difficulty difficulty in difficulties)
        {
            if (difficulty.name.Equals(name))
            {
                currentDifficulty = difficulty;
                return;
            }
        }
    }
}
