namespace ZFC
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Strings;
	using Maths;


	#region Lists & Arrays

	/// <summary>
	/// ICopyable interface.
	/// </summary>
	/// <typeparam name="T">Copy</typeparam>
	public interface ICopyable<out T>
	{
		/// <summary>
		/// Creates a deep copy of this object.
		/// </summary>
		/// <returns>A deep copy of this object.</returns>
		T Copy();
	}


	/// <summary>
	/// The more advanced version of standard List.
	/// </summary>
	/// <typeparam name="T">The type of elements in this BaseList.</typeparam>
	public class	BaseList<T>		: List<T>
	{
		#region Public Properties

		/// <summary>
		/// Gets or sets the last element in this list.
		/// </summary>
		public T			Last		{	get {   return Count < 1 ? default(T) : this[Count-1]; }
											set {	if (Count >= 1)	 this[Count-1] = value;	}}
		/// <summary>
		/// Gets or sets the penultimate element in this list.
		/// </summary>
		public T			Penult		{	get {   return Count < 2 ? default(T) : this[Count-2]; }
											set {	if (Count >= 2)	 this[Count-2] = value;	}}
		/// <summary>
		/// Defines if all elements in this BaseList has custom value (not null).
		/// </summary>
		public bool			AllElementsHasValues
		{	get
			{	
				if (Count == 0)
					return false;
				for (var i = 0; i < Count; i++)	 
					if (this[i] == null)	
						return false;	
				return true;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Adds an element with default value to the end of this list.
		/// </summary>
		public void			Add()
		{
			var typeConstructor = typeof(T).GetConstructor(Type.EmptyTypes);
			if (typeConstructor != null)	
				Add((T)typeConstructor.Invoke(new object[]{}));
			else Add(default(T));
		}
		/// <summary>
		/// Adds an element with default value to the end of this list.
		/// </summary>
		public void			AddLast()
		{
			if (Count != 0)
			{
				var lastObject = Last as ICopyable<T>;
				if (lastObject != null)
				{
					Add(lastObject.Copy());	
					return;
				}
			}
			Add();
		}
		/// <summary>
		/// Removes the last element in this list.
		/// </summary>
		public void			RemoveLast()
		{
			if (Count != 0)	 
				RemoveAt(Count-1);
		}
		/// <summary>
		/// Truncates this list starting at given index.
		/// </summary>
		/// <param name="index">Truncating starts at this index.</param>
		public void			Truncate(int index)
		{
			if (index < Count)
				RemoveRange(index, Count-index);
		}

		#endregion

		#region Copy, Rotate, ToString

		/// <summary>
		/// Creates a deep copy of this BaseList.
		/// </summary>
		/// <returns>Returns a deep copy of this BaseList.</returns>
		public BaseList<T>	Copy()
		{
			var tempArray = new T[Count];
			Array.Copy(this.ToArray(), tempArray, tempArray.Length);
			var resultList = new BaseList<T>();
			resultList.AddRange(tempArray);
			return resultList;
		}

		/// <summary>
		/// Rotate this list.
		/// </summary>
		/// <param name="dx">Offset to rotate at, may be positive or negative.</param>
		/// <returns>Returns the resulting list.</returns>
		public BaseList<T>	Rotate(int dx)
		{
			var resultList = new BaseList<T>();
			for (int i = 0; i < this.Count; i++)
			{
				int k = i + dx;
				if (k < 0)	k += this.Count;
				k = k % this.Count;
				resultList.Add(this[k]);
			}
			return resultList;
		}

		/// <summary>
		/// Returns the string representation of this BaseList instance.
		/// </summary>
		/// <returns>Returns the string representation of this BaseList instance.</returns>
		public new string	ToString()
		{
			return "Count = " + Count;
		}

		#endregion
	}



	/// <summary>
	/// The more advanced version of standard List.
	/// </summary>
	/// <typeparam name="T">The type of elements in this BaseList.</typeparam>
	public class	BaseListSimple<T> : IEnumerable<T>
	{
		#region Fields & Properties

		internal List<T>	_lst;

		/// <summary>
		/// Gets or sets the last element in this list.
		/// </summary>
		public T			Last		{	get {	if (Count < 1)	 return default(T);		else return _lst[Count-1];	}
											set {	if (Count >= 1)	 _lst[Count-1] = value;	}}
		/// <summary>
		/// Gets the count of elements.
		/// </summary>
		public int			Count	{	get {	return _lst.Count;		}}
		/// <summary>
		/// Gets or sets the element with specified index.
		/// </summary>
		/// <param name="Index">Index of the element to get or set.</param>
		/// <returns>Returns the element with specified index.</returns>
		public T			this[int Index]
		{
			get {	if (Index >= 0  &&  Index < Count)	return _lst[Index];	 return default(T);	}
			set {	if (Index >= 0  &&  Index < Count)	_lst[Index] = value;	}
		}
		/// <summary>
		/// Defines if all elements in this BaseList has custom value (not null).
		/// </summary>
		public bool			AllElementsHasValues
		{	get
			{	
				if (this.Count == 0)	return false;
				for (int i = 0; i < Count; i++)	 if (_lst[i] == null)	return false;	return true;
			}
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Adds the specified element to the end of this list.
		/// </summary>
		/// <param name="element">The element to add.</param>
		public void			Add(T element)
		{
			_lst.Add(element);
		}
		/// <summary>
		/// Adds an element with default value to the end of this list.
		/// </summary>
		public void			Add()
		{
			var C = typeof(T).GetConstructor(Type.EmptyTypes);
			if (C != null)	_lst.Add((T)C.Invoke(new object[]{}));
			else _lst.Add(default(T));
		}
		/// <summary>
		/// Adds the specified range of elements to the end of this list.
		/// </summary>
		/// <param name="element">The range of elements to add.</param>
		public void			Add(IEnumerable<T> element)
		{
			_lst.AddRange(element);
		}
		/// <summary>
		/// Inserts the specified element at the specified position in this list.
		/// </summary>
		/// <param name="index">Index of the position where the specified element should be inserted.</param>
		/// <param name="element">The element to insert.</param>
		public void			Insert(int index, T element)
		{
			if (index >= 0  &&  index <= Count)	_lst.Insert(index, element);
		}
		/// <summary>
		/// Removes the specified element from this list.
		/// </summary>
		/// <param name="element">The element to remove.</param>
		public void			Remove(T element)
		{
			_lst.Remove(element);
		}
		/// <summary>
		/// Removes the element with specified index from this list.
		/// </summary>
		/// <param name="index">Index of the element to remove.</param>
		public void			Remove(int index)
		{
			if (index >= 0  &&  index < Count)	 _lst.RemoveAt(index);
		}
		/// <summary>
		/// Removes the last element in this list.
		/// </summary>
		public void			Remove()
		{
			if (Count != 0)	 _lst.RemoveAt(Count-1);
		}
		/// <summary>
		/// Removes the specified range of elements from this list.
		/// </summary>
		/// <param name="startIndex">Index of the first element in the range to remove.</param>
		/// <param name="count">Count of elements to remove.</param>
		public void			Remove(int startIndex, int count)
		{
			_lst.RemoveRange(startIndex, count);
		}
		/// <summary>
		/// Removes the elements by specified predicate.
		/// </summary>
		/// <param name="match">The predicate to use.</param>
		public void			Remove(Predicate<T> match)
		{	_lst.RemoveAll(match);	}
		/// <summary>
		/// Truncates this list starting at given index.
		/// </summary>
		/// <param name="index">Truncating starts at this index.</param>
		public void			Truncate(int index)
		{
			if (index < this.Count)
				_lst.RemoveRange(index, this.Count-index);
		}
		/// <summary>
		/// Gets whether this list contains the specified element.
		/// </summary>
		/// <param name="element">The element to search for.</param>
		/// <returns>Returns TRUE if specified element is found in this list, otherwise returns FALSE.</returns>
		public bool			Contains(T element)
		{	return _lst.Contains(element); }
		/// <summary>
		/// Gets the index of specified element in this list.
		/// </summary>
		/// <param name="element">The element to search for.</param>
		/// <returns>Returns the index of specified element in this list if it exists, otherwise return -1.</returns>
		public int			IndexOf(T element)
		{	return _lst.IndexOf(element);	}
		/// <summary>
		/// Clears the list.
		/// </summary>
		public void			Clear()
		{	_lst.Clear();	}
		/// <summary>
		/// Gets the array representation for this list.
		/// </summary>
		/// <returns>Returns the array representation for this list.</returns>
		public T[]			ToArray()
		{	return _lst.ToArray();	}

		#endregion


		#region Copy, Rotate, ToString

		/// <summary>
		/// Creates a deep copy of this BaseList.
		/// </summary>
		/// <returns>Returns a deep copy of this BaseList.</returns>
		public BaseList<T>	Copy()
		{
			var TRA = new T[this.Count];
			Array.Copy(_lst.ToArray(), TRA, TRA.Length);
			var TRL = new BaseList<T>();
			for (int i = 0; i < TRA.Length; i++)	TRL.Add(TRA[i]);
			return TRL;
		}

		/// <summary>
		/// Rotate this list.
		/// </summary>
		/// <param name="DX">Offset to rotate at, may be positive or negative.</param>
		/// <returns>Returns the resulting list.</returns>
		public BaseList<T>	Rotate(int DX)
		{
			var N = new BaseList<T>();
			for (int i = 0; i < this.Count; i++)
			{
				int k = i + DX;
				if (k < 0)	k += this.Count;
				k = k % this.Count;
				N.Add(_lst[k]);
			}
			return N;
		}

		/// <summary>
		/// Returns the string representation of this BaseList instance.
		/// </summary>
		/// <returns>Returns the string representation of this BaseList instance.</returns>
		public new string	ToString()
		{	return "Count = " + Count;	}

		#endregion


		#region Constructor  &  Enumerator

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>Returns the enumerator.</returns>
		public IEnumerator<T>	GetEnumerator()
		{
			return _lst.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Constructor of this BaseList.
		/// </summary>
		public BaseListSimple()
		{
			_lst = new List<T>();
		}

		#endregion
	}




	/// <summary>
	/// This class defines the list of byte.
	/// </summary>
	public class ByteList		: BaseList<byte>
	{
		/// <summary>
		/// Returns the sum of elements, or -1 if this ByteList is empty.
		/// </summary>
		public long		Sum			{	get	{	if (Count == 0) return -1;	return ZMath.Sum_List(this);	}}

		/// <summary>
		/// Defines if all elements in this ByteList has value (not zero).
		/// </summary>
		public new bool	AllHasValues
		{	get
			{	
				if (this.Count == 0)	return false;
				for (int i = 0; i < Count; i++)	 if (this[i] == 0)	return false;	return true;
			}
		}

		//	Min/Max
		#region
		/// <summary>
		/// Returns the minimal element, or 255 if this ByteList is empty.
		/// </summary>
		public byte		MinElement	
		{	
			get
			{	
				if (this.Count == 0)	return 255;
				byte N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] < N)	N = this[i];
				return N;	
			}
		}
		/// <summary>
		/// Returns the maximal element, or 0 if this ByteList is empty.
		/// </summary>
		public byte		MaxElement	
		{	
			get
			{	
				if (this.Count == 0)	return 0;
				byte N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] > N)	N = this[i];
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of minimal element, or 255 if this ByteList is empty.
		/// </summary>
		public int		MinIndex	
		{	get
			{	
				if (this.Count == 0)	return 255;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] < this[N])	N = i;
				return N;	
			}
		}
		/// <summary>
		/// Returns the index of maximal element, or 255 if this ByteList is empty.
		/// </summary>
		public int		MaxIndex	
		{	get
			{	
				if (this.Count == 0)	return 255;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] > this[N])	N = i;
				return N;	
			}
		}
		#endregion

		//	Constructors
		#region
		/// <summary>
		/// Constructor of ByteList.
		/// </summary>
		public ByteList()	{}
		/// <summary>
		/// Constructor of ByteList with given count of elements.
		/// </summary>
		/// <param name="CountOfElements">Count of elements.</param>
		public ByteList(int CountOfElements)
		{
			this.AddRange(new byte[CountOfElements]);
		}
		/// <summary>
		/// Constructor of ByteList from source array of integer values.
		/// </summary>
		/// <param name="SourceArray">Source array int[].</param>
		public ByteList(int[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add((byte)SourceArray[i]);
		}
		/// <summary>
		/// Constructor of ByteList from source array of float values.
		/// </summary>
		/// <param name="SourceArray">Source array float[].</param>
		public ByteList(float[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add((byte)SourceArray[i]);
		}
		/// <summary>
		/// Constructor of ByteList from source byte array.
		/// </summary>
		/// <param name="SourceArray">Source array byte[].</param>
		public ByteList(byte[] SourceArray)
		{
			this.AddRange(SourceArray);
		}
		#endregion
	}



	/// <summary>
	/// This class defines the list of integer values.
	/// </summary>
	public class IntList		: BaseList<int>
	{
		/// <summary>
		/// Returns the sum of elements, or -1 if this IntList is empty.
		/// </summary>
		public long		Sum			{	get	{	if (Count == 0) return -1;	else return ZMath.Sum_List(this);	}}

		/// <summary>
		/// Defines if all elements in this IntList has value (not zero).
		/// </summary>
		public new bool	AllHasValues
		{	get
			{	
				if (this.Count == 0)	return false;
				for (int i = 0; i < Count; i++)	 if (this[i] == 0)	return false;	return true;
			}
		}

		/// <summary>
		/// Add given value if it's not zero.
		/// </summary>
		/// <param name="Value">Value to add.</param>
		public void		AddNotZero(int Value)
		{
			AddNotZero(Value, false);
		}
		/// <summary>
		/// Add given value if it's not zero.
		/// </summary>
		/// <param name="Value">Value to add.</param>
		/// <param name="AddIfContains">If true, add even if this list already contains this value,
		/// otherwise only if this list doesn't cointain this value yet.</param>
		public void		AddNotZero(int Value, bool AddIfContains)
		{
			if (Value == 0)	 return;
			if (!this.Contains(Value)  ||  AddIfContains)
				this.Add(Value);
		}

		//	Min/Max
		#region
		/// <summary>
		/// Returns the minimal element, or -1 if this IntList is empty.
		/// </summary>
		public int		MinElement	
		{	
			get
			{	
				if (this.Count == 0)	return -1;
				int N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] < N)	N = this[i];
				return N;	
			}
		}
		/// <summary>
		/// Returns the maximal element, or -1 if this IntList is empty.
		/// </summary>
		public int		MaxElement	
		{	
			get
			{	
				if (this.Count == 0)	return -1;
				int N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] > N)	N = this[i];
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of minimal element, or -1 if this IntList is empty.
		/// </summary>
		public int		MinIndex	
		{	get
			{	
				if (this.Count == 0)	return -1;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] < this[N])	N = i;
				return N;	
			}
		}
		/// <summary>
		/// Returns the index of maximal element, or -1 if this IntList is empty.
		/// </summary>
		public int		MaxIndex	
		{	get
			{	
				if (this.Count == 0)	return -1;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] > this[N])	N = i;
				return N;	
			}
		}
		#endregion

		//	Constructors
		#region
		/// <summary>
		/// Constructor of IntList.
		/// </summary>
		public IntList()	{}
		/// <summary>
		/// Constructor of IntList with given count of elements.
		/// </summary>
		/// <param name="CountOfElements">Count of elements.</param>
		public IntList(int CountOfElements)
		{
			this.AddRange(new int[CountOfElements]);
		}
		/// <summary>
		/// Constructor of IntList from source array of integer values.
		/// </summary>
		/// <param name="SourceArray">Source array int[].</param>
		public IntList(int[] SourceArray)
		{
			this.AddRange(SourceArray);
		}
		/// <summary>
		/// Constructor of IntList from source array of float values.
		/// </summary>
		/// <param name="SourceArray">Source array float[].</param>
		public IntList(float[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add((int)SourceArray[i]);
		}
		/// <summary>
		/// Constructor of IntList from source byte array.
		/// </summary>
		/// <param name="SourceArray">Source array byte[].</param>
		public IntList(byte[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add(SourceArray[i]);
		}
		#endregion
	}



	/// <summary>
	/// This class defines the list of float values.
	/// </summary>
	public class FloatList		: BaseList<float>
	{
		/// <summary>
		/// Returns the sum of all elements, or -1 if this FloatList is empty.
		/// </summary>
		public float		Sum		{	get { return Count == 0 ? -1 : ZMath.Sum_List(this); }
		}

		/// <summary>
		/// Defines if all elements in this FloatList has value (not zero).
		/// </summary>
		public new bool		AllHasValues
		{	get
			{	
				if (this.Count == 0)	return false;
				for (int i = 0; i < Count; i++)	 if (this[i] == 0f)	return false;	return true;
			}
		}

		/// <summary>
		/// Add given value if it's not zero.
		/// </summary>
		/// <param name="value">Value to add.</param>
		public void			AddNotZero(float value)
		{
			AddNotZero(value, false);
		}
		/// <summary>
		/// Add given value if it's not zero.
		/// </summary>
		/// <param name="value">Value to add.</param>
		/// <param name="addIfContains">If true, add even if this list already contains this value,
		/// otherwise only if this list doesn't cointain this value yet.</param>
		public void			AddNotZero(float value, bool addIfContains)
		{
			if (value == 0)	 return;
			if (!Contains(value)  ||  addIfContains)
				Add(value);
		}

		//	Min/Max
		#region
		/// <summary>
		/// Returns the minimal element, or -1 if this FloatList is empty.
		/// </summary>
		public float	MinElement	
		{	get
			{	
				if (this.Count == 0)	return -1f;
				float N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] < N)	N = this[i];
				return N;	
			}
		}

		/// <summary>
		/// Returns the maximal element, or -1 if this FloatList is empty.
		/// </summary>
		public float	MaxElement	
		{	get
			{	
				if (this.Count == 0)	return -1;
				float N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i] > N)	N = this[i];
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of minimal element, or -1 if this FloatList is empty.
		/// </summary>
		public int		MinIndex	
		{	get
			{	
				if (this.Count == 0)	return -1;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] < this[N])	N = i;
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of maximal element, or -1 if this FloatList is empty.
		/// </summary>
		public int		MaxIndex	
		{	get
			{	
				if (this.Count == 0)	return -1;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i] > this[N])	N = i;
				return N;	
			}
		}
		#endregion

		//	Constructors
		#region
		/// <summary>
		/// Constructor of FloatList.
		/// </summary>
		public FloatList()	{}
		/// <summary>
		/// Constructor of FloatList with given count of elements.
		/// </summary>
		/// <param name="CountOfElements">Count of elements.</param>
		public FloatList(int CountOfElements)
		{
			this.AddRange(new float[CountOfElements]);
		}
		/// <summary>
		/// Constructor of FloatList from source array of integer values.
		/// </summary>
		/// <param name="SourceArray">Source array int[].</param>
		public FloatList(int[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add(SourceArray[i]);
		}
		/// <summary>
		/// Constructor of FloatList from source array of float values.
		/// </summary>
		/// <param name="SourceArray">Source array float[].</param>
		public FloatList(float[] SourceArray)
		{
			this.AddRange(SourceArray);
		}
		/// <summary>
		/// Constructor of FloatList from source byte array.
		/// </summary>
		/// <param name="SourceArray">Source array byte[].</param>
		public FloatList(byte[] SourceArray)
		{
			for (int i = 0; i < SourceArray.Length; i++)
				this.Add(SourceArray[i]);
		}
		#endregion
	}



	/// <summary>
	/// This class defines the list of strings.
	/// </summary>
	public class StringList		: BaseList<string>
	{
		//	Main fields & Properties
		#region
		/// <summary>
		/// Gets entire text of this StringList.
		/// </summary>
		public string	EntireText
		{
			get
			{
				string S = "";
				for (int i = 0; i < this.Count; i++)	S += this[i];
				return S;
			}
		}

		/// <summary>
		/// Checks if any of strings in this StringList contains given text.
		/// </summary>
		/// <param name="Text">The text string to find.</param>
		/// <returns>True if any of strings in this StringList contains given text, otherwise false.</returns>
		public bool		ContainsText(string Text)
		{
			for (int i = 0; i < this.Count; i++)
				if (this[i] != null  &&  this[i].Contains(Text))	 return true;
			return false;
		}

		/// <summary>
		/// Defines if all elements in this IntList has value (not zero).
		/// </summary>
		public new bool	AllHasValues{	get
			{	
				if (this.Count == 0)	return false;
				for (int i = 0; i < Count; i++)	 if (this[i] == null  ||  this[i] == "")  return false;	return true;
			}
		}
		#endregion

		//	Shortest/Longest
		#region
		/// <summary>
		/// Returns the shortest string element, or empty string if this StringList is empty.
		/// </summary>
		public string	ShortestString
		{	
			get
			{	
				if (this.Count == 0)	return "";
				string N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i].Length < N.Length)	N = this[i];
				return N;	
			}
		}
		/// <summary>
		/// Returns the longest string element, or empty string if this StringList is empty.
		/// </summary>
		public string	LongestString	
		{	
			get
			{	
				if (this.Count == 0)	return "";
				string N = this[0];
				for (int i = 1; i < this.Count; i++)	if (this[i].Length > N.Length)	N = this[i];
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of shortest string, or -1 if this StringList is empty.
		/// </summary>
		public int		ShortestIndex	
		{	get
			{	
				if (this.Count == 0)	return -1;
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Length < this[N].Length)	N = i;
				return N;	
			}
		}
		/// <summary>
		/// Returns the index of longest string, or -1 if this StringList is empty.
		/// </summary>
		public int		LongestIndex	
		{	get
			{	
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Length > this[N].Length)	N = i;
				return N;	
			}
		}
		#endregion

		//	Constructors
		#region
		/// <summary>
		/// Constructor of StringList.
		/// </summary>
		public StringList()		{}

		/// <summary>
		/// Constructor of StringList from given source string array.
		/// </summary>
		/// <param name="Source">Source string array to construct from.</param>
		public StringList(string[] Source)
		{
			for (int i = 0; i < Source.Length; i++)		this.Add(Source[i]);
		}
		#endregion
	}


	/// <summary>
	/// This class defines the additional properties and methods for Dictionary[string, string]
	/// </summary>
	public class DictionaryStringString : Dictionary<string, string>
	{
	}

	#endregion



	#region Matrixes

	/// <summary>
	/// This class defines the integer matrix.
	/// </summary>
	public class IntMatrix		: BaseList<IntList>
	{
		/// <summary>
		/// Gets the sum of all elements in this matrix.
		/// </summary>
		public long			Sum
		{ 
			get
			{
				if (Count == 0) return -1;
				
				long N = 0;
				for (int i = 0; i < this.Count; i++)	N += ZMath.Sum_List(this[i]);
				return N;
			}
		}

		/// <summary>
		/// Returns minimal element.
		/// </summary>
		public long			MinSum	
		{	
			get
			{	
				long N = this[0].Sum;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum < N)	N = this[i].Sum;
				return N;	
			}
		}

		/// <summary>
		/// Returns maximal element.
		/// </summary>
		public long			MaxSum	
		{	
			get
			{	
				long N = this[0].Sum;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum > N)	N = this[i].Sum;
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of minimal element.
		/// </summary>
		public int			MinIndex
		{	
			get
			{	
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum < this[N].Sum)	N = i;
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of maximal element.
		/// </summary>
		public int			MaxIndex
		{	
			get
			{	
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum > this[N].Sum)	N = i;
				return N;	
			}
		}

		/// <summary>
		/// Defines if all elements in this IntMatrix has value (not zero).
		/// </summary>
		public new bool		AllHasValues
		{	get	{	for (int i = 0; i < Count; i++)	if (this[i].Sum == 0)	return false;	return true;	}}
	}



	/// <summary>
	/// This class defines the Float matrix.
	/// </summary>
	public class FloatMatrix	: BaseList<FloatList>
	{
		/// <summary>
		/// Gets the sum of all elements in this matrix.
		/// </summary>
		public float		Sum
		{	get
			{
				if (Count == 0) return -1;
				else
				{
					float N = 0;
					for (int i = 0; i < this.Count; i++)	N += ZMath.Sum_List(this[i]);
					return N;
				}
			}
		}

		/// <summary>
		/// Gets maximal count of elements in any of FloatLists in this FloatMatrix.
		/// </summary>
		public int			MaxElementsCount
		{	get
			{
				if (Count == 0) return -1;
				else
				{
					int N = 0;
					for (int i = 0; i < this.Count; i++)	if (N < this[i].Count)	N = this[i].Count;
					return N;
				}
			}
		}

		/// <summary>
		/// Returns minimal element.
		/// </summary>
		public float		MinSum	
		{	get
			{	
				float N = this[0].Sum;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum < N)	N = this[i].Sum;
				return N;	
			}
		}

		/// <summary>
		/// Returns maximal element.
		/// </summary>
		public float		MaxSum	
		{	get
			{	
				float N = this[0].Sum;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum > N)	N = this[i].Sum;
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of minimal element.
		/// </summary>
		public int			MinIndex
		{	get
			{	
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum < this[N].Sum)	N = i;
				return N;	
			}
		}

		/// <summary>
		/// Returns the index of maximal element.
		/// </summary>
		public int			MaxIndex
		{	get
			{	
				int N = 0;
				for (int i = 1; i < this.Count; i++)	if (this[i].Sum > this[N].Sum)	N = i;
				return N;	
			}
		}

		/// <summary>
		/// Defines if all elements in this FloatMatrix has value (not zero).
		/// </summary>
		public new bool		AllHasValues
		{	get	{	for (int i = 0; i < Count; i++)	if (this[i].Sum == 0f)	return false;	return true;	}}

		/// <summary>
		/// Writes this matrix into .csv file.
		/// </summary>
		/// <param name="FileName">Name of result file.</param>
		public void			WriteCSV(string FileName)
		{
			//if (FileName.EndsWith(".csv") == false)	FileName += ".csv";
			//var F = File.Open(FileName, FileMode.Create, FileAccess.Write);

			//for (int i = 0; i < Count; i++)
			//{
			//    for (int j = 0; j < this[i].Count; j++)
			//    {
			//        var A = Tools.Get_Bytes(this[i][j].ToString());
			//        F.Write(A, 0, A.Length);
			//        if (j < this[i].DC)	F.WriteByte((byte)';');
			//    }
			//    F.WriteByte(13);	F.WriteByte(10);
			//}
			//F.Close();
		}
	}

	#endregion



	#region Common classes

	/// <summary>
	/// This class defines the set of properties and methods for data/time manipulations.
	/// </summary>
	public class ZTimeDate
	{
		#region Private Fields

		private int		_day;
		private int		_month;
		private int		_year;
		private int		_dayOfWeek;
		private int		_hours;
		private int		_mins;
		private int		_secs;

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the Day of Month value.
		/// </summary>
		public int		Day			{	get	{	return _day;	}		set {	_day	= ZMath.GetBound(value, 1, 31);	}}
		/// <summary>
		/// Gets or sets the Month value.
		/// </summary>
		public int		Month		{	get	{	return _month;	}		set {	_month	= ZMath.GetBound(value, 1, 12);	}}
		/// <summary>
		/// Gets or sets the Year value.
		/// </summary>
		public int		Year		{	get	{	return _year;	}		set {	_year	= ZMath.GetBound(value, 1980, 2100); }}
		/// <summary>
		/// Gets or sets the Day of Week value.
		/// </summary>
		public int		DayOfWeek	{	get	{	return _dayOfWeek;	}	set {	_dayOfWeek	= ZMath.GetBound(value, 0, 6);	}}
		/// <summary>
		/// Gets or sets the Hours value.
		/// </summary>
		public int		Hours		{	get	{	return _hours;	}		set {	_hours	= ZMath.GetBound(value, 0, 23);	}}
		/// <summary>
		/// Gets or sets the Minutes value.
		/// </summary>
		public int		Mins		{	get	{	return _mins;	}		set {	_mins	= ZMath.GetBound(value, 0, 59);	}}
		/// <summary>
		/// Gets or sets the Seconds value.
		/// </summary>
		public int		Secs		{	get	{	return _secs;	}		set {	_secs	= ZMath.GetBound(value, 0, 59);	}}

		#endregion

		#region Copy, ToString, Validate, Equals

		/// <summary>
		/// Creates a deep copy of this ZTimeDate instance.
		/// </summary>
		/// <returns>A deep copy of this ZTimeDate instance.</returns>
		public ZTimeDate		Copy()
		{
			var newTimeDate = new ZTimeDate
				         {
					         _month = _month,
					         _year	= _year,
					         _day	= _day,
					         _hours = _hours,
					         _mins	= _mins,
					         _secs	= _secs,
							 _dayOfWeek = _dayOfWeek
				         };
			return newTimeDate;
		}

		/// <summary>
		/// Returns the string representation of this ZTimeDate instance.
		/// </summary>
		/// <returns>Returns the string representation of this ZTimeDate instance.</returns>
		public new string		ToString()
		{	
			if (Year == 0  ||  Month == 0   ||  Day == 0)
				return "Not specified";
			return (Year+1900) + "-" + ZString.Get_2Digit(Month) + "-" + ZString.Get_2Digit(Day)
				+ ", " + ZString.Get_2Digit(Hours) + ":" + ZString.Get_2Digit(Mins);
		}

		/// <summary>
		/// Gets whether this ZTimeDate instance is equal to another (compared by their fields).
		/// </summary>
		/// <param name="timeDate">The ZTimeDate instance to compare with.</param>
		/// <returns>Returns TRUE if this ZTimeDate is equal to given ZTimeDate, otherwise returns FALSE.</returns>
		public bool				Equals(ZTimeDate timeDate)
		{
			return  Year  == timeDate.Year	&&  Month == timeDate.Month	&&  Day  == timeDate.Day	&&  DayOfWeek == timeDate.DayOfWeek
			    &&  Hours == timeDate.Hours	&&  Mins  == timeDate.Mins	&&  Secs == timeDate.Secs;
		}

		#endregion
	}

	public enum BitsPerColor
	{
		Monochrome  = 1,
		Bits_4		= 4,
		Bits_8		= 8,
		Bits_16		= 16,
		Bits_24		= 24,
		Bits_32		= 32,
	}

	#endregion
}
