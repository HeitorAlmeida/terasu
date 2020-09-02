using System.Collections;
using UnityEngine;

public class MusicController
{
	public const float LOWERED_VOLUME = 0.13f;

	private static MusicController instance;

	private AudioSource audioSource;
	private float fadeInSpeed;
	private float fadeOutSpeed;
	private bool started;
	private bool finished;
	private float currentNoFadeDuration;
	private float currentNoFadeElapsed;
	private float nextNoFadeDuration;
	private bool endLoop;
	private float currentVolume;
	private float targetVolume;
	private float maxVolume;
	private bool changeClip;
	private AudioClip currentClip;
	private AudioClip nextClip;

	public static MusicController Instance
	{
		get
		{
			if( instance == null )
			{
				instance = new MusicController();
			}
			return instance;
		}
	}

	private MusicController()
	{
		audioSource = null;
		Reset();
	}

	private void Reset()
	{
		if( audioSource != null )
		{
			if( audioSource.isPlaying )
			{
				audioSource.Stop ();
			}
			audioSource.clip = null;
		}
		fadeInSpeed = 0f;
		fadeOutSpeed = 0f;
		started = false;
		finished = false;
		currentNoFadeDuration = 0f;
		currentNoFadeElapsed = 0f;
		nextNoFadeDuration = 0f;
		endLoop = false;
		currentVolume = 0f;
		maxVolume = 1f;
		targetVolume = maxVolume;
		changeClip = false;
		currentClip = null;
		nextClip = null;
	}

	public void Configure( AudioSource newAudioSource, float speedForFadeIn, float speedForFadeOut, float newMaxVolume )
	{
		if( !started || finished )
		{
			if( newAudioSource != null )
			{
				Reset();
				audioSource = newAudioSource;
				fadeInSpeed = speedForFadeIn;
				if( fadeInSpeed < 0.0f )
				{
					fadeInSpeed *= -1;
				}
				fadeOutSpeed = speedForFadeOut;
				if( fadeOutSpeed < 0.0f )
				{
					fadeOutSpeed *= -1;
				}
				maxVolume = newMaxVolume;
				if( maxVolume < 0f )
				{
					maxVolume = 0f;
				}
				if( targetVolume > maxVolume )
				{
					targetVolume = maxVolume;
				}
			}
		}
	}

	public void Play( AudioClip newAudioClip, float durationWithoutFade )
	{
		if( audioSource != null )
		{
			if( started && !finished )
			{
				/*halmeida - cannot play immediately. We request the current song to stop and wait for it.*/
				targetVolume = 0f;
				nextClip = newAudioClip;
				nextNoFadeDuration = durationWithoutFade;
				if( nextNoFadeDuration < 0f )
				{
					nextNoFadeDuration = 0f;
				}
				changeClip = true;
			}
			else
			{
				currentClip = newAudioClip;
				currentNoFadeDuration = durationWithoutFade;
				if( currentNoFadeDuration < 0f )
				{
					currentNoFadeDuration = 0f;
				}
				currentNoFadeElapsed = 0f;
				currentVolume = 0f;
				targetVolume = maxVolume;
				audioSource.clip = currentClip;
				audioSource.volume = currentVolume;
				if( currentClip != null )
				{
					audioSource.Play();
					started = true;
					finished = false;
				}
			}
		}
	}

	public void Play()
	{
		if( (audioSource != null) && (currentClip != null) && !started && !finished )
		{
			audioSource.Play();
			started = true;
		}
	}

	public void Pause()
	{
		if( (audioSource != null) && (currentClip != null) && started && !finished )
		{
			audioSource.Pause ();
			started = false;
		}
	}
	
	public void StopLoop( bool immediately )
	{
		if( started && !finished && (currentNoFadeDuration == 0.0f) )
		{
			if( immediately )
			{
				if( audioSource != null )
				{
					audioSource.Stop ();
				}
				finished = true;
				endLoop = false;
			}
			else
			{
				targetVolume = 0.0f;
				endLoop = true;
			}
		}
	}

	public void Progress( float timeStep )
	{
		float previousVolume = 0.0f;
	
		if( (audioSource != null) && started && !finished )
		{
			previousVolume = audioSource.volume;
			if( previousVolume < targetVolume )
			{
				currentVolume = previousVolume + (fadeInSpeed * timeStep);
				if( (currentVolume == previousVolume) || (currentVolume > targetVolume) )
				{
					currentVolume = targetVolume;
				}
				audioSource.volume = currentVolume;
			}
			else if( previousVolume > targetVolume )
			{
				currentVolume = previousVolume - (fadeOutSpeed * timeStep);
				if( (currentVolume == previousVolume) || (currentVolume < targetVolume) )
				{
					currentVolume = targetVolume;
				}
				audioSource.volume = currentVolume;
			}
			else
			{
				if( (currentVolume == 0) && changeClip )
				{
					audioSource.Stop();
					audioSource.clip = null;
					currentClip = nextClip;
					currentNoFadeDuration = nextNoFadeDuration;
					currentNoFadeElapsed = 0f;
					targetVolume = maxVolume;
					audioSource.clip = currentClip;
					if( currentClip != null )
					{
						audioSource.Play();
					}
					else
					{
						finished = true;
					}
					changeClip = false;
				}
				else
				{
					if( currentNoFadeDuration > 0.0f )
					{
						if( audioSource.isPlaying )
						{
							if( currentNoFadeElapsed < currentNoFadeDuration )
							{
								currentNoFadeElapsed += timeStep;
								if( currentNoFadeElapsed >= currentNoFadeDuration )
								{
									targetVolume = 0.0f;
								}
							}
							else
							{
								audioSource.Stop ();
								finished = true;
							}
						}
						else
						{
							finished = true;
						}
					}
					else
					{
						if( endLoop )
						{
							audioSource.Stop ();
							finished = true;
							endLoop = false;
						}
						else
						{
							/*halmeida - this is the loop implementation itself. When the song ends, the audioSource
							stops playing. I ask it to play again so that the music repeats endlessly.*/
							if( !audioSource.isPlaying )
							{
								audioSource.Play ();
							}
						}
					}
				}
			}
		}
	}
	
	public void SetTargetVolume( float newTargetVolume )
	{
		if( started && !finished && !endLoop )
		{
			targetVolume = newTargetVolume;
			if( targetVolume < 0.0f )
			{
				targetVolume = 0.0f;
			}
			if( targetVolume > maxVolume )
			{
				targetVolume = maxVolume;
			}
		}
	}

	public bool MusicAlreadyPlaying( AudioClip songClip )
	{
		return ( (currentClip == songClip) && started && !finished );
	}

	public AudioClip GetCurrentMusic()
	{
		return currentClip;
	}

	public float GetCurrentVolume()
	{
		return currentVolume;
	}

	public bool AtMaxVolume()
	{
		return (currentVolume == maxVolume);
	}

	public bool CurrentlyStarted()
	{
		return started;
	}

	public bool CurrentlyFinished()
	{
		return finished;
	}
}
