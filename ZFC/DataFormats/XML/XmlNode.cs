using System;
using System.Collections.Generic;
using ZFC.Maths;



namespace ZFC.Xml
{
	/// <summary>
	/// This class defines the properties of a single XML node.
	/// </summary>
	public class ZXmlNode
	{
		//	Public Properties
		#region
		/// <summary>
		/// Gets or sets the name of this XML node.
		/// </summary>
		public string			Name			{	get	{	return new string(_name);	}	set	{	_name = value.ToCharArray();	}}
		/// <summary>
		/// Gets or sets the text content of this XML node.
		/// </summary>
		public string			Text			{	get	{	return new string(_text);	}	set	{	_text = value.ToCharArray();	}}		
		/// <summary>
		/// Gets the list of attributes for this XML node.
		/// </summary>
	public ZXmlAttributeList	Attributes		{	get	{	return _attrs;		}}
		/// <summary>
		/// Gets the list of child nodes for this XML node.
		/// </summary>
		public ZXmlNodeList		Nodes			{	get	{	return _nodes;		}}
		/// <summary>
		/// Returns the parent XML node.
		/// </summary>
		public ZXmlNode			Parent			{	get	{	return _parent;		}}
		/// <summary>
		/// Gets the previous sibling node if it exists.
		/// </summary>
		public ZXmlNode			PrevSibling		{	get	{	if (_pIndex > 0)				return _pNodes[_pIndex-1];	return null;	}}
		/// <summary>
		/// Gets the next sibling node if it exists.
		/// </summary>
		public ZXmlNode			NextSibling		{	get	{	if (_pIndex != -1 && _pIndex < _pNodes.Count-1)	return _pNodes[_pIndex+1];	return null;	}}
		/// <summary>
		/// Gets the root parent XML node.
		/// </summary>
		public ZXmlNode			RootParent		{	get	{	return _rootParent;		}}
		#endregion

		//	Flags
		#region
		/// <summary>
		/// Gets whether this XML node has attributes.
		/// </summary>
		public bool				HasAttributes	{	get	{	return _attrs.Count > 0;		}}
		/// <summary>
		/// Gets whether this XML node has child nodes.
		/// </summary>
		public bool				HasChilds		{	get	{	return _nodes.Count > 0;		}}
		/// <summary>
		/// Gets whether this XML is the root node.
		/// </summary>
		public bool				IsRoot			{	get	{	return _parent == null;			}}
		#endregion

		//	Internal fields
		#region
		internal char[]				_text;
		internal char[]				_name;
		internal ZXmlAttributeList	_attrs;
		internal ZXmlNodeList		_nodes;
		internal ZXmlNode			_parent;
		internal ZXmlNode			_rootParent;
		private ZXmlNodeList		_pNodes			{	get	{	if (IsRoot)	return null;	return _parent._nodes;	}}
		private int					_pIndex			{	get	{	if (IsRoot)	return -1;		return _pNodes.IndexOf(this);	}}
		#endregion
		
		/// <summary>
		/// Returns the string representation of this XML node.
		/// </summary>
		/// <returns>Returns the string representation of this XML node.</returns>
		public new string	ToString()
		{
			return "<" + Name + ">";
		}


		internal ZXmlNode(ZXmlNode Parent, string Name)
		{
			this.Name		= XMT.GetValidNodeName(Name);
			_parent		= Parent;
			if (Parent != null)		_rootParent	= Parent._rootParent;
			_attrs		= new ZXmlAttributeList(this);
			_nodes		= new ZXmlNodeList(this);
		}

		/// <summary>
		/// Creates new XML node with specified name.
		/// </summary>
		/// <param name="Name">Name of the new node.</param>
		/// <returns>Returns new XML node with specified name.</returns>
		public static ZXmlNode		CreateRootNode(string Name)
		{	
			return new ZXmlNode(null, XMT.GetValidNodeName(Name));	}
	}



	/// <summary>
	/// This class defines the properties of a single list of XML nodes.
	/// </summary>
	public class ZXmlNodeList
	{
		//	Properties
		#region
		private	ZXmlNode		P;
		private List<ZXmlNode>	_nodes;

		/// <summary>
		/// Gets the count of XML nodes in this list.
		/// </summary>
		public int			Count
		{	get	{	if (_nodes == null)	return 0;	return _nodes.Count;	}}
		/// <summary>
		/// Gets the XML node with specified index.
		/// </summary>
		/// <param name="Index">Index of the node to get.</param>
		/// <returns>Returns the XML node with specified index.</returns>
		public ZXmlNode		this[int Index]
		{	get	{	if (_nodes == null  ||  Index < 0  ||  Index >= _nodes.Count)  return null;		return _nodes[Index];	}}	
		/// <summary>
		/// Gets the index of the specified node.
		/// </summary>
		/// <param name="Node">The node to get index of.</param>
		/// <returns>Returns the index of the specified node.</returns>
		public int			IndexOf(ZXmlNode Node)
		{	if (_nodes == null)  return -1;		return _nodes.IndexOf(Node);	}
		/// <summary>
		/// Gets whether this list of nodes contains the node with specified name.
		/// </summary>
		/// <param name="Name">Name of the node to check.</param>
		/// <returns>Returns TRUE if this list of nodes contains the node with specified name, otherwise retursn FALSE.</returns>
		public bool			Contains(string Name)
		{
			if (_nodes == null)  return false;
			for (int j = 0; j < _nodes.Count; j++)
				if (_nodes[j].Name.Equals(Name, StringComparison.InvariantCulture))	return true;
			return false;
		}
		#endregion
	
		//	Add / Remove Child Nodes
		#region
		/// <summary>
		/// Adds the specified node at the end of this list.
		/// </summary>
		/// <param name="Node">The node to add.</param>
		/// <returns>Returns the added node.</returns>
		public ZXmlNode		Add(ZXmlNode Node)
		{	if (_nodes != null)  return Add(_nodes.Count, Node);	return  Add(0, Node);	}
		/// <summary>
		/// Inserts the specified node at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where specified node should be inserted.</param>
		/// <param name="Node">The node to insert.</param>
		/// <returns>Returns the inserted node.</returns>
		public ZXmlNode		Add(int Index, ZXmlNode Node)
		{
			if (_nodes == null)	_nodes = new List<ZXmlNode>();
			Index = ZMath.GetBound(Index, 0, _nodes.Count);
			Node._parent		= this.P;
			Node._rootParent	= this.P.RootParent;
			_nodes.Insert(Index, Node);
			return Node;
		}
		/// <summary>
		/// Adds the specified arrays of nodes at the end of this list.
		/// </summary>
		/// <param name="Nodes">The array of nodes to insert.</param>
		public void			Add(ZXmlNode[] Nodes)
		{	if (_nodes != null)  Add(_nodes.Count, Nodes);	Add(0, Nodes);	}
		/// <summary>
		/// Inserts the specified array of nodes at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where specified array of nodes should be inserted.</param>
		/// <param name="Nodes">The array of nodes to insert.</param>
		public void			Add(int Index, ZXmlNode[] Nodes)
		{
			if (_nodes == null)	 _nodes = new List<ZXmlNode>();
			Index = ZMath.GetBound(Index, 0, _nodes.Count);
			for (int i = 0; i < Nodes.Length; i++)
			{
				Nodes[i]._parent		= this.P;
				Nodes[i]._rootParent	= this.P.RootParent;
			}
			_nodes.InsertRange(Index, Nodes);
		}

		/// <summary>
		/// Adds the new node with specified name at the end of this list.
		/// </summary>
		/// <param name="Name">Name of the new node to add.</param>
		/// <returns>Returns the new node with specified name.</returns>
		public ZXmlNode		Add(string Name)
		{	if (_nodes != null)	return Add(_nodes.Count, Name);		return Add(0, Name);	}
		/// <summary>
		/// Inserts the new node with specified name at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where new node should be inserted.</param>
		/// <param name="Name">Name of the new node to insert.</param>
		/// <returns>Returns the inserted new node with specified name.</returns>
		public ZXmlNode		Add(int Index, string Name)
		{	return Add(Index, new ZXmlNode(this.P,Name));	}
		/// <summary>
		/// Adds the array of new nodes with specified names at the end of this list.
		/// </summary>
		/// <param name="Names">Array of names of the new nodes to add.</param>
		public void			Add(string[] Names)
		{	if (_nodes != null)		Add(_nodes.Count, Names);	else Add(0, Names);}
		/// <summary>
		/// Inserts the array of new nodes with specified names at the position with specified index.
		/// </summary>
		/// <param name="Index">Index of the position where new nodes should be inserted.</param>
		/// <param name="Names">Array of names of the new nodes to insert.</param>
		public void			Add(int Index, string[] Names)
		{
			if (_nodes == null)	 _nodes = new List<ZXmlNode>();
			Index = ZMath.GetBound(Index, 0, _nodes.Count);
			for (int i = 0; i < Names.Length; i++)
				Add(Index, new ZXmlNode(this.P, Names[i]));
		}

		/// <summary>
		/// Removes the node with specified index.
		/// </summary>
		/// <param name="Index">Index of the node to remove.</param>
		public void			Remove(int Index)
		{	if (_nodes != null  &&  Index >= 0  &&  Index < _nodes.Count)	_nodes.RemoveAt(Index);	}
		/// <summary>
		/// Removes the specified node.
		/// </summary>
		/// <param name="Node">The node to remove.</param>
		public void			Remove(ZXmlNode Node)
		{	if (_nodes != null)	_nodes.Remove(Node);	}
		/// <summary>
		/// Remove all nodes from this list.
		/// </summary>
		public void			RemoveAll()
		{	_nodes = null;	}
		#endregion

		/// <summary>
		/// Returns the string representation of this list of XML nodes.
		/// </summary>
		/// <returns>Returns the string representation of this list of XML nodes.</returns>
		public new string	ToString()
		{
			return "Count = " + _nodes.Count;
		}

        internal ZXmlNodeList(ZXmlNode Parent)
        {
			P = Parent;
        }
	}
}
