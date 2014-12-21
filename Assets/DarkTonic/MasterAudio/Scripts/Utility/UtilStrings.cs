using UnityEngine;
using System.Collections;

/// <summary>
/// This class contains various string utility methods.
/// </summary>
public static class UtilStrings
{
    /// <summary>
    /// This method is used to name Sound Groups, Variations and Playlists based on the first clip dragged in. It removes spaces at the end and beginning of the audio file name. 
    /// </summary>
    /// <param name="untrimmed">The string to trim.</param>
    /// <returns>The string with no spaces.</returns>
    public static string TrimSpace(string untrimmed)
    {
        if (string.IsNullOrEmpty(untrimmed))
        {
            return string.Empty;
        }

        return untrimmed.Trim();
    }
}