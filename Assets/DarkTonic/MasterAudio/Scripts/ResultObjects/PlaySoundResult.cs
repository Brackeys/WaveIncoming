using UnityEngine;
using System.Collections;

/// <summary>
/// This class is returned from MasterAudio.PlaySound methods unless you choose one of the 'AndForget' methods. It tells you whether the sound was played, scheduled or neither. It also contains a reference to the Variation used to play the sound.
/// </summary>
[SerializeField]
public class PlaySoundResult
{
    public PlaySoundResult()
    {
        SoundPlayed = false;
        SoundScheduled = false;
        ActingVariation = null;
    }

    /// <summary>
    /// This property will tell you whether the sound was played or not.
    /// </summary>
    public bool SoundPlayed { get; set; }

    /// <summary>
    /// This property will tell you whether the sound was scheduled or not.
    /// </summary>
    public bool SoundScheduled { get; set; }

    /// <summary>
    /// This property will give you a reference to the Variation chosen, so you can do things to it like fade out, get notified of completion, etc.
    /// </summary>
    public SoundGroupVariation ActingVariation { get; set; }
}