using System.Collections;
using UnityEngine;

public class VFXParticleController : MonoBehaviour
{
	public const float DEFAULT_PARTICLE_OPAQUE_DURATION = 0.3f;
	public const float DEFAULT_PARTICLE_ALPHA_SPEED = 0.3f;

	public GameObject[] particleModels;
	public float particleCreationWindow;
	public float particleOpaqueAlpha;
	public float particleOpaqueDuration;
	public float particleAlphaSpeed;
	public bool particleCreatedOpaque;
	public Vector2 particleMinSpeed;
	public Vector2 particleMaxSpeed;
	public Vector2 particleAcceleration;
	public float particleMinSpin;
	public float particleMaxSpin;
	public bool fixedFocus;
	public Vector2[] fixedFocusOffsets;
	public bool createBetweenRadiuses;
	public float focusMaximumRadius;
	public float focusMinimumRadius;
	public bool createWithinRectangle;
	public float focusRectangleWidth;
	public float focusRectangleHeight;
	public int creationsPerFocus;

	private BaseAnimator focusProvider;
	private float windowDuration;
	private float windowElapsed;
	private float windowTriggerOne;
	private float windowTriggerTwo;
	private bool particleCreatedOne;
	private bool particleCreatedTwo;
	private float focusMaxRadius;
	private float focusMinRadius;
	private float focusMidRadius;
	private float focusRadiusVariation;
	private float focusRadiusVariationHalf;
	private float focusRectWidth;
	private float focusRectWidthHalf;
	private float focusRectHeight;
	private float focusRectHeightHalf;
	private GameObject[] particleObjects;
	private SimpleAnimator[] particleAnimators;
	private float[] particleOpaqueDurations;
	private float[] particleOpaqueElapsed;
	private float[] particleAlphaSpeeds;
	private Vector2[] particleSpeeds;
	private float[] particleSpins;
	private float particleDepth;
	private int particlesPerFocus;
	private float baseOpaqueAlpha;
	private float baseOpaqueDuration;
	private float baseAlphaSpeed;
	private Vector2 baseMinSpeed;
	private Vector2 baseMaxSpeed;
	private float baseMinSpin;
	private float baseMaxSpin;

	void Awake()
	{
		float temp = 0f;

		focusProvider = null;
		SetWindowDuration( particleCreationWindow );
		ResetParticleCreation();
		SetFocusOffsetRadiusRange( focusMaximumRadius, focusMinimumRadius );
		SetFocusOffsetRectangle( focusRectangleWidth, focusRectangleHeight );
		particleObjects = null;
		particleAnimators = null;
		particleOpaqueDurations = null;
		particleOpaqueElapsed = null;
		particleAlphaSpeeds = null;
		particleSpeeds = null;
		particleSpins = null;
		particleDepth = DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / BaseAnimator.MAX_ANIMATORS_WITHIN_LAYER;
		creationsPerFocus = (creationsPerFocus < 0) ? 0 : creationsPerFocus;
		particlesPerFocus = 0;
		baseOpaqueAlpha = (particleOpaqueAlpha > 1f) ? 1f : particleOpaqueAlpha;
		baseOpaqueAlpha = (baseOpaqueAlpha < 0f) ? 0f : baseOpaqueAlpha;
		baseOpaqueDuration = (particleOpaqueDuration > 0f) ? particleOpaqueDuration : DEFAULT_PARTICLE_OPAQUE_DURATION;
		baseAlphaSpeed = (particleAlphaSpeed > 0f) ? particleAlphaSpeed : DEFAULT_PARTICLE_ALPHA_SPEED;
		baseMinSpeed = particleMinSpeed;
		baseMaxSpeed = particleMaxSpeed;
		if( baseMinSpeed.x > baseMaxSpeed.x )
		{
			temp = baseMinSpeed.x;
			baseMinSpeed.x = baseMaxSpeed.x;
			baseMaxSpeed.x = temp;
		}
		if( baseMinSpeed.y > baseMaxSpeed.y )
		{
			temp = baseMinSpeed.y;
			baseMinSpeed.y = baseMaxSpeed.y;
			baseMaxSpeed.y = temp;
		}
		baseMinSpin = particleMinSpin;
		baseMaxSpin = particleMaxSpin;
		if( baseMinSpin > baseMaxSpin )
		{
			temp = baseMinSpin;
			baseMinSpin = baseMaxSpin;
			baseMinSpin = temp;
		}
	}

	public void SetWindowDuration( float newWindowDuration )
	{
		windowDuration = (newWindowDuration < 0f) ? 0f : newWindowDuration;
	}

	public float GetWindowDuration()
	{
		return windowDuration;
	}

	private void ResetParticleCreation()
	{
		windowElapsed = 0f;
		windowTriggerOne = Random.Range( 0f, windowDuration );
		windowTriggerTwo = Random.Range( 0f, windowDuration );
		particleCreatedOne = false;
		particleCreatedTwo = false;
	}

	public void SetFocusOffsetRadiusRange( float newMaxRadius, float newMinRadius )
	{
		focusMaxRadius = (newMaxRadius < 0f) ? -newMaxRadius : newMaxRadius;
		focusMinRadius = (newMinRadius < 0f) ? -newMinRadius : newMinRadius;
		focusMaxRadius = (focusMaxRadius < focusMinRadius) ? focusMinRadius : focusMaxRadius;
		focusRadiusVariation = focusMaxRadius - focusMinRadius;
		focusRadiusVariationHalf = focusRadiusVariation / 2f;
		focusMidRadius = focusMinRadius + focusRadiusVariationHalf;
	}

	public void SetFocusOffsetRectangle( float newRectWidth, float newRectHeight )
	{
		focusRectWidth = (focusRectangleWidth < 0f) ? 0f : focusRectangleWidth;
		focusRectWidthHalf = focusRectWidth / 2f;
		focusRectHeight = (focusRectangleHeight < 0f) ? 0f : focusRectangleHeight;
		focusRectHeightHalf = focusRectHeight / 2f;
	}

	public float GetFocusMaximumRadius()
	{
		return focusMaxRadius;
	}

	public float GetFocusMinimumRadius()
	{
		return focusMinRadius;
	}

	public void SetFocusProvider( BaseAnimator newFocusProvider )
	{
		focusProvider = newFocusProvider;
	}

	public bool CreatesParticlesAutomatically()
	{
		return !fixedFocus;
	}

	public bool RequestParticleCreation( int amountPerFocus = 0 )
	{
		if( fixedFocus )
		{
			if( amountPerFocus == 0 )
			{
				amountPerFocus = creationsPerFocus;
			}
			particlesPerFocus += amountPerFocus;
			particlesPerFocus = (particlesPerFocus < 0) ? 0 : particlesPerFocus;
			return true;
		}
		return false;
	}

	public void Progress( float timeStep )
	{
		ProgressParticles( timeStep );
		ProgressParticleCreation( timeStep );
	}

	private void ProgressParticles( float timeStep )
	{
		float opaqueDuration = 0f;
		float opaqueElapsed = 0f;
		float alphaSpeed = 0f;
		GameObject particleObject = null;
		SimpleAnimator particleAnimator = null;
		bool particleRemoved = false;
		Vector2 particleSpeed = Vector2.zero;
		Vector2 speedVariation = Vector2.zero;
		Vector3 particlePosition = Vector3.zero;
		Vector3 rotationAngles = Vector3.zero;
		float rotationSpeed = 0f;
		float rotationVariation = 0f;

		if( particleAnimators != null )
		{
			speedVariation = particleAcceleration * timeStep;
			for( int i=0; i<particleAnimators.Length; i++ )
			{
				particleRemoved = false;
				particleAnimator = particleAnimators[i];
				if( particleAnimator != null )
				{
					particleAnimator.Progress( timeStep );
					/*halmeida - relying on the coherence of all the "particle" arrays.*/
					opaqueDuration = particleOpaqueDurations[i];
					opaqueElapsed = particleOpaqueElapsed[i];
					if( opaqueElapsed == 0f )
					{
						if( particleAnimator.IsOpaque() )
						{
							opaqueElapsed += timeStep;
							particleOpaqueElapsed[i] = opaqueElapsed;
						}
					}
					else
					{
						if( opaqueElapsed < opaqueDuration )
						{
							opaqueElapsed += timeStep;
							particleOpaqueElapsed[i] = opaqueElapsed;
							if( opaqueElapsed > opaqueDuration )
							{
								alphaSpeed = particleAlphaSpeeds[i];
								particleAnimator.StartAlphaFading( -alphaSpeed, false, true, true );
							}
						}
						else
						{
							if( particleAnimator.IsTransparent() )
							{
								RemoveParticle(i);
								if( particleAnimators == null )
								{
									break;
								}
								else
								{
									i--;
									particleRemoved = true;
								}
							}
						}
					}
					if( !particleRemoved && (particleAnimator.IsOver()) )
					{
						RemoveParticle(i);
						if( particleAnimators == null )
						{
							break;
						}
						else
						{
							i--;
							particleRemoved = true;
						}
					}
				}
				else
				{
					RemoveParticle(i);
					particleRemoved = true;
					if( particleAnimators == null )
					{
						break;
					}
					else
					{
						i--;
						particleRemoved = true;
					}
				}
				/*halmeida - if the particle wasn't removed, we update it's speed information and move it.*/
				if( !particleRemoved )
				{
					particleObject = particleObjects[i];
					particleSpeed = particleSpeeds[i];
					particleSpeed += speedVariation;
					particleSpeeds[i] = particleSpeed;
					particlePosition = particleObject.transform.localPosition;
					particlePosition += timeStep * new Vector3( particleSpeed.x, particleSpeed.y, 0f );
					particleObject.transform.localPosition = particlePosition;
					rotationSpeed = particleSpins[i];
					rotationVariation = rotationSpeed * timeStep;
					rotationAngles = particleObject.transform.eulerAngles;
					rotationAngles.z += rotationVariation;
					particleObject.transform.eulerAngles = rotationAngles;
				}
			}
		}
	}

	private void ProgressParticleCreation( float timeStep )
	{
		Vector2 currentFocus = Vector2.zero;
		bool focusValid = false;

		if( !fixedFocus )
		{
			if( focusProvider != null )
			{
				windowElapsed += timeStep;
				if( !particleCreatedOne && (windowElapsed > windowTriggerOne) )
				{
					currentFocus = focusProvider.GetCurrentFocusOffsetOne( ref focusValid );
					if( focusValid )
					{
						CreateParticle( currentFocus );
					}
					particleCreatedOne = true;
				}
				if( !particleCreatedTwo && (windowElapsed > windowTriggerTwo) )
				{
					currentFocus = focusProvider.GetCurrentFocusOffsetTwo( ref focusValid );
					if( focusValid )
					{
						CreateParticle( currentFocus );
					}
					particleCreatedTwo = true;
				}
				if( windowElapsed >= windowDuration )
				{
					ResetParticleCreation();
				}
			}
		}
		else
		{
			if( (particlesPerFocus > 0) && (fixedFocusOffsets != null) )
			{
				for( int i=0; i<fixedFocusOffsets.Length; i++ )
				{
					currentFocus = fixedFocusOffsets[i];
					for( int j=0; j<particlesPerFocus; j++ )
					{
						CreateParticle( currentFocus );
					}
				}
				particlesPerFocus = 0;
			}
		}
	}

	/*halmeida - The method below, as it is, does not yield a good vertical distribution of the particles.
	The upper and bottom sides of the square are rich in particles, while the left and right sides of
	the square are poor. This happens cause the y of the particles is only free when the x falls within
	a certain interval, and the sides are only populated with particles when the y is free. This means
	that the sides can only be populated in a restricted amount of cases, while the top and bottom
	can always be populated.*/ 
	private Vector2 GetOffsetFromFocusSquare()
	{
		Vector2 offsetFromFocus = Vector2.zero;
		bool limitY = false;

		/*halmeida - The x is freely chosen within the maxRadius, without respecting the minRadius.*/
		offsetFromFocus.x = (Random.Range( 0f, 2f ) - 1f) * focusMaxRadius;
		/*halmeida - If the x falls within the interval that the minRadius excludes, we limit the choices of y.*/
		limitY = ((offsetFromFocus.x > -focusMinRadius) && (offsetFromFocus.x < focusMinRadius));
		if( limitY )
		{
			offsetFromFocus.y = Random.Range( focusMinRadius, focusMaxRadius );
			offsetFromFocus.y *= (Random.Range( 0, 2 ) == 0) ? -1f : 1f;
		}
		else
		{
			offsetFromFocus.y = (Random.Range( 0f, 2f ) - 1f) * focusMaxRadius;
		}
		return offsetFromFocus;
	}

	private Vector2 GetOffsetFromFocusRhombus()
	{
		Vector2 offsetFromFocus = Vector2.zero;

		/*halmeida - The x is freely chosen within the midRadius, without respecting the minRadius.*/
		offsetFromFocus.x = (Random.Range( 0f, 2f ) - 1f) * focusMidRadius;
		/*halmeida - The y is always the complement towards the midRadius.*/
		if( offsetFromFocus.x < 0f )
		{
			offsetFromFocus.y = focusMidRadius + offsetFromFocus.x;
		}
		else
		{
			offsetFromFocus.y = focusMidRadius - offsetFromFocus.x;
		}
		offsetFromFocus.y *= (Random.Range( 0, 2 ) == 0) ? -1f : 1f;
		/*halmeida - at this point, the offset places a particle exactly at the midRadius rhombus, all in line.
		To make them occupy all the area between the minRadius and the maxRadius, deviating from the line, we add
		random values to x, ranging from -halfRadiusVariation to halfRadiusVariation. The top and bottom tips of
		the rombhus are never occupied.*/ 
		offsetFromFocus.x += (Random.Range( 0f, 2f ) - 1f) * focusRadiusVariationHalf;
		return offsetFromFocus;
	}

	private Vector2 GetOffsetFromFocusRectangle()
	{
		Vector2 offsetFromFocus = Vector2.zero;

		offsetFromFocus.x = (Random.Range( 0f, 2f ) - 1f) * focusRectWidthHalf;
		offsetFromFocus.y = (Random.Range( 0f, 2f ) - 1f) * focusRectHeightHalf;
		return offsetFromFocus;
	}

	private void CreateParticle( Vector2 offsetToFocus )
	{
		int modelIndex = -1;
		GameObject particleModel = null;
		GameObject particleObject = null;
		SimpleAnimator particleAnimator = null;
		Vector3 particlePosition = Vector3.zero;
		Vector2 offsetFromFocus = Vector2.zero;
		Vector2 particleSpeed = Vector2.zero;
		float particleSpin = 0f;
		float opaqueAlpha = 0f;
		float opaqueDuration = 0f;
		float alphaSpeed = 0f;

		if( particleModels != null )
		{
			modelIndex = Random.Range( 0, particleModels.Length );
			particleModel = particleModels[modelIndex];
			if( particleModel != null )
			{
				particlePosition = transform.position;
				particlePosition.x += offsetToFocus.x;
				particlePosition.y += offsetToFocus.y;
				if( createBetweenRadiuses )
				{
					offsetFromFocus = GetOffsetFromFocusRhombus();
				}
				else if( createWithinRectangle )
				{
					offsetFromFocus = GetOffsetFromFocusRectangle();
				}
				particlePosition.x += offsetFromFocus.x;
				particlePosition.y += offsetFromFocus.y;
				/*halmeida - reducing the depth of the particle to show it above the object generating it.*/
				particlePosition.z -= particleDepth;
				particleObject = Instantiate( particleModel, particlePosition, Quaternion.identity ) as GameObject;
				particleObject.transform.SetParent( transform, true );
				particleSpeed.x = Random.Range( baseMinSpeed.x, baseMaxSpeed.x );
				particleSpeed.y = Random.Range( baseMinSpeed.y, baseMaxSpeed.y );
				particleSpin = Random.Range( baseMinSpin, baseMaxSpin );
				particleAnimator = particleObject.GetComponent<SimpleAnimator>();
				if( particleAnimator != null )
				{
					opaqueAlpha = baseOpaqueAlpha;
					opaqueDuration = baseOpaqueDuration;
					alphaSpeed = baseAlphaSpeed;
					if( particleCreatedOpaque )
					{
						particleAnimator.SetOpaqueAlpha( opaqueAlpha );
						particleAnimator.StartAlphaFading( 1f, true, true, true );
					}
					else
					{
						particleAnimator.StartAlphaFading( -1f, true, true, true );
						particleAnimator.SetOpaqueAlpha( opaqueAlpha );
						particleAnimator.StartAlphaFading( alphaSpeed, false, true, true );
					}
				}
				UsefulFunctions.IncreaseArray<GameObject>( ref particleObjects, particleObject );
				UsefulFunctions.IncreaseArray<SimpleAnimator>( ref particleAnimators, particleAnimator );
				UsefulFunctions.IncreaseArray<float>( ref particleOpaqueDurations, opaqueDuration );
				UsefulFunctions.IncreaseArray<float>( ref particleOpaqueElapsed, 0f );
				UsefulFunctions.IncreaseArray<float>( ref particleAlphaSpeeds, alphaSpeed );
				UsefulFunctions.IncreaseArray<Vector2>( ref particleSpeeds, particleSpeed );
				UsefulFunctions.IncreaseArray<float>( ref particleSpins, particleSpin );
			}
		}
	}

	private void RemoveParticle( int particleIndex )
	{
		SimpleAnimator particleAnimator = null;
		GameObject particleObject = null;

		if( particleAnimators != null )
		{
			if( (particleIndex > -1) && (particleIndex < particleAnimators.Length) )
			{
				particleAnimator = particleAnimators[particleIndex];
				if( particleAnimator != null )
				{
					particleAnimator.Clear();
					particleAnimators[particleIndex] = null;
				}
				/*halmeida - relying on the coherence of all the "particle" arrays.*/
				particleObject = particleObjects[particleIndex];
				if( particleObject != null )
				{
					Destroy( particleObject );
					particleObjects[particleIndex] = null;
				}
				UsefulFunctions.DecreaseArray<float>( ref particleSpins, particleIndex );
				UsefulFunctions.DecreaseArray<Vector2>( ref particleSpeeds, particleIndex );
				UsefulFunctions.DecreaseArray<float>( ref particleAlphaSpeeds, particleIndex );
				UsefulFunctions.DecreaseArray<float>( ref particleOpaqueElapsed, particleIndex );
				UsefulFunctions.DecreaseArray<float>( ref particleOpaqueDurations, particleIndex );
				UsefulFunctions.DecreaseArray<SimpleAnimator>( ref particleAnimators, particleIndex );
				UsefulFunctions.DecreaseArray<GameObject>( ref particleObjects, particleIndex );
			}
		}
	}

	public void Clear()
	{
		SimpleAnimator particleAnimator = null;
		GameObject particleObject = null;

		particleOpaqueDurations = null;
		particleOpaqueElapsed = null;
		particleAlphaSpeeds = null;
		particleSpeeds = null;
		particleSpins = null;
		if( particleAnimators != null )
		{
			for( int i=0; i<particleAnimators.Length; i++ )
			{
				particleAnimator = particleAnimators[i];
				if( particleAnimator != null )
				{
					particleAnimator.Clear();
					particleAnimators[i] = null;
				}
			}
			particleAnimators = null;
		}
		if( particleObjects != null )
		{
			for( int i=0; i<particleObjects.Length; i++ )
			{
				particleObject = particleObjects[i];
				if( particleObject != null )
				{
					Destroy( particleObject );
					particleObjects[i] = null;
				}
			}
			particleObjects = null;
		}
	}

	public int GetCurrentParticleAmount()
	{
		if( particleObjects != null )
		{
			return particleObjects.Length;
		}
		return 0;
	}
}
