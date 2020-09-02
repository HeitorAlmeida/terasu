using UnityEngine;
using System.Collections;

public class BackgroundController
{
	private int totalPlanes;
	private Sprite[] planeSprites;
	private float[] planeCameraConnections;
	private bool[] planeContinuities;
	private float[] planeSpeeds;
	private int[] planeObjectRows;
	private int[] planeObjectColumns;
	private int[] planeObjectTotals;
	private GameObject[][] planeObjects;
	private SpriteRenderer[][] planeRenderers;
	private CameraController cameraController;
	private float cameraMaxWorldWidth;
	private float cameraMaxWorldHeight;
	private float cameraMaxHalfWorldWidth;
	private float cameraMaxHalfWorldHeight;
	private bool cameraDimensionsValid;
	private BackgroundDatabase backgroundDatabase;
	private int currentBackgroundID;
	private Vector2 backgroundOriginPoint;
	private GameObject backgroundParent;

	public BackgroundController()
	{
		totalPlanes = 0;
		planeSprites = null;
		planeCameraConnections = null;
		planeContinuities = null;
		planeSpeeds = null;
		planeObjectRows = null;
		planeObjectColumns = null;
		planeObjectTotals = null;
		planeObjects = null;
		planeRenderers = null;
		cameraController = null;
		cameraMaxWorldWidth = 0f;
		cameraMaxWorldHeight = 0f;
		cameraMaxHalfWorldWidth = 0f;
		cameraMaxHalfWorldHeight = 0f;
		cameraDimensionsValid = false;
		backgroundDatabase = null;
		currentBackgroundID = BackgroundDatabase.INVALID_BACKGROUND_ID;
		backgroundOriginPoint = Vector2.zero;
		backgroundParent = null;
		backgroundParent = new GameObject("BackgroundParent");
		backgroundParent.transform.position = Vector3.zero;
	}

	public void SetCameraController( CameraController newCameraController )
	{
		ClearPlanes();
		cameraDimensionsValid = false;
		cameraController = newCameraController;
		if( cameraController != null )
		{
			if( cameraController.GetCameraMaxWorldDimensions( ref cameraMaxWorldWidth, ref cameraMaxWorldHeight ) )
			{
				cameraMaxHalfWorldWidth = cameraMaxWorldWidth / 2f;
				cameraMaxHalfWorldHeight = cameraMaxWorldHeight / 2f;
				cameraDimensionsValid = true;
			}
		}
		if( !cameraDimensionsValid )
		{
			cameraMaxWorldWidth = 0f;
			cameraMaxWorldHeight = 0f;
			cameraMaxHalfWorldWidth = 0f;
			cameraMaxHalfWorldHeight = 0f;
		}
	}

	public void SetBackgroundDatabase( BackgroundDatabase newBackgroundDatabase )
	{
		backgroundDatabase = newBackgroundDatabase;
	}

	public void SetBackgroundOriginPoint( Vector2 newBackgroundOriginPoint )
	{
		backgroundOriginPoint = newBackgroundOriginPoint;
	}

	public void SetBackground( int newBackgroundID )
	{
		BackgroundPlane[] planesToLoad = null;
		BackgroundPlane plane = null;

		if( newBackgroundID != currentBackgroundID )
		{
			ClearPlanes();
			currentBackgroundID = newBackgroundID;
			if( backgroundDatabase != null )
			{
				planesToLoad = backgroundDatabase.GetBackgroundPlanes( currentBackgroundID );
				if( planesToLoad != null )
				{
					for( int i=0; i<planesToLoad.Length; i++ )
					{
						plane = planesToLoad[i];
						if( plane != null )
						{
							AddPlane( plane.planeSprite, plane.planeCameraConnection, plane.planeContinuity, plane.planeIndependentSpeed );
						}
					}
				}
			}
		}
	}

	private bool AddPlane( Sprite planeSprite, float cameraConnection, bool planeContinuity, float planeSpeed )
	{
		int planeIndex = -1;
		float planeDepth = 0f;
		float spriteWidth = 0f;
		float spriteHeight = 0f;
		int spriteRows = 0;
		int spriteColumns = 0;
		int spriteTotal = 0;
		GameObject newPlaneObject = null;
		GameObject[] newPlaneObjects = null;
		SpriteRenderer newPlaneRenderer = null;
		SpriteRenderer[] newPlaneRenderers = null;

		if( (planeSprite == null) || !cameraDimensionsValid || (cameraController == null) || (backgroundParent == null) )
		{
			return false;
		}
		cameraConnection = (cameraConnection < 0f ? 0f : cameraConnection);
		cameraConnection = (cameraConnection > 1f ? 1f : cameraConnection);
		planeIndex = totalPlanes;
		totalPlanes++;
		UsefulFunctions.IncreaseArray<Sprite>( ref planeSprites, planeSprite );
		UsefulFunctions.IncreaseArray<float>( ref planeCameraConnections, cameraConnection );
		UsefulFunctions.IncreaseArray<bool>( ref planeContinuities, planeContinuity );
		UsefulFunctions.IncreaseArray<float>( ref planeSpeeds, planeSpeed );

		/*halmeida - need to find out how many times I must repeat the sprite across the screen to cover it.*/
		spriteWidth = planeSprite.bounds.size.x;
		if( !planeContinuity )
		{
			spriteColumns = 1;
			spriteRows = 1;
		}
		else
		{
			if( spriteWidth >= cameraMaxWorldWidth )
			{
				spriteColumns = 2;
			}
			else
			{
				spriteColumns = (int)(cameraMaxWorldWidth / spriteWidth);
				spriteColumns += 2;
			}
			spriteHeight = planeSprite.bounds.size.y;
			if( spriteHeight >= cameraMaxWorldHeight )
			{
				spriteRows = 2;
			}
			else
			{
				spriteRows = (int)(cameraMaxWorldHeight / spriteHeight);
				spriteRows += 2;
			}
		}
		spriteTotal = spriteRows * spriteColumns;
		UsefulFunctions.IncreaseArray<int>( ref planeObjectRows, spriteRows );
		UsefulFunctions.IncreaseArray<int>( ref planeObjectColumns, spriteColumns );
		UsefulFunctions.IncreaseArray<int>( ref planeObjectTotals, spriteTotal );
		planeDepth = DisplayDepthManager.GetElementDepth( DisplayDepthManager.ELEMENT_CODE_BACKGROUND + planeIndex );
		for( int i=0; i<spriteTotal; i++ )
		{
			newPlaneObject = new GameObject("BGPlane"+planeIndex+"Object"+i);
			newPlaneObject.transform.SetParent( backgroundParent.transform, false );
			newPlaneObject.transform.localPosition = new Vector3( backgroundOriginPoint.x, backgroundOriginPoint.y, planeDepth );
			newPlaneRenderer = newPlaneObject.AddComponent<SpriteRenderer>();
			newPlaneRenderer.sprite = planeSprite;
			UsefulFunctions.IncreaseArray<GameObject>( ref newPlaneObjects, newPlaneObject );
			UsefulFunctions.IncreaseArray<SpriteRenderer>( ref newPlaneRenderers, newPlaneRenderer );
		}
		UsefulFunctions.IncreaseArray<GameObject[]>( ref planeObjects, newPlaneObjects );
		UsefulFunctions.IncreaseArray<SpriteRenderer[]>( ref planeRenderers, newPlaneRenderers );
		return true;
	}

	public void ClearPlanes()
	{
		SpriteRenderer renderer = null;
		SpriteRenderer[] renderArray = null;
		GameObject planeObject = null;
		GameObject[] planeObjectArray = null;

		if( planeRenderers != null )
		{
			for( int i=0; i<planeRenderers.Length; i++ )
			{
				renderArray = planeRenderers[i];
				if( renderArray != null )
				{
					for( int j=0; j<renderArray.Length; j++ )
					{
						renderer = renderArray[j];
						if( renderer != null )
						{
							renderer.sprite = null;
							renderer = null;
							renderArray[j] = null;
						}
					}
					renderArray = null;
					planeRenderers[i] = null;
				}
			}
			planeRenderers = null;
		}
		if( planeObjects != null )
		{
			for( int i=0; i<planeObjects.Length; i++ )
			{
				planeObjectArray = planeObjects[i];
				if( planeObjectArray != null )
				{
					for( int j=0; j<planeObjectArray.Length; j++ )
					{
						planeObject = planeObjectArray[j];
						if( planeObject != null )
						{
							GameObject.Destroy( planeObject );
							planeObject = null;
							planeObjectArray[j] = null;
						}
					}
					planeObjectArray = null;
					planeObjects[i] = null;
				}
			}
			planeObjects = null;
		}
		planeObjectRows = null;
		planeObjectColumns = null;
		planeObjectTotals = null;
		planeSpeeds = null;
		planeContinuities = null;
		planeCameraConnections = null;
		if( planeSprites != null )
		{
			for( int i=0; i<planeSprites.Length; i++ )
			{
				planeSprites[i] = null;
			}
			planeSprites = null;
		}
		totalPlanes = 0;
	}

	public void FixedProgress( float timeStep )
	{
		Vector3 cameraPosition = Vector3.zero;
		Vector2 camera2DPosition = Vector2.zero;
		Vector2 cameraOffset = Vector2.zero;
		Vector3 objectPosition = Vector3.zero;
		Sprite planeSprite = null;
		GameObject[] planeObjectArray = null;
		GameObject planeObject = null;
		float cameraConnection = 0f;
		bool continuity = false;
		float placementX = 0f;
		float placementY = 0f;
		float cameraMinX = 0f;
		float cameraMaxX = 0f;
		float cameraMinY = 0f;
		float cameraMaxY = 0f;
		int objectRows = 0;
		int objectColumns = 0;

		if( (cameraController != null) && (totalPlanes > 0) )
		{
			cameraPosition = cameraController.GetCameraObjectPosition();
			camera2DPosition = new Vector2( cameraPosition.x, cameraPosition.y );
			cameraOffset = camera2DPosition - backgroundOriginPoint;
			cameraMinX = camera2DPosition.x - cameraMaxHalfWorldWidth;
			cameraMaxX = camera2DPosition.x + cameraMaxHalfWorldWidth;
			cameraMinY = camera2DPosition.y - cameraMaxHalfWorldHeight;
			cameraMaxY = camera2DPosition.y + cameraMaxHalfWorldHeight;
			for( int i=0; i<totalPlanes; i++ )
			{
				planeSprite = planeSprites[i];
				if( planeSprite != null )
				{
					cameraConnection = planeCameraConnections[i];
					continuity = planeContinuities[i];
					placementX = backgroundOriginPoint.x + (cameraConnection * cameraOffset.x);
					placementY = backgroundOriginPoint.y + (cameraConnection * cameraOffset.y);
					if( continuity )
					{
						placementX = FindFirstPlacement( cameraMinX, cameraMaxX, placementX, planeSprite.bounds.extents.x );
						placementY = FindFirstPlacement( cameraMinY, cameraMaxY, placementY, planeSprite.bounds.extents.y );
					}
					objectRows = planeObjectRows[i];
					objectColumns = planeObjectColumns[i];
					planeObjectArray = planeObjects[i];
					if( planeObjectArray != null )
					{
						for( int j=0; j<objectRows; j++ )
						{
							for( int k=0; k<objectColumns; k++ )
							{
								planeObject = planeObjectArray[(j * objectColumns) + k];
								if( planeObject != null )
								{
									objectPosition = planeObject.transform.localPosition;
									objectPosition.x = placementX + (k * planeSprite.bounds.size.x);
									objectPosition.y = placementY + (j * planeSprite.bounds.size.y);
									planeObject.transform.localPosition = objectPosition;
								}
							}
						}
					}
				}
			}
		}
	}

	private float FindFirstPlacement( float cameraMinBound, float cameraMaxBound, float modelSpriteCenter, float modelSpriteExtent )
	{
		float modelSpriteMaxBound = 0f;
		float modelSpriteSize = 0f;

		modelSpriteMaxBound = modelSpriteCenter + modelSpriteExtent;
		modelSpriteSize = 2f * modelSpriteExtent;
		while( modelSpriteMaxBound > cameraMinBound )
		{
			modelSpriteCenter -= modelSpriteSize;
			modelSpriteMaxBound = modelSpriteCenter + modelSpriteExtent;
		}
		while( modelSpriteMaxBound <= cameraMinBound )
		{
			modelSpriteCenter += modelSpriteSize;
			modelSpriteMaxBound = modelSpriteCenter + modelSpriteExtent;
		}
		return modelSpriteCenter;
	}

	public void Clear()
	{
		SetCameraController( null );
		backgroundDatabase = null;
		currentBackgroundID = BackgroundDatabase.INVALID_BACKGROUND_ID;
		if( backgroundParent != null )
		{
			GameObject.Destroy( backgroundParent );
			backgroundParent = null;
		}
	}
}
