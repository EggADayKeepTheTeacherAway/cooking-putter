using UnityEngine;

[System.Serializable]
public struct RandomTimer
{
    public int minSecond;
    public int maxSecond;

    public int GetRandomDelay()
    {
        return Random.Range(minSecond, maxSecond + 1);
    }
}
