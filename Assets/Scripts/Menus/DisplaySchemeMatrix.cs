using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisplaySchemeMatrixNew", menuName = "Matrix Display Scheme")]
public class DisplaySchemeMatrix : ScriptableObject
{
	public GameObject buttonElementModel;
	public Vector2 buttonElementScale;
	public GameObject buttonReturnModel;
	public Vector2 buttonReturnScale;
	public GameObject buttonAdvanceModel;
	public Vector2 buttonAdvanceScale;
	public Vector2[] elementPositionRates;
	public Vector2 returnPositionRates;
	public Vector2 advancePositionRates;
	public Vector2[] elementContentOffsetRates;
	public int elementsScrolled;
}
