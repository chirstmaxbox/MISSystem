﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExportDomain.Handler
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Sqllhdb")]
	public partial class ExportHandlerDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertExportHandler(ExportHandler instance);
    partial void UpdateExportHandler(ExportHandler instance);
    partial void DeleteExportHandler(ExportHandler instance);
    #endregion
		
		public ExportHandlerDataContext() : 
				base(global::ExportDomain.Properties.Settings.Default.SqllhdbConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public ExportHandlerDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ExportHandlerDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ExportHandlerDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ExportHandlerDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<ExportHandler> ExportHandlers
		{
			get
			{
				return this.GetTable<ExportHandler>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ExportHandler")]
	public partial class ExportHandler : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ExportID;
		
		private string _Text;
		
		private string _UrlA;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnExportIDChanging(int value);
    partial void OnExportIDChanged();
    partial void OnTextChanging(string value);
    partial void OnTextChanged();
    partial void OnUrlAChanging(string value);
    partial void OnUrlAChanged();
    #endregion
		
		public ExportHandler()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ExportID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int ExportID
		{
			get
			{
				return this._ExportID;
			}
			set
			{
				if ((this._ExportID != value))
				{
					this.OnExportIDChanging(value);
					this.SendPropertyChanging();
					this._ExportID = value;
					this.SendPropertyChanged("ExportID");
					this.OnExportIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Text", DbType="NVarChar(150)")]
		public string Text
		{
			get
			{
				return this._Text;
			}
			set
			{
				if ((this._Text != value))
				{
					this.OnTextChanging(value);
					this.SendPropertyChanging();
					this._Text = value;
					this.SendPropertyChanged("Text");
					this.OnTextChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UrlA", DbType="NVarChar(200)")]
		public string UrlA
		{
			get
			{
				return this._UrlA;
			}
			set
			{
				if ((this._UrlA != value))
				{
					this.OnUrlAChanging(value);
					this.SendPropertyChanging();
					this._UrlA = value;
					this.SendPropertyChanged("UrlA");
					this.OnUrlAChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
