public class DisplayDepthManager
{
	public const float ELEMENT_FIRST_DEPTH = 0f; /*halmeida - the higher it goes, the deeper into the screen it goes.*/
	public const float ELEMENT_TO_ELEMENT_OFFSET = 0.2f;
	
	public const int SMALLEST_ELEMENT_CODE = 0;
	public const int ELEMENT_CODE_COLLIDER_MARKER = 0;
	public const int ELEMENT_CODE_SPAWN_MARKER = 1;
	public const int ELEMENT_CODE_SIZE_MARKER = 2;
	public const int ELEMENT_CODE_TILE_DIVIDER = 3;
	public const int ELEMENT_CODE_TEXT = 4;
	public const int ELEMENT_CODE_VFX = 5;
	public const int ELEMENT_CODE_TRANSACTION = 6;
	public const int ELEMENT_CODE_ROOM_DARKENER = 7;
	public const int ELEMENT_CODE_TILE_DEBRIS = 8;
	public const int ELEMENT_CODE_WAYPOINT = 9;
	public const int ELEMENT_CODE_PLAYER = 10;
	public const int ELEMENT_CODE_ENEMY = 11;
	public const int ELEMENT_CODE_NPC = 12;
	public const int ELEMENT_CODE_ROOM_BLOCKER = 13;
	public const int ELEMENT_CODE_OBSTACLE = 14;
	public const int ELEMENT_CODE_TILE = 15;
	public const int ELEMENT_CODE_ITEM = 16;
	public const int ELEMENT_CODE_BACK_TILE = 17;
	public const int ELEMENT_CODE_BACKGROUND = 18; /*halmeida - the background needs to be the last because its depth expands limitlessly.*/
	public const int BIGGEST_ELEMENT_CODE = 18;
	
	public static float GetElementDepth( int elementCode )
	{
		float totalDepth = 0.0f;
		int steps = 0;
		
		totalDepth = ELEMENT_FIRST_DEPTH;
		steps = elementCode - SMALLEST_ELEMENT_CODE;
		totalDepth += steps * ELEMENT_TO_ELEMENT_OFFSET;
		return totalDepth;
	}
}

