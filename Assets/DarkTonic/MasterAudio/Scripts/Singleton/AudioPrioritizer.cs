using UnityEngine;
using System.Collections;

public static class AudioPrioritizer {
	public const int MAX_PRIORITY = 255;

	private const int HIGHEST_PRIORITY = 16;
    private const int LOWEST_PRIORITY = 128;
	
	public static void Set2dSoundPriority(AudioSource audio) {
		audio.priority = HIGHEST_PRIORITY;
	}
	
	public static void Set3dPriority(AudioSource audio)
    {
        float distanceToListener = Vector3.Distance(audio.transform.position, MasterAudio.AudioListenerTransform.position);
        float perceivedVolume;
        
        if (audio.rolloffMode == AudioRolloffMode.Logarithmic) {
            perceivedVolume = audio.volume / Mathf.Max(audio.minDistance, distanceToListener - audio.minDistance); // Unity seems to just use a 1/distance model for this
        } else if (audio.rolloffMode == AudioRolloffMode.Linear) {
            perceivedVolume = Mathf.Lerp(audio.volume, 0, Mathf.Max(0, distanceToListener - audio.minDistance) / (audio.maxDistance - audio.minDistance)); // Linearly interpolate from max volume to zero as we go from the minimum distance to the max
        } else {
            perceivedVolume = Mathf.Lerp(audio.volume, 0, Mathf.Max(0, distanceToListener - audio.minDistance) / (audio.maxDistance - audio.minDistance)); // Not possible to deal with custom rolloffs since it's not accessible by script.  Let's pretend it's linear.
        }

        if (!audio.loop)
        {//Don't make looping sounds lessen in priority over time
            perceivedVolume = Mathf.Lerp(perceivedVolume, perceivedVolume * 0.1f, AudioUtil.GetAudioPlayedPercentage(audio) * .01f);//Set the factor lower when this non-looping sound has played for a few seconds so that newer sounds get a slightly higher priority.
        }

        audio.priority = (int)Mathf.Lerp(HIGHEST_PRIORITY, LOWEST_PRIORITY, Mathf.InverseLerp(1f, 0f, perceivedVolume)); // Transform our perceived volume from the [0...1] range to the [16...128] range so that the higher the perceived volume the lower the priority number.
	}
}
