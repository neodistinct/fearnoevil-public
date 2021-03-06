﻿
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Library
{
    // ONE: Constructor of kosher SINGLETON shall always be hidden, so no one can instantiate it
    private Library() { }

    // TWO: Declare the actual STATIC variable of its own type
    private static Library _instance;

    // THREE: Do the pattern-based kabbalistic magic GetInstance function
    public static Library GetInstance()
    {
        if (_instance == null)
        {
            _instance = new Library();
            return _instance;
        } 

        return _instance;
    }

    /*------------------------------------------------------------------*/

    private static string playerName = "Player 1"; // Not a constant, just keeping a default value

    // And now... do the actual implementation of your beloved methods
    public static void RotateTowards(Transform target, Transform transform, float rotationSpeed)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public static string GetPlayerName()
    {
        return playerName;
    }

    public int GetCurrentSceneIndex(List<String> levels)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = levels.IndexOf(currentSceneName);

        return currentSceneIndex;
    }
}
