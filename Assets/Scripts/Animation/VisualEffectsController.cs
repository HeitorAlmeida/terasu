using UnityEngine;
using System.Collections;

public class VisualEffectsController
{
	public const int MAX_EFFECTS_IN_LAYER = 100;
	/*halmeida - exceeding effects will invade depth layers of other elements.*/

	private static VisualEffectsController instance;

	private GameObject[] effectObjects;
	private SimpleAnimator[] effects;
	private float baseZ;
	private float offsetZ;

	private VisualEffectsController()
	{
		effectObjects = null;
		effects = null;
		baseZ = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_VFX );
		offsetZ = DisplayDepthManager.ELEMENT_TO_ELEMENT_OFFSET / MAX_EFFECTS_IN_LAYER;
	}

	public static VisualEffectsController GetInstance()
	{
		if( instance == null )
		{
			instance = new VisualEffectsController();
		}
		return instance;
	}

	public void AddEffect( GameObject effectObjectModel, Vector2 worldPosition )
	{
		SimpleAnimator effect = null;
		Vector3 completePosition = Vector3.zero;
		GameObject newEffectObject = null;
		SimpleAnimator newEffect = null;
		GameObject[] newEffectObjects = null;
		SimpleAnimator[] newEffects = null;
		int length = 0;

		if( effectObjectModel != null )
		{
			effect = effectObjectModel.GetComponent<SimpleAnimator>();
			if( effect != null )
			{
				newEffectObject = GameObject.Instantiate( effectObjectModel, Vector3.zero, Quaternion.identity ) as GameObject;
				newEffect = newEffectObject.GetComponent<SimpleAnimator>();
				if( effectObjects != null )
				{
					length = effectObjects.Length;
				}
				completePosition.x = worldPosition.x;
				completePosition.y = worldPosition.y;
				completePosition.z = baseZ + length * offsetZ;
				newEffectObject.transform.position = completePosition;
				newEffectObjects = new GameObject[length+1];
				newEffects = new SimpleAnimator[length+1];
				if( effectObjects != null )
				{
					for( int i=0; i<length; i++ )
					{
						newEffectObjects[i] = effectObjects[i];
						newEffects[i] = effects[i];
					}
				}
				newEffectObjects[length] = newEffectObject;
				newEffects[length] = newEffect;
				effectObjects = newEffectObjects;
				effects = newEffects;
			}
		}
	}

	public void Progress( float timeStep )
	{
		SimpleAnimator effect = null;

		if( effects != null )
		{
			for( int i=0; i<effects.Length; i++ )
			{
				effect = effects[i];
				if( effect != null )
				{
					effect.Progress( timeStep );
					if( effect.IsOver() )
					{
						RemoveEffect( i );
						if( effects == null )
						{
							return;
						}
						i--;
					}
				}
			}
		}
	}

	private void RemoveEffect( int index )
	{
		SimpleAnimator effect = null;
		GameObject effectObject = null;
		GameObject[] newEffectObjects = null;
		SimpleAnimator[] newEffects = null;
		int length = 0;
		Vector3 completePosition = Vector3.zero;

		if( (effectObjects != null) && (effects != null) )
		{
			if( (effectObjects.Length == effects.Length) && (effects.Length > index) )
			{
				effect = effects[index];
				if( effect != null )
				{
					effect.Clear();
					effect = null;
					effects[index] = null;
				}
				effectObject = effectObjects[index];
				if( effectObject != null )
				{
					GameObject.Destroy( effectObject );
					effectObject = null;
					effectObjects[index] = null;
				}
				length = effects.Length;
				if( length > 1 )
				{
					newEffectObjects = new GameObject[length-1];
					newEffects = new SimpleAnimator[length-1];
					for( int i=0; i<index; i++ )
					{
						newEffectObjects[i] = effectObjects[i];
						newEffects[i] = effects[i];
					}
					for( int i=index; i<(length-1); i++ )
					{
						effectObject = effectObjects[i+1];
						newEffectObjects[i] = effectObject;
						if( effectObject != null )
						{
							completePosition = effectObject.transform.position;
							completePosition.z = baseZ + i * offsetZ;
							effectObject.transform.position = completePosition;
						}
						newEffects[i] = effects[i+1];
					}
					effectObjects = newEffectObjects;
					effects = newEffects;
				}
				else
				{
					effectObjects = null;
					effects = null;
				}
			}
		}
	}

	public void Clear()
	{
		SimpleAnimator effect = null;
		GameObject effectObject = null;

		if( effects != null )
		{
			for( int i=0; i<effects.Length; i++ )
			{
				effect = effects[i];
				if( effect != null )
				{
					effect.Clear();
					effect = null;
					effects[i] = null;
				}
			}
			effects = null;
		}
		if( effectObjects != null )
		{
			for( int i=0; i<effectObjects.Length; i++ )
			{
				effectObject = effectObjects[i];
				if( effectObject != null )
				{
					GameObject.Destroy( effectObject );
					effectObject = null;
					effectObjects[i] = null;
				}
			}
			effectObjects = null;
		}
	}
}
