﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using UICore;

namespace SlimTuneUI.CoreVis
{
	[DisplayName("Query Debugger")]
	public partial class QueryDebugger : UserControl, IVisualizer
	{
		Connection m_connection;

		public QueryDebugger()
		{
			InitializeComponent();
			string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			QueryEditor.ConfigurationManager.Language = "mssql";			
		}

		public string DisplayName
		{
			get { return "Query Debugger"; }
		}

		public bool Initialize(ProfilerWindowBase mainWindow, Connection connection)
		{
			if(connection == null)
				throw new ArgumentNullException("connection");

			m_connection = connection;
			this.Text = Utilities.GetStandardCaption(connection);

			return true;
		}

		public void Show(Control.ControlCollection parent)
		{
			this.Dock = DockStyle.Fill;
			parent.Add(this);
		}

		public void OnClose()
		{
		}

		private void QueryButton_Click(object sender, EventArgs e)
		{
			if(SqlRadioButton.Checked)
				QuerySql();
			else
				QueryHql();
		}

		private void QuerySql()
		{
			try
			{
				DataSet ds = m_connection.DataEngine.RawQuery(QueryEditor.Text);
				if(ds != null)
				{
					DataViewer.DataSource = ds;
					DataViewer.DataMember = "Query";
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void QueryHql()
		{
			try
			{
				using(var session = m_connection.DataEngine.OpenSession())
				{
					var query = session.CreateQuery(QueryEditor.Text);
					var list = query.List();
					DataViewer.DataSource = list;
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}