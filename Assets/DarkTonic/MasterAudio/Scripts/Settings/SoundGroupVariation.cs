using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This class contains the actual Audio Source, Unity Filter FX components and other convenience methods having to do with playing sound effects.
/// </summary>
public class SoundGroupVariation : MonoBehaviour
{
    public Texture logoTexture;
    public int weight = 1;
    public float randomPitch = 0f;
    public float randomVolume = 0f;
    public MasterAudio.AudioLocation audLocation = MasterAudio.AudioLocation.Clip;
    public string resourceFileName;
    public float fxTailTime = 0f;
    public bool useFades = false;
    public float fadeInTime = 0f;
    public float fadeOutTime = 0f;
    public float original_Pitch;
    public bool useIntroSilence = false;
    public float introSilenceMin = 0f;
    public float introSilenceMax = 0f;

    private AudioSource _audio;
    private float fadeMaxVolume;
    private FadeMode curFadeMode = FadeMode.None;
    private DetectEndMode curDetectEndMode = DetectEndMode.None;
    private PlaySoundParams playSndParams = null;


    private AudioDistortionFilter distFilter;
    private AudioEchoFilter echoFilter;
    private AudioHighPassFilter hpFilter;
    private AudioLowPassFilter lpFilter;
    private AudioReverbFilter reverbFilter;
    private AudioChorusFilter chorusFilter;
    private bool isWaitingForDelay = false;

    public delegate void SoundFinishedEventHandler();

    /// <summary>
    /// Subscribe to this event to be notified when the sound stops playing.
    /// </summary>
    public event SoundFinishedEventHandler SoundFinished;

    private Transform trans;
    private Transform objectToFollow = null;
	private Transform objectToTriggerFrom = null;
	private MasterAudioGroup parentGroupScript;
    private int timesLocationUpdated = 0;
    private bool attachToSource = false;
    private float lastTimePlayed = 0f;

    public class PlaySoundParams
    {
        public string soundType;
        public float volumePercentage;
        public float? pitch;
        public Transform sourceTrans;
        public bool attachToSource;
        public float delaySoundTime;
        public bool isChainLoop;
        public float groupCalcVolume;
		
        public PlaySoundParams(string _soundType, float _volPercent, float _groupCalcVolume, float? _pitch, Transform _sourceTrans, bool _attach, float _delaySoundTime, bool _isChainLoop)
        {
            soundType = _soundType;
            volumePercentage = _volPercent;
            groupCalcVolume = _groupCalcVolume;
            pitch = _pitch;
            sourceTrans = _sourceTrans;
            attachToSource = _attach;
            delaySoundTime = _delaySoundTime;
            isChainLoop = _isChainLoop;
        }
    }

    public enum FadeMode
    {
        None,
        FadeInOut,
        FadeOutEarly,
        GradualFade
    }

    public enum DetectEndMode
    {
        None,
        DetectEnd
    }

    void Awake()
    {
        _audio = this.audio;
        this.trans = this.transform;
    }

    void Start()
    {
        // this code needs to wait for cloning (for weight).
        var theParent = this.trans.parent;
        if (theParent == null)
        {
            Debug.LogError("Sound Variation '" + this.name + "' has no parent!");
            return;
        }
    }

    void OnDestroy()
    {
        StopSoundEarly();
    }

    void OnDisable()
    {
        StopSoundEarly();
    }

    private void StopSoundEarly()
    {
        if (MasterAudio.AppIsShuttingDown)
        {
            return;
        }

        Stop(false); // maybe unload clip from Resources
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, MasterAudio.GIZMO_FILE_NAME, true);
    }

    /// <summary>
    /// This method is called automatically from MasterAudio.PlaySound and MasterAudio.PlaySound3D. 
    /// </summary>
    /// <param name="maxVolume">If fade in time is not zero on this Variation, max volume is the fully faded in clip's target volume. Otherwise this is not used.</param>
    public void Play(float? pitch, float maxVolume, PlaySoundParams playParams = null)
    {
        SoundFinished = null; // clear it out so subscribers don't have to clean up
        isWaitingForDelay = false;
		playSndParams = playParams;

        // compute pitch
        if (pitch.HasValue)
        {
            _audio.pitch = pitch.Value;
        }
        else
        {
            var randPitch = 0f;

            if (randomPitch != 0f)
            {
                randPitch = UnityEngine.Random.Range(-randomPitch, randomPitch);
            }
            _audio.pitch = OriginalPitch + randPitch;
        }

        // set fade mode
        this.curFadeMode = FadeMode.None;
        curDetectEndMode = DetectEndMode.DetectEnd;

        StopAllCoroutines();
		
        if (audLocation == MasterAudio.AudioLocation.Clip) {
			FinishSetupToPlay(maxVolume);
			return;
		}

		StartCoroutine(LoadResourceFileAsync(resourceFileName, maxVolume));
    }
	
	private IEnumerator LoadResourceFileAsync(string resourceFileName, float maxVolume) {
		yield return new WaitForSeconds(Time.deltaTime); // this lets the thread continue without blocking; For some reason CoRoutine alone isn't enough.
		
		AudioResourceOptimizer.PopulateSourcesWithResourceClip(resourceFileName);
		FinishSetupToPlay(maxVolume);
		yield break;
	}
	
	private void FinishSetupToPlay(float maxVolume) {
		//Debug.Log("finishing setup");
		timesLocationUpdated = 0;

        if (!_audio.isPlaying && _audio.time > 0f)
        {
            // paused. Do nothing except Play
        }
        else if (useFades && (fadeInTime > 0f || fadeOutTime > 0f))
        {
            fadeMaxVolume = maxVolume;
            _audio.volume = 0f;
            StartCoroutine(FadeInOut());
        }

        if (playSndParams != null && playSndParams.isChainLoop)
        {
            _audio.loop = false;
        }

        ParentGroup.AddActiveAudioSourceId(this);

        StartCoroutine(DetectSoundFinished(playSndParams.delaySoundTime));

        attachToSource = false;

        bool useClipAgePriority = MasterAudio.Instance.prioritizeOnDistance && (MasterAudio.Instance.useClipAgePriority || ParentGroup.useClipAgePriority);
        if (playSndParams.attachToSource || useClipAgePriority)
        {
            attachToSource = playSndParams.attachToSource;
	        if (ObjectToFollow != null && ObjectToFollow.root.GetInstanceID() == MasterAudio.ListenerID)
			{
				AudioUpdater updater = gameObject.AddComponent<AudioUpdater>();
				updater.FollowTransform = ObjectToFollow;
				attachToSource = false;	// Do not modify playParams as the sound group may contain more variations
				ObjectToFollow = null;
			}
            StartCoroutine(FollowSoundMaker());
        }
	}
	
    /// <summary>
    /// This method allows you to jump to a specific time in an already playing or just triggered Audio Clip.
    /// </summary>
    /// <param name="timeToJumpTo">The time in seconds to jump to.</param>
    public void JumpToTime(float timeToJumpTo)
    {
        if (!audio.isPlaying || playSndParams == null)
        {
            return;
        }

        audio.time = timeToJumpTo;
    }

    /// <summary>
    /// This method allows you to adjust the volume of an already playing clip, accounting for bus volume, mixer volume and group volume.
    /// </summary>
    /// <param name="volumePercentage"></param>
    public void AdjustVolume(float volumePercentage)
    {
        if (!audio.isPlaying || playSndParams == null)
        {
            return;
        }

        var newVol = playSndParams.groupCalcVolume * volumePercentage;
        _audio.volume = newVol;

        playSndParams.volumePercentage = volumePercentage;
    }

    /// <summary>
    /// This method allows you to pause the audio being played by this Variation. This is automatically called by MasterAudio.PauseSoundGroup and MasterAudio.PauseBus.
    /// </summary>
    public void Pause()
    {
        if (audLocation == MasterAudio.AudioLocation.ResourceFile && !MasterAudio.Instance.resourceClipsPauseDoNotUnload)
        {
            Stop();
            return;
        }

        _audio.Pause();
        this.curFadeMode = FadeMode.None;

        MaybeUnloadClip();
    }

    private void MaybeUnloadClip()
    {
        if (audLocation == MasterAudio.AudioLocation.ResourceFile)
        {
            AudioResourceOptimizer.UnloadClipIfUnused(resourceFileName);
        }
    }

    /// <summary>
    /// This method allows you to stop the audio being played by this Variation. 
    /// </summary>
    public void Stop(bool stopEndDetection = false)
    {
        if (stopEndDetection || isWaitingForDelay)
        {
            curDetectEndMode = DetectEndMode.None; // turn off the chain loop endless repeat
        }
		
		objectToFollow = null;	
        objectToTriggerFrom = null;
		ParentGroup.RemoveActiveAudioSourceId(this);

        _audio.Stop();

        playSndParams = null;

        if (SoundFinished != null)
        {
            SoundFinished(); // parameters aren't used
        }

        MaybeUnloadClip();
    }

    /// <summary>
    /// This method allows you to fade the sound from this Variation to a specified volume over X seconds.
    /// </summary>
    /// <param name="newVolume">The target volume to fade to.</param>
    /// <param name="fadeTime">The time it will take to fully fade to the target volume.</param>
    public void FadeToVolume(float newVolume, float fadeTime)
    {
        this.curFadeMode = FadeMode.GradualFade;

        StartCoroutine(FadeOverTimeToVolume(newVolume, fadeTime));
    }

    private IEnumerator FadeOverTimeToVolume(float targetVolume, float fadeTime)
    {
        if (fadeTime <= MasterAudio.INNER_LOOP_CHECK_INTERVAL)
        {
            _audio.volume = targetVolume; // time really short, just do it at once.
            if (_audio.volume <= 0f)
            {
                Stop();
            }
            curFadeMode = FadeMode.None;
            yield break;
        }

        yield return MasterAudio.loopDelay; // wait for the clip to start playing :)

        var volStep = (targetVolume - _audio.volume) / (fadeTime / MasterAudio.INNER_LOOP_CHECK_INTERVAL);
        float newVol;
        while (_audio.volume != targetVolume && curFadeMode == FadeMode.GradualFade)
        {
            newVol = _audio.volume + volStep;

            if (volStep > 0f)
            {
                newVol = Math.Min(newVol, targetVolume);
            }
            else
            {
                newVol = Math.Max(newVol, targetVolume);
            }

            _audio.volume = newVol;

            yield return MasterAudio.loopDelay; // wait for the clip to start playing :)
        }

        if (_audio.volume <= 0f)
        {
            Stop();
        }

        if (curFadeMode != FadeMode.GradualFade)
        {
            yield break; // in case another fade cancelled this one
        }

        curFadeMode = FadeMode.None;
    }

    /// <summary>
    /// This method will fully fade out the sound from this Variation to zero using its existing fadeOutTime.
    /// </summary>
    public void FadeOutNow()
    {
        if (MasterAudio.AppIsShuttingDown)
        {
            return;
        }
        StartCoroutine(FadeOutEarly(fadeOutTime));
    }

    /// <summary>
    /// This method will fully fade out the sound from this Variation to zero using over X seconds.
    /// </summary>
    /// <param name="fadeTime">The time it will take to fully fade to the target volume.</param>
    public void FadeOutNow(float fadeTime)
    {
        if (MasterAudio.AppIsShuttingDown)
        {
            return;
        }
        StartCoroutine(FadeOutEarly(fadeTime));
    }

    private IEnumerator FadeOutEarly(float fadeTime)
    {
        curFadeMode = FadeMode.FadeOutEarly; // cancel the FadeInOut loop, if it's going.

        var stepVolumeDown = _audio.volume / fadeTime * MasterAudio.INNER_LOOP_CHECK_INTERVAL;

        while (_audio.isPlaying && curFadeMode == FadeMode.FadeOutEarly && _audio.volume > 0)
        {
            _audio.volume -= stepVolumeDown;
            yield return MasterAudio.loopDelay;
        }

        _audio.volume = 0f;
        Stop();

        if (curFadeMode != FadeMode.FadeOutEarly)
        {
            yield break; // in case another fade cancelled this one
        }

        curFadeMode = FadeMode.None;
    }

    private IEnumerator FadeInOut()
    {
        var fadeOutStartTime = _audio.clip.length - (fadeOutTime * _audio.pitch);

        yield return MasterAudio.loopDelay; // wait for the clip to start playing :)

        var stepVolumeUp = fadeMaxVolume / fadeInTime * MasterAudio.INNER_LOOP_CHECK_INTERVAL;

        curFadeMode = FadeMode.FadeInOut; // wait to set this so it stops the previous one if it's still going.

        if (fadeInTime > 0f)
        {
            while (_audio.isPlaying && curFadeMode == FadeMode.FadeInOut && _audio.time < fadeInTime)
            {
                _audio.volume += stepVolumeUp;
                yield return MasterAudio.loopDelay;
            }
        }

        if (curFadeMode != FadeMode.FadeInOut)
        {
            yield break; // in case another fade cancelled this one
        }

        _audio.volume = fadeMaxVolume; // just in case it didn't align exactly

        if (fadeOutTime == 0f || _audio.loop)
        {
            yield break; // nothing more to do!
        }

        // wait for fade out time.
        while (_audio.isPlaying && curFadeMode == FadeMode.FadeInOut && _audio.time < fadeOutStartTime)
        {
            yield return MasterAudio.loopDelay;
        }

        if (curFadeMode != FadeMode.FadeInOut)
        {
            yield break; // in case another fade cancelled this one
        }

        var stepVolumeDown = fadeMaxVolume / fadeOutTime * MasterAudio.INNER_LOOP_CHECK_INTERVAL;

        while (_audio.isPlaying && curFadeMode == FadeMode.FadeInOut && _audio.volume > 0)
        {
            _audio.volume -= stepVolumeDown;
            yield return MasterAudio.loopDelay;
        }

        audio.volume = 0f;
        Stop();

        if (curFadeMode != FadeMode.FadeInOut)
        {
            yield break; // in case another fade cancelled this one
        }

        curFadeMode = FadeMode.None;
    }

    private void UpdateAudioLocationAndPriority(bool rePrioritize, bool updateLocation)
    {
        // update location
        if (updateLocation && objectToFollow != null)
        {
            this.trans.position = objectToFollow.position;
        }

        // re-set priority
        if (!MasterAudio.Instance.prioritizeOnDistance || !rePrioritize)
        {
            return;
        }

        timesLocationUpdated++;

        if (timesLocationUpdated > MasterAudio.Instance.rePrioritizeEverySecIndex)
        {
            AudioPrioritizer.Set3dPriority(_audio);
            timesLocationUpdated = 0;
        }
    }

    private IEnumerator FollowSoundMaker()
    {
        UpdateAudioLocationAndPriority(false, attachToSource);

        while (curDetectEndMode == DetectEndMode.DetectEnd)
        {
			var timeToWait = MasterAudio.INNER_LOOP_CHECK_INTERVAL;
			yield return new WaitForSeconds(timeToWait);

            UpdateAudioLocationAndPriority(true, attachToSource);
        }
    }

    private IEnumerator DetectSoundFinished(float delaySound)
    {
        if (useIntroSilence && introSilenceMax > 0f)
        {
            var rndSilence = UnityEngine.Random.Range(introSilenceMin, introSilenceMax);
            isWaitingForDelay = true;
            yield return new WaitForSeconds(rndSilence);
            isWaitingForDelay = false;
        }

        if (delaySound > 0f)
        {
            isWaitingForDelay = true;
            yield return new WaitForSeconds(delaySound);
            isWaitingForDelay = false;
        }

        if (curDetectEndMode != DetectEndMode.DetectEnd)
        {
            yield break;
        }

        _audio.Play();
        lastTimePlayed = Time.time;
		
        // sound play worked! Duck music if a ducking sound.
		MasterAudio.DuckSoundGroup(ParentGroup.name, _audio);
		
        var clipLength = _audio.clip.length / _audio.pitch;
        yield return new WaitForSeconds(clipLength); // wait for clip to play

        if (HasActiveFXFilter && fxTailTime > 0f)
        {
            yield return new WaitForSeconds(fxTailTime);
        }

        var playSnd = playSndParams;

        if (!_audio.loop || (playSndParams != null && playSndParams.isChainLoop))
        {
            Stop();
        }

        if (curDetectEndMode != DetectEndMode.DetectEnd)
        {
            yield break;
        }

        if (playSnd != null && playSnd.isChainLoop)
        {
			// check if loop count is over.
			if (ParentGroup.chainLoopMode == MasterAudioGroup.ChainedLoopLoopMode.NumberOfLoops && ParentGroup.ChainLoopCount >= ParentGroup.chainLoopNumLoops) {
				// done looping
				yield break;
			}
			
            var rndDelay = playSnd.delaySoundTime;
            if (ParentGroup.chainLoopDelayMin > 0f || ParentGroup.chainLoopDelayMax > 0f)
            {
                rndDelay = UnityEngine.Random.Range(ParentGroup.chainLoopDelayMin, ParentGroup.chainLoopDelayMax);
            }

            // cannot use "AndForget" methods! Chain loop needs to check the status.
            if (playSnd.attachToSource || playSnd.sourceTrans != null)
            {
                if (playSnd.attachToSource)
                {
                    MasterAudio.PlaySound3DFollowTransform(playSnd.soundType, playSnd.sourceTrans, playSnd.volumePercentage, playSnd.pitch, rndDelay, null, true);
                }
                else
                {
                    MasterAudio.PlaySound3DAtTransform(playSnd.soundType, playSnd.sourceTrans, playSnd.volumePercentage, playSnd.pitch, rndDelay, null, true);
                }
            }
            else
            {
                MasterAudio.PlaySound(playSnd.soundType, playSnd.volumePercentage, playSnd.pitch, rndDelay, null, true);
            }
        }
    }
	
	public bool WasTriggeredFromTransform(Transform trans) {
		return ObjectToFollow == trans || ObjectToTriggerFrom == trans;
	}
	
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
	
    public Transform ObjectToFollow
    {
        get
        {
            return objectToFollow;
        }
        set
        {
            objectToFollow = value;
        }
    }
	
	public Transform ObjectToTriggerFrom {
		get {
			return objectToTriggerFrom;
		}
		set {
			objectToTriggerFrom = value;
		}
	}
			
    public bool HasActiveFXFilter
    {
        get
        {
            if (HighPassFilter != null && HighPassFilter.enabled)
            {
                return true;
            }
            if (LowPassFilter != null && LowPassFilter.enabled)
            {
                return true;
            }
            if (ReverbFilter != null && ReverbFilter.enabled)
            {
                return true;
            }
            if (DistortionFilter != null && DistortionFilter.enabled)
            {
                return true;
            }
            if (EchoFilter != null && EchoFilter.enabled)
            {
                return true;
            }
            if (ChorusFilter != null && ChorusFilter.enabled)
            {
                return true;
            }

            return false;
        }
    }

    public MasterAudioGroup ParentGroup
    {
        get
        {
            if (this.parentGroupScript == null)
            {
                this.parentGroupScript = this.trans.parent.GetComponent<MasterAudioGroup>();
            }

            if (this.parentGroupScript == null)
            {
                Debug.LogError("The Group that Sound Variation '" + this.name + "' is in does not have a MasterAudioGroup script in it!");
            }

            return this.parentGroupScript;
        }
    }

    public float OriginalPitch
    {
        get
        {
            if (this.original_Pitch == 0f)
            {
                original_Pitch = _audio.pitch;
            }

            return this.original_Pitch;
        }
    }

    public bool IsAvailableToPlay
    {
        get
        {
            if (weight == 0)
            {
                return false;
            }

            if (!_audio.isPlaying)
            {
                return true;
            }

            return AudioUtil.GetAudioPlayedPercentage(_audio) >= ParentGroup.retriggerPercentage;
        }
    }

    public float LastTimePlayed
    {
        get
        {
            return lastTimePlayed;
        }
    }
}
