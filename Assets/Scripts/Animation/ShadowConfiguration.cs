using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowConfiguration : MonoBehaviour
{
	public float creationInterval;
	public bool fixedPosition;
	public bool fixedSprite;
	public Material renderingMaterial;
	public float alphaOrigin;
	public float[] alphaTargets;
	public float[] fadeDurations;
	public Vector3 scaleOrigin;
	public Vector3[] scaleTargets;
	public float[] scalingDurations;
	public Color[] outlineColors;
	public float outlineColorInterval;
	public float outlineWidth;
	public float outlineAlphaOrigin;
	public float[] outlineAlphaTargets;
	public float[] outlineFadeDurations;
	public Vector3 contentColorAddition;
	public float totalDuration;

	public void Copy( ShadowConfiguration otherConfig )
	{
		if( otherConfig != null )
		{
			creationInterval = otherConfig.creationInterval;
			fixedPosition = otherConfig.fixedPosition;
			fixedSprite = otherConfig.fixedSprite;
			renderingMaterial = otherConfig.renderingMaterial;
			alphaOrigin = otherConfig.alphaOrigin;
			alphaTargets = null;
			if( otherConfig.alphaTargets != null )
			{
				alphaTargets = (float[]) otherConfig.alphaTargets.Clone();
			}
			fadeDurations = null;
			if( otherConfig.fadeDurations != null )
			{
				fadeDurations = (float[]) otherConfig.fadeDurations.Clone();
			}
			scaleOrigin = otherConfig.scaleOrigin;
			scaleTargets = null;
			if( otherConfig.scaleTargets != null )
			{
				scaleTargets = (Vector3[]) otherConfig.scaleTargets.Clone();
			}
			scalingDurations = null;
			if( otherConfig.scalingDurations != null )
			{
				scalingDurations = (float[]) otherConfig.scalingDurations.Clone();
			}
			outlineColors = null;
			if( otherConfig.outlineColors != null )
			{
				outlineColors = (Color[]) otherConfig.outlineColors.Clone();
			}
			outlineColorInterval = otherConfig.outlineColorInterval;
			outlineWidth = otherConfig.outlineWidth;
			outlineAlphaOrigin = otherConfig.outlineAlphaOrigin;
			outlineAlphaTargets = null;
			if( otherConfig.outlineAlphaTargets != null )
			{
				outlineAlphaTargets = (float[]) otherConfig.outlineAlphaTargets.Clone();
			}
			outlineFadeDurations = null;
			if( otherConfig.outlineFadeDurations != null )
			{
				outlineFadeDurations = (float[]) otherConfig.outlineFadeDurations.Clone();
			}
			contentColorAddition = otherConfig.contentColorAddition;
			totalDuration = otherConfig.totalDuration;
		}
	}
}
