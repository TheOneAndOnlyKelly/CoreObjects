using System;
using System.Collections;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using LongOffset = System.UInt32;
using Offset = System.UInt16;

namespace CoreObjects.Disposable
{
	[Serializable()]
	public abstract class Base : IDisposable, ICloneable
	{
		protected delegate void CloneOccurredHandler(ref object NewObject);
		protected event CloneOccurredHandler CloneOccurred;
		private ArrayList _errors;
		protected const Offset __NullOffset = 0;
		protected const LongOffset __NullLongOffset = 0;

		#region [ Contructors ]

		public Base()
		{
			if (disposed)
			{
				GC.ReRegisterForFinalize(true);
			}
			disposed = false;
			_errors = new ArrayList();
			// ---------------------------------------------------------------
			// Do Not put Initialize(true) here.  That causes major problems!
			// ---------------------------------------------------------------
		}

		#endregion

		#region [ Destructors ]

		protected bool disposed;

		public void Dispose()
		{

			// Execute the code that does the cleanup.
			Dispose(true);

			// Let the common language runtime know that Finalize doesn't have to be called.
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// Exit if we've already cleaned up this object.
			if (disposed)
				return;

			if (disposing)
			{
				//  ny General Cleanup goes here
				Initialize(false);
				_errors?.Clear();
			}
			
			// Remember that we've executed this code
			disposed = true;
		}

		~Base()
		{
			//Execute the code that does the cleanup.
			Dispose(false);
		}

		#endregion

		protected abstract void Initialize(bool setObjects);

		protected void SetErrorInformation(System.Exception ex)
		{
			var ErrorHandler = new ErrorHandler(ex);
			var CallStack = new StackTrace();        //The call stack
			var Frame = CallStack.GetFrame(1);       //The frame that called me
			var Method = Frame.GetMethod();          //The method that called me

			_errors.Clear();
			ErrorHandler.SetErrorInformation(Method.DeclaringType.FullName + "." + Method.Name, ex);
			_errors.Add(ErrorHandler.Description);

			ErrorHandler = null;
			Method = null;
			Frame = null;
			CallStack = null;

			CheckErrors();
		}

		protected void SetErrorInformation(string addlInfo, System.Exception ex)
		{
			var ErrorHandler = new ErrorHandler();
			var CallStack = new StackTrace();           // The call stack
			var Frame = CallStack.GetFrame(1);          // The frame that called me
			var Method = Frame.GetMethod();             // The method that called me

			_errors.Clear();
			ErrorHandler.SetErrorInformation(Method.DeclaringType.FullName + "." + Method.Name, addlInfo, ex);
			_errors.Add(ErrorHandler.Description);

			ErrorHandler = null;
			Method = null;
			Frame = null;
			CallStack = null;

			CheckErrors();
		}

		/// <summary>
		/// If there is an error int he array, then throws the error. Also spits out the information into the Debug pane.
		/// </summary>
		protected void CheckErrors()
		{
			var ErrorMessage = string.Empty;

			if (_errors.Count > 0)
			{
				ErrorMessage = (string)_errors[0];
				_errors.Clear();
				Debug.WriteLine(ErrorMessage);
				throw new Exception(ErrorMessage);
			}
		}

		protected void CheckErrors(ref ErrorHandler ErrorHandler)
		{
			if (ErrorHandler != null)
			{
				if (ErrorHandler.Description.Length > 0)
					throw new System.Exception(ErrorHandler.Description);
				ErrorHandler = null;
			}
		}

		protected void Destroy(ref SqlDataReader objectToSacrifice)
		{
			if (objectToSacrifice != null)
			{
				if (!objectToSacrifice.IsClosed)
				{
					objectToSacrifice.Close();
				}
				objectToSacrifice = null;
			}
		}

		protected void Destroy(ref Object objectToSacrifice)
		{
			if (objectToSacrifice != null)
			{
				((IDisposable)objectToSacrifice).Dispose();
				objectToSacrifice = null;
			}
		}

		protected void Destroy(ref Base objectToSacrifice)
		{
			if (objectToSacrifice != null)
			{
				objectToSacrifice.Dispose();
				objectToSacrifice = null;
			}
		}

		protected bool IsAnonymous()
		{
			return System.Security.Principal.WindowsIdentity.GetCurrent().IsAnonymous;
		}

		protected bool IsWeb()
		{
			return (System.Web.HttpContext.Current != null);
		}

		protected string GetUserID()
		{
			try
			{
				if (System.Web.HttpContext.Current == null)
				{
					return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
				}
				else
				{
					return System.Web.HttpContext.Current.User.Identity.Name;
				}
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
		}

		protected string DbSafeString(string input)
		{
			return "'" + input.Replace("'", "''") + "'";
		}

		protected string DbSafeString(string input, bool wildCardSearch)
		{
			return "'%" + input.Replace("'", "''") + "%'";
		}

		///<summary>
		///Creates a shallow copy, value type objects are copied, reference type objects are not!
		///</summary>
		public virtual object Clone()
		{
			object Obj = this.MemberwiseClone();
			CloneOccurred?.Invoke(ref Obj);
			return Obj;
		}

		// typeName must be a fully formed data type name:
		// ex. CMSData.ECP.V2.Location.Item
		public object ObjectFactory(string typeName)
		{
			// Add the assembly name to the back. This is assuming that the first namespace 
			// entry for this class is, indeed, the assembly name. (Dll name)
			// ex. CMSData.ECP.Location.Item, the assembly name would be CMSData
			typeName += ", " + typeName.Substring(0, typeName.IndexOf("."));

			return Activator.CreateInstance(ResolveType(typeName));
		}

		// the typestring is formatted thusly: ClassName, AssemblyName
		// ex. CMSData.ECP.Location.List, CMSData
		private System.Type ResolveType(string typeString)
		{
			int CommaIndex = 1;
			string ClassName = null;
			string AssemblyName = null;
			Assembly TargetAssembly = null;

			try
			{
				CommaIndex = typeString.LastIndexOf(",");
				ClassName = typeString.Substring(0, CommaIndex).Trim();
				AssemblyName = typeString.Substring(CommaIndex + 1).Trim();
				TargetAssembly = Assembly.Load(AssemblyName);
			}
			catch
			{
				try
				{ TargetAssembly = Assembly.Load(AssemblyName); }
				catch
				{ throw new ArgumentException("Can't load assembly " + AssemblyName); }
			}
			//Get the type
			return TargetAssembly.GetType(ClassName, false, true);
		}

		public static bool IsNumeric(object Object)
		{
			return Microsoft.VisualBasic.Information.IsNumeric(Object);
		}

		public static bool IsDate(object Object)
		{
			return Microsoft.VisualBasic.Information.IsDate(Object);
		}
	}
}
