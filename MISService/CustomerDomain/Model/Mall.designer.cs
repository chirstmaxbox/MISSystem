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

namespace CustomerDomain.Model
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
	public partial class MallDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertVenderMall(VenderMall instance);
    partial void UpdateVenderMall(VenderMall instance);
    partial void DeleteVenderMall(VenderMall instance);
    #endregion
		
		public MallDataContext() : 
				base("Data Source=F01\\SqlExpress;Initial Catalog=Sqllhdb;Persist Security Info=True;Use" +
						"r ID=sa;Password=sa123456", mappingSource)
		{
			OnCreated();
		}
		
		public MallDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MallDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MallDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public MallDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<VenderMall> VenderMalls
		{
			get
			{
				return this.GetTable<VenderMall>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.VenderMall")]
	public partial class VenderMall : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _MallID;
		
		private string _MallName;
		
		private string _Address;
		
		private string _City;
		
		private string _State;
		
		private string _Zipcode;
		
		private string _Country;
		
		private string _Intersection;
		
		private bool _Active;
		
		private string _Remark;
		
		private string _tempPhone;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnMallIDChanging(int value);
    partial void OnMallIDChanged();
    partial void OnMallNameChanging(string value);
    partial void OnMallNameChanged();
    partial void OnAddressChanging(string value);
    partial void OnAddressChanged();
    partial void OnCityChanging(string value);
    partial void OnCityChanged();
    partial void OnStateChanging(string value);
    partial void OnStateChanged();
    partial void OnZipcodeChanging(string value);
    partial void OnZipcodeChanged();
    partial void OnCountryChanging(string value);
    partial void OnCountryChanged();
    partial void OnIntersectionChanging(string value);
    partial void OnIntersectionChanged();
    partial void OnActiveChanging(bool value);
    partial void OnActiveChanged();
    partial void OnRemarkChanging(string value);
    partial void OnRemarkChanged();
    partial void OntempPhoneChanging(string value);
    partial void OntempPhoneChanged();
    #endregion
		
		public VenderMall()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MallID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int MallID
		{
			get
			{
				return this._MallID;
			}
			set
			{
				if ((this._MallID != value))
				{
					this.OnMallIDChanging(value);
					this.SendPropertyChanging();
					this._MallID = value;
					this.SendPropertyChanged("MallID");
					this.OnMallIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MallName", DbType="NVarChar(500)")]
		public string MallName
		{
			get
			{
				return this._MallName;
			}
			set
			{
				if ((this._MallName != value))
				{
					this.OnMallNameChanging(value);
					this.SendPropertyChanging();
					this._MallName = value;
					this.SendPropertyChanged("MallName");
					this.OnMallNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Address", DbType="NVarChar(500)")]
		public string Address
		{
			get
			{
				return this._Address;
			}
			set
			{
				if ((this._Address != value))
				{
					this.OnAddressChanging(value);
					this.SendPropertyChanging();
					this._Address = value;
					this.SendPropertyChanged("Address");
					this.OnAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_City", DbType="NVarChar(60)")]
		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				if ((this._City != value))
				{
					this.OnCityChanging(value);
					this.SendPropertyChanging();
					this._City = value;
					this.SendPropertyChanged("City");
					this.OnCityChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_State", DbType="NVarChar(50)")]
		public string State
		{
			get
			{
				return this._State;
			}
			set
			{
				if ((this._State != value))
				{
					this.OnStateChanging(value);
					this.SendPropertyChanging();
					this._State = value;
					this.SendPropertyChanged("State");
					this.OnStateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Zipcode", DbType="VarChar(12)")]
		public string Zipcode
		{
			get
			{
				return this._Zipcode;
			}
			set
			{
				if ((this._Zipcode != value))
				{
					this.OnZipcodeChanging(value);
					this.SendPropertyChanging();
					this._Zipcode = value;
					this.SendPropertyChanged("Zipcode");
					this.OnZipcodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Country", DbType="NVarChar(50)")]
		public string Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				if ((this._Country != value))
				{
					this.OnCountryChanging(value);
					this.SendPropertyChanging();
					this._Country = value;
					this.SendPropertyChanged("Country");
					this.OnCountryChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Intersection", DbType="NVarChar(300)")]
		public string Intersection
		{
			get
			{
				return this._Intersection;
			}
			set
			{
				if ((this._Intersection != value))
				{
					this.OnIntersectionChanging(value);
					this.SendPropertyChanging();
					this._Intersection = value;
					this.SendPropertyChanged("Intersection");
					this.OnIntersectionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Active", DbType="Bit NOT NULL")]
		public bool Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				if ((this._Active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Remark", DbType="NVarChar(300)")]
		public string Remark
		{
			get
			{
				return this._Remark;
			}
			set
			{
				if ((this._Remark != value))
				{
					this.OnRemarkChanging(value);
					this.SendPropertyChanging();
					this._Remark = value;
					this.SendPropertyChanged("Remark");
					this.OnRemarkChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_tempPhone", DbType="NVarChar(50)")]
		public string tempPhone
		{
			get
			{
				return this._tempPhone;
			}
			set
			{
				if ((this._tempPhone != value))
				{
					this.OntempPhoneChanging(value);
					this.SendPropertyChanging();
					this._tempPhone = value;
					this.SendPropertyChanged("tempPhone");
					this.OntempPhoneChanged();
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
