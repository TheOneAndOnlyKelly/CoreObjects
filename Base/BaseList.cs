using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;

namespace CoreObjects.List
{
	// -> Disposable.Base -> Enum.Base -> List.Base -> List.BaseWithIO

	#region [ List ]

	[Serializable()]
	public abstract class Base : CoreObjects.Enum.Base
	{
		#region [ Protected Variables ]

		protected bool _cancelLoad;
		protected bool _dirty;
		protected bool _isLoaded;

		#endregion

		#region [ Property Assignments ]

		/// <summary>
		/// Has this object been modified?
		/// </summary>
		[Browsable(false)]
		public virtual bool Dirty
		{
			get { return _dirty; }
			set { _dirty = value; }
		}

		/// <summary>
		/// Boolean flag that lets us know if the object has been loaded.
		/// Upon setting, we flip off the Dirty bit.
		/// </summary>
		[Browsable(false)]
		public new bool IsLoaded
		{
			get { return _isLoaded; }
			set
			{
				_isLoaded = value;
				if (_isLoaded)
					_dirty = false;
			}
		}

		#endregion

		#region [ Constructors ]

		public Base()
		{
			Initialize(true);
		}
	
		#endregion

		#region [ Event Handlers ]

		public delegate void ItemAddedEventHandler(ref Item.Base item);
		private ItemAddedEventHandler ItemAddedEvent;

		public event ItemAddedEventHandler ItemAdded
		{
			add
			{ ItemAddedEvent = (ItemAddedEventHandler)System.Delegate.Combine(ItemAddedEvent, value); }
			remove
			{ ItemAddedEvent = (ItemAddedEventHandler)System.Delegate.Remove(ItemAddedEvent, value); }
		}

		public delegate void ItemAddedWithCancelEventHandler(ref Item.Base item, ref bool cancel);
		private ItemAddedWithCancelEventHandler ItemAddedWithCancelEvent;

		public event ItemAddedWithCancelEventHandler ItemAddedWithCancel
		{
			add
			{ ItemAddedWithCancelEvent = (ItemAddedWithCancelEventHandler)System.Delegate.Combine(ItemAddedWithCancelEvent, value); }
			remove
			{ ItemAddedWithCancelEvent = (ItemAddedWithCancelEventHandler)System.Delegate.Remove(ItemAddedWithCancelEvent, value); }
		}

		public delegate void ItemRemovedEventHandler(ref Item.Base item, ref bool cancel);
		private ItemRemovedEventHandler ItemRemovedEvent;

		public event ItemRemovedEventHandler ItemRemoved
		{
			add
			{ ItemRemovedEvent = (ItemRemovedEventHandler)System.Delegate.Combine(ItemRemovedEvent, value); }
			remove
			{ ItemRemovedEvent = (ItemRemovedEventHandler)System.Delegate.Remove(ItemRemovedEvent, value); }
		}

		#endregion

		#region [ Item ]

		/// <summary>
		/// Retrieves the Item at Index position
		/// </summary>
		/// <param name="index">int</param>
		/// <returns>Item.Base</returns>
		public virtual Item.Base Item(int index)
		{
			if (_useSortedList)
				return ((Item.Base)_sortedList[index]);
			else
				return ((Item.Base)_list[index]);
		}

		/// <summary>
		/// Retrieves the Item with the Key
		/// </summary>
		/// <param name="key">string</param>
		/// <returns>Item.Base</returns>
		public virtual Item.Base Item(string key)
		{
			if (_useSortedList)
				return (Item.Base)_sortedList[key];

			for (int Idx = 0; Idx <= _list.Count - 1; Idx++)
			{
				if (((Item.Base)_list[Idx]).Key == key)
				{
					return ((Item.Base)_list[Idx]);
				}
			}

			return null;
		}

		#endregion

		public virtual ArrayList List()
		{
			return _list;
		}

		public SortedList SortedList
		{
			get { return _sortedList; }
		}

		#region [ Add ]

		public virtual void Add(object value)
		{
			Add(string.Empty, value);
		}

		public virtual void Add(string key, object value)
		{
			if (_useSortedList)
			{
				if (key.Length == 0)
					key = "KEY_" + _sortedList.Count.ToString();
				_sortedList.Add(key, value);
			}
			else
				_list.Add(value);
		}

		public virtual void AddWithCancel(Item.Base myItem)
		{
			_list.Add(myItem);
			if (base.SuppressEvents == false)
			{
				if (ItemAddedWithCancelEvent != null)
					ItemAddedWithCancelEvent(ref myItem, ref _cancelLoad);
			}
			if (!_cancelLoad)
				_dirty = true;
		}
		
		#endregion

		#region [ Remove ]

		/// <summary>
		/// removes the Item at Index
		/// </summary>
		/// <param name="index">int</param>
		public virtual void RemoveAt(int index)
		{
			bool Cancel = false;
			Item.Base MyItem = null;

			if (SuppressEvents == false)
			{
				if (ItemRemovedEvent != null)
					MyItem = (Item.Base)_list[index];
				ItemRemovedEvent(ref MyItem, ref Cancel);
			}

			if (Cancel)
				return;

			if (_useSortedList)
				_sortedList.RemoveAt(index);
			else
				_list.RemoveAt(index);
			_dirty = true;
		}
		
		/// <summary>
		/// Removes the Item that has Key
		/// </summary>
		/// <param name="key">string</param>
		public virtual void Remove(string key)
		{
			bool Cancel = false;
			Item.Base MyItem = null;

			if (_useSortedList)
			{
				if (SuppressEvents == false)
				{
					if (ItemRemovedEvent != null)
						MyItem = (Item.Base)_sortedList[key];
					ItemRemovedEvent(ref MyItem, ref Cancel);
				}
				if (Cancel) return;

				_sortedList.Remove(key);
				_dirty = true;
			}
			else
			{
				for (int Idx = 0; Idx <= _list.Count - 1; Idx++)
				{
					if (((Item.Base)_list[Idx]).Key == key)
					{
						RemoveAt(Idx);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Removes the Item that corresponds to value.
		/// </summary>
		/// <param name="value">object</param>
		public virtual void Remove(Item.Base value)
		{
			bool Cancel = false;

			if (SuppressEvents == false)
			{
				if (ItemRemovedEvent != null)
					ItemRemovedEvent(ref value, ref Cancel);
			}

			if (Cancel)
				return;

			if (_useSortedList)
				_sortedList.Remove(value);
			else
				_list.Remove(value);
		}

		#endregion

		#region [ Insert ]

		/// <summary>
		/// Inserts the Item at Index position. If we are using a SortedList, then just
		/// adds the Item (it will be sorted!)
		/// </summary>
		/// <param name="index">int</param>
		/// <param name="value">object</param>
		public virtual void Insert(int index, object value)
		{
			if (_useSortedList)
				Add(value);
			else
				_list.Insert(index, value);
		}

		#endregion

		/// <summary>
		/// Dynamically Populates the properties of an object from an SQL Data Reader using Reflection.
		/// </summary>
		/// <param name="objectToPopulate">The object to populate.</param>
		/// <param name="sqlDataReader">The SQL data reader.</param>
		public void DataReaderToObjectProperties(ref object objectToPopulate, IDataReader sqlDataReader)
		{
			try
			{
				foreach (PropertyInfo pi in objectToPopulate.GetType().GetProperties())
				{
					if (pi.CanWrite)
					{
						try
						{
							if (sqlDataReader.GetName(sqlDataReader.GetOrdinal(pi.Name)) != null && !sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal(pi.Name)))
							{
								pi.SetValue(objectToPopulate, sqlDataReader.GetValue(sqlDataReader.GetOrdinal(pi.Name)), null);
							}
							else
							{
								pi.SetValue(objectToPopulate, null, null);
							}
						}
						catch
						{
							pi.SetValue(objectToPopulate, null, null);
						}
					}
				}
			}
			finally
			{
			}
		}

	}

	#endregion

	#region [ BaseWithIO ]

	[Serializable()]
	public abstract class BaseWithIO : Base
	{
		public abstract bool Load(BinaryReader reader);
		public abstract bool Save(BinaryWriter writer);

		#region [ Constructors ]

		public BaseWithIO(): base()
		{ }

		public BaseWithIO(object tag)
			: this()
		{
			_tag = tag;
		}

		#endregion

	}

	#endregion

	#region [ Generic List ]

	[Serializable()]
	public class GenericList : List.BaseWithIO
	{

		#region [ Constructors ]

		public GenericList(): base()
		{ }

		#endregion

		public override bool Save(BinaryWriter writer)
		{
			return true;
		}

		public override bool Load(BinaryReader reader)
		{
			return true;
		}

	}

	#endregion

	#region [ Bindable ]

	namespace Bindable
	{
		[Serializable()]
		public abstract class BaseList : BaseWithIO
		{
			protected System.Data.SqlClient.SqlDataReader _reader;

			public abstract System.Data.SqlClient.SqlDataReader DataSource();
						public abstract void CloseDB();

			#region [ Constructors ]

			public BaseList()
			{ }

			#endregion

			protected override void Initialize(bool setObjects)
			{
				base.Initialize(setObjects);

				if (setObjects)
				{ }
				else
				{
					Destroy(ref _reader);
					CloseDB();
				}
			}
		}

	}

	#endregion
}