using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicGroupVariation : MonoBehaviour {
	public Texture logoTexture;
	public Texture playTexture;
	public Texture stopTrackTexture;
	
    public float randomPitch = 0f;
    public float randomVolume = 0f;
    public int weight = 1;
	public MasterAudio.AudioLocation audLocation = MasterAudio.AudioLocation.Clip;
    public string resourceFileName;

	public float fxTailTime = 0f;
	public bool useFades = false;
    public float fadeInTime = 0f;
    public float fadeOutTime = 0f;
	
	public bool useIntroSilence;
	public float introSilenceMin;
	public float introSilenceMax;
	
    private AudioDistortionFilter distFilter;
    private AudioEchoFilter echoFilter;
    private AudioHighPassFilter hpFilter;
    private AudioLowPassFilter lpFilter;
    private AudioReverbFilter reverbFilter;
    private AudioChorusFilter chorusFilter;
	
    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity Distortion Filter FX component.
    /// </summary>
    public AudioDistortionFilter DistortionFilter
    {
        get
        {
            if (distFilter == null)
            {
                distFilter = this.GetComponent<AudioDistortionFilter>();
            }

            return distFilter;
        }
    }

    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity Reverb Filter FX component.
    /// </summary>
    public AudioReverbFilter ReverbFilter
    {
        get
        {
            if (reverbFilter == null)
            {
                reverbFilter = this.GetComponent<AudioReverbFilter>();
            }

            return reverbFilter;
        }
    }

    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity Chorus Filter FX component.
    /// </summary>
    public AudioChorusFilter ChorusFilter
    {
        get
        {
            if (chorusFilter == null)
            {
                chorusFilter = this.GetComponent<AudioChorusFilter>();
            }

            return chorusFilter;
        }
    }

    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity Echo Filter FX component.
    /// </summary>
    public AudioEchoFilter EchoFilter
    {
        get
        {
            if (echoFilter == null)
            {
                echoFilter = this.GetComponent<AudioEchoFilter>();
            }

            return echoFilter;
        }
    }

    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity Low Pass Filter FX component.
    /// </summary>
    public AudioLowPassFilter LowPassFilter
    {
        get
        {
            if (lpFilter == null)
            {
                lpFilter = this.GetComponent<AudioLowPassFilter>();
            }

            return lpFilter;
        }
    }

    /// <summary>
    /// This property returns you a lazy-loaded reference to the Unity High Pass Filter FX component.
    /// </summary>
    public AudioHighPassFilter HighPassFilter
    {
        get
        {
            if (hpFilter == null)
            {
                hpFilter = this.GetComponent<AudioHighPassFilter>();
            }

            return hpFilter;
        }
    }
	
	public bool HasActiveFXFilter {
		get {
			if (HighPassFilter != null && HighPassFilter.enabled) {
				return true;
			}
			if (LowPassFilter != null && LowPassFilter.enabled) {
				return true;
			}
			if (ReverbFilter != null && ReverbFilter.enabled) {
				return true;
			}
			if (DistortionFilter != null && DistortionFilter.enabled) {
				return true;
			}
			if (EchoFilter != null && EchoFilter.enabled) {
				return true;
			}
			if (ChorusFilter != null && ChorusFilter.enabled) {
				return true;
			}
			
			return false;
		}
	}
}
