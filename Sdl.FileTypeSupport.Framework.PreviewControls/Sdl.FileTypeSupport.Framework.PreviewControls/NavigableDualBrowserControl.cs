using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// NavigableDualBrowserControl
	/// </summary>
	[ComVisible(true)]
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class NavigableDualBrowserControl : UserControl
	{
		private string _puSegId;

		private string _activeSegId = string.Empty;

		private string _activeJumpId = string.Empty;

		private Color _bestMatchingColor = Color.Aquamarine;

		private bool _segmentSelectedFromBrowser;

		private Timer _timer;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private TableLayoutPanel tableLayoutPanel1;

		private Label messageLabel;

		private Panel panel1;

		private SplitContainer splitContainer;

		private WebBrowser webBrowserSrc;

		private WebBrowser webBrowserTrg;

		private VScrollBar vScrollBarDual;

		/// <summary>
		/// Allows browser scripts to access the desired HighlightColor
		/// </summary>
		public string HighlightColor
		{
			get
			{
				byte r = _bestMatchingColor.R;
				byte g = _bestMatchingColor.G;
				byte b = _bestMatchingColor.B;
				string str = $"{r:X}";
				string str2 = $"{g:X}";
				string str3 = $"{b:X}";
				return "#" + str + str2 + str3;
			}
		}

		/// <summary>
		/// Allows browser scripts to access the current active segment
		/// </summary>
		public string SelectedSegmentId => _activeSegId;

		/// <summary>
		/// The browser control that displays the source language content.
		/// </summary>
		public WebBrowser WebBrowserSrc => webBrowserSrc;

		/// <summary>
		/// The browser control that displays the target language content.
		/// </summary>
		public WebBrowser WebBrowserTrg => webBrowserTrg;

		/// <summary>
		/// Raised when <see cref="M:Sdl.FileTypeSupport.Framework.PreviewControls.NavigableDualBrowserControl.SelectSegment(System.String)" /> is called.
		/// </summary>
		public event PreviewControlHandler WindowSelectionChanged;

		/// <summary>
		/// NavigableDualBrowserControl()
		/// </summary>
		public NavigableDualBrowserControl()
		{
			InitializeComponent();
			splitContainer.SplitterDistance = (splitContainer.Width - splitContainer.SplitterWidth) / 2;
			messageLabel.Visible = false;
			webBrowserSrc.IsWebBrowserContextMenuEnabled = false;
			webBrowserSrc.AllowWebBrowserDrop = false;
			webBrowserSrc.ScriptErrorsSuppressed = true;
			webBrowserSrc.AllowNavigation = false;
			webBrowserSrc.ObjectForScripting = this;
			webBrowserSrc.DocumentCompleted += webBrowserSrc_DocumentCompleted;
			webBrowserTrg.IsWebBrowserContextMenuEnabled = false;
			webBrowserTrg.AllowWebBrowserDrop = false;
			webBrowserTrg.ScriptErrorsSuppressed = true;
			webBrowserTrg.AllowNavigation = false;
			webBrowserTrg.ObjectForScripting = this;
			webBrowserTrg.DocumentCompleted += webBrowserTrg_DocumentCompleted;
			_timer = new Timer();
			_timer.Tick += _timer_Tick;
			_timer.Interval = 500;
		}

		private void webBrowserTrg_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			ScrollToElement(_activeJumpId, webBrowserTrg);
			HtmlDocument document = webBrowserTrg.Document;
			object[] args = new string[1]
			{
				_activeSegId
			};
			document.InvokeScript("sdl_setActiveHighlight", args);
		}

		private void webBrowserSrc_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			ScrollToElement(_activeJumpId, webBrowserSrc);
			HtmlDocument document = webBrowserSrc.Document;
			object[] args = new string[1]
			{
				_activeSegId
			};
			document.InvokeScript("sdl_setActiveHighlight", args);
		}

		/// <summary>
		/// Gets the currently selected segment in the Browser
		/// </summary>
		/// <returns>A Segment Marker indicating which segment is currently selected</returns>
		public SegmentReference GetSelectedSegment()
		{
			if (!string.IsNullOrEmpty(_puSegId))
			{
				string id = _puSegId.Substring(0, _puSegId.IndexOf('_'));
				string id2 = _puSegId.Substring(_puSegId.IndexOf('_') + 1, _puSegId.Length - _puSegId.IndexOf('_') - 1);
				return new SegmentReference(default(FileId), new ParagraphUnitId(id), new SegmentId(id2));
			}
			return null;
		}

		/// <summary>
		/// Scrolls to the segment in the Browser referenced by the SegmentMarker passed in
		/// </summary>
		/// <param name="segment">A reference to the segment to scroll to</param>
		public void ScrollToSegment(SegmentReference segment)
		{
			if (base.InvokeRequired)
			{
				Invoke(new Action<SegmentReference>(ScrollToSegment), segment);
				return;
			}
			string text = "sdl_jump_" + segment.ParagraphUnitId.Id + "_" + segment.SegmentId.Id;
			string text2 = "sdl_span_" + segment.ParagraphUnitId.Id + "_" + segment.SegmentId.Id;
			if (!_segmentSelectedFromBrowser)
			{
				ScrollToElement(text, webBrowserSrc);
				ScrollToElement(text, webBrowserTrg);
			}
			HtmlDocument document = webBrowserSrc.Document;
			object[] args = new string[1]
			{
				text2
			};
			document.InvokeScript("sdl_setActiveHighlight", args);
			HtmlDocument document2 = webBrowserTrg.Document;
			args = new string[1]
			{
				text2
			};
			document2.InvokeScript("sdl_setActiveHighlight", args);
			messageLabel.Visible = (webBrowserSrc.Document.All.GetElementsByName(text).Count == 0);
			messageLabel.Text = string.Format(StringResources.CannotNavigateToSegment, segment.SegmentId.Id);
			_activeSegId = text2;
			_activeJumpId = text;
			if (_segmentSelectedFromBrowser)
			{
				_segmentSelectedFromBrowser = false;
			}
		}

		/// <summary>
		/// Refresh the target browser control and re-highlight the active segment in it.
		/// </summary>
		public void RefreshTargetBrowser()
		{
			if (base.InvokeRequired)
			{
				Invoke(new MethodInvoker(RefreshTargetBrowser));
				return;
			}
			webBrowserTrg.Refresh();
			JumpToActiveElement();
		}

		/// <summary>
		/// Highlight active segment by kicking off a timer.
		/// </summary>
		public void JumpToActiveElement()
		{
			if (base.InvokeRequired)
			{
				Invoke(new MethodInvoker(JumpToActiveElement));
			}
			else
			{
				_timer.Start();
			}
		}

		private void ScrollAndJumpToActiveElement()
		{
			if (base.InvokeRequired)
			{
				Invoke(new MethodInvoker(ScrollAndJumpToActiveElement));
			}
			else if (webBrowserTrg.ReadyState == WebBrowserReadyState.Complete)
			{
				_timer.Stop();
				ScrollToElement(_activeJumpId, webBrowserTrg);
				HtmlDocument document = webBrowserTrg.Document;
				object[] args = new string[1]
				{
					_activeSegId
				};
				document.InvokeScript("sdl_setActiveHighlight", args);
			}
		}

		private void _timer_Tick(object sender, EventArgs e)
		{
			ScrollAndJumpToActiveElement();
		}

		/// <summary>
		/// Set the highlight color to the color that best matches the specified color.
		/// </summary>
		/// <param name="clr">The color of the highlight to use</param>
		public void SetBestMatchingHighlightColor(Color clr)
		{
			_bestMatchingColor = clr;
		}

		/// <summary>
		/// Public method called from HTML Browser control when a segment has been selected
		/// </summary>
		/// <param name="puSegId">A string representing the pu/segment id of the segment which has been selected</param>
		public void SelectSegment(string puSegId)
		{
			_puSegId = puSegId;
			_segmentSelectedFromBrowser = true;
			FireWindowSelectionChanged();
		}

		/// <summary>
		/// Fired when the Window Selection has been changed
		/// </summary>
		protected void FireWindowSelectionChanged()
		{
			if (this.WindowSelectionChanged != null)
			{
				this.WindowSelectionChanged(null);
			}
		}

		private void ScrollToElement(string elemName, WebBrowser browser)
		{
			if (!(browser.Document != null))
			{
				return;
			}
			HtmlDocument document = browser.Document;
			HtmlElementCollection elementsByName = document.All.GetElementsByName(elemName);
			if (elementsByName != null && elementsByName.Count > 0)
			{
				HtmlElement htmlElement = elementsByName[0];
				htmlElement.ScrollIntoView(alignWithTop: false);
				if (browser.Document.Body.ScrollTop >= 5)
				{
					ScrollBy(120, browser);
				}
			}
			UpdateSynchronizedScrollBar();
		}

		private void webBrowserSrc_ClientSizeChanged(object sender, EventArgs e)
		{
			UpdateSynchronizedScrollBar();
		}

		private void webBrowserTrg_ClientSizeChanged(object sender, EventArgs e)
		{
			UpdateSynchronizedScrollBar();
		}

		private void UpdateSynchronizedScrollBar()
		{
			if (!(webBrowserSrc.Document == null) && !(webBrowserTrg.Document == null) && !(webBrowserSrc.Document.Body == null) && !(webBrowserTrg.Document.Body == null))
			{
				int val = Math.Min(webBrowserSrc.Document.Body.ScrollTop, webBrowserTrg.Document.Body.ScrollTop);
				val = Math.Min(val, 0);
				vScrollBarDual.Minimum = val;
				vScrollBarDual.SmallChange = 1;
				vScrollBarDual.LargeChange = webBrowserSrc.Height;
				int maximum = Math.Max(webBrowserSrc.Document.Body.ScrollRectangle.Height, webBrowserTrg.Document.Body.ScrollRectangle.Height) - val;
				vScrollBarDual.Maximum = maximum;
				vScrollBarDual.Value = Math.Min(webBrowserSrc.Document.Body.ScrollTop, webBrowserTrg.Document.Body.ScrollTop);
			}
		}

		private static Point GetCurrentScrollPosition(WebBrowser browser)
		{
			return new Point(browser.Document.Body.ScrollLeft, browser.Document.Body.ScrollTop);
		}

		private static void ScrollBy(int scrollBy, WebBrowser toScroll)
		{
			Point point = new Point(toScroll.Document.Body.ScrollLeft, toScroll.Document.Body.ScrollTop + scrollBy);
			toScroll.Document.Window.ScrollTo(point);
		}

		private static void ScrollTo(int scrollTo, WebBrowser toScroll)
		{
			Point point = new Point(toScroll.Document.Body.ScrollLeft, scrollTo);
			toScroll.Document.Window.ScrollTo(point);
		}

		private void webBrowserSrc_SizeChanged(object sender, EventArgs e)
		{
			UpdateSynchronizedScrollBar();
		}

		private void webBrowserTrg_SizeChanged(object sender, EventArgs e)
		{
			UpdateSynchronizedScrollBar();
		}

		private void vScrollBarDual_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
			{
				int scrollBy = e.NewValue - e.OldValue;
				ScrollBy(scrollBy, webBrowserSrc);
				ScrollBy(scrollBy, webBrowserTrg);
				UpdateSynchronizedScrollBar();
			}
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			messageLabel = new System.Windows.Forms.Label();
			panel1 = new System.Windows.Forms.Panel();
			vScrollBarDual = new System.Windows.Forms.VScrollBar();
			splitContainer = new System.Windows.Forms.SplitContainer();
			webBrowserSrc = new System.Windows.Forms.WebBrowser();
			webBrowserTrg = new System.Windows.Forms.WebBrowser();
			tableLayoutPanel1.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			SuspendLayout();
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
			tableLayoutPanel1.Controls.Add(messageLabel, 0, 0);
			tableLayoutPanel1.Controls.Add(panel1, 0, 1);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20f));
			tableLayoutPanel1.Size = new System.Drawing.Size(276, 213);
			tableLayoutPanel1.TabIndex = 1;
			messageLabel.AutoSize = true;
			messageLabel.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
			messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			messageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
			messageLabel.Location = new System.Drawing.Point(3, 0);
			messageLabel.Name = "messageLabel";
			messageLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			messageLabel.Size = new System.Drawing.Size(270, 27);
			messageLabel.TabIndex = 0;
			panel1.Controls.Add(vScrollBarDual);
			panel1.Controls.Add(splitContainer);
			panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			panel1.Location = new System.Drawing.Point(0, 27);
			panel1.Margin = new System.Windows.Forms.Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(276, 186);
			panel1.TabIndex = 1;
			vScrollBarDual.Dock = System.Windows.Forms.DockStyle.Right;
			vScrollBarDual.Location = new System.Drawing.Point(258, 0);
			vScrollBarDual.Name = "vScrollBarDual";
			vScrollBarDual.Size = new System.Drawing.Size(18, 186);
			vScrollBarDual.TabIndex = 2;
			vScrollBarDual.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBarDual_Scroll);
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.Location = new System.Drawing.Point(0, 0);
			splitContainer.Name = "splitContainer";
			splitContainer.Panel1.Controls.Add(webBrowserSrc);
			splitContainer.Panel2.Controls.Add(webBrowserTrg);
			splitContainer.Size = new System.Drawing.Size(276, 186);
			splitContainer.SplitterDistance = 134;
			splitContainer.TabIndex = 0;
			webBrowserSrc.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowserSrc.Location = new System.Drawing.Point(0, 0);
			webBrowserSrc.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowserSrc.Name = "webBrowserSrc";
			webBrowserSrc.Size = new System.Drawing.Size(134, 186);
			webBrowserSrc.TabIndex = 0;
			webBrowserSrc.ClientSizeChanged += new System.EventHandler(webBrowserSrc_ClientSizeChanged);
			webBrowserSrc.SizeChanged += new System.EventHandler(webBrowserSrc_SizeChanged);
			webBrowserTrg.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowserTrg.Location = new System.Drawing.Point(0, 0);
			webBrowserTrg.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowserTrg.Name = "webBrowserTrg";
			webBrowserTrg.Size = new System.Drawing.Size(138, 186);
			webBrowserTrg.TabIndex = 0;
			webBrowserTrg.ClientSizeChanged += new System.EventHandler(webBrowserTrg_ClientSizeChanged);
			webBrowserTrg.SizeChanged += new System.EventHandler(webBrowserTrg_SizeChanged);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(tableLayoutPanel1);
			base.Name = "NavigableDualBrowserControl";
			base.Size = new System.Drawing.Size(276, 213);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			panel1.ResumeLayout(false);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
			splitContainer.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
