using System;
using System.Collections.Generic;
using ZFC.Maths;



namespace ZFC.Xml
{
	/// <summary>
	/// This class defines the properies of a single XML attribute.
	/// </summary>
	public struct ZXmlAttribute
	{
		/// <summary>
		/// Gets or sets the name of the attribute.
		/// </summary>
		public string	Name		{	get	{	return new string(_name);	}	set	{	_name = value.ToCharArray();	}}
		/// <summary>
		/// Gets or sets the value of the attribute.
		/// </summary>
		public string	Value		{	get	{	return new string(_value);	}	set	{	_value = value.ToCharArray();	}}

		private char[]	_name;
		private char[]	_value;

		/// <summary>
		/// Returns the string representation of this XML attribute.
		/// </summary>
		/// <returns>Returns the string representation of this XML attribute.</returns>
		public new string	ToString()
		{
			return Name + "=\"" + Value + "\"";
		}

        internal ZXmlAttribute(string name, string value)
        {
            _name	= name.ToCharArray();
            _value	= value.ToCharArray();
        }
	}



	/// <summary>
	/// This class defines the properies of a list of XML attributes.
	/// </summary>
	public class ZXmlAttributeList
	{
		//	Fields & Properties
		#region
		private	ZXmlNode			P;
		private List<ZXmlAttribute>	_ats;

		/// <summary>
		/// Count of attributes in this list.
		/// </summary>
		public int				Count
		{	get	{	return _ats.Count;	}}
		/// <summary>
		/// Gets the attribute with specified index.
		/// </summary>
		/// <param name="Index">Index of the attribute to get.</param>
		/// <returns>Returns the attribute with specified index.</returns>
		public ZXmlAttribute	this[int Index]
		{	get	{	if (Index < 0  ||  Index >= _ats.Count)  return new ZXmlAttribute();	return _ats[Index];	}}
		/// <summary>
		/// Gets the attribute with specified name.
		/// </summary>
		/// <param name="Name">Name of the attribute to get.</param>
		/// <returns>Returns the attribute with specified name.</returns>
		public ZXmlAttribute	this[string Name]
		{	get	{	
				for (int i = 0; i < _ats.Count; i++)
					if (_ats[i].Name.Equals(Name, StringComparison.InvariantCulture))	return _ats[i];
				return new ZXmlAttribute();
		}}
		/// <summary>
		/// Gets the index of specified XML attribute.
		/// </summary>
		/// <param name="Attribute">The attribute to find the index of.</param>
		/// <returns>Return the index of specified XML attribute.</returns>
		public int				IndexOf(ZXmlAttribute Attribute)
		{	return _ats.IndexOf(Attribute);	}
		/// <summary>
		/// Gets whether this list of attributes contains the attribute with specified name.
		/// </summary>
		/// <param name="Name">Name of the attribute to check.</param>
		/// <returns>Returns TRUE if this list of attributes contains the attribute with specified name, otherwise retursn FALSE.</returns>
		public bool				Contains(string Name)
		{
			for (int j = 0; j < _ats.Count; j++)
				if (_ats[j].Name.Equals(Name, StringComparison.InvariantCulture))	return true;
			return false;
		}
		#endregion

		//	Add / Remove
		#region
		/// <summary>
		/// Adds the specified attriubute at the end of this list.
		/// </summary>
		/// <param name="Attribute">The attriubute to add.</param>
		/// <returns>Returns the added attriubute.</returns>
		public ZXmlAttribute	Add(ZXmlAttribute Attribute)
		{	if (_ats != null)	 return Add(_ats.Count, Attribute);	  return Add(0, Attribute);	}
		/// <summary>
		/// Inserts the specified attriubute at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where specified attriubute should be inserted.</param>
		/// <param name="Attribute">The attriubute to insert.</param>
		/// <returns>Returns the inserted attriubute.</returns>
		public ZXmlAttribute	Add(int Index, ZXmlAttribute Attribute)
		{
			if (_ats == null)	_ats = new List<ZXmlAttribute>();
			Index = ZMath.GetBound(Index, 0, _ats.Count);
			if (!Contains(Attribute.Name))
			{
				_ats.Insert(Index, Attribute);
				return Attribute;
			}
			return new ZXmlAttribute();
		}
		/// <summary>
		/// Adds the specified arrays of attriubutes at the end of this list.
		/// </summary>
		/// <param name="Attributes">The array of attriubutes to insert.</param>
		public void				Add(ZXmlAttribute[] Attributes)
		{	if (_ats != null)	 Add(_ats.Count, Attributes);	else Add(0, Attributes);}
		/// <summary>
		/// Inserts the specified array of attriubutes at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where specified array of attriubutes should be inserted.</param>
		/// <param name="Attributes">The array of attriubutes to insert.</param>
		public void				Add(int Index, ZXmlAttribute[] Attributes)
		{
			if (_ats == null)	_ats = new List<ZXmlAttribute>();
			Index = ZMath.GetBound(Index, 0, _ats.Count);
			for (int i = 0; i < Attributes.Length; i++)
				if (!Contains(Attributes[i].Name))	Add(Index, Attributes[i]);
		}

		/// <summary>
		/// Adds the new attribute with specified name and value at the end of this list.
		/// </summary>
		/// <param name="Name">Name of the attribute to add.</param>
		/// <param name="Value">Value of the attribute to add.</param>
		/// <returns>Returns the new added attribute with specified name and value.</returns>
		public ZXmlAttribute	Add(string Name, string Value)
		{	if (_ats != null)	return Add(_ats.Count, Name, Value);	return Add(0, Name, Value);	}
		/// <summary>
		/// Inserts the new attribute with specified name and value at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where new node should be inserted.</param>
		/// <param name="Name">Name of the attribute to insert.</param>
		/// <param name="Value">Value of the attribute to insert.</param>
		/// <returns>Returns the inserted attribute with specified name and value</returns>
		public ZXmlAttribute	Add(int Index, string Name, string Value)
		{	return Add(Index, new ZXmlAttribute(Name, Value));	}

		/// <summary>
		/// Removes the attribute with specified index.
		/// </summary>
		/// <param name="Index">Index of the attribute to remove.</param>
		public void				Remove(int Index)
		{	if (_ats != null  &&  Index >= 0  &&  Index < _ats.Count)		_ats.RemoveAt(Index);	}
		/// <summary>
		/// Removes the specified attribute.
		/// </summary>
		/// <param name="Attribute">The attribute to remove.</param>
		public void				Remove(ZXmlAttribute Attribute)
		{	if (_ats != null)	_ats.Remove(Attribute);	}
		/// <summary>
		/// Remove all attributes from this list.
		/// </summary>
		public void				RemoveAll()
		{	_ats = null;	}
		#endregion

		/// <summary>
		/// Returns the string representation of this list of XML attributes.
		/// </summary>
		/// <returns>Returns the string representation of this list of XML attributes.</returns>
		public new string	ToString()
		{
			return "Count = " + Count;
		}

		internal ZXmlAttributeList(ZXmlNode Parent)
        {
			P = Parent;
        }
	}
}
