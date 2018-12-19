namespace CoreObjects.ListBoxControls
{
	public class Item : CoreObjects.Item.Base
	{
		#region [ Properties ]

		public bool IsDeleted { get; set; }

		public string Text { get; set; }

		public object StoredObject { get; set; }

		#endregion [ Properties ]

		#region [ Constructors ]

		public Item()
			: base()
		{
			this.IsDeleted = false;
			this.Text = string.Empty;
		}

		public Item(string text)
				: this()
		{
			this.Key = text;
			this.Text = text;
		}

		public Item(string key, string text, object storedObject = null)
			: this()
		{
			this.Key = key;
			this.Text = text;
			this.StoredObject = storedObject;
		}

		public Item(string text, int key, object storedObject = null)
				: this(text, key.ToString(), storedObject)
		{ }

		public Item(string key, string text, bool isDeleted) 
			: this(key, text, null)
		{
			this.IsDeleted = isDeleted;
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		public override string ToString()
		{
			return Text;
		}

		#endregion [ Methods ]
	}

}
