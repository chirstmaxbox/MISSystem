﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace EmployeeDomain.Models
{
    public partial class EmployeeDbModelEntities : DbContext
    {
        public EmployeeDbModelEntities()
            : base("name=EmployeeDbModelEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<CR_Timecard_Substitude> CR_Timecard_Substitude { get; set; }
        public DbSet<FW_Employee_Shift> FW_Employee_Shift { get; set; }
        public DbSet<FW_Employee_Substitude> FW_Employee_Substitude { get; set; }
        public DbSet<tblTimeCard_Holiday> tblTimeCard_Holiday { get; set; }
        public DbSet<tblTimeCard_Shift> tblTimeCard_Shift { get; set; }
        public DbSet<FW_Employee_WorkingHistory> FW_Employee_WorkingHistory { get; set; }
        public DbSet<FW_Department1> FW_Department1 { get; set; }
        public DbSet<FW_Employee_Overtime> FW_Employee_Overtime { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<FW_EmployeesTeam> FW_EmployeesTeam { get; set; }
        public DbSet<FwLabourTicketStationEmployee> FwLabourTicketStationEmployees { get; set; }
        public DbSet<CR_TimeCard> CR_TimeCard { get; set; }
        public DbSet<CR_Timecard_Summary> CR_Timecard_Summary { get; set; }
        public DbSet<FW_Employees> FW_Employees { get; set; }
    }
}
