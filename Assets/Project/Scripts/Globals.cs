using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    private static float foodLevelCount = 3;
    private static float modelDistance = 2f;
    private static float gangsterSpawnRate = 0.5f;
    private static FoodType currentFoodType = FoodType.Food1;

    private static Vector3 dropSpacing = new Vector3(4,0,3);
    private static float dropSmooth = 10;
    private static float plateSize = 0.5f;
    private static float currentLevel = 0;
    private static int currentCharacterModel = 0; 



    public static float GetModelDistance()
    {
        return modelDistance;
    }

    public static float GetSpawnRate()
    {
        return gangsterSpawnRate;
    }

    public static float GetFoodLevelCount()
    {
        return foodLevelCount;
    }

    public static FoodType GetCurrentFoodType()
    {
        return currentFoodType;
    }

    public static Vector3 GetDropSpacing()
    {
        return dropSpacing;
    }

    public static float GetDropSmooth()
    {
        return dropSmooth;
    }

    public static float GetPlateSize()
    {
        return plateSize;
    }

    public static float GetCurrentLevel()
    {
        return currentLevel;
    }

    public static int GetCurrentCharacterModel()
    {
        return currentCharacterModel;
    }
}
