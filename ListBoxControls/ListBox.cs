using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace CoreObjects.ListBoxControls
{
	public class ListBoxUtil : Disposable.Base
    {
		#region [ Constants ]

		public const string NOT_SELECTED_TEXT = "<Not Selected>";
		public const int NOT_SELECTED_KEY = -1;

		#endregion [ Constants ]

		#region [ Private Variables ]

		private bool _controlKey = false;
		private bool _resetComboBox;

		#endregion [ Private Variables ]

		#region [ Constructors ]

		public ListBoxUtil()
        {
            Initialize(true);
        }

		protected override void Initialize(bool setObjects)
        { }

		public void Initialize(ref ComboBox control, bool clear, bool loadNotSelectedEntry)
		{
			if (loadNotSelectedEntry)
				LoadNotSelected(ref control, clear, "<Not Selected>", -1);
			else
				ClearList(ref control, clear);
		}

		#endregion [ Constructors ]

		#region [ Methods ]

		/// <summary>
		/// Found snippet on http://weblogs.asp.net/eporter/archive/2004/09/27/234773.aspx
		/// </summary>
		/// <param name="control">ComboBox control to set the width</param>
		public void SetDropDownWidth(ref ComboBox control)
		{
			System.Drawing.Graphics g = control.CreateGraphics();
			int WidestWidth = control.DropDownWidth;
			string ValueToMeasure;
			int CurrentWidth;

			try
			{
				for (int i = 0; i <= control.Items.Count - 1; i++)
				{
					ValueToMeasure = ((Item)control.Items[i]).ToString();
					CurrentWidth = System.Convert.ToInt32(g.MeasureString(ValueToMeasure, control.Font).Width);
					if (CurrentWidth > WidestWidth)
					{
						WidestWidth = CurrentWidth;
					}
				}

				if (WidestWidth > control.DropDownWidth)
				{
					WidestWidth += 20; // Add a little for the scroll bar
				}

				// Make sure we are inbounds of the screen
				int left = control.PointToScreen(new System.Drawing.Point(0, control.Left)).X;
				if (WidestWidth > Screen.PrimaryScreen.WorkingArea.Width - left)
				{
					WidestWidth = Screen.PrimaryScreen.WorkingArea.Width - left;
				}

				control.DropDownWidth = WidestWidth;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (g != null)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		public static void SetDropDownWidth(ToolStripComboBox comboBox)
		{
			//Found snippet on http://weblogs.asp.net/eporter/archive/2004/09/27/234773.aspx

			Graphics g = comboBox.GetCurrentParent().CreateGraphics();
			int WidestWidth = comboBox.DropDownWidth;
			string ValueToMeasure;
			int CurrentWidth;

			try
			{

				for (int i = 0; i <= comboBox.Items.Count - 1; i++)
				{
					ValueToMeasure = comboBox.Items[i].ToString();
					CurrentWidth = System.Convert.ToInt32(g.MeasureString(ValueToMeasure, comboBox.Font).Width);
					if (CurrentWidth > WidestWidth)
					{
						WidestWidth = CurrentWidth;
					}
				}

				if (WidestWidth > comboBox.DropDownWidth)
				{
					WidestWidth += 20; //Add a little for the scroll bar
				}

				comboBox.DropDownWidth = WidestWidth;

			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
				throw;
			}
			finally
			{
				if (g != null)
				{
					g.Dispose();
					g = null;
				}
			}
		}

		/// <summary>
		/// Found at http://rajeshkm.blogspot.com/2006/11/adjust-combobox-drop-down-list-width-c.html
		/// </summary>
		/// <param name="sender"></param>		
		[DebuggerHidden()]
		public void SetComboScrollWidth(ComboBox comboBox)
		{
			int newWidth;
			string s;

			try
			{
				int width = comboBox.Width;
				Graphics g = comboBox.CreateGraphics();
				Font font = comboBox.Font;

				// checks if a scrollbar will be displayed.
				// If yes, then get its width to adjust the size of the drop down list.
				int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;

				foreach (ListBoxControls.Item Item in comboBox.Items)
				{
					if (Item != null)
					{
						s = Item.Text;
						newWidth = (int)g.MeasureString(s.Trim(), font).Width + vertScrollBarWidth;
						if (width < newWidth)
						{
							width = newWidth;
						}
					}
				}

				comboBox.DropDownWidth = width;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}
		}

		#region [ Add ]

		#region [ Windows.Forms.ComboBox ]

		public void AddDeleted(ref ComboBox control, CoreObjects.Item.Base item)
		{
			Add(ref control, item, string.Empty, true);
			SetDropDownWidth(ref control);
		}

		public void AddDeleted(ref ComboBox control, string text, int key)
		{
			Add(ref control, text, key, string.Empty, true);
			SetDropDownWidth(ref control);
		}

		public void AddDeleted(ref ComboBox control, CoreObjects.Item.Base item, string formatString)
		{
			Add(ref control, item.ToString(), item.Key, formatString, true);
			SetDropDownWidth(ref control);
		}

		public void AddDeleted(ref ComboBox control, Item item, string formatString)
		{
			Add(ref control, item.ToString(), item.Key, formatString, true);
			SetDropDownWidth(ref control);
		}

		public void Add(ref ComboBox control, CoreObjects.Item.Base item, bool isDeleted)
		{
			Add(ref control, item, string.Empty, isDeleted);
		}

		public void Add(ref ComboBox control, CoreObjects.Item.Base item)
		{
			Add(ref control, item.ToString(), item.Key, string.Empty, false);
		}

		public void Add(ref ComboBox control, CoreObjects.Item.Base item, string formatString, bool isDeleted)
		{
			Add(ref control, item.ToString(), item.Key, formatString, isDeleted);
		}

		public void Add(ref ComboBox control, string text, string key, bool isDeleted)
		{
			Add(ref control, text, key, string.Empty, isDeleted);
		}

		public void Add(ref ComboBox control, string text, int key, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), string.Empty, isDeleted);
		}

		public void Add(ref ComboBox control, string text, string key)
		{
			Add(ref control, text, key, string.Empty, false);
		}

		public void Add(ref ComboBox control, string text, int key)
		{
			Add(ref control, text, key.ToString(), string.Empty, false);
		}

		public void Add(ref ComboBox control, int numberEntry)
		{
			Add(ref control, numberEntry.ToString(), numberEntry.ToString(), string.Empty, false);
		}

		public void Add(ref ComboBox control, Item item)
		{
			control.BeginUpdate();
			control.Items.Add(item);
			control.EndUpdate();
		}

		public void Add(ref ComboBox control, string text, int key, string formatString, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), formatString, isDeleted);
		}

		public void Add(ref ComboBox control, string text, string key, string formatString, bool isDeleted)
		{
			control.BeginUpdate();
			if (formatString.Trim().Length > 0)
				text = string.Format(text, formatString);
			control.Items.Add(new Item(text, key.ToString(), isDeleted));
			control.EndUpdate();
		}

		#endregion

		#region [ Windows.Forms.ListBox ]

		public void Add(ref ListBox control, ref CoreObjects.Item.Base item, bool isDeleted)
		{
			Add(ref control, ref item, isDeleted);
		}

		public void Add(ref ListBox control, ref CoreObjects.Item.Base item, string formatString, bool isDeleted)
		{
			Add(ref control, item.ToString(), item.Key, formatString, isDeleted);
		}

		public void Add(ref ListBox control, string text, string key, bool isDeleted)
		{
			Add(ref control, text, key, string.Empty, isDeleted);
		}

		public void Add(ref ListBox control, string text, int key, bool isDeleted)
		{
			Add(ref control, text, key, string.Empty, isDeleted);
		}

		public void Add(ref ListBox control, string text, int key, string formatString, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), formatString, isDeleted);
		}

		public void Add(ref ListBox control, string text, string key, string formatString, bool isDeleted)
		{
			control.BeginUpdate();

			if (formatString.Trim().Length > 0)
				text = string.Format(text, formatString);

			control.Items.Add(new Item(text, key, isDeleted));
			control.EndUpdate();
		}

		#endregion

		#region [ Windows.Forms.CheckedListBox ]

		public void AddDeleted(ref CheckedListBox control, CoreObjects.Item.Base item)
		{
			Add(ref control, ref item, string.Empty, true);
		}

		public void AddDeleted(ref CheckedListBox control, string text, int key)
		{
			Add(ref control, text, key, string.Empty, true);
		}

		public void AddDeleted(ref CheckedListBox control, CoreObjects.Item.Base item, string formatString)
		{
			Add(ref control, item.ToString(), item.Key, formatString, true);
		}

		public void Add(ref CheckedListBox control, ref CoreObjects.Item.Base item, bool isDeleted)
		{
			Add(ref control, ref item, isDeleted);
		}

		public void Add(ref CheckedListBox control, ref CoreObjects.Item.Base item, string formatString, bool isDeleted)
		{
			Add(ref control, item.ToString(), item.Key, formatString, isDeleted);
		}

		public void Add(ref CheckedListBox control, string text, int key, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), string.Empty, isDeleted);
		}

		public void Add(ref CheckedListBox control, string text, string key, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), string.Empty, isDeleted);
		}

		public void Add(ref CheckedListBox control, string text, int key, string formatString, bool isDeleted)
		{
			Add(ref control, text, key.ToString(), formatString, isDeleted);
		}

		public void Add(ref CheckedListBox control, string text, string key, string formatString, bool isDeleted)
		{
			control.BeginUpdate();
			control.Items.Add(new Item(text, key, isDeleted));
			control.EndUpdate();
		}

		#endregion

		#region [ Windows.Forms.ToolStripComboBox ]

		public void Add(ToolStripComboBox comboBox, string text, string key)
		{
			Add(comboBox, text, key);
		}

		public void Add(ToolStripComboBox comboBox, Item listItem)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(listItem);
			comboBox.EndUpdate();
		}

		public void Add(ToolStripComboBox comboBox, string text, string key, string formatString)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key));
			comboBox.EndUpdate();
		}

		public void Add(ToolStripComboBox comboBox, string text, string key, object storage)
		{
			comboBox.BeginUpdate();
			comboBox.Items.Add(new Item(text, key, storage));
			comboBox.EndUpdate();
		}

		public void Add(ToolStripComboBox control, string text)
		{
			control.BeginUpdate();
			control.Items.Add(new Item(text));
			control.EndUpdate();
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#endregion [ Add ]

		#region [ Clear ]

		#region [ Windows.Forms.ComboBox ]

		public void ClearList(ref ComboBox control)
		{
			control.Items.Clear();
		}

		private void ClearList(ref ComboBox control, bool clear)
		{
			if (clear)
				ClearList(ref control);
		}

		public bool ClearDeleted(ref ComboBox control)
		{
			return ClearDeleted(ref control, null);
		}

		public bool ClearDeleted(ref ComboBox control, CoreObjects.List.Base list)
		{
			ArrayList ClearItems = new ArrayList();

			try
			{
				if (control == null)
					return false;

				foreach (Item Item in control.Items)
				{
					if (Item.IsDeleted)
					{
						ClearItems.Add(Item);

						if (list != null)
						{
							try
							{
								list.Remove(Item.Key);
							}
							catch { }
						}
					}
				}

				foreach (Item item in ClearItems)
				{
					control.Items.Remove(item);
				}

				control.SelectedIndex = -1;

				if (ClearItems.Count > 0)
					return true;

				return false;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (ClearItems == null)
				{
					ClearItems.Clear();
					ClearItems = null;
				}
			}
		}

		#endregion [ Windows.Forms.ComboBox ]

		#region [ Windows.Forms.ListBox ]

		public void ClearList(ListBox control)
		{
			control.Items.Clear();
		}

		private void ClearList(ref ListBox control, bool clear)
		{
			if (clear)
				ClearList(control);
		}

		public bool ClearDeleted(ref ListBox control)
		{
			ArrayList ClearItems = new ArrayList();

			try
			{
				if (control == null)
					return false;

				foreach (Item Item in control.Items)
				{
					if (Item.IsDeleted)
						ClearItems.Add(Item);
				}

				foreach (Item item in ClearItems)
				{
					control.Items.Remove(item);
				}

				if (ClearItems.Count > 0)
					return true;

				return false;

			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (ClearItems == null)
				{
					ClearItems.Clear();
					ClearItems = null;
				}
			}
		}

		#endregion [ Windows.Forms.ListBox ]

		#region [ Windows.Forms.CheckedListBox ]

		public void ClearChecked(ref CheckedListBox control)
		{
			for (int Idx = 0; Idx <= control.Items.Count - 1; Idx++)
			{
				control.SetItemCheckState(Idx, CheckState.Unchecked);
			}
		}

		public void ClearList(ref CheckedListBox control)
		{
			control.Items.Clear();
		}

		private void ClearList(ref CheckedListBox control, bool clear)
		{
			if (clear)
				ClearList(ref control);
		}

		public bool ClearDeleted(ref CheckedListBox control, CoreObjects.List.Base List, bool destroy)
		{
			ArrayList ClearItems = new ArrayList();

			try
			{
				if (control == null)
					return false;

				foreach (Item Item in control.Items)
				{
					if (Item.IsDeleted)
					{
						ClearItems.Add(Item);
						if (List != null)
						{
							try
							{
								List.Remove(Item.Key);
							}
							catch { }
						}
					}
				}

				foreach (Item item in ClearItems)
				{
					control.Items.Remove(item);
				}

				if (ClearItems.Count > 0)
					return true;

				return false;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
			finally
			{
				if (ClearItems == null)
				{
					ClearItems.Clear();
					ClearItems = null;
				}
				if (destroy && List != null)
				{
					List.Dispose();
					List = null;
				}
			}
		}

		public bool ClearDeleted(ref CheckedListBox control)
		{
			return ClearDeleted(ref control, null, false);
		}

		#endregion [ Windows.Forms.CheckedListBox ]

		#endregion [ Clear ]

		#region [ Find ]

		#region [ Windows.Forms.ComboBox ]

		public int Find(ref ComboBox control, string value)
		{
			return control.FindString(value);
		}

		public int Find(ref ComboBox control, int value)
		{
			try
			{
				for (int Counter = 0; Counter <= control.Items.Count - 1; Counter++)
				{
					if (value.ToString() == ((Item)control.Items[Counter]).Key)
						return Counter;
				}
				return -1;

			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		public int Find(ref ComboBox control, bool value)
		{
			return Find(ref control, (value ? 1 : 0));
		}

		#endregion

		#region [ Windows.Forms.ListBox ]

		public int Find(ref ListBox control, string value)
		{
			return control.FindStringExact(value.Trim());
		}

		public int Find(ref ListBox control, int value)
		{
			try
			{
				for (int Counter = 0; Counter <= control.Items.Count - 1; Counter++)
				{
					if (value.ToString() == ((Item)control.Items[Counter]).Key)
						return Counter;
				}
				return -1;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		#endregion

		#region [ Windows.Forms.CheckedListBox ]

		public int Find(ref CheckedListBox control, string value)
		{
			return control.FindStringExact(value.Trim());
		}

		public int Find(ref CheckedListBox control, int value)
		{
			try
			{
				for (int Counter = 0; Counter <= control.Items.Count - 1; Counter++)
				{
					if (value.ToString() == ((Item)control.Items[Counter]).Key)
						return Counter;
				}
				return -1;
			}
			catch (Exception ex)
			{
				throw (ex);
			}
		}

		#endregion

		#region [ Windows.Forms.ToolStripComboBox ]

		public int Find(ToolStripComboBox comboBox, int value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ToolStripComboBox comboBox, long value)
		{
			return Find(comboBox, value.ToString(), false, false);
		}

		public int Find(ToolStripComboBox comboBox, string value)
		{
			return Find(comboBox, value, false, false);
		}

		public int Find(ToolStripComboBox comboBox, string value, bool findFromName)
		{
			return Find(comboBox, value, findFromName, false);
		}

		public int Find(ToolStripComboBox comboBox, string value, bool findFromName, bool stringFragment)
		{
			if ((value == null) || (comboBox.Items.Count == 0))
				return NOT_SELECTED_KEY;

			if (findFromName || !(comboBox.Items[0] is Item))
			{
				if (!stringFragment)
					return comboBox.FindStringExact(value.Trim());
				else
					return comboBox.FindString(value.Trim());
			}

			for (int i = 0; i <= comboBox.Items.Count - 1; i++)
			{
				if (string.Compare(value.ToString(), ((Item)comboBox.Items[i]).Key, true) == 0)
					return i;
			}
			return NOT_SELECTED_KEY;
		}

		public int Find(ToolStripComboBox comboBox, bool value)
		{
			return Find(comboBox, (value ? "1" : "0"), false, false);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#endregion [ Find ]

		#region [ Gets ]

		#region [ Windows.Forms.ComboBox ]

		public Item GetItem(ref ComboBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		public Item GetItem(ref ComboBox control)
		{
			if (control.SelectedIndex > -1)
			{
				return GetItem(ref control, control.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		public string GetKey(ref ComboBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(ref ComboBox control)
		{
			if (control.SelectedIndex > -1)
			{
				return GetKey(ref control, control.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		public string GetText(ref ComboBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(ref ComboBox control)
		{
			if (control.SelectedIndex > -1)
			{
				return GetText(ref control, control.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		public bool GetBoolean(ref ComboBox comboBox)
		{
			return GetBoolean(ref comboBox, comboBox.SelectedIndex);
		}

		public bool GetBoolean(ref ComboBox control, int index)
		{
			return (GetKey(ref control, index) == "1");
		}

		#endregion

		#region [ Windows.Forms.ListBox ]

		public Item GetItem(ref ListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		public Item GetItem(ref ListBox control)
		{
			if (control.SelectedIndex > -1)
			{
				return GetItem(ref control, control.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		public string GetKey(ref ListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(ref ListBox control)
		{
			return GetKey(ref control, control.SelectedIndex);
		}

		public string GetText(ref ListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(ComboBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(object control, int index)
		{
			try
			{
				return GetText(((ListBox)control), index);
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(object control, int index)
		{
			return GetKey(((ListBox)control), index);
		}

		public string GetText(ref ListBox control)
		{
			return GetText(ref control, control.SelectedIndex);
		}

		#endregion

		#region [ Windows.Forms.CheckedListBox ]

		public Item GetItem(ref CheckedListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		public Item GetItem(ref CheckedListBox control)
		{
			if (control.SelectedIndex > -1)
			{
				return GetItem(ref control, control.SelectedIndex);
			}
			else
			{
				return null;
			}
		}

		public string GetKey(ref CheckedListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(ref CheckedListBox control)
		{
			return GetKey(ref control, control.SelectedIndex);
		}

		public string GetText(ref CheckedListBox control, int index)
		{
			try
			{
				return ((Item)control.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(ref CheckedListBox control)
		{
			return GetText(ref control, control.SelectedIndex);
		}

		#endregion

		#region [ Windows.Forms.ToolStripComboBox ]

		public Item GetItem(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Retrieves the currently selected item from a ToolStripComboBox
		/// </summary>
		/// <param name="comboBox"></param>
		/// <returns></returns>
		public Item GetItem(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
				return GetItem(comboBox, comboBox.SelectedIndex);
			else
				return null;
		}

		public string GetKey(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).Key;
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetKey(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
				return GetKey(comboBox, comboBox.SelectedIndex);
			else
				return string.Empty;
		}

		public string GetText(ToolStripComboBox comboBox, int index)
		{
			try
			{
				return ((Item)comboBox.Items[index]).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}

		public string GetText(ToolStripComboBox comboBox)
		{
			if (comboBox.SelectedIndex > -1)
			{
				return GetText(comboBox, comboBox.SelectedIndex);
			}
			else
			{
				return string.Empty;
			}
		}

		public bool GetBoolean(ToolStripComboBox comboBox)
		{
			return GetBoolean(comboBox, comboBox.SelectedIndex);
		}

		public bool GetBoolean(ToolStripComboBox comboBox, int index)
		{
			return (GetKey(comboBox, index) == "1");
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]

		#endregion [ Gets ]

		#region [ Load ]

		#region [ Windows.Forms.ComboBox ]

		public void LoadNotSelected(ref ComboBox control, bool clear, string text, int index)
		{
			ClearList(ref control, clear);
			if (Find(ref control, index) == -1)
			{
				Add(ref control, text, index, false);
				SetDropDownWidth(ref control);
			}
		}

		public void Load(ref ComboBox control, string[] list)
		{
			Load(ref control, list, true, false, string.Empty);
		}

		public void Load(ref ComboBox control, string[] list, bool clear,
						 bool loadNotSelectedEntry, string formatString)
		{
			Initialize(ref control, clear, loadNotSelectedEntry);

			for (int Counter = 0; Counter <= list.Length - 1; Counter++)
			{
				Add(ref control, list[Counter], Counter, formatString, false);
			}

			SetDropDownWidth(ref control);
		}

		public void Load(ref ComboBox toControl, ComboBox fromControl)
		{
			foreach (Item ListItem in fromControl.Items)
			{
				Add(ref toControl, (Item)ListItem.Clone());
			}
		}

		public void Load(ref ComboBox control, CoreObjects.List.Base list)
		{
			Load(ref control, list, true, false, string.Empty, false);
		}

		public void Load(ref ComboBox control, CoreObjects.List.Base list, bool clear)
		{
			Load(ref control, list, clear, false, string.Empty, false);
		}

		public void Load(ref ComboBox control, CoreObjects.List.Base list, bool clear, bool loadNotSelectedEntry)
		{
			Load(ref control, list, clear, loadNotSelectedEntry, string.Empty, false);
		}

		public void Load(ref ComboBox control, CoreObjects.List.Base list, bool clear, bool loadNotSelectedEntry, string formatString, bool destroy)
		{
			int Width = control.Width;

			Initialize(ref control, clear, loadNotSelectedEntry);

			foreach (CoreObjects.Item.Base objItem in list)
			{
				Add(ref control, objItem, formatString, false);
			}

			SetDropDownWidth(ref control);

			if (destroy && list != null)
			{
				list.Dispose();
				list = null;
			}
		}

		public void Load(ref ComboBox control, string text, int key, bool clear, string formatString, bool isDeleted)
		{
			Load(ref control, text, key.ToString(), clear, formatString, isDeleted);
		}

		public void Load(ref ComboBox control, string text, string key, bool clear, string formatString, bool isDeleted)
		{
			Initialize(ref control, clear, false);
			Add(ref control, text, key, formatString, isDeleted);
		}

		public void LoadYesNo(ref ComboBox control)
		{
			LoadYesNo(ref control, true, false);
		}

		public void LoadYesNo(ref ComboBox control, bool clear, bool loadNotSelectedEntry)
		{
			Initialize(ref control, clear, loadNotSelectedEntry);
			Add(ref control, "Yes", 1, false);
			Add(ref control, "No", 0, false);
			SetDropDownWidth(ref control);
		}

		public void Load(ref ComboBox control, string text, int key, bool clear, bool loadNotSelectedEntry, string formatString, bool isDeleted)
		{
			Initialize(ref control, clear, loadNotSelectedEntry);
			Add(ref control, text, key, formatString, isDeleted);
			SetDropDownWidth(ref control);
		}

		public void LoadYears(ref ComboBox control, int startYear, int endYear, bool clear, bool loadNotSelectedEntry)
		{
			int YearCnt;
			Initialize(ref control, clear, loadNotSelectedEntry);
			for (YearCnt = startYear; YearCnt <= endYear; YearCnt++)
			{
				Add(ref control, YearCnt.ToString(), YearCnt, "0000", false);
			}
			SetDropDownWidth(ref control);
		}

		public void LoadYears(ref ComboBox control, short numYears, bool clear, bool loadNotSelectedEntry)
		{
			LoadYears(ref control, System.Convert.ToInt32(DateTime.Today.AddYears(numYears * -1).ToString("yyyy")),
						System.Convert.ToInt32(DateTime.Today.AddYears(numYears).ToString("yyyy")),
						clear, loadNotSelectedEntry);
		}

		public void LoadMonths(ref ComboBox control, bool clear, bool loadNotSelectedEntry)
		{
			Initialize(ref control, clear, loadNotSelectedEntry);
			for (int MonthNum = 1; MonthNum <= 12; MonthNum++)
			{
				Add(ref control, DateAndTime.MonthName(MonthNum, false), MonthNum, false);
			}
			SetDropDownWidth(ref control);
		}

		public void LoadAndSet(ref ComboBox control, CoreObjects.List.Base list, CoreObjects.Item.Base selectedItem)
		{
			LoadAndSet(ref control, list, selectedItem, false, false, false);
		}

		public void LoadAndSet(ref ComboBox control, CoreObjects.List.Base list, CoreObjects.Item.Base selectedItem, bool refreshList, bool addDeletedToList)
		{
			LoadAndSet(ref control, list, selectedItem, false, false);
		}

		public void LoadAndSet(ref ComboBox control, CoreObjects.List.Base list, CoreObjects.Item.Base selectedItem,
							   bool refreshList, bool addDeletedToList, bool destroy)
		{
			try
			{
				control.BeginUpdate();
				ClearDeleted(ref control, list);

				if (control.Items.Count == 0)
				{
					//if (refreshList)
					//list.Load();
					Load(ref control, list, true, false, string.Empty, false);
				}

				if (selectedItem != null && selectedItem.Key.Length > 0)
				{
					if (Set(ref control, selectedItem) == false)
					{
						AddDeleted(ref control, selectedItem);
						Set(ref control, selectedItem);
						if (addDeletedToList)
							list.Add(selectedItem);
					}
					return;
				}
			}
			//catch (Exception ex)
			//{
			//}
			finally
			{
				control.EndUpdate();
				if (destroy && (list != null))
				{
					list.Dispose();
					list = null;
				}
			}
		}

		public void LoadAndSet(ref ComboBox control, CoreObjects.List.Base list, string selectedKey)
		{
			LoadAndSet(ref control, list, selectedKey, false);
		}

		public void LoadAndSet(ref ComboBox control, CoreObjects.List.Base list, string selectedKey, bool destroy)
		{
			try
			{
				control.BeginUpdate();
				Load(ref control, list, true, false, string.Empty, false);
				Set(ref control, selectedKey);
			}
			//catch (Exception ex)
			//{
			//((CMSSharedWindows.BaseForms.BaseForm)comboBox.FindForm()).DisplayError(ex);
			//}
			finally
			{
				control.EndUpdate();
				if (destroy && list != null)
				{
					list.Dispose();
					list = null;
				}
			}
		}

		#endregion

		#region [ Windows.Forms.ListBox ]

		public void Load(ref ListBox control, string text, int key, bool clear, string formatString, bool isDeleted)
		{
			ClearList(ref control, clear);
			Add(ref control, text, key, formatString, isDeleted);
		}

		public void Load(ListBox control, string[] List, bool clear, string formatString)
		{
			ClearList(ref control, clear);
			for (int Counter = 0; Counter <= List.Length - 1; Counter++)
			{
				Add(ref control, List[Counter], Counter, formatString, false);
			}
		}

		public void Load(ref ListBox control, CoreObjects.List.Base List, bool clear, string formatString, bool destroy)
		{
			CoreObjects.Item.Base TempItem = null;

			ClearList(ref control, clear);

			foreach (CoreObjects.Item.Base objItem in List)
			{
				TempItem = objItem;
				Add(ref control, ref TempItem, formatString, false);
			}
			if (destroy && List != null)
			{
				List.Dispose();
				List = null;
			}
		}

		#endregion

		#region [ Windows.Forms.CheckedListBox ]

		public void LoadAndSet(ref CheckedListBox control, CoreObjects.List.Base List, CoreObjects.Item.Base selectedItem,
							   bool RefreshList, bool AddDeletedToList, bool ClearChecked, bool destroy)
		{
			try
			{
				control.BeginUpdate();
				ClearDeleted(ref control, List, false);

				if (ClearChecked)
				{
					this.ClearChecked(ref control);
				}

				if (control.Items.Count == 0)
				{
					//if (RefreshList)
					//List.Load();
					Load(ref control, List, true, string.Empty, false);
				}

				if (selectedItem != null && selectedItem.Key.Length > 0)
				{
					if (Set(ref control, selectedItem) == false)
					{
						AddDeleted(ref control, selectedItem);
						Set(ref control, selectedItem);
						if (AddDeletedToList)
							List.Add(selectedItem);
					}
					return;
				}

			}
			//catch (Exception ex)
			//{
			//    //((CMSSharedWindows.BaseForms.BaseForm)ListBox.FindForm()).DisplayError(ex);

			//}
			finally
			{
				control.EndUpdate();
				if (destroy && List != null)
				{
					List.Dispose();
					List = null;
				}
			}
		}

		public void Load(ref CheckedListBox control, string text, int key, bool clear, string formatString, bool isDeleted)
		{
			ClearList(ref control, clear);
			Add(ref control, text, key.ToString(), formatString, isDeleted);
		}

		public void Load(ref CheckedListBox control, string[] List, bool clear, string formatString)
		{
			ClearList(ref control, clear);
			for (int Counter = 0; Counter <= List.Length - 1; Counter++)
			{
				Add(ref control, List[Counter], Counter.ToString(), formatString, false);
			}
		}

		public void Load(ref CheckedListBox control, CoreObjects.List.Base list, bool clear, string formatString, bool destroy)
		{
			CoreObjects.Item.Base TempItem = null;
			ClearList(ref control, clear);
			foreach (CoreObjects.Item.Base objItem in list)
			{
				TempItem = objItem;
				Add(ref control, ref TempItem, formatString, false);
			}
			if (destroy && list != null)
			{
				list.Dispose();
				list = null;
			}
		}

		#endregion

		#region [ Windows.Forms.ToolStripComboBox ]

		public void Initialize(ToolStripComboBox ComboBox, bool clear, bool loadNotSelectedEntry)
		{
			if (loadNotSelectedEntry)
				LoadNotSelected(ComboBox, clear, NOT_SELECTED_TEXT, NOT_SELECTED_KEY);
			else
				ClearList(ComboBox, clear);
		}

		public void ClearList(ToolStripComboBox ComboBox)
		{
			ComboBox.Items.Clear();
		}

		private void ClearList(ToolStripComboBox ComboBox, bool clear)
		{
			if (clear)
				ClearList(ComboBox);
		}

		public void LoadNotSelected(ToolStripComboBox ComboBox, bool clear, string desc, long index)
		{
			ClearList(ComboBox, clear);
			if (Find(ComboBox, index) == NOT_SELECTED_KEY)
			{
				Add(ComboBox, desc, index.ToString(), false);
				SetDropDownWidth(ComboBox);
			}
		}

		public void Load(ToolStripComboBox ComboBox, List<Item> list)
		{
			Initialize(ComboBox, true, false);
			foreach (Item Item in list)
			{
				Add(ComboBox, Item);
			}
			SetDropDownWidth(ComboBox);
		}

		public void Load(ToolStripComboBox ComboBox, string[] list)
		{
			Load(ComboBox, list, true, false, string.Empty);
		}

		public void Load(ToolStripComboBox ComboBox, string[] list, bool clear,
						 bool loadNotSelectedEntry, string formatString)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);

			for (int i = 0; i <= list.Length - 1; i++)
			{
				Add(ComboBox, list[i], i.ToString(), formatString);
			}
			SetDropDownWidth(ComboBox);
		}

		/// <summary>
		///  Populates a ToolStripComboBox with the contects of a list of strings
		/// </summary>
		/// <param name="ComboBox">Control to populate</param>
		/// <param name="list">Generic list of entries to populate the control</param>
		/// <param name="loadNotSelectedEntry">Load an entry for not selected?</param>
		public void Load(ToolStripComboBox ComboBox, List<string> list, bool loadNotSelectedEntry)
		{
			int Width = ComboBox.Width;
			Initialize(ComboBox, true, loadNotSelectedEntry);

			foreach (string entry in list)
			{
				Add(ComboBox, entry, entry, false);
			}
			SetDropDownWidth(ComboBox);
		}

			public void Load(ToolStripComboBox ComboBox, string text, string key, bool clear, string formatString)
		{
			Initialize(ComboBox, clear, false);
			Add(ComboBox, text, key, formatString);
		}

		public void LoadYesNo(ToolStripComboBox ToolStripComboBox)
		{
			LoadYesNo(ToolStripComboBox, true, false);
		}

		public void LoadYesNo(ToolStripComboBox ComboBox, bool clear, bool loadNotSelectedEntry)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);
			Add(ComboBox, "Yes", "1", false);
			Add(ComboBox, "No", "0", false);
			//SetDropDownWidth(ComboBox);
		}

		public void Load(ToolStripComboBox ComboBox, string text, string key, bool clear, bool loadNotSelectedEntry, string formatString)
		{
			Initialize(ComboBox, clear, loadNotSelectedEntry);
			Add(ComboBox, text, key, formatString);
			//SetDropDownWidth(ComboBox);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]
		
		#endregion [ Load ]

		#region [ Sets ]

		#region [ Windows.Forms.ComboBox ]

		public bool Set(ref ComboBox control, CoreObjects.Item.Base itemBase)
		{
			control.SelectedIndex = Find(ref control, itemBase.Key);
			return (control.SelectedIndex > -1);
		}

		public bool Set(ref ComboBox control, bool value)
		{
			control.SelectedIndex = Find(ref control, value);
			return (control.SelectedIndex > -1);
		}

		public bool Set(ref ComboBox control, string value)
		{
			control.SelectedIndex = Find(ref control, value);
			return (control.SelectedIndex > -1);
		}

		public bool Set(ref ComboBox control, int value)
		{
			control.SelectedIndex = Find(ref control, value);
			return (control.SelectedIndex > -1);
		}

        #endregion

        #region [ Windows.Forms.ListBox ]

		public bool Set(ref ListBox control, int value)
		{
			control.SelectedIndex = Find(ref control, value);
			return (control.SelectedIndex > -1);
		}

		public bool Set(ref ListBox control, string value)
		{
			control.SelectedIndex = Find(ref control, value);
			return (control.SelectedIndex > -1);
		}

        #endregion

        #region [ Windows.Forms.CheckedListBox ]

		public bool Set(ref CheckedListBox control, CoreObjects.Item.Base itemBase)
		{
			return Set(ref control, itemBase);
		}

		public bool Set(ref CheckedListBox control, int value)
		{
			return Set(ref control, value, true);
		}

		public bool Set(ref CheckedListBox control, int value, bool checkedValue)
		{
			if (Find(ref control, value) > -1)
			{
				control.SetItemChecked(Find(ref control, value), checkedValue);
				return true;
			}
			return false;
		}

		public bool Set(ref CheckedListBox control, string value, bool checkedValue)
		{
			if (Find(ref control, value) > -1)
			{
				control.SetItemChecked(Find(ref control, value), checkedValue);
				return true;
			}
			return false;
		}

		#endregion

		#region [ Windows.Forms.ToolStripComboBox ]

		/// <summary>
		/// Finds the item in the ComboBox whose key corresponds to the value passed in and sets that entry as the Selected.
		/// </summary>
		/// <param name="comboBox">Control to search within.</param>
		/// <param name="value">Key to the item to be selected</param>
		/// <returns>Returns true if the item was found and set to be Selected, otherwise falee.</returns>
		public bool Set(ToolStripComboBox comboBox, bool value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		/// <summary>
		/// Finds the item in the ComboBox whose key corresponds to the value passed in and sets that entry as the Selected.
		/// </summary>
		/// <param name="comboBox">Control to search within.</param>
		/// <param name="value">Key to the item to be selected</param>
		/// <returns>Returns true if the item was found and set to be Selected, otherwise falee.</returns>
		public bool Set(ToolStripComboBox comboBox, string value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		/// <summary>
		/// Finds the item in the ComboBox whose key corresponds to the value passed in and sets that entry as the Selected.
		/// If the useText flag is set, then it will, instead, use the Text property. If the useStringFragment flag is set, it will do a fuzzy
		/// search of the text.
		/// </summary>
		/// <param name="comboBox">Control to search within.</param>
		/// <param name="value">Key to or Text of the item to be selected</param>
		/// <param name="useText">Indicates that the Text of the list item should be searched for, not the key.</param>
		/// <param name="useStringFragment">Indicates that a fuzzy search method should be use, instead of an exact text match.</param>
		/// <returns>Returns true if the item was found and set to be Selected, otherwise falee.</returns>
		public bool Set(ToolStripComboBox comboBox, string value, bool useText, bool useStringFragment)
		{
			comboBox.SelectedIndex = Find(comboBox, value, useText, useStringFragment);
			return (comboBox.SelectedIndex > -1);
		}

		/// <summary>
		/// Finds the item in the ComboBox whose key corresponds to the value passed in and sets that entry as the Selected.
		/// </summary>
		/// <param name="comboBox">Control to search within.</param>
		/// <param name="value">Key to the item to be selected</param>
		/// <returns>Returns true if the item was found and set to be Selected, otherwise falee.</returns>
		public bool Set(ToolStripComboBox comboBox, int value)
		{
			return Set(comboBox, ((long)value));
		}

		/// <summary>
		/// Finds the item in the ComboBox whose key corresponds to the value passed in and sets that entry as the Selected.
		/// </summary>
		/// <param name="comboBox">Control to search within.</param>
		/// <param name="value">Key to the item to be selected</param>
		/// <returns>Returns true if the item was found and set to be Selected, otherwise falee.</returns>
		public bool Set(ToolStripComboBox comboBox, long value)
		{
			comboBox.SelectedIndex = Find(comboBox, value);
			return (comboBox.SelectedIndex > -1);
		}

		#endregion [ Windows.Forms.ToolStripComboBox ]
		
		#endregion [ Sets ]		

		#endregion [ Methods ]

		#region [ Event Overrides ]

		public void ComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			// Determine whether the keystroke is a backspace.
			_resetComboBox = (e.KeyCode == Keys.Back);
		}

		public void ComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (_resetComboBox)
			{
				((ComboBox)sender).SelectedIndex = -1;
				e.Handled = true;
			}
		}

		// perform the text substitution
		public void AutoComplete_KeyDown(object sender, KeyEventArgs e)
		{
			ComboBox control = (ComboBox)sender;

			if (control.Text != string.Empty && !_controlKey)
			{
				// search for matching entry
				string MatchText = control.Text;
				int Match = control.FindString(MatchText);

				// If a matching entry is found, insert it now
				if (Match != -1)
				{
					control.SelectedIndex = Match;

					// select the added text so it can be replaced
					// if the user keeps typing.
					control.SelectionStart = MatchText.Length;
					control.SelectionLength = control.Text.Length - control.SelectionStart;
				}
			}
			control = null;
		}

		public void AutoComplete_KeyPress(object sender, KeyPressEventArgs e)
		{
			ComboBox Combo = (ComboBox)sender;
			_controlKey =(char.IsControl(e.KeyChar));
		}

		/// <summary>
		/// Adjusts the width of the drop down section of ToolStripComboBoxes to show the widest member
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void AdjustedWidthToolStripComboBox_DropDown(object sender, System.EventArgs e)
		{
			ToolStripComboBox ComboBox = (ToolStripComboBox)sender;
			int width = ComboBox.DropDownWidth;
			Graphics g = ComboBox.GetCurrentParent().CreateGraphics();
			Font font = ComboBox.Font;
			int vertScrollBarWidth = (ComboBox.Items.Count > ComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0;
			string s;

			int newWidth;
			foreach (object o in ((ToolStripComboBox)sender).Items)
			{
				s = o.ToString();
				newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;
				if (width < newWidth)
					width = newWidth;
			}
			ComboBox.DropDownWidth = width;
		}

		#endregion [ Event Overrides ]

	}
}
