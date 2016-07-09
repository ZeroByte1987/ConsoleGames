using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;



namespace ZFC
{
	/// <summary>
	/// Defines the JSON object data types.
	/// </summary>
	public enum ZJSON_DataType
	{	
		/// <summary>
		/// Undefined (Invalid)
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Double value.
		/// </summary>
		Number	= 1,
		/// <summary>
		/// String value.
		/// </summary>
		String	= 2,
		/// <summary>
		/// Boolean value.
		/// </summary>
		Bool	= 3,
		/// <summary>
		/// Array of unnamed fields.
		/// </summary>
		Array	= 4,
		/// <summary>
		/// Object with fields.
		/// </summary>
		Object	= 5
	}



	/// <summary>
	/// This class defines a single JSON object. Also this class provides static methods for work with JSON strings and objects.
	/// </summary>
	public class ZJSON_Object
	{
		//	Public Properties
		#region
		/// <summary>
		/// Gets the type of this JSON object.
		/// </summary>
		public ZJSON_DataType	Type			{	get	{	return _type;		}}
		/// <summary>
		/// Gets or sets the name of this JSON object.
		/// </summary>
		public string			Name			{	get	{	return _name;		}	set	{	_name = value;	}}
		/// <summary>
		/// Gets or sets the number (integer of float) value of this JSON object.
		/// </summary>
		public double			ValueNum		{	get	{	return _valueNum;	}	set	{	_valueNum = value;	_type = ZJSON_DataType.Number;	}}
		/// <summary>
		/// Gets or sets the string value of this JSON object.
		/// </summary>
		public string			ValueStr		{	get	{	return _valueStr;	}	set	{	_valueStr = value;	_type = ZJSON_DataType.String;	}}
		/// <summary>
		/// Gets or sets the boolean value of this JSON object.
		/// </summary>
		public bool				ValueBool		{	get	{	return _valueBool;	}	set	{	_valueBool = value;	_type = ZJSON_DataType.Bool;	}}
		/// <summary>
		/// Gets or sets the array of objects for this JSON object, if its type is Array or Object.
		/// </summary>
		public ZJSON_ObjectList	ValueArray		{	get	{	return _valueArray;	}}
		/// <summary>
		/// Gets whether this JSON object is an element of parent JSON array object.
		/// </summary>
		public bool				IsArrayElement	{	get	{	return (_P != null  &&  _P._type == ZJSON_DataType.Array);	}}
		/// <summary>
		/// Gets whether this JSON object is a root element of the tree.
		/// </summary>
		public bool				IsRootElement	{	get	{	return _isRootElement;	}}
		#endregion

		//	Private Fields
		#region
		private ZJSON_Object	_P;
		private string			_name;
		private ZJSON_DataType	_type;
		private double			_valueNum = -1;
		private string			_valueStr;
		private bool			_valueBool;
		private ZJSON_ObjectList _valueArray;
		private bool			_isRootElement;
		#endregion


		//	Methods & Constructors
		#region
		//	Add a new child
		#region
		private ZJSON_Object	AddChild()
		{
			if (_valueArray == null)	_valueArray = new ZJSON_ObjectList();
			_valueArray.Add(new ZJSON_Object(this));
			return _valueArray.Last;
		}
		/// <summary>
		/// Adds new element to this array object.
		/// </summary>
		/// <param name="Val">Value of the new element.</param>
		/// <returns>Returns the result object.</returns>
		public ZJSON_Object		AddArrayChild(object Val)
		{	return AddChild(null, Val);	}
		/// <summary>
		///	Adds new element to this object.
		/// </summary>
		/// <param name="Name">Name of the new element.</param>
		/// <param name="Val">Value of the new element.</param>
		/// <returns>Returns the result object.</returns>
		public ZJSON_Object		AddChild(string Name, object Val)
		{
			if (_valueArray == null)	_valueArray = new ZJSON_ObjectList();
			ZJSON_Object N = new ZJSON_Object(this, Name, ZJSON_DataType.Object);
			if (Name == null)	_type = ZJSON_DataType.Array;
			this._valueStr		= null;
			this._valueBool		= false;
			this._valueNum		= 0;
			if (Val != null)
			{
				if (Val is bool)		{	N._valueBool = (bool)Val;			N._type = ZJSON_DataType.Bool;		}
				if (Val is string)		{	N._valueStr	= (string)Val;			N._type = ZJSON_DataType.String;	}
				else	try				{	N._valueNum	= Convert.ToDouble(Val); N._type = ZJSON_DataType.Number;	}
				catch					{	return null;	}
			}
			_valueArray.Add(N);
			return _valueArray.Last;
		}
		/// <summary>
		/// Adds new element of object type to this object.
		/// </summary>
		/// <param name="Name">Name of the new element.</param>
		/// <returns>Returns the result object.</returns>
		public ZJSON_Object		AddChild(string Name)
		{	return AddChild(Name, null);	}
		#endregion

		//	Get element by name
		#region
		/// <summary>
		/// Gets the value element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <returns>Returns the element with specified name.</returns>
		public ZJSON_Object			Get_Element(string Name)
		{	return Get_Element(Name, false);		}
		/// <summary>
		/// Gets the value element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <returns>Returns the element with specified name.</returns>
		public ZJSON_Object			Get_Element(string Name, bool AllLevels)
		{	return Get_Element(Name, AllLevels, false);		}	
		/// <summary>
		/// Gets the value element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <param name="MatchCase">Sets whether element name must match the case.</param>
		/// <returns>Returns the element with specified name.</returns>
		public ZJSON_Object			Get_Element(string Name, bool AllLevels, bool MatchCase)
		{
			var SC = StringComparison.InvariantCultureIgnoreCase;
			if (MatchCase)	SC = StringComparison.InvariantCulture;
			if (_valueArray == null)	return null;
			for (int i = 0; i < _valueArray.Count; i++)
			{
				var V = _valueArray[i];
				if (string.Equals(V._name, Name, SC))	return V;
				if (AllLevels  &&  V._valueArray != null)
				{
					var VV = V.Get_Element(Name, true, MatchCase);
					if (VV != null)	return VV;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <returns>Returns the element with specified name.</returns>
		public string				Get_ElementStr(string Name)
		{	return Get_ElementStr(Name, false, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <returns>Returns the element with specified name.</returns>
		public string				Get_ElementStr(string Name, bool AllLevels)
		{return Get_ElementStr(Name, AllLevels, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <param name="MatchCase">Sets whether element name must match the case.</param>
		/// <returns>Returns the element with specified name.</returns>
		public string				Get_ElementStr(string Name, bool AllLevels, bool MatchCase)
		{
			var El = Get_Element(Name, AllLevels, MatchCase);
			if (El == null)	 return null;
			return El.ValueStr;
		}
		
		/// <summary>
		/// Gets the double value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <returns>Returns the element with specified name.</returns>
		public double				Get_ElementDouble(string Name)
		{	return Get_ElementDouble(Name, false, false);	}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <returns>Returns the element with specified name.</returns>
		public double				Get_ElementDouble(string Name, bool AllLevels)
		{	return Get_ElementDouble(Name, AllLevels, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <param name="MatchCase">Sets whether element name must match the case.</param>
		/// <returns>Returns the element with specified name.</returns>
		public double				Get_ElementDouble(string Name, bool AllLevels, bool MatchCase)
		{
			var El = Get_Element(Name, AllLevels, MatchCase);
			if (El == null)	 return -1;
			return El.ValueNum;
		}
	
		/// <summary>
		/// Gets the integer value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <returns>Returns the element with specified name.</returns>
		public int					Get_ElementInt(string Name)
		{	return Get_ElementInt(Name, false, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <returns>Returns the element with specified name.</returns>
		public int					Get_ElementInt(string Name, bool AllLevels)
		{	return Get_ElementInt(Name, AllLevels, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <param name="MatchCase">Sets whether element name must match the case.</param>
		/// <returns>Returns the element with specified name.</returns>
		public int					Get_ElementInt(string Name, bool AllLevels, bool MatchCase)
		{
			var El = Get_Element(Name, AllLevels, MatchCase);
			if (El == null)	 return -1;
			return (int)El.ValueNum;
		}

		/// <summary>
		/// Gets the boolean value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <returns>Returns the element with specified name.</returns>
		public bool					Get_ElementBool(string Name)
		{	return Get_ElementBool(Name, false, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <returns>Returns the element with specified name.</returns>
		public bool					Get_ElementBool(string Name, bool AllLevels)
		{	return Get_ElementBool(Name, AllLevels, false);		}
		/// <summary>
		/// Gets the string value for element with specified name.
		/// </summary>
		/// <param name="Name">Name of the element to get.</param>
		/// <param name="AllLevels">Enables search on all levels.</param>
		/// <param name="MatchCase">Sets whether element name must match the case.</param>
		/// <returns>Returns the element with specified name.</returns>
		public bool					Get_ElementBool(string Name, bool AllLevels, bool MatchCase)
		{
			var El = Get_Element(Name, AllLevels, MatchCase);
			if (El == null)	 return false;
			return El.ValueBool;
		}
		#endregion

		//	Constructors & ToString
		#region
		/// <summary>
		/// Returns a string representation of this JSON_Object instance.
		/// </summary>
		/// <returns>A string representation of this JSON_Object instance.</returns>
		public new string		ToString()
		{
			string S = _name + " : ";
			if (IsArrayElement)	S = "";
			if (IsRootElement)	S = "Root ";
			switch (_type)
			{ 
				case ZJSON_DataType.Undefined:	S += "Undefined";	break;
				case ZJSON_DataType.Number	:	S += _valueNum;		break;
				case ZJSON_DataType.String	:	S += _valueStr;		break;
				case ZJSON_DataType.Bool	:	S += (_valueBool ? "true" : "false");	break;
				case ZJSON_DataType.Array	:	
					if (_valueArray == null)	S += "<Array = 0>";
					else S += "<Array = " + _valueArray.Count + ">";	break;
				case ZJSON_DataType.Object	:	S += "Object";		break;
			}
			return S;
		}

		/// <summary>
		/// Creates a deep copy of this JSON_Object instance.
		/// </summary>
		/// <returns>A deep copy of this JSON_Object instance.</returns>
		public ZJSON_Object		Copy()
		{
			var J = new ZJSON_Object(null);
			J._isRootElement = this._isRootElement;
			J._name			= this._name;
			J._type			= this._type;
			J._valueBool	= this._valueBool;
			J._valueNum		= this._valueNum;
			J._valueStr		= this._valueStr;
			if (this._valueArray != null)
			{
				J._valueArray = new ZJSON_ObjectList();
				for (int i = 0; i < this._valueArray.Count; i++)
					J._valueArray.Add(this._valueArray[i].Copy());
			}
			return J;
		}
		
		private ZJSON_Object(ZJSON_Object parent)
		{
			_P = parent;
			if (parent == null)		_isRootElement = true;
		}
		private ZJSON_Object(ZJSON_Object parent, string name, ZJSON_DataType type) : this(parent)
		{
			_name	= name;
			_type	= type;
		}
		#endregion
		#endregion


		//	Public static methods
		#region
		/// <summary>
		/// Returns a new instance of JSON_Object class.
		/// </summary>
		/// <returns>A new instance of JSON_Object class.</returns>
		public static ZJSON_Object	Create_New()
		{	return new ZJSON_Object(null);	}


		/// <summary>
		/// Gets a JSON object from a specified string.
		/// </summary>
		/// <param name="Source">The source string.</param>
		/// <returns>Returns the result JSON object if source string is valid JSON string, otherwise returns NULL.</returns>
		public static ZJSON_Object	Get_Object(string Source)
		{
			try
			{
				//	Init
				#region
				S = Source.ToCharArray();
				int SL	= S.Length;
				var CJ	= new ZJSON_Object(null, null, ZJSON_DataType.Object);
				CJ		= CJ.AddChild();
				bool IsValue = false;
				#endregion

				for (int i = 1; i < SL; i++)
				{
					char C = S[i];
					if (char.IsWhiteSpace(C))	continue;

					//	Switch
					#region
					switch (C)
					{
						case '{':
							#region
							if (CJ._type == ZJSON_DataType.Undefined)		CJ._type = ZJSON_DataType.Object;
							CJ		= CJ.AddChild();
							IsValue = false;
							break;
							#endregion

						case '}':
						case ']':
							#region
							GetCT(CJ, i);
							CJ	= CJ._P;
							break;
							#endregion

						case '[':
							#region
							CJ._type = ZJSON_DataType.Array;
							CJ		= CJ.AddChild();
							break;
							#endregion

						case ',':
							#region
							IsValue	= false;
							GetCT(CJ, i);
							CJ = CJ._P.AddChild();
							if (CJ._P._type == ZJSON_DataType.Array)		IsValue = true;
							break;
							#endregion

						case '"':
							#region
							oldI = i+1;
							while (S[++i] != '"')	;
							string CT = new string(S, oldI, i-oldI);
							oldI = -1;
							if (IsValue)	{	CJ._valueStr = CT;	CJ._type = ZJSON_DataType.String;	}
							if (!IsValue)		CJ._name = CT;
							break;
							#endregion
						
						case ':':	IsValue = true;		break;
						default	:	if (oldI == -1)	 oldI = i;	break;
					}
					#endregion
				}
				return CJ;
			}
			catch	{	return null;	}
		}


		/// <summary>
		/// Writes JSON object to a file with specified name.
		/// </summary>
		/// <param name="FileName">Name of the destination file.</param>
		/// <returns>Returns TRUE if operation was successful, otherwise returns FALSE.</returns>
		public bool					Write_Object(string FileName)
		{	return Write_Object(FileName, false);		}
		/// <summary>
		/// Writes JSON object to a file with specified name.
		/// </summary>
		/// <param name="FileName">Name of the destination file.</param>
		/// <param name="CompactMode">Gets whether to write in compact mode.</param>
		/// <returns>Returns TRUE if operation was successful, otherwise returns FALSE.</returns>
		public bool					Write_Object(string FileName, bool CompactMode)
		{
			try
			{
				CM	= CompactMode;
				CL	= -1;
				var SB = new StringBuilder();
				Write_Obj(SB, this, 1, 1);
				File.WriteAllText(FileName, SB.ToString());
				return true;
			}
			catch	{	return false;	}
		}

		
		/// <summary>
		/// Prints JSON object on a screen.
		/// </summary>
		public void					Print_Object()
		{	Print_Object(false);		}
		/// <summary>
		/// Prints JSON object on a screen.
		/// </summary>
		/// <param name="CompactMode">Gets whether to write in compact mode.</param>
		public void					Print_Object(bool CompactMode)
		{	try
			{
				CL = -1;
				var SB = new StringBuilder();
				Write_Obj(SB, this, 1, 1);
				Console.WriteLine(SB.ToString());
			}
			catch	{	}
		}
		#endregion


		//	Private service methods
		#region
		private static char[]		S;
		private static bool			CM = false;
		private static int			oldI = -1;
		private static int			CL;
		private static string[]		Br = new string[] { "[", "{", "]", "}" };
		private static string		DoPad()
		{	if (CM)	return "";	return new string(' ', 2*CL);	}
		private static string		GS(string S)
		{	return "\"" + S + "\"";	}

		//	Write specified JSON object to StringBulder
		private static void			Write_Obj(StringBuilder SB, ZJSON_Object J, int TC, int CI)
		{
			CL++;
			string S = GS(J._name);
			if (CM)	 S += ":";	else S += " : ";
			if (J.IsArrayElement  ||  J._isRootElement)	S = "";
			SB.Append(DoPad() + S);
			var JV = J._valueArray;
			switch (J._type)
			{
				case ZJSON_DataType.Object:	case ZJSON_DataType.Array:
					#region
					if (!CM)	SB.AppendLine(Br[(int)J._type-4]);		else SB.Append(Br[(int)J._type-4]);
					for (int i = 0; i < JV.Count; i++)	Write_Obj(SB, JV[i], JV.Count, i);
					SB.Append(DoPad() + Br[(int)J._type-2]);	break;
					#endregion
				case ZJSON_DataType.Number	:	SB.Append(J._valueNum);			break;
				case ZJSON_DataType.String	:	SB.Append(GS(J._valueStr));		break;
				case ZJSON_DataType.Bool	:	SB.Append(J._valueBool ? "true" : "false");	break;
			}
			if (CI < TC-1)	SB.Append(",");
			if (!CM)		SB.AppendLine();
			CL--;
		}
		
		//	Get value from current text string
		private static void			GetCT(ZJSON_Object CJ, int i)
		{
			if (oldI == -1)	return;
			string CT = new string(S, oldI, i-oldI);
			if (CT != "")
			{
				CJ._valueNum = Get_FloatFromString(CT);
				CJ._type	= ZJSON_DataType.Number;
				if (CJ._valueNum == -1f)
				{
					CJ._valueBool = string.Equals(CT.ToLower(), "true");
					CJ._type	= ZJSON_DataType.Bool;
				}
			}
			oldI = -1;
		}

		/// <summary>
		/// Get a double value from string.
		/// </summary>
		/// <param name="Source">Source string.</param>
		/// <returns>Returns obtained double value, or -1 if failed.</returns>
		public static double		Get_FloatFromString(string Source)
		{
			string RS = Regex.Match(Source, @"[-+]?[0-9]*\.?[0-9]+").Value;
			if (RS != "")	return double.Parse(RS);	 else return -1f;
		}
		#endregion
	}



	/// <summary>
	/// This class defines a list of JSON objects.
	/// </summary>
	public class ZJSON_ObjectList : List<ZJSON_Object>
	{
		/// <summary>
		/// Gets the last element in this list of JSON objects.
		/// </summary>
		public ZJSON_Object	Last	{	get	{	if (this.Count > 0)	  return this[this.Count-1];	return null;	}}
		/// <summary>
		/// Removes the last element in this list of JSON objects.
		/// </summary>
		public void		RemoveLast()
		{
			if (this.Count > 0)	  base.RemoveAt(this.Count-1);
		}


		internal ZJSON_ObjectList()
		{
		}
	}
}