using UnityEngine;
using System.Collections;
using System; //halmeida - for String methods.

public class UsefulFunctions
{
	public const float SIN_23_DEGREES = 0.391f;
	public const float SIN_67_DEGREES = 0.921f;

	public UsefulFunctions()
	{
		return;
	}

	public static bool AreaContainsPoint( float areaLeftX, float areaTopY, float areaWidth, float areaHeight, float pointX, float pointY )
	{
		if( (pointX < areaLeftX) || (pointX > areaLeftX + areaWidth) || (pointY > areaTopY) || (pointY < areaTopY - areaHeight) )
		{
			return false;
		}
		return true;
	}

	public static bool AreaContainsSlot( int areaLeftColumn, int areaTopRow, int areaRightColumn, int areaBottomRow, int pointColumn, int pointRow )
	{
		if( (pointColumn < areaLeftColumn) || (pointColumn > areaRightColumn) || (pointRow > areaTopRow) || (pointRow < areaBottomRow) )
		{
			return false;
		}
		return true;
	}

	public static bool AreasCollideDimensions( float aLeftX, float aTopY, float aWidth, float aHeight,
	float bLeftX, float bTopY, float bWidth, float bHeight )
	{
		if( (aLeftX > bLeftX + bWidth) || (aLeftX + aWidth < bLeftX) || (aTopY < bTopY - bHeight) || (aTopY - aHeight > bTopY) )
		{
			return false;
		}
		return true;
	}

	public static bool AreasCollideLimits( float aLeftX, float aTopY, float aRightX, float aBottomY,
	float bLeftX, float bTopY, float bRightX, float bBottomY )
	{
		if( (aLeftX > bRightX) || (aRightX < bLeftX) || (aTopY < bBottomY) || (aBottomY > bTopY) )
		{
			return false;
		}
		return true;
	}

	public static bool AreasCollideLimits( int aLeftX, int aTopY, int aRightX, int aBottomY,
	int bLeftX, int bTopY, int bRightX, int bBottomY )
	{
		if( (aLeftX > bRightX) || (aRightX < bLeftX) || (aTopY < bBottomY) || (aBottomY > bTopY) )
		{
			return false;
		}
		return true;
	}

	public static int GetDigitAmount( int number )
	{
		int houses = 0;

		if( number == 0 )
		{
			return 1;
		}
		number *= ( number < 0 ? -1 : 1 );
		while( number > 0 )
		{
			number = number / 10;
			houses++;
		}
		return houses;
	}

	public static float GetDigitAverage( int number )
	{
		float dividedNumber = 0f;
		int houses = 0;
		int biggestDivider = 1;
		int digit = 0;
		int sum = 0;

		if( number == 0 )
		{
			return 0f;
		}
		number *= ( number < 0 ? -1 : 1 );
		dividedNumber = (float)number;
		while( dividedNumber >= 1f )
		{
			dividedNumber = dividedNumber / 10f;
			houses++;
			if( dividedNumber >= 1f )
			{
				biggestDivider *= 10;
			}
		}
		dividedNumber = (float)number;
		for( int i=0; i<houses; i++ )
		{
			digit = (int) (dividedNumber / biggestDivider);
			sum += digit;
			dividedNumber -= digit * biggestDivider;
			biggestDivider = biggestDivider / 10;
		}
		return ((float)sum/(float)houses);
	}

	public static int GetPercentage( float numerator, float denominator )
	{
		float fraction = 0f;
		int percentage = 0;
		bool roundUp = false;

		if( denominator == 0f )
		{
			return 0;
		}
		fraction = numerator / denominator;
		percentage = (int)(fraction * 100f);
		roundUp = (((int)(fraction * 1000f)) % 10 > 4);
		if( roundUp )
		{
			percentage++;
		}
		return percentage;
	}

	public static int GetPercentage( float fraction )
	{
		int percentage = 0;
		bool roundUp = false;

		percentage = (int)(fraction * 100f);
		roundUp = (((int)(fraction * 1000f)) % 10 > 4);
		if( roundUp )
		{
			percentage++;
		}
		return percentage;
	}

	public static void Quicksort<T>( float[] valuesArray, T[] anexArray, int startIndex = -1, int endIndex = -1 )
	{
		int pivotIndex = -1;
		float pivotValue = 0f;
		T pivotAnex = default(T);
		float checkingValue = 0f;
		T checkingAnex = default(T);
		float swappedDownValue = 0f;
		T swappedDownAnex = default(T);
		bool anexArrayValid = false;

		if( valuesArray != null )
		{
			if( anexArray != null )
			{
				anexArrayValid = (anexArray.Length == valuesArray.Length);
			}
			if( (startIndex == -1) && (endIndex == -1) )
			{
				startIndex = 0;
				endIndex = valuesArray.Length - 1;
			}
			if( (startIndex > -1) && (endIndex > startIndex) )
			{
				pivotIndex = endIndex;
				pivotValue = valuesArray[pivotIndex];
				if( anexArrayValid )
				{
					pivotAnex = anexArray[pivotIndex];
				}
				for( int i=0; i<pivotIndex; i++ )
				{
					checkingValue = valuesArray[i];
					if( anexArrayValid )
					{
						checkingAnex = anexArray[i];
					}
					if( i == (pivotIndex - 1) )
					{
						if( checkingValue > pivotValue )
						{
							valuesArray[i] = pivotValue;
							valuesArray[pivotIndex] = checkingValue;
							if( anexArrayValid )
							{
								anexArray[i] = pivotAnex;
								anexArray[pivotIndex] = checkingAnex;
							}
							pivotIndex--;
						}
						/*halmeida - this is the end condition. This partition is ordered.*/
					}
					else
					{
						if( checkingValue > pivotValue )
						{
							swappedDownValue = valuesArray[pivotIndex - 1];
							valuesArray[pivotIndex - 1] = pivotValue;
							valuesArray[pivotIndex] = checkingValue;
							valuesArray[i] = swappedDownValue;
							if( anexArrayValid )
							{
								swappedDownAnex = anexArray[pivotIndex - 1];
								anexArray[pivotIndex - 1] = pivotAnex;
								anexArray[pivotIndex] = checkingAnex;
								anexArray[i] = swappedDownAnex;
							}
							pivotIndex--;
							i--;
						}
					}
				}
				/*halmeida - with the partition ordered, I will order the smaller values partition and the bigger
				values partition separately.*/
				Quicksort<T>( valuesArray, anexArray, startIndex, pivotIndex - 1 );
				Quicksort<T>( valuesArray, anexArray, pivotIndex + 1, endIndex );
			}
		}
	} 

	public static void IncreaseArray<T>( ref T[] array, T newElement )
	{
		T[] newArray = null;
		int length = 0;

		if( array != null )
		{
			length = array.Length;
			newArray = new T[length + 1];
			for( int i=0; i<length; i++ )
			{
				newArray[i] = array[i];
			}
		}
		else
		{
			newArray = new T[1];
		}
		newArray[newArray.Length - 1] = newElement;
		array = newArray;
		newArray = null;
	}

	public static void IncreaseArrayWithArray<T>( ref T[] array, T[] additionalArray )
	{
		T[] newArray = null;
		int originalLength = 0;
		int additionalLength = 0;

		if( additionalArray != null )
		{
			additionalLength = additionalArray.Length;
			if( additionalLength > 0 )
			{
				if( array != null )
				{
					originalLength = array.Length;
					newArray = new T[originalLength + additionalLength];
					for( int i=0; i<originalLength; i++ )
					{
						newArray[i] = array[i];
					}
				}
				else
				{
					newArray = new T[additionalLength];
				}
				for( int i=0; i<additionalLength; i++ )
				{
					newArray[originalLength + i] = additionalArray[i];
				}
				array = newArray;
				newArray = null;
			}
		}
	}

	public static void DecreaseArray<T>( ref T[] array, int removeIndex )
	{
		T[] newArray = null;
		int length = 0;

		if( array != null )
		{
			length = array.Length;
			if( (removeIndex > -1) && (removeIndex < length) )
			{
				if( length > 1 )
				{
					newArray = new T[length - 1];
					for( int i=0; i<removeIndex; i++ )
					{
						newArray[i] = array[i];
					}
					for( int i=removeIndex; i<(length-1); i++ )
					{
						newArray[i] = array[i+1];
					}
					array = newArray;
					newArray = null;
				}
				else
				{
					array = null;
				}
			}
		}
	}

	public static int FindIndexOfArray<T>( T[][] arrayOfArrays, T[] targetArray )
	{
		int index = -1;
		T[] currentArray = null;
		bool differenceFound = false;

		if( arrayOfArrays != null )
		{
			for( int i=0; i<arrayOfArrays.Length; i++ )
			{
				currentArray = arrayOfArrays[i];
				if( currentArray == null )
				{
					if( targetArray == null )
					{
						index = i;
						break;
					}
				}
				else
				{
					differenceFound = false;
					if( targetArray == null )
					{
						differenceFound = true;
					}
					if( !differenceFound )
					{
						for( int j=0; j<targetArray.Length; j++ )
						{
							if( currentArray.Length <= j )
							{
								differenceFound = true;
								break;
							}
							else
							{
								if( !currentArray[j].Equals( targetArray[j] ) )
								{
									differenceFound = true;
									break;
								}
							}
						}
					}
					if( !differenceFound )
					{
						index = i;
						break;
					}
				}
			}
		}
		return index;
	}

	public static string StringFromArray<T>( string separator, T[] array )
	{
		string joined = "";

		if( array != null )
		{
			for( int i=0; i<array.Length; i++ )
			{
				if( array[i] != null )
				{
					joined = joined + array[i].ToString();
				}
				if( i < (array.Length-1) )
				{
					joined += separator;
				}
			}
		}
		else
		{
			joined = "null";
		}
		return joined;
	}

	public static int[] CopyArrayInt( int[] array )
	{
		int[] result = null;

		if( array != null )
		{
			result = new int[array.Length];
			Buffer.BlockCopy( array, 0, result, 0, array.Length * sizeof(int) );
		}
		return result;
	}

	public static bool IsNullOrWhiteSpace( string value )
	{
		return ( String.IsNullOrEmpty(value) || (value.Trim().Length == 0) );
	}

	/*halmeida - I have made plenty of experiences and my method is way faster than the
	built-in method of the Vector2 class. I believe it is due to the fact that my method
	already demands the direction to be normalized and the built-in method of the vector
	classes always does normalization within its body.*/
	/*halmeida - returns the angle that the direction creates with Vector2.right. The
	interval is [180,-180[. The -180 value is never returned, for it is equivalent to
	180.*/
	public static float GetDirectionAngle( Vector2 normalizedDirection )
	{
		float sine = 0f;
		float arcSine = 0f;
		float angle1 = 0f;
		//float angle2 = 0f;
		//float oldTime = 0f;
		//Vector2 reference = Vector2.right;

		//oldTime = Time.realtimeSinceStartup;
		sine = normalizedDirection.y;
		arcSine = Mathf.Asin( sine );
		angle1 = arcSine * Mathf.Rad2Deg;
		if( normalizedDirection.x < 0f )
		{
			angle1 = ( angle1 >= 0f ? 180f : -180f ) - angle1;
		}
		//Debug.Log("Debug : ProjectileController : time method 1 = "+(Time.realtimeSinceStartup - oldTime)+".");
		//Debug.Log("Debug : ProjectileController : angle method 1 = "+angle1+".");

		/*oldTime = Time.realtimeSinceStartup;
		angle2 = Vector2.SignedAngle( reference, newDirection );
		if( newDirection == Vector2.zero )
		{
			angle2 = 0f;
		}
		Debug.Log("Debug : ProjectileController : time method 2 = "+(Time.realtimeSinceStartup - oldTime)+".");
		Debug.Log("Debug : ProjectileController : angle method 2 = "+angle2+".");*/
		return angle1;
	}

	public static void LoadSoundsFromResources( string[] soundPaths, ref AudioClip[] sounds )
	{
		int length = 0;

		if( (soundPaths != null) && (sounds == null) )
		{
			length = soundPaths.Length;
			if( length > 0 )
			{
				sounds = new AudioClip[length];
				for( int i=0; i<length; i++ )
				{
					sounds[i] = Resources.Load<AudioClip>("Sounds/"+soundPaths[i]);
				}
			}
		}
	}

	public static void ClearSoundsFromResources( ref AudioClip[] sounds )
	{
		AudioClip resource = null;

		if( sounds != null )
		{
			for( int i=0; i<sounds.Length; i++ )
			{
				resource = sounds[i];
				if( resource != null )
				{
					Resources.UnloadAsset( resource );
					sounds[i] = null;
				}
			}
			sounds = null;
		}
	}

	public static void LoadSoundsIntoAudioCenter( string[] soundPaths, ref int[] soundIDs )
	{
		int length = 0;

		if( (soundPaths != null) && (soundIDs == null) )
		{
			length = soundPaths.Length;
			if( length > 0 )
			{
				soundIDs = new int[length];
				for( int i=0; i<length; i++ )
				{
					soundIDs[i] = AudioCenter.loadSound(soundPaths[i]);
				}
			}
		}
	}

	public static void ClearSoundsFromAudioCenter( ref int[] soundIDs )
	{
		int soundID = -1;

		if( soundIDs != null )
		{
			for( int i=0; i<soundIDs.Length; i++ )
			{
				soundID = soundIDs[i];
				if( soundID != -1 )
				{
					AudioCenter.unloadSound( soundID );
				}
			}
			soundIDs = null;
		}
	}

	public static Sprite CreateSolidSprite( Color spriteColor, int spriteResolution = 4, float spriteSize = 1f )
	{
		Texture2D texture = null;
		Rect textureRect = default( Rect );
		float pixelsPerUnit = 0f;
		Sprite sprite = null;

		texture = new Texture2D( spriteResolution, spriteResolution );
		for( int i=0; i<spriteResolution; i++ )
		{
			for( int j=0; j<spriteResolution; j++ )
			{
				texture.SetPixel( i, j, spriteColor );
			}
		}
		texture.Apply();
		textureRect = new Rect( 0f, 0f, spriteResolution, spriteResolution );
		spriteSize = (spriteSize <= 0f) ? 1f : spriteSize;
		pixelsPerUnit = (float)spriteResolution/spriteSize;
		sprite = Sprite.Create( texture, textureRect, new Vector2( 0.5f, 0.5f ), pixelsPerUnit );
		return sprite;
	}

	public static Texture2D CopyTextureMirrored( Texture2D original, bool horizontal )
	{
		Texture2D copy = null;
		Color colorAtOriginal = default( Color );
		
		if( original != null )
		{
			copy = new Texture2D( original.width, original.height );
			for( int i=0; i<original.height; i++ )
			{
				for( int j=0; j<original.width; j++ )
				{
					colorAtOriginal = original.GetPixel ( j, i );
					if( horizontal )
					{
						copy.SetPixel ( (original.width - 1) - j, i, colorAtOriginal );
					}
					else
					{
						copy.SetPixel ( j, (original.height - 1) - i, colorAtOriginal );
					}
				}
			}
			copy.Apply ();
		}
		return copy;
	}
	
	public static Vector3[] DistributeAnchorRates( int totalColumns, int totalItems, float areaXRate, float areaYRate, float areaWidthRate,
		float areaHeightRate, float separationRate, out float maxItemWidthRate, out float maxItemHeightRate )
	{
		float startingXRate = 0.0f;
		float startingYRate = 1.0f;
		float itemWidthRate = 0.0f;
		float itemWidthRateReduced = 0.0f;
		float itemHeightRate = 0.0f;
		float itemHeightRateReduced = 0.0f;
		int itemsPerColumn = 0;
		int itemsPerColumnRest = 0;
		Vector3[] anchors = null;
		int currentColumn = -1;
		int currentRow = -1;
		
		startingXRate = areaXRate;
		if( startingXRate < 0.0f )
		{
			startingXRate = 0.0f;
		}
		if( startingXRate >= 1.0f )
		{
			startingXRate = 0.5f;
		}
		startingYRate = areaYRate;
		if( startingYRate <= 0.0f )
		{
			startingYRate = 0.5f;
		}
		if( startingYRate > 1.0f )
		{
			startingYRate = 1.0f;
		}
		if( areaWidthRate <= 0.0f )
		{
			areaWidthRate = 1.0f;
		}
		if( areaWidthRate > (1.0f - startingXRate) )
		{
			areaWidthRate = 1.0f - startingXRate;
		}
		if( totalColumns < 1 )
		{
			totalColumns = 1;
		}
		if( totalItems < 1 )
		{
			totalItems = 1;
		}
		if( areaHeightRate <= 0.0f )
		{
			areaHeightRate = 1.0f;
		}
		if( areaHeightRate > startingYRate )
		{
			areaHeightRate = startingYRate;
		}
		itemsPerColumn = totalItems / totalColumns;
		itemsPerColumnRest = totalItems % totalColumns;
		if( itemsPerColumnRest > 0 )
		{
			itemsPerColumn++;
		}
		itemWidthRate = areaWidthRate / totalColumns;
		itemHeightRate = areaHeightRate / itemsPerColumn;
		if( separationRate >= itemWidthRate )
		{
			separationRate = itemWidthRate - 0.01f;
		}
		if( separationRate >= itemHeightRate )
		{
			separationRate = itemHeightRate - 0.01f;
		}
		if( separationRate < 0.0f )
		{
			separationRate = 0.0f;
		}
		itemWidthRateReduced = itemWidthRate - separationRate;
		itemHeightRateReduced = itemHeightRate - separationRate;
		maxItemWidthRate = itemWidthRateReduced;
		maxItemHeightRate = itemHeightRateReduced;
		anchors = new Vector3[totalItems];
		for( int i=0; i<totalItems; i++ )
		{
			currentRow = i / totalColumns;
			currentColumn = i % totalColumns;
			anchors[i] = new Vector3( startingXRate + (currentColumn * itemWidthRate) + (separationRate / 2.0f),
				startingYRate - (currentRow * itemHeightRate) - (separationRate / 2.0f), 1.0f );
		}
		return anchors;
	}
}
