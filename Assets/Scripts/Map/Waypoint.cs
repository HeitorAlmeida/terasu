using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
	public const int INVALID_WAYPOINT_ID = -1;

	private Vector2 position;

	public int ID{ get; set; }
	/*halmeida - a C# auto-property. The backing field remains unknown. Why use it?
	Clients can access ID as if it was a public variable, but they will actually be
	accessing it through methods. This way, if I want to insert any logic at the ID
	assignment later on, I will be able to do so without breaking the interface. This
	means that clients will not need to modify their code just cause I modified mine.*/

	public Waypoint( Vector2 newPosition, int newID = INVALID_WAYPOINT_ID )
	{
		position = newPosition;
		ID = newID;
	}

	public void SetPosition( Vector2 newPosition )
	{
		position = newPosition;
	}

	public Vector2 GetPosition()
	{
		return position;
	}

	public void Clear()
	{
		/*halmeida - do nothing for now. Just reserving rights to take and clear any memory.*/ 
	}
}
