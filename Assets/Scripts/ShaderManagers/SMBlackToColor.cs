using System.Collections;
using UnityEngine;

/*halmeida - ShaderManagerBlackToColor*/
public class SMBlackToColor : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;
	private Material material;
	private int propertyID;
	private bool propertyValid;

	void Awake()
	{
		spriteRenderer = null;
		material = null;
		propertyID = 0;
		propertyValid = false;
		spriteRenderer = GetComponent<SpriteRenderer>();
		if( spriteRenderer != null )
		{
			/*halmeida - when I extract the material from the renderer, a new material instance is automatically
			created. I must destroy it later or this new clone of the material will be leaked when the object is
			destroyed.*/
			material = spriteRenderer.material;
			if( material != null )
			{
				propertyID = Shader.PropertyToID("_NewColor");
				propertyValid = material.HasProperty( propertyID );
			}
		}
	}

	public void SetBlackReplacementColor( Color newColor )
	{
		if( propertyValid )
		{
			material.SetColor( propertyID, newColor );
		}
	}

	public void Clear()
	{
		if( material != null )
		{
			Destroy( material );
			material = null;
		}
	}
}
