using System;
using System.ComponentModel;

namespace CoreObjects.Item
{
	[Serializable()]
	public abstract class Base : Disposable.Base
	{

		#region [ Private Variables ]

		private bool _suppressEvents = false;
		private bool _dirty;
		private bool _isLoaded;

		#endregion

		#region [ Projected Variables ]

		protected string _key;
		protected object _tag;

		#endregion

		#region [ Properties ]

		/// <summary>
		/// This is a generic property that we can attach strings/objects to the class that inherits.
		/// </summary>
		[Browsable(false)]
		public object Tag
		{
			get { return _tag; }
			set
			{
				if (_tag != value)
				{
					_tag = value;
					_dirty = true;
				}
			}
		}

		[Browsable(false)]
		public virtual string Key
		{
			get { return _key; }
			set
			{
				if (_key != value)
				{
					if (!(_suppressEvents))
					{
						if (KeyChangedEvent != null)
							KeyChangedEvent(this, value, _key);
					}

					_key = value;
					_dirty = true;
				}
			}
		}

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
		public virtual bool IsLoaded
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

		/// <summary>
		/// Base constructor
		/// </summary>
		public Base()
		{
			Init();
		}

		/// <summary>
		/// Constructor that sets the key value
		/// </summary>
		/// <param name="key">string</param>		
		public Base(string key)
			: this()
		{
			_key = key;
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		protected void Init()
		{
			_key = string.Empty;
			_dirty = false;
			_tag = null;
			_isLoaded = false;

			Initialize(true);
		}

		protected override void Initialize(bool setObjects)
		{
			if (setObjects)
			{ }
			else
			{ }
		}

		#endregion

		#region [ Event/Delegates ]

		public delegate void KeyChangedEventHandler(object sender, string newValue, string oldValue);
		private KeyChangedEventHandler KeyChangedEvent;

		public event KeyChangedEventHandler KeyChanged
		{
			add
			{ KeyChangedEvent = (KeyChangedEventHandler)System.Delegate.Combine(KeyChangedEvent, value); }
			remove
			{ KeyChangedEvent = (KeyChangedEventHandler)System.Delegate.Remove(KeyChangedEvent, value); }
		}

		#endregion

	}
}