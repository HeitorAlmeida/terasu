using UnityEngine;
using System.Collections;

public class TileFactory : MonoBehaviour
{
	public const int UNDEFINED_FACTORY_ID = -1;

	public string factoryName;
	public int factoryID;
	public PhysicsMaterial2D material;
	public Sprite tileSeparated;  //01
	public Sprite tileU;          //02
	public Sprite tileL;          //03
	public Sprite tileD;          //04
	public Sprite tileR;          //05
	public Sprite tileUL;         //06
	public Sprite tileUD;         //07
	public Sprite tileUR;         //08
	public Sprite tileLD;         //09
	public Sprite tileLR;         //10
	public Sprite tileDR;         //11
	public Sprite tileULD;        //12
	public Sprite tileULR;        //13
	public Sprite tileUDR;        //14
	public Sprite tileLDR;        //15
	public Sprite tileUULL;       //16
	public Sprite tileLLDD;       //17
	public Sprite tileDDRR;       //18
	public Sprite tileURUR;       //19
	public Sprite tileULDR;       //20
	public Sprite tileULLDD;      //21
	public Sprite tileUDDRR;      //22
	public Sprite tileUULLD;      //23
	public Sprite tileUDRUR;      //24
	public Sprite tileULRUR;      //25
	public Sprite tileUULLR;      //26
	public Sprite tileLDDRR;      //27
	public Sprite tileLLDDR;      //28
	public Sprite tileUULLLDD;    //29
	public Sprite tileLLDDDRR;    //30
	public Sprite tileUDDRRUR;    //31
	public Sprite tileUULLRUR;    //32
	public Sprite tileULDRUR;     //33
	public Sprite tileUULLDR;     //34
	public Sprite tileULLDDR;     //35
	public Sprite tileULDDRR;     //36
	public Sprite tileUULLDRUR;   //37
	public Sprite tileUULLDDRR;   //38
	public Sprite tileULDDRRUR;   //39
	public Sprite tileUULLLDDR;   //40
	public Sprite tileULLDDRUR;   //41
	public Sprite tileULLDDDRR;   //42
	public Sprite tileUULLDDRRUR; //43
	public Sprite tileUULLLDDRUR; //44
	public Sprite tileUULLLDDDRR; //45
	public Sprite tileULLDDDRRUR; //46
	public Sprite tileConnected;  //47
	public Sprite tileTriUULL;    //48
	public Sprite tileTriURUR;    //49
	public Sprite tileTriLLDD;    //50
	public Sprite tileTriDDRR;    //51
	public Sprite[] tileConnectedVariations;
	public Sprite[] tileUULLLDDVariations;
	public Sprite[] tileLLDDDRRVariations;
	public Sprite[] tileUDDRRURVariations;
	public Sprite[] tileUULLRURVariations;

	private Sprite[] tileSprites;
	private float tileWidth;
	private float tileHeight;

	void Awake()
	{
		int totalIndexes = Tile.TOTAL_REQUIRED_SHAPE_INDEXES;
		Sprite tile = null;

		/*halmeida - tileSprites will be a hashtable. According to the neighboring connections of a tile, a value is constructed.
		This value is already an index within the tileSprites array, allowing the recovery of a tile's sprite in constant time O(1).
		Before the implementation as hashtable, there was an additional array of shape codes. The value constructed by evaluating
		the neighboring connections was a shape code. This code should be searched in the shape codes array, to find the index of
		the tile's sprite in the sprites array.*/
		tileSprites = new Sprite[totalIndexes];
		for( int i=0; i<tileSprites.Length; i++ )
		{
			switch( i )
			{
				case Tile.SHAPE_INDEX_CONNECTED://1
					tileSprites[i] = tileConnected;
					break;
				case Tile.SHAPE_INDEX_ULLDDDRRUR://2
					tileSprites[i] = tileULLDDDRRUR;
					break;
				case Tile.SHAPE_INDEX_UULLLDDDRR://3
					tileSprites[i] = tileUULLLDDDRR;
					break;
				case Tile.SHAPE_INDEX_UULLLDDRUR://4
					tileSprites[i] = tileUULLLDDRUR;
					break;
				case Tile.SHAPE_INDEX_UULLDDRRUR://5
					tileSprites[i] = tileUULLDDRRUR;
					break;
				case Tile.SHAPE_INDEX_ULLDDDRR://6
					tileSprites[i] = tileULLDDDRR;
					break;
				case Tile.SHAPE_INDEX_ULLDDRUR://7
					tileSprites[i] = tileULLDDRUR;
					break;
				case Tile.SHAPE_INDEX_UULLLDDR://8
					tileSprites[i] = tileUULLLDDR;
					break;
				case Tile.SHAPE_INDEX_ULDDRRUR://9
					tileSprites[i] = tileULDDRRUR;
					break;
				case Tile.SHAPE_INDEX_UULLDDRR://10
					tileSprites[i] = tileUULLDDRR;
					break;
				case Tile.SHAPE_INDEX_UULLDRUR://11
					tileSprites[i] = tileUULLDRUR;
					break;
				case Tile.SHAPE_INDEX_ULDDRR://12
					tileSprites[i] = tileULDDRR;
					break;
				case Tile.SHAPE_INDEX_ULLDDR://13
					tileSprites[i] = tileULLDDR;
					break;
				case Tile.SHAPE_INDEX_UULLDR://14
					tileSprites[i] = tileUULLDR;
					break;
				case Tile.SHAPE_INDEX_ULDRUR://15
					tileSprites[i] = tileULDRUR;
					break;
				case Tile.SHAPE_INDEX_UULLRUR://16
					tileSprites[i] = tileUULLRUR;
					break;
				case Tile.SHAPE_INDEX_UDDRRUR://17
					tileSprites[i] = tileUDDRRUR;
					break;
				case Tile.SHAPE_INDEX_LLDDDRR://18
					tileSprites[i] = tileLLDDDRR;
					break;
				case Tile.SHAPE_INDEX_UULLLDD://19
					tileSprites[i] = tileUULLLDD;
					break;
				case Tile.SHAPE_INDEX_LLDDR://20
					tileSprites[i] = tileLLDDR;
					break;
				case Tile.SHAPE_INDEX_LDDRR://21
					tileSprites[i] = tileLDDRR;
					break;
				case Tile.SHAPE_INDEX_UULLR://22
					tileSprites[i] = tileUULLR;
					break;
				case Tile.SHAPE_INDEX_ULRUR://23
					tileSprites[i] = tileULRUR;
					break;
				case Tile.SHAPE_INDEX_UDRUR://24
					tileSprites[i] = tileUDRUR;
					break;
				case Tile.SHAPE_INDEX_UULLD://25
					tileSprites[i] = tileUULLD;
					break;
				case Tile.SHAPE_INDEX_UDDRR://26
					tileSprites[i] = tileUDDRR;
					break;
				case Tile.SHAPE_INDEX_ULLDD://27
					tileSprites[i] = tileULLDD;
					break;
				case Tile.SHAPE_INDEX_ULDR://28
					tileSprites[i] = tileULDR;
					break;
				case Tile.SHAPE_INDEX_URUR://29
					tileSprites[i] = tileURUR;
					break;
				case Tile.SHAPE_INDEX_DDRR://30
					tileSprites[i] = tileDDRR;
					break;
				case Tile.SHAPE_INDEX_LLDD://31
					tileSprites[i] = tileLLDD;
					break;
				case Tile.SHAPE_INDEX_UULL://32
					tileSprites[i] = tileUULL;
					break;
				case Tile.SHAPE_INDEX_LDR://33
					tileSprites[i] = tileLDR;
					break;
				case Tile.SHAPE_INDEX_UDR://34
					tileSprites[i] = tileUDR;
					break;
				case Tile.SHAPE_INDEX_ULR://35
					tileSprites[i] = tileULR;
					break;
				case Tile.SHAPE_INDEX_ULD://36
					tileSprites[i] = tileULD;
					break;
				case Tile.SHAPE_INDEX_DR://37
					tileSprites[i] = tileDR;
					break;
				case Tile.SHAPE_INDEX_LR://38
					tileSprites[i] = tileLR;
					break;
				case Tile.SHAPE_INDEX_LD://39
					tileSprites[i] = tileLD;
					break;
				case Tile.SHAPE_INDEX_UR://40
					tileSprites[i] = tileUR;
					break;
				case Tile.SHAPE_INDEX_UD://41
					tileSprites[i] = tileUD;
					break;
				case Tile.SHAPE_INDEX_UL://42
					tileSprites[i] = tileUL;
					break;
				case Tile.SHAPE_INDEX_R://43
					tileSprites[i] = tileR;
					break;
				case Tile.SHAPE_INDEX_D://44
					tileSprites[i] = tileD;
					break;
				case Tile.SHAPE_INDEX_L://45
					tileSprites[i] = tileL;
					break;
				case Tile.SHAPE_INDEX_U://46
					tileSprites[i] = tileU;
					break;
				case Tile.SHAPE_INDEX_SEPARATED://47
					tileSprites[i] = tileSeparated;
					break;
				case Tile.SHAPE_INDEX_TUULL://48
					tileSprites[i] = tileTriUULL;
					break;
				case Tile.SHAPE_INDEX_TURUR://49
					tileSprites[i] = tileTriURUR;
					break;
				case Tile.SHAPE_INDEX_TLLDD://50
					tileSprites[i] = tileTriLLDD;
					break;
				case Tile.SHAPE_INDEX_TDDRR://51
					tileSprites[i] = tileTriDDRR;
					break;
			}
		}

		tileWidth = 0f;
		tileHeight = 0f;
		if( tileConnected != null )
		{
			tile = tileConnected;
		}
		else
		{
			tile = tileSeparated;
		}
		if( tile != null )
		{
			tileWidth = tile.bounds.size.x;
			tileHeight = tile.bounds.size.y;
		}
	}

	public GameObject GetTileByNeighborhood( bool U = false, bool UL = false, bool L = false, bool LD = false, bool D = false,
		bool DR = false, bool R = false, bool UR = false, bool T = false, bool withCollider = true )
	{
		int shapeIndex = -1;
		Tile tileComponent = null;

		if( tileSprites == null )
		{
			return null;
		}
		shapeIndex = Tile.GetShapeIndexByNeighborhood( U, UL, L, LD, D, DR, R, UR, T );
		return GetTileByShapeIndex( shapeIndex, 0, withCollider, ref tileComponent );
	}

	public GameObject GetTileByShapeIndex( int shapeIndex, int textureVariation, bool withCollider, ref Tile tileComponent )
	{
		Sprite tileSprite = null;
		GameObject tileObject = null;
		SpriteRenderer tileSpriteRenderer = null;
		Sprite alternateSprite = null;

		tileComponent = null;
		if( tileSprites == null )
		{
			return null;
		}
		if( (shapeIndex < 0) || (shapeIndex >= tileSprites.Length) )
		{
			return null;
		}
		tileSprite = tileSprites[shapeIndex];
		if( HasTextureVariation( shapeIndex, textureVariation, ref alternateSprite ) )
		{
			tileSprite = alternateSprite;
		}
		if( tileSprite != null )
		{
			tileObject = new GameObject( "Tile" );
			tileObject.transform.position = Vector3.zero;
			tileObject.transform.rotation = Quaternion.identity;
			tileObject.transform.localScale = Vector3.one;
			tileComponent = tileObject.AddComponent<Tile>();
			if( tileComponent != null )
			{
				tileComponent.SetShapeIndex( shapeIndex );
				tileSpriteRenderer = tileObject.AddComponent<SpriteRenderer>();
				tileComponent.SetTileRenderer( tileSpriteRenderer );
				tileComponent.SetSprite( tileSprite );
				if( withCollider )
				{
					AddPropperCollider( tileComponent );
				}
			}
			return tileObject;
		}
		return null;
	}

	public bool HasTextureVariation( int shapeIndex, int textureVariation, ref Sprite alternateSprite )
	{
		alternateSprite = null;
		if( textureVariation < 1 )
		{
			return false;
		}
		switch( shapeIndex )
		{
			case Tile.SHAPE_INDEX_CONNECTED:
				if( tileConnectedVariations != null )
				{
					textureVariation--;
					if( textureVariation < tileConnectedVariations.Length )
					{
						alternateSprite = tileConnectedVariations[textureVariation];
					} 
				}
				break;
			case Tile.SHAPE_INDEX_UULLLDD:
				if( tileUULLLDDVariations != null )
				{
					textureVariation--;
					if( textureVariation < tileUULLLDDVariations.Length )
					{
						alternateSprite = tileUULLLDDVariations[textureVariation];
					}
				}
				break;
			case Tile.SHAPE_INDEX_LLDDDRR:
				if( tileLLDDDRRVariations != null )
				{
					textureVariation--;
					if( textureVariation < tileLLDDDRRVariations.Length )
					{
						alternateSprite = tileLLDDDRRVariations[textureVariation];
					}
				}
				break;
			case Tile.SHAPE_INDEX_UDDRRUR:
				if( tileUDDRRURVariations != null )
				{
					textureVariation--;
					if( textureVariation < tileUDDRRURVariations.Length )
					{
						alternateSprite = tileUDDRRURVariations[textureVariation];
					}
				}
				break;
			case Tile.SHAPE_INDEX_UULLRUR:
				if( tileUULLRURVariations != null )
				{
					textureVariation--;
					if( textureVariation < tileUULLRURVariations.Length )
					{
						alternateSprite = tileUULLRURVariations[textureVariation];
					}
				}
				break;
		}
		return (alternateSprite != null);
	}

	public Vector2 GetTileWorldDimensions()
	{
		return new Vector2( tileWidth, tileHeight );
	}

	public void AddPropperCollider( Tile tileComponent )
	{
		GameObject tileObject = null;
		BoxCollider2D tileColliderBox = null;
		PolygonCollider2D tileColliderPolygon = null;
		Vector2[] trianglePoints = null;
		int shapeIndex = -1;

		if( tileComponent != null )
		{
			RemoveColliders( tileComponent );
			tileObject = tileComponent.gameObject;
			if( !tileComponent.IsTriangle() )
			{
				tileColliderBox = tileObject.AddComponent<BoxCollider2D>();
				if( tileColliderBox != null )
				{
					tileColliderBox.sharedMaterial = material;
					tileColliderBox.offset = Vector2.zero;
					tileColliderBox.size = new Vector2( tileWidth, tileHeight );
					tileComponent.SetColliderBox( tileColliderBox );
				}
			}
			else
			{
				tileColliderPolygon = tileObject.AddComponent<PolygonCollider2D>();
				if( tileColliderPolygon != null )
				{
					tileColliderPolygon.sharedMaterial = material;
					tileColliderPolygon.offset = Vector2.zero;
					trianglePoints = new Vector2[3];
					shapeIndex = tileComponent.GetShapeIndex();
					switch( shapeIndex )
					{
						case Tile.SHAPE_INDEX_TUULL:
							trianglePoints[0] = new Vector2( -tileWidth/2f, tileHeight/2f );
							trianglePoints[1] = new Vector2( tileWidth/2f, tileHeight/2f );
							trianglePoints[2] = new Vector2( -tileWidth/2f, -tileHeight/2f );
							break;
						case Tile.SHAPE_INDEX_TURUR:
							trianglePoints[0] = new Vector2( -tileWidth/2f, tileHeight/2f );
							trianglePoints[1] = new Vector2( tileWidth/2f, tileHeight/2f );
							trianglePoints[2] = new Vector2( tileWidth/2f, -tileHeight/2f );
							break;
						case Tile.SHAPE_INDEX_TLLDD:
							trianglePoints[0] = new Vector2( -tileWidth/2f, tileHeight/2f );
							trianglePoints[1] = new Vector2( -tileWidth/2f, -tileHeight/2f );
							trianglePoints[2] = new Vector2( tileWidth/2f, -tileHeight/2f );
							break;
						case Tile.SHAPE_INDEX_TDDRR:
							trianglePoints[0] = new Vector2( tileWidth/2f, tileHeight/2f );
							trianglePoints[1] = new Vector2( -tileWidth/2f, -tileHeight/2f );
							trianglePoints[2] = new Vector2( tileWidth/2f, -tileHeight/2f );
							break;
					}
					tileColliderPolygon.points = trianglePoints;
					tileComponent.SetColliderPolygon( tileColliderPolygon );
				}
			}
		}
	}

	public static void RemoveColliders( Tile tileComponent )
	{
		BoxCollider2D tileColliderBox = null;
		PolygonCollider2D tileColliderPolygon = null;

		if( tileComponent != null )
		{
			tileColliderBox = tileComponent.GetColliderBox();
			if( tileColliderBox != null )
			{
				tileComponent.SetColliderBox( null );
				Destroy( tileColliderBox );
				tileColliderBox = null;
			}
			tileColliderPolygon = tileComponent.GetColliderPolygon();
			if( tileColliderPolygon != null )
			{
				tileComponent.SetColliderPolygon( null );
				Destroy( tileColliderPolygon );
				tileColliderPolygon = null;
			}
		}
	}
}
