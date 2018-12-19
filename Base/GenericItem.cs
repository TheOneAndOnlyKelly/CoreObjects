using System;
using System.IO;

namespace CoreObjects.Item
{

	[Serializable()]
	public class GenericItem : BaseWithIO
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					base.Dirty = true;
				}
			}
		}

		#region [ Constructors ]

		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericItem()
			: base()
		{ }

		/// <summary>
		/// Constructor that sets the Key value and the Name value
		/// </summary>
		/// <param name="key">string</param>
		/// <param name="name">string</param>
		public GenericItem(string key, string name) : base()
		{
			_key = key;
			_name = name;
		}

		protected override void Initialize(bool setObjects)
		{
			_name = string.Empty;
		}

		#endregion

		#region [ I/O Methods ]

		public override bool Load(BinaryReader reader)
		{
			return true;
		}

		public override bool Save(BinaryWriter writer)
		{
			return true;
		}

		#endregion

		public override string ToString()
		{
			return _name;
		}
	}
}