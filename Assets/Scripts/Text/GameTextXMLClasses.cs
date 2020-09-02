using System.Xml.Serialization;

/*halmeida - classes implemented using C#'s resource called auto-properties. These are
properties with compiler generated backing fields. The difference between auto-properties
and public fields is that if, for some reason, a field that was public has to be made
private to implement some getting or setting logic, the change will break the interface.
Other classes that used that variable as public will need to be rewritten. When we use
properties, the backing field is always private, the way to access it is always public,
and there is always the possiblity of inserting some logic at that point.*/

[XmlRoot("gameText")]
public class GameText
{
	[XmlArray("platformTexts")]
	[XmlArrayItem("ptext")]
	public PlatformText[] Platform { get; set; }

	[XmlArray("systemTexts")]
	[XmlArrayItem("stext")]
	public SystemText[] System { get; set; }

	[XmlArray("dialogueTexts")]
	[XmlArrayItem("dtext")]
	public DialogueText[] Dialogue { get; set; }

	[XmlArray("itemTexts")]
	[XmlArrayItem("itext")]
	public ItemText[] Items { get; set; }

	public void Clear()
	{
		PlatformText ptext = null;
		SystemText stext = null;
		DialogueText dtext = null;
		ItemText itext = null;

		if( Platform != null )
		{
			for( int i=0; i<Platform.Length; i++ )
			{
				ptext = Platform[i];
				if( ptext != null )
				{
					ptext.Clear();
					Platform[i] = null;
				}
			}
			Platform = null;
		}
		if( System != null )
		{
			for( int i=0; i<System.Length; i++ )
			{
				stext = System[i];
				if( stext != null )
				{
					stext.Clear();
					System[i] = null;
				}
			}
			System = null;
		}
		if( Dialogue != null )
		{
			for( int i=0; i<Dialogue.Length; i++ )
			{
				dtext = Dialogue[i];
				if( dtext != null )
				{
					dtext.Clear();
					Dialogue[i] = null;
				}
			}
			Dialogue = null;
		}
		if( Items != null )
		{
			for( int i=0; i<Items.Length; i++ )
			{
				itext = Items[i];
				if( itext != null )
				{
					itext.Clear();
					Items[i] = null;
				}
			}
			Items = null;
		}
	}
}

public class PlatformText
{
	[XmlAttribute("id")]
	public int Id { get; set; }

	[XmlAttribute("platformId")]
	public int PlatformId { get; set; }

	[XmlAttribute("autoClose")]
	public bool AutoClose { get; set; }

	[XmlElement("text")]
	public string Text { get; set; }

	public void Clear()
	{
		Text = null;
	}
}

public class SystemText
{
	[XmlAttribute("id")]
	public int Id { get; set; }

	[XmlAttribute("autoClose")]
	public bool AutoClose { get; set; }

	[XmlElement("text")]
	public string Text { get; set; }

	public void Clear()
	{
		Text = null;
	}
}

public class DialogueText
{
	[XmlAttribute("id")]
	public int Id { get; set; }

	[XmlAttribute("speakerName")]
	public string SpeakerName { get; set; }

	[XmlAttribute("tryAbove")]
	public bool TryAbove { get; set; }

	[XmlAttribute("autoClose")]
	public bool AutoClose { get; set; }

	[XmlElement("text")]
	public string Text { get; set; }

	public void Clear()
	{
		SpeakerName = null;
		Text = null;
	}
}

public class ItemText
{
	[XmlAttribute("id")]
	public int Id { get; set; }

	[XmlAttribute("name")]
	public string Name { get; set; }

	[XmlElement("text")]
	public string Description { get; set; }

	public void Clear()
	{
		Name = null;
		Description = null;
	}
}