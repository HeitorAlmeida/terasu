using UnityEngine;
using System.Collections;

public class AnimationSequence : MonoBehaviour
{
	public Sprite[] sprites;
	public Sprite[] spriteNormals;
	public int framesPerSecond;
	public bool loop;
	public Vector2[] pixelOffsetsOne;
	public Vector2[] pixelOffsetsTwo;
	/*halmeida - the bool below will indicate to sprite extractors if they should try to get a fake light component from this object.*/
	public bool hasFakeLight;
}
