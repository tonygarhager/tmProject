using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.FileTypeSupport.Framework.PreviewControls
{
	/// <summary>
	/// Helper control that hosts two web browser controls
	/// and implements basic synchronized scrolling for them.
	/// </summary>
	public class DualBrowserControl : UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components;

		private Panel panel1;

		private VScrollBar vScrollBarDual;

		private WebBrowser webBrowserSrc;

		private WebBrowser webBrowserTrg;

		private SplitContainer splitContainer;

		/// <summary>
		/// The browser control that displays the source language content.
		/// </summary>
		public WebBrowser WebBrowserSrc => webBrowserSrc;

		/// <summary>
		/// The browser control that displays the target language content.
		/// </summary>
		public WebBrowser WebBrowserTrg => webBrowserTrg;

		/// <summary>
		/// Create the child controls etc.
		/// </summary>
		public DualBrowserControl()
		{
			InitializeComponent();
			splitContainer.SplitterDistance = (splitContainer.Width - splitContainer.SplitterWidth) / 2;
			webBrowserSrc.IsWebBrowserContextMenuEnabled = false;
			webBrowserSrc.AllowWebBrowserDrop = false;
			webBrowserTrg.IsWebBrowserContextMenuEnabled = false;
			webBrowserTrg.AllowWebBrowserDrop = false;
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
			panel1 = new System.Windows.Forms.Panel();
			splitContainer = new System.Windows.Forms.SplitContainer();
			webBrowserSrc = new System.Windows.Forms.WebBrowser();
			webBrowserTrg = new System.Windows.Forms.WebBrowser();
			vScrollBarDual = new System.Windows.Forms.VScrollBar();
			panel1.SuspendLayout();
			splitContainer.Panel1.SuspendLayout();
			splitContainer.Panel2.SuspendLayout();
			splitContainer.SuspendLayout();
			SuspendLayout();
			panel1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
			panel1.Controls.Add(splitContainer);
			panel1.Location = new System.Drawing.Point(0, 0);
			panel1.Margin = new System.Windows.Forms.Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(260, 211);
			panel1.TabIndex = 0;
			splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer.Location = new System.Drawing.Point(0, 0);
			splitContainer.Name = "splitContainer";
			splitContainer.Panel1.Controls.Add(webBrowserSrc);
			splitContainer.Panel2.Controls.Add(webBrowserTrg);
			splitContainer.Size = new System.Drawing.Size(260, 211);
			splitContainer.SplitterDistance = 127;
			splitContainer.TabIndex = 0;
			webBrowserSrc.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowserSrc.Location = new System.Drawing.Point(0, 0);
			webBrowserSrc.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowserSrc.Name = "webBrowserSrc";
			webBrowserSrc.Size = new System.Drawing.Size(127, 211);
			webBrowserSrc.TabIndex = 0;
			webBrowserSrc.ClientSizeChanged += new System.EventHandler(webBrowserSrc_ClientSizeChanged);
			webBrowserSrc.SizeChanged += new System.EventHandler(webBrowserSrc_SizeChanged);
			webBrowserTrg.Dock = System.Windows.Forms.DockStyle.Fill;
			webBrowserTrg.Location = new System.Drawing.Point(0, 0);
			webBrowserTrg.MinimumSize = new System.Drawing.Size(20, 20);
			webBrowserTrg.Name = "webBrowserTrg";
			webBrowserTrg.Size = new System.Drawing.Size(129, 211);
			webBrowserTrg.TabIndex = 0;
			webBrowserTrg.ClientSizeChanged += new System.EventHandler(webBrowserTrg_ClientSizeChanged);
			webBrowserTrg.SizeChanged += new System.EventHandler(webBrowserTrg_SizeChanged);
			vScrollBarDual.Dock = System.Windows.Forms.DockStyle.Right;
			vScrollBarDual.Location = new System.Drawing.Point(260, 0);
			vScrollBarDual.Name = "vScrollBarDual";
			vScrollBarDual.Size = new System.Drawing.Size(18, 211);
			vScrollBarDual.TabIndex = 1;
			vScrollBarDual.Scroll += new System.Windows.Forms.ScrollEventHandler(vScrollBarDual_Scroll);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(vScrollBarDual);
			base.Controls.Add(panel1);
			base.Name = "DualBrowserControl";
			base.Size = new System.Drawing.Size(278, 211);
			panel1.ResumeLayout(false);
			splitContainer.Panel1.ResumeLayout(false);
			splitContainer.Panel2.ResumeLayout(false);
			splitContainer.ResumeLayout(false);
			ResumeLayout(false);
		}
	}
}
