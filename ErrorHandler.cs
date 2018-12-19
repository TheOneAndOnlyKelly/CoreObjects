using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CoreObjects
{
	public class ErrorHandler
	{
		protected string _desc = string.Empty;

		#region [ Constructors ]

		public ErrorHandler()
		{ }

		public ErrorHandler(Exception ex)
		{
			SetErrorInformation(ex);
		}

		public ErrorHandler(StringBuilder addlInformation, Exception ex)
		{
			SetErrorInformation(addlInformation, ex);
		}

		#endregion

		#region [ Property Assignments ]

		public string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}

		#endregion

		public void SetErrorInformation(Exception ex)
		{
			StackTrace CallStack = new StackTrace();  // The call stack
			StackFrame Frame = CallStack.GetFrame(1); // The frame that called me
			MethodBase Method = Frame.GetMethod();    // The method that called me

			SetErrorInformation(Method.DeclaringType.FullName + "." + Method.Name, "", ex);

			Method = null;
			Frame = null;
			CallStack = null;
		}
		public void SetErrorInformation(StringBuilder addlInformation, Exception ex)
		{
			StackTrace CallStack = new StackTrace();      // The call stack
			StackFrame Frame = CallStack.GetFrame(1); // The frame that called me
			MethodBase Method = Frame.GetMethod();        // The method that called me

			SetErrorInformation(Method.DeclaringType.FullName + "." + Method.Name, addlInformation.ToString(), ex);

			Method = null;
			Frame = null;
			CallStack = null;
		}
		public void SetErrorInformation(string method, Exception ex)
		{
			SetErrorInformation(method, string.Empty, ex);
		}
		public void SetErrorInformation(string method, string addlInformation, Exception ex)
		{
			try
			{
				if (ex == null)
					return;

				if (ex.ToString().IndexOf("+++++++++ Start of Error +++++++++") > 0) // We don't want to keep logging every Catch!
				{
					_desc = ex.ToString().Substring(0, ex.ToString().IndexOf("+++++++++ End of Error +++++++++")) + "+++++++++ End of Error +++++++++" + Environment.NewLine + Environment.NewLine;
					_desc = _desc.Replace("Exception:", string.Empty);
					return;
				}

				if (ex.InnerException != null)
					SetErrorInformation(string.Empty, string.Empty, ex.InnerException);

				if (addlInformation.Length == 0)
					addlInformation = string.Empty;

				if (method.Length == 0)
					method = string.Empty;

				_desc = Environment.NewLine + Environment.NewLine + "+++++++++ Start of Error +++++++++" + Environment.NewLine + Environment.NewLine;

				_desc += "*** Source ***" + Environment.NewLine + ex.Source + Environment.NewLine + Environment.NewLine;

				if (method.Trim().Length > 0)
					_desc += "*** Method ***" + Environment.NewLine + method + Environment.NewLine + Environment.NewLine;

				if (addlInformation.Trim().Length > 0)
					_desc += "*** Additional Information ***" + Environment.NewLine + addlInformation + Environment.NewLine + Environment.NewLine;

				_desc += "*** Exception ***" + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine;
				_desc += "*** Stack Trace ***" + Environment.NewLine + ex.StackTrace + Environment.NewLine + Environment.NewLine;
				_desc += "+++++++++ End of Error +++++++++" + Environment.NewLine + Environment.NewLine;
			}
			catch
			{
				return;
			}
		}

		public void SetErrorInformation(string method, StringBuilder addlInformation, Exception ex)
		{
			try
			{
				string addl = string.Empty;

				if (ex == null)
					return;

				if (ex.InnerException != null)
					SetErrorInformation(string.Empty, string.Empty, ex.InnerException);

				if (addlInformation != null)
					addl = addlInformation.ToString().Trim();

				if (method == null)
					method = string.Empty;

				_desc += " at " + method + Environment.NewLine;

				if (addl.Length > 0)
					_desc += " (" + addl + ") " + Environment.NewLine;

				_desc += ex.ToString() + Environment.NewLine;
				_desc += " Stack Trace: " + ex.StackTrace + Environment.NewLine;

			}
			catch
			{
				return;
			}
		}
	}
}