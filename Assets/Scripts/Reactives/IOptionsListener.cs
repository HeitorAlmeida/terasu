using UnityEngine;
using System.Collections;

public interface IOptionsListener
{
	void ChooseOption( int optionIndex );
	bool ListensFromUI();
}