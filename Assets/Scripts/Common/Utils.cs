using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    public static float NormalizeAngle(float angleDegrees)
    {
        while (angleDegrees > 180f)
            angleDegrees -= 360f;
        
        while (angleDegrees <= -180f)
            angleDegrees += 360f;
        
        return angleDegrees;
    }

    public static int RandomSigned(int start, int end)
    {
        int sign = Random.Range(0, 2) > 0? -1 : 1;
        var num = sign * Random.Range(start, end);
        return num;
    }

    public static float RandomSigned(float start, float end)
    {
        int sign = Random.Range(0, 2) > 0? -1 : 1;
        var num = sign * Random.Range(start, end);
        return num;
    }

    public static void PlayRandomPitch(AudioSource source, AudioClip clip)
    {
        source.pitch = Random.Range(.8f, 1.2f);
        source.clip = clip;
        source.Play();
    }

    public static T GetRandom<T>(T[] array)
    {
        if (array.Length < 0) 
            return default(T);
        int index = Random.Range(0, array.Length);
        return array[index];
    }

    public static T GetRandom<T>(List<T> list)
    {
        if (list.Count < 0) 
            return default(T);
        int index = Random.Range(0, list.Count);
        return list[index];
    }
    
}