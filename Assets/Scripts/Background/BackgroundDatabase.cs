using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDatabase : MonoBehaviour
{
	public const int INVALID_BACKGROUND_ID = -1;

	public GameObject[] backgroundPlaneSets;

	private BackgroundPlaneSet[] backgroundComponents;
	private int[] backgroundIDs;
	private BackgroundPlane[][] backgroundPlanes;

	void Awake()
	{
		backgroundComponents = null;
		backgroundIDs = null;
		backgroundPlanes = null;

		/*halmeida - I will already initialize everything because the BackgroundPlaneSets and their BackgroundPlanes have no
		Awake methods, which means I don't need to wait for them to do anything before I can analyze them.*/
		ExtractAllBackgroundComponents();
	}

	private void ExtractAllBackgroundComponents()
	{
		int totalBackgrounds = 0;
		GameObject backgroundObject = null;
		BackgroundPlaneSet backgroundComponent = null;
		GameObject[] backgroundPlaneObjects = null;
		GameObject planeObject = null;
		BackgroundPlane[] planesOfBackground = null;
		int totalPlanes = 0;

		if( backgroundPlaneSets != null )
		{
			totalBackgrounds = backgroundPlaneSets.Length;
			if( totalBackgrounds > 0 )
			{
				backgroundComponents = new BackgroundPlaneSet[totalBackgrounds];
				backgroundIDs = new int[totalBackgrounds];
				backgroundPlanes = new BackgroundPlane[totalBackgrounds][];
				for( int i=0; i<totalBackgrounds; i++ )
				{
					backgroundObject = backgroundPlaneSets[i];
					if( backgroundObject != null )
					{
						backgroundComponent = backgroundObject.GetComponent<BackgroundPlaneSet>();
					}
					if( backgroundComponent != null )
					{
						backgroundComponents[i] = backgroundComponent;
						backgroundIDs[i] = backgroundComponent.backgroundID;
						backgroundPlaneObjects = backgroundComponent.backgroundPlanes;
						if( backgroundPlaneObjects != null )
						{
							totalPlanes = backgroundPlaneObjects.Length;
							if( totalPlanes > 0 )
							{
								planesOfBackground = new BackgroundPlane[totalPlanes];
								for( int j=0; j<totalPlanes; j++ )
								{
									planeObject = backgroundPlaneObjects[j];
									if( planeObject != null )
									{
										planesOfBackground[j] = planeObject.GetComponent<BackgroundPlane>();
									}
									else
									{
										planesOfBackground[j] = null;
									}
								}
							}
						}
						backgroundPlanes[i] = planesOfBackground;
					}
					else
					{
						backgroundComponents[i] =  null;
						backgroundIDs[i] = INVALID_BACKGROUND_ID;
						backgroundPlanes[i] = null;
					}
					backgroundComponent = null;
					planesOfBackground = null;
				}
			}
		}
	}

	public BackgroundPlane[] GetBackgroundPlanes( int backgroundID )
	{
		if( (backgroundIDs != null) && (backgroundID != INVALID_BACKGROUND_ID) )
		{
			for( int i=0; i<backgroundIDs.Length; i++ )
			{
				if( backgroundIDs[i] == backgroundID )
				{
					/*halmeida - relying on the coherence of all the "background" arrays.*/
					return backgroundPlanes[i];
				}
			}
		}
		return null;
	}
}
