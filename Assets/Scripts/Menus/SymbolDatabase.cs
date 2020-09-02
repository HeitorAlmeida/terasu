using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SymbolDatabase : MonoBehaviour
{
	public Sprite digitZero;
	public Sprite digitOne;
	public Sprite digitTwo;
	public Sprite digitThree;
	public Sprite digitFour;
	public Sprite digitFive;
	public Sprite digitSix;
	public Sprite digitSeven;
	public Sprite digitEight;
	public Sprite digitNine;
	public Sprite signMultiply;
	public Sprite signDivide;
	public Sprite signPlus;
	public Sprite signMinus;
	public Sprite signMod;
	public Sprite signExclamation;
	public Sprite signInterrogation; 
	public Sprite signPeriod;
	public Sprite signColon;
	public Sprite signDollar;
	public Sprite signComma;
	public Sprite signApostrofe;
	public Sprite signQuotes;
	public Sprite signUnderline;
	public Sprite signSpace;
	public Sprite characterA;
	public Sprite characterB;
	public Sprite characterC;
	public Sprite characterD;
	public Sprite characterE;
	public Sprite characterF;
	public Sprite characterG;
	public Sprite characterH;
	public Sprite characterI;
	public Sprite characterJ;
	public Sprite characterK;
	public Sprite characterL;
	public Sprite characterM;
	public Sprite characterN;
	public Sprite characterO;
	public Sprite characterP;
	public Sprite characterQ;
	public Sprite characterR;
	public Sprite characterS;
	public Sprite characterT;
	public Sprite characterU;
	public Sprite characterV;
	public Sprite characterW;
	public Sprite characterX;
	public Sprite characterY;
	public Sprite characterZ;
	public Sprite characterAMin;
	public Sprite characterBMin;
	public Sprite characterCMin;
	public Sprite characterDMin;
	public Sprite characterEMin;
	public Sprite characterFMin;
	public Sprite characterGMin;
	public Sprite characterHMin;
	public Sprite characterIMin;
	public Sprite characterJMin;
	public Sprite characterKMin;
	public Sprite characterLMin;
	public Sprite characterMMin;
	public Sprite characterNMin;
	public Sprite characterOMin;
	public Sprite characterPMin;
	public Sprite characterQMin;
	public Sprite characterRMin;
	public Sprite characterSMin;
	public Sprite characterTMin;
	public Sprite characterUMin;
	public Sprite characterVMin;
	public Sprite characterWMin;
	public Sprite characterXMin;
	public Sprite characterYMin;
	public Sprite characterZMin;
	public Material fontModMaterial;

	private float symbolHeightStandard;
	private float symbolHeightStandardUI;

	void Awake()
	{
		Sprite[] checker = null;
		Sprite checking = null;
		GameObject testObject = null;
		RectTransform testTransform = null;
		Image testImage = null;

		checker = new Sprite[77];
		checker[0] = digitZero;
		checker[1] = digitOne;
		checker[2] = digitTwo;
		checker[3] = digitThree;
		checker[4] = digitFour;
		checker[5] = digitFive;
		checker[6] = digitSix;
		checker[7] = digitSeven;
		checker[8] = digitEight;
		checker[9] = digitNine;
		checker[10] = signMultiply;
		checker[11] = signDivide;
		checker[12] = signPlus;
		checker[13] = signMinus;
		checker[14] = signMod;
		checker[15] = signExclamation;
		checker[16] = signInterrogation; 
		checker[17] = signPeriod;
		checker[18] = signDollar;
		checker[19] = signComma;
		checker[20] = signUnderline;
		checker[21] = signSpace;
		checker[22] = characterA;
		checker[23] = characterB;
		checker[24] = characterC;
		checker[25] = characterD;
		checker[26] = characterE;
		checker[27] = characterF;
		checker[28] = characterG;
		checker[29] = characterH;
		checker[30] = characterI;
		checker[31] = characterJ;
		checker[32] = characterK;
		checker[33] = characterL;
		checker[34] = characterM;
		checker[35] = characterN;
		checker[36] = characterO;
		checker[37] = characterP;
		checker[38] = characterQ;
		checker[39] = characterR;
		checker[40] = characterS;
		checker[41] = characterT;
		checker[42] = characterU;
		checker[43] = characterV;
		checker[44] = characterW;
		checker[45] = characterX;
		checker[46] = characterY;
		checker[47] = characterZ;
		checker[48] = characterAMin;
		checker[49] = characterBMin;
		checker[50] = characterCMin;
		checker[51] = characterDMin;
		checker[52] = characterEMin;
		checker[53] = characterFMin;
		checker[54] = characterGMin;
		checker[55] = characterHMin;
		checker[56] = characterIMin;
		checker[57] = characterJMin;
		checker[58] = characterKMin;
		checker[59] = characterLMin;
		checker[60] = characterMMin;
		checker[61] = characterNMin;
		checker[62] = characterOMin;
		checker[63] = characterPMin;
		checker[64] = characterQMin;
		checker[65] = characterRMin;
		checker[66] = characterSMin;
		checker[67] = characterTMin;
		checker[68] = characterUMin;
		checker[69] = characterVMin;
		checker[70] = characterWMin;
		checker[71] = characterXMin;
		checker[72] = characterYMin;
		checker[73] = characterZMin;
		checker[74] = signApostrofe;
		checker[75] = signQuotes;
		checker[76] = signColon;

		symbolHeightStandard = 0f;
		symbolHeightStandardUI = 0f;
		/*halmeida - the character we use as the standard height is the uppercase A.*/
		checking = checker[22];
		if( checking != null )
		{
			symbolHeightStandard = checking.bounds.size.y;
			testObject = new GameObject("UIHeightTestObject", typeof(RectTransform));
			testTransform = testObject.GetComponent<RectTransform>();
			testImage = testObject.AddComponent<Image>();
			testImage.sprite = checking;
			testImage.SetNativeSize();
			symbolHeightStandardUI = testTransform.rect.height;
			Destroy( testObject );
		}
	}

	public Sprite GetStandardSymbolSprite()
	{
		return GetSymbolSprite( 'A' );
	}

	public float GetStandardSymbolWorldHeight()
	{
		return symbolHeightStandard;
	}

	public float GetStandardSymbolUIHeight()
	{
		return symbolHeightStandardUI;
	}

	public Sprite GetSymbolSprite( char character )
	{
		switch( character )
		{
			case '0':
				return digitZero;
			case '1':
				return digitOne;
			case '2':
				return digitTwo;
			case '3':
				return digitThree;
			case '4':
				return digitFour;
			case '5':
				return digitFive;
			case '6':
				return digitSix;
			case '7':
				return digitSeven;
			case '8':
				return digitEight;
			case '9':
				return digitNine;
			case '*':
				return signMultiply;
			case '/':
				return signDivide;
			case '+':
				return signPlus;
			case '-':
				return signMinus;
			case '%':
				return signMod;
			case '!':
				return signExclamation;
			case '?':
				return signInterrogation;
			case '.':
				return signPeriod;
			case ':':
				return signColon;
			case '$':
				return signDollar;
			case ',':
				return signComma;
			case '\'':
				return signApostrofe;
			case '\"':
				return signQuotes;
			case '_':
				return signUnderline;
			case ' ':
				return signSpace;
			case 'a':
				return characterAMin;
			case 'A':
				return characterA;
			case 'b':
				return characterBMin;
			case 'B':
				return characterB;
			case 'c':
				return characterCMin;
			case 'C':
				return characterC;
			case 'd':
				return characterDMin;
			case 'D':
				return characterD;
			case 'e':
				return characterEMin;
			case 'E':
				return characterE;
			case 'f':
				return characterFMin;
			case 'F':
				return characterF;
			case 'g':
				return characterGMin;
			case 'G':
				return characterG;
			case 'h':
				return characterHMin;
			case 'H':
				return characterH;
			case 'i':
				return characterIMin;
			case 'I':
				return characterI;
			case 'j':
				return characterJMin;
			case 'J':
				return characterJ;
			case 'k':
				return characterKMin;
			case 'K':
				return characterK;
			case 'l':
				return characterLMin;
			case 'L':
				return characterL;
			case 'm':
				return characterMMin;
			case 'M':
				return characterM;
			case 'n':
				return characterNMin;
			case 'N':
				return characterN;
			case 'o':
				return characterOMin;
			case 'O':
				return characterO;
			case 'p':
				return characterPMin;
			case 'P':
				return characterP;
			case 'q':
				return characterQMin;
			case 'Q':
				return characterQ;
			case 'r':
				return characterRMin;
			case 'R':
				return characterR;
			case 's':
				return characterSMin;
			case 'S':
				return characterS;
			case 't':
				return characterTMin;
			case 'T':
				return characterT;
			case 'u':
				return characterUMin;
			case 'U':
				return characterU;
			case 'v':
				return characterVMin;
			case 'V':
				return characterV;
			case 'w':
				return characterWMin;
			case 'W':
				return characterW;
			case 'x':
				return characterXMin;
			case 'X':
				return characterX;
			case 'y':
				return characterYMin;
			case 'Y':
				return characterY;
			case 'z':
				return characterZMin;
			case 'Z':
				return characterZ;
		}
		return signInterrogation;
	}

	public Sprite GetDigitSprite( int digit )
	{
		switch( digit )
		{
			case 0:
				return digitZero;
			case 1:
				return digitOne;
			case 2:
				return digitTwo;
			case 3:
				return digitThree;
			case 4:
				return digitFour;
			case 5:
				return digitFive;
			case 6:
				return digitSix;
			case 7:
				return digitSeven;
			case 8:
				return digitEight;
			case 9:
				return digitNine;
		}
		return signInterrogation;
	}

	public float GetStringWidthWorldSpace( string characters )
	{
		float totalWidth = 0f;
		Sprite symbol = null;

		if( !string.IsNullOrEmpty(characters) )
		{
			for( int i=0; i<characters.Length; i++ )
			{
				symbol = GetSymbolSprite( characters[i] );
				if( symbol != null )
				{
					totalWidth += symbol.bounds.size.x;
				}
			}
		}
		return totalWidth;
	}

	public float GetStringWidthUI( string characters )
	{
		float totalWidth = 0f;
		Sprite symbol = null;

		if( !string.IsNullOrEmpty(characters) )
		{
			for( int i=0; i<characters.Length; i++ )
			{
				symbol = GetSymbolSprite( characters[i] );
				if( symbol != null )
				{
					totalWidth += symbol.rect.width;
				}
			}
		}
		return totalWidth;
	}
}
