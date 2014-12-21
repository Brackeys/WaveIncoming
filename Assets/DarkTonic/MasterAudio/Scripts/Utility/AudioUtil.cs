using UnityEngine;
using System.Collections;

/// <summary>
/// This class contains frequently used methods for audio in general.
/// </summary>
public static class AudioUtil
{
    /// <summary>
    /// This method will tell you the percentage of the clip that is done Playing (0-100).
    /// </summary>
    /// <param name="source">The Audio Source to calculate for.</param>
    /// <returns>(0-100 float)</returns>
    public static float GetAudioPlayedPercentage(AudioSource source)
    {
        var playedPercentage = (source.time / source.clip.length) * 100;
        return playedPercentage;
    }
}
