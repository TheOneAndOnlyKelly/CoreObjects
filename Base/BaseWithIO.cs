using System;
using System.IO;

namespace CoreObjects.Item
{
	[Serializable()]
	public abstract class BaseWithIO : Base
	{

		#region [ Constructors ]

		public BaseWithIO() { }

		public BaseWithIO(string tag)
		{
			_tag = tag;
		}

		public BaseWithIO(object tag)
		{
			_tag = tag;
		}

		#endregion

		/// <summary>
		/// Load in the data using a BinaryReader, reading from a datastream.
		/// </summary>
		/// <param name="reader">BinaryReader</param>
		/// <returns>bool</returns>
		public abstract bool Load(BinaryReader reader);

		/// <summary>
		/// Save the data to a datastream with a BinaryWriter.
		/// </summary>
		/// <param name="writer">BinaryWriter</param>
		/// <returns>bool</returns>
		public abstract bool Save(BinaryWriter writer);
	}

}