using System;
using System.Collections.Generic;



namespace ZFC.Data
{
	/// <summary>
	/// This class defines the basic Tree Node implementation.
	/// </summary>
	/// <typeparam name="T">Type of data field in this ZNode instance.</typeparam>
	public class	ZNode<T>
	{
		//	Private Fields
		#region
		private	T				_data;
		private ZNodeList<T>	_childs;
		private ZNode<T>		_p;
		private ZNodeList<T>	_parentChilds		{	get	{	return _p._childs;		}}
		private List<ZNode<T>>	_nodes				{	get	{	return _childs._nodes;	}}
		#endregion

		//	Public Properties
		#region
		/// <summary>
		/// Gets or sets the data value of this node.
		/// </summary>
		public T				Data		{	get	{	return _data;	}	set	{	_data = value;		}}
		/// <summary>
		/// Gets or sets the list of child nodes.
		/// </summary>
		public ZNodeList<T>		Childs		{	get	{	return _childs;	}	set	{	_childs = value;	}}
		/// <summary>
		/// Gets the parental node for this node.
		/// </summary>
		public ZNode<T>			Parent		{	get	{	return _p;		}}
		/// <summary>
		/// Gets the root parent node for this node.
		/// </summary>
		public ZNode<T>			RootParent	{	get	{	var P = this;	while (P._p != null)	P = P._p;	return P;	}}
		/// <summary>
		/// Gets the previous sibling of this node if it exists, otherwise returns NULL.
		/// </summary>
		public ZNode<T>			Prev
		{	
			get	
			{	
				if (_p == null)		return null;
				int ind = _parentChilds.IndexOf(this);
				if (ind > 0)	return _parentChilds[ind-1];
				return null;
			}
		}
		/// <summary>
		/// Gets the next sibling of this node if it exists, otherwise returns NULL.
		/// </summary>
		public ZNode<T>			Next
		{	
			get	
			{	
				if (_p == null)		return null;
				int ind = _parentChilds.IndexOf(this);
				if (ind < _parentChilds.Count-1)	return _parentChilds[ind+1];
				return null;
			}
		}
		#endregion

		//	Methods
		#region
		/// <summary>
		/// Adds the new child node with specified data field at the end of the list of child nodes.
		/// </summary>
		/// <param name="data">Value of the data field for new child node.</param>
		public ZNode<T>			AddChild(T data)
		{
			var N = new ZNode<T>(this, data);
			_nodes.Add(N);
			return N;
		}
		/// <summary>
		/// Adds the specified node as a child node at the end of the list of child nodes.
		/// </summary>
		/// <param name="node">The node to add as a child node.</param>
		public ZNode<T>			AddChild(ZNode<T> node)
		{
			node._p = this;
			_nodes.Add(node);
			return node;
		}
		/// <summary>
		/// Removes the child node with specified index.
		/// </summary>
		/// <param name="Index">Index of the child node to remove.</param>
		public void				RemoveChild(int Index)
		{
			if (Index >= 0  &&  Index < _childs.Count)	_nodes.RemoveAt(Index);
		}
		/// <summary>
		/// Removes the specified child node.
		/// </summary>
		/// <param name="node">The node to remove.</param>
		public void				RemoveChild(ZNode<T> node)
		{	_nodes.Remove(node);	}
		/// <summary>
		/// Clears the list of child nodes.
		/// </summary>
		public void				Clear()
		{	_nodes.Clear();			}
		/// <summary>
		/// Gets whether this node contains the specified node as a child.
		/// </summary>
		/// <param name="node">The node to search for.</param>
		/// <returns>Returns TRUE if this node contains the specified node as a child, otherwise returns FALSE.</returns>
		public bool				ContainsChild(ZNode<T> node)
		{	return _nodes.Contains(node);	}
		/// <summary>
		/// Gets whether this node contains the node with specified data field as a child.
		/// </summary>
		/// <param name="data">The data to search for.</param>
		/// <returns>Returns TRUE if this node contains the node with specified data as a child, otherwise returns FALSE.</returns>
		public bool				ContainsChild(T data)
		{
			for (int i = 0; i < _nodes.Count; i++)
				if (_nodes[i]._data.Equals(data))	return true;
			return false;
		}
		#endregion

		//	Constructors,  ToString
		#region
		/// <summary>
		/// Gets the string representation of this ZNode instance.
		/// </summary>
		/// <returns>Returns the string representation of this ZNode instance.</returns>
		public override string	ToString()
		{
			return _data.ToString();
		}

		/// <summary>
		/// Creates a new instance of ZNode with specified parameters.
		/// </summary>
		/// <param name="parent">Parental node, NULL if this node is a root node.</param>
		/// <param name="data">Data field for this node.</param>
		public ZNode(ZNode<T> parent, T data)
		{
			_p		= parent;
			_childs	= new ZNodeList<T>();
			_data	= data;
		}
		#endregion
	}



	/// <summary>
	/// This class defines the set of properties for a single list of ZNode instances.
	/// </summary>
	/// <typeparam name="T">Type of data field in ZNode instances.</typeparam>
	public class	ZNodeList<T>
	{
		//	Fields & Properties
		#region
		internal List<ZNode<T>>	_nodes;

		/// <summary>
		/// Gets the count of nodes in this list of nodes.
		/// </summary>
		public int			Count				{	get	{	return _nodes.Count;	}}
		/// <summary>
		/// Gets or sets the node with specified index.
		/// </summary>
		/// <param name="Index">Index of the node to get or set.</param>
		/// <returns>Returns the node with specified index if it exists, otherwise returns NULL.</returns>
		public ZNode<T>		this[int Index]
		{	get	{	if (Index >= 0  &&  Index < _nodes.Count)	return _nodes[Index];	return null;	}
			set	{	if (Index >= 0  &&  Index < _nodes.Count)	_nodes[Index] = value;	}}
		/// <summary>
		/// Gets the first node in this list of nodes.
		/// </summary>
		public ZNode<T>		First				{	get	{	if (_nodes.Count > 0)	return _nodes[0];	return null;	}}
		/// <summary>
		/// Gets the last node in this list of nodes.
		/// </summary>
		public ZNode<T>		Last				{	get	{	if (_nodes.Count > 0)	return _nodes[Count-1];		return null;	}}
		#endregion

		//	Methods
		#region
		internal int		IndexOf(ZNode<T> node)
		{	return _nodes.IndexOf(node);	}
		#endregion

		internal ZNodeList()
		{
			_nodes	= new List<ZNode<T>>();
		}
	}
}
