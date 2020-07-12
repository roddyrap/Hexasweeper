using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Difficulty
{
    private static Difficulty Test = new Difficulty(5, 3, 6, "Test", "yellow");
    private static Difficulty Beginner = new Difficulty(15, 27, 0, "Beginner", "green");
    private static Difficulty Intermediate = new Difficulty(24, 80, 1, "Intermediate", "orange");
    private static Difficulty Expert = new Difficulty(35, 180, 2, "Expert", "red");
    private static Difficulty Nightmare = new Difficulty(60, 540, 3, "Nightmare", "blue");

    private static Difficulty[] difficulties = new Difficulty[]{Test, Beginner, Intermediate, Expert, Nightmare};

    public static Difficulty currentDifficulty;

    public String name;
    public short boardSize;
    public short bombAmount;
    public String color;
    private int id;

    private Difficulty(short boardSize, short bombAmount, byte id, String name, String color)
    {
        this.bombAmount = bombAmount;
        this.boardSize = boardSize;
        this.id = id;
        this.name = name;
        this.color = color;
    }

    public static void ChangeDifficultyByName(String name)
    {
        Regex re = new Regex("(?:<.*?>)*([^<>]*)(?:<.*?>)*");
        name = re.Match(name).Groups[1].Captures[0].Value;
        Debug.Log(name);
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
