
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Library
{
    // ONE: Constructor of kosher SINGLETON shall always be hidden, so no one can instantiate it
    private Library() { }

    // TWO: Declare the actual STATIC variable of its own type
    private static Library instance;

    // THREE: Do the pattern-based kabbalistic magic GetInstance function
    public static Library GetInstance()
    {
        if (instance == null)
        {
            instance = new Library();
            return instance;
        } 

        return instance;
    }

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

    private static string playerName = "Player 1";

    public int GetCurrentSceneIndex(List<String> levels)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int currentSceneIndex = levels.IndexOf(currentSceneName);

        return currentSceneIndex;
    }
}
