using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class UICreator
{
	/*halmeida - The difference between "ref" and "out":
	when you use out, you are obliged to attribute a value to the variable inside the method, before using it inside the method.
	This means that you will necessarily lose the value with witch the variable entered the method. When you use ref, you are
	obliged to attribute a value to the variable before entering the method, which means you are able to get the variable's value
	from outside the method.*/

	public static bool CreateImage( GameObject imagePrefab, ref GameObject imageObject, ref Image imageComponent, GameObject interfaceCanvas )
	{
		if( interfaceCanvas != null )
		{
			if( (imagePrefab != null) && (imageObject == null) )
			{
				imageObject = GameObject.Instantiate( imagePrefab ) as GameObject;
				imageComponent = imageObject.GetComponent<Image>();
				if( imageComponent != null )
				{
					imageObject.transform.SetParent ( interfaceCanvas.transform, false );
					return true;
				}
				else
				{
					GameObject.Destroy( imageObject );
					imageObject = null;
				}
			}
		}
		return false;
	}

	public static void DestroyImage( ref GameObject imageObject, ref Image imageComponent )
	{
		imageComponent = null;
		if( imageObject != null )
		{
			GameObject.Destroy( imageObject );
			imageObject = null;
		}
	}

	public static bool CreateText( GameObject textPrefab, ref GameObject textObject, ref Text textComponent, GameObject interfaceCanvas )
	{
		if( interfaceCanvas != null )
		{
			if( (textPrefab != null) && (textObject == null) )
			{
				textObject = GameObject.Instantiate( textPrefab ) as GameObject;
				textComponent = textObject.GetComponent<Text>();
				if( textComponent != null )
				{
					textObject.transform.SetParent( interfaceCanvas.transform, false );
					return true;
				}
				else
				{
					GameObject.Destroy( textObject );
					textObject = null;
				}
			}
		}
		return false;
	}

	public static void DestroyText( ref GameObject textObject, ref Text textComponent )
	{
		textComponent = null;
		if( textObject != null )
		{
			GameObject.Destroy( textObject );
			textObject = null;
		}
	}

	public static bool CreateButton( GameObject buttonPrefab, ref GameObject buttonObject, ref Button buttonComponent, GameObject interfaceCanvas )
	{
		if( interfaceCanvas != null )
		{
			if( (buttonPrefab != null) && (buttonObject == null) )
			{
				buttonObject = GameObject.Instantiate ( buttonPrefab ) as GameObject;
				buttonComponent = buttonObject.GetComponent<Button>();
				if( buttonComponent != null )
				{
					buttonObject.transform.SetParent ( interfaceCanvas.transform, false );
					return true;
				}
				else
				{
					GameObject.Destroy( buttonObject );
					buttonObject = null;
				}
			}
		}
		return false;
	}

	public static void DestroyButton( ref GameObject buttonObject, ref Button buttonComponent, ref UnityAction action )
	{
		if( buttonComponent != null )
		{
			buttonComponent.onClick.RemoveListener ( action );
		}
		action = null;
		buttonComponent = null;
		if( buttonObject != null )
		{
			GameObject.Destroy( buttonObject );
			buttonObject = null;
		}
	}

	public static bool CreateInputField( GameObject inputPrefab, ref GameObject inputObject, ref InputField inputComponent, GameObject interfaceCanvas )
	{
		if( interfaceCanvas != null )
		{
			if( (inputPrefab != null) && (inputObject == null) )
			{
				inputObject = GameObject.Instantiate ( inputPrefab ) as GameObject;
				inputComponent = inputObject.GetComponent<InputField>();
				if( inputComponent != null )
				{
					inputObject.transform.SetParent ( interfaceCanvas.transform, false );
					return true;
				}
				else
				{
					GameObject.Destroy( inputObject );
					inputObject = null;
				}
			}
		}
		return false;
	}

	public static void DestroyInputField( ref GameObject inputObject, ref InputField inputComponent )
	{
		inputComponent = null;
		if( inputObject != null )
		{
			GameObject.Destroy( inputObject );
			inputObject = null;
		}
	}

	public static bool CreateToggle( GameObject togglePrefab, ref GameObject toggleObject, ref Toggle toggleComponent, GameObject interfaceCanvas )
	{
		if( interfaceCanvas != null )
		{
			if( (togglePrefab != null) && (toggleObject == null) )
			{
				toggleObject = GameObject.Instantiate ( togglePrefab ) as GameObject;
				toggleComponent = toggleObject.GetComponent<Toggle>();
				if( toggleComponent != null )
				{
					toggleObject.transform.SetParent ( interfaceCanvas.transform, false );
					return true;
				}
				else
				{
					GameObject.Destroy( toggleObject );
					toggleObject = null;
				}
			}
		}
		return false;
	}

	public static void DestroyToggle( ref GameObject toggleObject, ref Toggle toggleComponent, ref UnityAction<bool> action )
	{
		if( toggleComponent != null )
		{
			toggleComponent.onValueChanged.RemoveListener( action );
		}
		action = null;
		toggleComponent = null;
		if( toggleObject != null )
		{
			GameObject.Destroy( toggleObject );
			toggleObject = null;
		}
	}
}
