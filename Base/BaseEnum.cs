using System;
using System.Collections;

// -> Disposable.Base -> Enum.Base

namespace CoreObjects.Enum
{
	#region [ Base ]

	// Abstract class is a class that has no direct instances, but whose descendants may have direct instances.

	[Serializable()]
	public abstract class Base : Disposable.Base, IEnumerable
	{
		#region [ Private Variables ]

		private bool _isLoaded = false;
		private bool _suppressEvents = false;

		#endregion

		#region [ Protected Variables ]

		protected ArrayList _list;
		protected SortedList _sortedList;
		protected bool _useSortedList = false;
		protected object _tag;

		#endregion

		#region [ Property Assignments ]

		/// <summary>
		/// Flag to indicate if Events should fire
		/// </summary>
		public bool SuppressEvents
		{
			get { return _suppressEvents; }
			set { _suppressEvents = value; }
		}

		/// <summary>
		/// Flag to indicate if this class uses the SortedList
		/// </summary>
		public bool UsingSortedList
		{
			get { return _useSortedList; }
		}

		/// <summary>
		/// Polymorphic property.
		/// </summary>
		public virtual object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		/// <summary>
		/// Returns the size of the List.
		/// </summary>
		public virtual int Count
		{
			get
			{
				if (_useSortedList)
					return _sortedList.Count;
				else
					return _list.Count;
			}
		}

		/// <summary>
		/// Flag to indicate whether or not data has been loaded into this object.
		/// </summary>
		public virtual bool IsLoaded
		{
			get { return _isLoaded; }
			set { _isLoaded = value; }
		}

		#endregion

		#region [ Constructors ]

		/// <summary>
		/// Basic Constructor for this class
		/// </summary>
		public Base()
		{
			// Do Not put initialize here.  That causes major problems!
			_tag = null;
			_useSortedList = false;
			_isLoaded = false;
		}

		/// <summary>
		/// Constructor that indicated we need to use the SortedList instead of the ArrayList
		/// </summary>
		/// <param name="useSortedList">bool</param>
		public Base(bool useSortedList)
			: this()
		{
			//Do Not put initialize here.  That causes major problems!
			_useSortedList = useSortedList;
		}

		/// <summary>
		/// Initialize variables/objects on creation, and destroy object on destruction
		/// </summary>
		/// <param name="setObjects">bool</param>
		protected override void Initialize(bool setObjects)
		{
			_isLoaded = false;

			if (setObjects)
			{
				if (_useSortedList)
					_sortedList = new SortedList();
				else
					_list = new ArrayList();
			}
			else
			{
				_list = null;
				_sortedList = null;
				_tag = null;
			}
		}

		#endregion

		#region [ Events/Delegates ]

		public delegate void ClearingListEventHandler(ref bool Cancel);
		private ClearingListEventHandler ClearingListEvent;

		public event ClearingListEventHandler ClearingList
		{
			add
			{
				ClearingListEvent = (ClearingListEventHandler)System.Delegate.Combine(ClearingListEvent, value);
			}
			remove
			{
				ClearingListEvent = (ClearingListEventHandler)System.Delegate.Remove(ClearingListEvent, value);
			}
		}

		#endregion

		/// <summary>
		/// Empties out the list.
		/// </summary>
		public virtual void Clear()
		{
			bool Cancel = false;
			if (SuppressEvents == false)
			{
				if (ClearingListEvent != null)
					ClearingListEvent(ref Cancel);
			}

			if (!Cancel)
			{
				if (_useSortedList)
					_sortedList.Clear();
				else
					_list.Clear();

				_isLoaded = false;

			}
		}

		/// <summary>
		/// Resets the list (same as Clear())
		/// </summary>
		protected virtual void ResetList()
		{
			Clear();
		}

		/// <summary>
		/// This is used with the foreach statement.
		/// </summary>
		/// <returns>IEnumerator</returns>
		public virtual IEnumerator GetEnumerator()
		{ 
			if (_useSortedList)
				return new BaseEnumerator(_sortedList);
			else
				return new BaseEnumerator(_list); 
		}
	}
	
	#endregion [ Base ]

	#region [ Base Enumerator ]

	internal class BaseEnumerator : Disposable.Base, IEnumerator
	{
		#region [ Private Variables ]

		private ArrayList _list;
		private int _position = -1;
		private SortedList _sortedList;
		private bool _useSortedList = true;

		#endregion

		#region [ Properties ]

		public object Current
		{
			get
			{
				if (_useSortedList)
					return _sortedList.GetByIndex(_position);
				else
					return _list[_position];
			}
		}

		#endregion

		#region [ Constructors ]

		public BaseEnumerator(ArrayList Lists)
		{
			_list = Lists;
			_useSortedList = false;
		}

		public BaseEnumerator(SortedList Lists)
		{
			_sortedList = Lists;
			_useSortedList = true;
		}

		#endregion

		protected override void Initialize(bool setObjects)
		{
			if (_list != null)
				_list = null;

			if (_sortedList != null)
				_sortedList = null;
		}

		public bool MoveNext()
		{
			_position++;
			if (_useSortedList)
				return (_position < _sortedList.Count);
			else
				return (_position < _list.Count);
		}

		public void Reset()
		{
			_position = -1;
		}
	}

	#endregion [ Base Enumerator ]

}
