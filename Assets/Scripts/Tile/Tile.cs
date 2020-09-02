using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
	/*
	public const int SHAPE_CODE_INVALID = -1;
	public const int SHAPE_CODE_CONNECTED  = 11111111;
	public const int SHAPE_CODE_ULLDDDRRUR = 10111111;
	public const int SHAPE_CODE_UULLLDDDRR = 11111110;
	public const int SHAPE_CODE_UULLLDDRUR = 11111011;
	public const int SHAPE_CODE_UULLDDRRUR = 11101111;
	public const int SHAPE_CODE_ULLDDDRR =   10111110;
	public const int SHAPE_CODE_ULLDDRUR =   10111011;
	public const int SHAPE_CODE_UULLLDDR =   11111010;
	public const int SHAPE_CODE_ULDDRRUR =   10101111;
	public const int SHAPE_CODE_UULLDDRR =   11101110;
	public const int SHAPE_CODE_UULLDRUR =   11101011;
	public const int SHAPE_CODE_ULDDRR =     10101110;
	public const int SHAPE_CODE_ULLDDR =     10111010;
	public const int SHAPE_CODE_UULLDR =     11101010;
	public const int SHAPE_CODE_ULDRUR =     10101011;
	public const int SHAPE_CODE_UULLRUR =    11100011;
	public const int SHAPE_CODE_UDDRRUR =    10001111;
	public const int SHAPE_CODE_LLDDDRR =    00111110;
	public const int SHAPE_CODE_UULLLDD =    11111000;
	public const int SHAPE_CODE_LLDDR =      00111010;
	public const int SHAPE_CODE_LDDRR =      00101110;
	public const int SHAPE_CODE_UULLR =      11100010;
	public const int SHAPE_CODE_ULRUR =      10100011;
	public const int SHAPE_CODE_UDRUR =      10001011;
	public const int SHAPE_CODE_UULLD =      11101000;
	public const int SHAPE_CODE_UDDRR =      10001110;
	public const int SHAPE_CODE_ULLDD =      10111000;
	public const int SHAPE_CODE_ULDR =       10101010;
	public const int SHAPE_CODE_URUR =       10000011;
	public const int SHAPE_CODE_DDRR =       00001110;
	public const int SHAPE_CODE_LLDD =       00111000;
	public const int SHAPE_CODE_UULL =       11100000;
	public const int SHAPE_CODE_LDR =        00101010;
	public const int SHAPE_CODE_UDR =        10001010;
	public const int SHAPE_CODE_ULR =        10100010;
	public const int SHAPE_CODE_ULD =        10101000;
	public const int SHAPE_CODE_DR =         00001010;
	public const int SHAPE_CODE_LR =         00100010;
	public const int SHAPE_CODE_LD =         00101000;
	public const int SHAPE_CODE_UR =         10000010;
	public const int SHAPE_CODE_UD =         10001000;
	public const int SHAPE_CODE_UL =         10100000;
	public const int SHAPE_CODE_R =          00000010;
	public const int SHAPE_CODE_D =          00001000;
	public const int SHAPE_CODE_L =          00100000;
	public const int SHAPE_CODE_U =          10000000;
	public const int SHAPE_CODE_SEPARATED =  00000000;
	public const int SHAPE_CODE_TUULL =      22200000;
	public const int SHAPE_CODE_TURUR =      20000022;
	public const int SHAPE_CODE_TLLDD =      00222000;
	public const int SHAPE_CODE_TDDRR =      00002220;
	*/
											//128, 64, 32, 16, 8, 4, 2, 1
	//public const int SHAPE_CODE_CONNECTED  = 11111111;
	public const int SHAPE_INDEX_CONNECTED  = 255;
	//public const int SHAPE_CODE_ULLDDDRRUR = 10111111;
	public const int SHAPE_INDEX_ULLDDDRRUR = 191;
	//public const int SHAPE_CODE_UULLLDDDRR = 11111110;
	public const int SHAPE_INDEX_UULLLDDDRR = 254;
	//public const int SHAPE_CODE_UULLLDDRUR = 11111011;
	public const int SHAPE_INDEX_UULLLDDRUR = 251;
	//public const int SHAPE_CODE_UULLDDRRUR = 11101111;
	public const int SHAPE_INDEX_UULLDDRRUR = 239;
	//public const int SHAPE_CODE_ULLDDDRR =   10111110;
	public const int SHAPE_INDEX_ULLDDDRR = 190;
	//public const int SHAPE_CODE_ULLDDRUR =   10111011;
	public const int SHAPE_INDEX_ULLDDRUR = 187;
	//public const int SHAPE_CODE_UULLLDDR =   11111010;
	public const int SHAPE_INDEX_UULLLDDR = 250;
	//public const int SHAPE_CODE_ULDDRRUR =   10101111;
	public const int SHAPE_INDEX_ULDDRRUR = 175;
	//public const int SHAPE_CODE_UULLDDRR =   11101110;
	public const int SHAPE_INDEX_UULLDDRR = 238;
	//public const int SHAPE_CODE_UULLDRUR =   11101011;
	public const int SHAPE_INDEX_UULLDRUR = 235;
	//public const int SHAPE_CODE_ULDDRR =     10101110;
	public const int SHAPE_INDEX_ULDDRR = 174;
	//public const int SHAPE_CODE_ULLDDR =     10111010;
	public const int SHAPE_INDEX_ULLDDR = 186;
	//public const int SHAPE_CODE_UULLDR =     11101010;
	public const int SHAPE_INDEX_UULLDR = 234;
	//public const int SHAPE_CODE_ULDRUR =     10101011;
	public const int SHAPE_INDEX_ULDRUR = 171;
	//public const int SHAPE_CODE_UULLRUR =    11100011;
	public const int SHAPE_INDEX_UULLRUR = 227;
	//public const int SHAPE_CODE_UDDRRUR =    10001111;
	public const int SHAPE_INDEX_UDDRRUR = 143;
	//public const int SHAPE_CODE_LLDDDRR =    00111110;
	public const int SHAPE_INDEX_LLDDDRR = 62;
	//public const int SHAPE_CODE_UULLLDD =    11111000;
	public const int SHAPE_INDEX_UULLLDD = 248;
	//public const int SHAPE_CODE_LLDDR =      00111010;
	public const int SHAPE_INDEX_LLDDR = 58;
	//public const int SHAPE_CODE_LDDRR =      00101110;
	public const int SHAPE_INDEX_LDDRR = 46;
	//public const int SHAPE_CODE_UULLR =      11100010;
	public const int SHAPE_INDEX_UULLR = 226;
	//public const int SHAPE_CODE_ULRUR =      10100011;
	public const int SHAPE_INDEX_ULRUR = 163;
	//public const int SHAPE_CODE_UDRUR =      10001011;
	public const int SHAPE_INDEX_UDRUR = 139;
	//public const int SHAPE_CODE_UULLD =      11101000;
	public const int SHAPE_INDEX_UULLD = 232;
	//public const int SHAPE_CODE_UDDRR =      10001110;
	public const int SHAPE_INDEX_UDDRR = 142;
	//public const int SHAPE_CODE_ULLDD =      10111000;
	public const int SHAPE_INDEX_ULLDD = 184;
	//public const int SHAPE_CODE_ULDR =       10101010;
	public const int SHAPE_INDEX_ULDR = 170;
	//public const int SHAPE_CODE_URUR =       10000011;
	public const int SHAPE_INDEX_URUR = 131;
	//public const int SHAPE_CODE_DDRR =       00001110;
	public const int SHAPE_INDEX_DDRR = 14;
	//public const int SHAPE_CODE_LLDD =       00111000;
	public const int SHAPE_INDEX_LLDD = 56;
	//public const int SHAPE_CODE_UULL =       11100000;
	public const int SHAPE_INDEX_UULL = 224;
	//public const int SHAPE_CODE_LDR =        00101010;
	public const int SHAPE_INDEX_LDR = 42;
	//public const int SHAPE_CODE_UDR =        10001010;
	public const int SHAPE_INDEX_UDR = 138;
	//public const int SHAPE_CODE_ULR =        10100010;
	public const int SHAPE_INDEX_ULR = 162;
	//public const int SHAPE_CODE_ULD =        10101000;
	public const int SHAPE_INDEX_ULD = 168;
	//public const int SHAPE_CODE_DR =         00001010;
	public const int SHAPE_INDEX_DR = 10;
	//public const int SHAPE_CODE_LR =         00100010;
	public const int SHAPE_INDEX_LR = 34;
	//public const int SHAPE_CODE_LD =         00101000;
	public const int SHAPE_INDEX_LD = 40;
	//public const int SHAPE_CODE_UR =         10000010;
	public const int SHAPE_INDEX_UR = 130;
	//public const int SHAPE_CODE_UD =         10001000;
	public const int SHAPE_INDEX_UD = 136;
	//public const int SHAPE_CODE_UL =         10100000;
	public const int SHAPE_INDEX_UL = 160;
	//public const int SHAPE_CODE_R =          00000010;
	public const int SHAPE_INDEX_R = 2;
	//public const int SHAPE_CODE_D =          00001000;
	public const int SHAPE_INDEX_D = 8;
	//public const int SHAPE_CODE_L =          00100000;
	public const int SHAPE_INDEX_L = 32;
	//public const int SHAPE_CODE_U =          10000000;
	public const int SHAPE_INDEX_U = 128;
	//public const int SHAPE_CODE_SEPARATED =  00000000;
	public const int SHAPE_INDEX_SEPARATED = 0;
	//public const int SHAPE_CODE_TUULL =      22200000;
	public const int SHAPE_INDEX_TUULL = 480;
	//public const int SHAPE_CODE_TURUR =      20000022;
	public const int SHAPE_INDEX_TURUR = 387;
	//public const int SHAPE_CODE_TLLDD =      00222000;
	public const int SHAPE_INDEX_TLLDD = 312;
	//public const int SHAPE_CODE_TDDRR =      00002220;
	public const int SHAPE_INDEX_TDDRR = 270;

	public const int TOTAL_REQUIRED_SHAPE_INDEXES = 481;

	private int shapeIndex;
	private bool triangle;
	private Sprite sprite;
	private SpriteRenderer tileRenderer;
	private BoxCollider2D colliderBox;
	private PolygonCollider2D colliderPolygon;

	void Awake()
	{
		shapeIndex = -1;
		triangle = false;
		sprite = null;
		/*halmeida - I do not seek for the rendering component within this object because the
		rendering component will be added to this object during its life cycle, after the awake.
		It should be provided to this class through SetTileRenderer, sparing the component search.*/
		tileRenderer = null;
		colliderBox = null;
		colliderPolygon = null;
	}

	public bool IsTriangle()
	{
		return triangle;
	}

	public void SetTileRenderer( SpriteRenderer newTileRenderer )
	{
		tileRenderer = newTileRenderer;
	}

	public void SetSprite( Sprite newSprite )
	{
		if( tileRenderer != null )
		{
			sprite = newSprite;
			tileRenderer.sprite = sprite;
		}
	}

	public void SetRenderingAlpha( float newAlpha )
	{
		Color color = Color.white;

		if( tileRenderer != null )
		{
			color = tileRenderer.color;
			color.a = newAlpha;
			tileRenderer.color = color;
		}
	}

	public void SetColliderBox( BoxCollider2D newColliderBox )
	{
		colliderBox = newColliderBox;
	}

	public void SetColliderPolygon( PolygonCollider2D newColliderPolygon )
	{
		colliderPolygon = newColliderPolygon;
	}

	public BoxCollider2D GetColliderBox()
	{
		return colliderBox;
	}

	public PolygonCollider2D GetColliderPolygon()
	{
		return colliderPolygon;
	}

	public void Clear()
	{
		/*halmeida - do nothing since no memory is allocated by the instance.*/
	}

	public static int GetShapeCodeByNeighborhood( bool U = false, bool UL = false, bool L = false, bool LD = false, bool D = false,
		bool DR = false, bool R = false, bool UR = false, bool T = false )
	{
		int checksum = 0;

		/*halmeida - we have to remove the diagonals in case they are not effective, or the checksum
		will produce values that correspond to no tiles at all. For example, if a tile has its U, UL,
		L, LD, R and UR occupied, the produced checksum would be 11110011, which corresponds to no
		code in the list of codes. That happens because the code list was produced with basis on
		connections to the neighbors, not occupation of the neighboring positions. By removing the
		ineffective diagonals we will be transforming the information of occupation into the needed
		information of connections.*/
		if( !( U && L ) )
		{
			UL = false;
		}
		if( !( L && D ) )
		{
			LD = false;
		}
		if( !( D && R ) )
		{
			DR = false;
		}
		if( !( U && R ) )
		{
			UR = false;
		}
		/*halmeida - now we have transformed the information of occupation into the information
		of connections, for the diagonals are only connected if both adjacent directions are
		occupied.*/
		if( U )
		{
			checksum += 10000000;
		}
		if( UL )
		{
			checksum += 01000000;
		}
		if( L )
		{
			checksum += 00100000;
		}
		if( LD )
		{
			checksum += 00010000;
		}
		if( D )
		{
			checksum += 00001000;
		}
		if( DR )
		{
			checksum += 00000100;
		}
		if( R )
		{
			checksum += 00000010;
		}
		if( UR )
		{
			checksum += 00000001;
		}
		//if( ShapeCodeCanBecomeTriangle( checksum ) )
		//{
			/*halmeida - if the neighboring connections allow the tile to be triangular and it wants to be...*/ 
		//	if( T )
		//	{
		//		checksum *= 2;
		//	}
		//}
		return checksum;
	}

	/*halmeida - this is part of the effort to create a HashTable for tile shapes. If I record a shape code for a tile,
	later when I want to create the look for this tile, based on its shape, I will have to run through an array of shape codes,
	until I find the index of the corresponding code. This index will be the index of the look of the tile (a sprite, for example).
	Instead of recording the code, I will record the index itself which will spare the effort of the search.*/
	public void SetShapeIndex( int index )
	{
		shapeIndex = index;
		triangle = ((index == SHAPE_INDEX_TUULL) || (index == SHAPE_INDEX_TURUR) || (index == SHAPE_INDEX_TLLDD) || (index == SHAPE_INDEX_TDDRR));
	}

	public int GetShapeIndex()
	{
		return shapeIndex;
	}

	public static bool ShapeIndexCanBecomeTriangle( int index )
	{
		return ((index == SHAPE_INDEX_UULL) || (index == SHAPE_INDEX_URUR) || (index == SHAPE_INDEX_LLDD) || (index == SHAPE_INDEX_DDRR));
	}

	public static bool ShapeIndexShouldBeTriangle( int index )
	{
		return ((index == SHAPE_INDEX_TUULL) || (index == SHAPE_INDEX_TURUR) || (index == SHAPE_INDEX_TLLDD) || (index == SHAPE_INDEX_TDDRR));
	}

	public static int GetShapeIndexByNeighborhood( bool U = false, bool UL = false, bool L = false, bool LD = false, bool D = false,
		bool DR = false, bool R = false, bool UR = false, bool T = false )
	{
		int checksum = 0;

		/*halmeida - we have to remove the diagonals in case they are not effective, or the checksum
		will produce values that correspond to no tiles at all. For example, if a tile has its U, UL,
		L, LD, R and UR occupied, the produced checksum would be 11110011, which corresponds to no
		code in the list of codes. That happens because the code list was produced with basis on
		connections to the neighbors, not occupation of the neighboring positions. By removing the
		ineffective diagonals we will be transforming the information of occupation into the needed
		information of connections.*/
		if( !( U && L ) )
		{
			UL = false;
		}
		if( !( L && D ) )
		{
			LD = false;
		}
		if( !( D && R ) )
		{
			DR = false;
		}
		if( !( U && R ) )
		{
			UR = false;
		}
		/*halmeida - now we have transformed the information of occupation into the information
		of connections, for the diagonals are only connected if both adjacent directions are
		occupied.*/
		if( U )
		{
			checksum += 128;
		}
		if( UL )
		{
			checksum += 64;
		}
		if( L )
		{
			checksum += 32;
		}
		if( LD )
		{
			checksum += 16;
		}
		if( D )
		{
			checksum += 8;
		}
		if( DR )
		{
			checksum += 4;
		}
		if( R )
		{
			checksum += 2;
		}
		if( UR )
		{
			checksum += 1;
		}
		if( ShapeIndexCanBecomeTriangle( checksum ) )
		{
			/*halmeida - if the neighboring connections allow the tile to be triangular and it wants to be...*/ 
			if( T )
			{
				checksum += 256;
			}
		}
		return checksum;
	}
}
