using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Dark Tonic/Master Audio/EventCalcSounds")]
public class EventCalcSounds : MonoBehaviour {
	public const int FRAMES_EARLY_TO_TRIGGER = 2;
	
	public MasterAudio.SoundSpawnLocationMode soundSpawnMode = MasterAudio.SoundSpawnLocationMode.CallerLocation;
	public bool disableSounds = false;
	
	public AudioEvent audioSourceEndedSound;
	
	public bool useAudioSourceEndedSound = false;
	
	private AudioSource _audio;
	private Transform trans;
	
	void Awake() {
		this.trans = this.transform;
		this._audio = this.audio;
		this.SpawnedOrAwake();
	}
	
	protected virtual void SpawnedOrAwake() {
	}
	
	protected virtual void _AudioSourceEnded() {
		PlaySound(audioSourceEndedSound);
	}
	
	private void PlaySound(AudioEvent aEvent) {
		if (disableSounds) {
			return;
		}
		
		float volume = aEvent.volume;
		string sType = aEvent.soundType;
		float? pitch = aEvent.pitch;
		if (!aEvent.useFixedPitch) {
			pitch = null;
		}
		
		PlaySoundResult soundPlayed = null;
		
		switch (soundSpawnMode) {
			case MasterAudio.SoundSpawnLocationMode.CallerLocation:
				soundPlayed = MasterAudio.PlaySound3DAtTransform(sType, this.trans, volume, pitch);
				break;
			case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
				soundPlayed = MasterAudio.PlaySound3DFollowTransform(sType, this.trans, volume, pitch);
				break;
			case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
				soundPlayed = MasterAudio.PlaySound(sType, volume);
				break;
		}

		if (soundPlayed == null || !soundPlayed.SoundPlayed) {
			return;
		}
		
		if (aEvent.emitParticles) {
			MasterAudio.TriggerParticleEmission(this.trans, aEvent.particleCountToEmit);
		}
	}
	
	void Update() {
		CheckForEvents();
	}
	
	private void CheckForEvents() {
		if (this.useAudioSourceEndedSound && this._audio != null && this._audio.clip != null) {
			if (this._audio.clip.length - this._audio.time < Time.deltaTime * FRAMES_EARLY_TO_TRIGGER) {
				// just looped
				this._audio.Stop();
				if (this._audio.loop) {
					this._audio.Play();
				}
				this._AudioSourceEnded();
			}
		}
	}
}
