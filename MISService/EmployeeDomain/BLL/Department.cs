using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EmployeeDomain.Models;
using MyCommon;

namespace EmployeeDomain.BLL
{
    public class Department
    {
        //private readonly int _dID;
        public int DepartmentID { get; private set; }
        public string DepartmentName { get; private set; }
        private readonly DataRow _row;



        public string InforDepartmentID
        {
            get
            {
                string department = "DoesNotExist";
                if (_row != null)
                {
                    department = Convert.ToString(_row["NameInInfor"]);
                }
                return department;
            }
        }


        public Department(int departmentID)
        {
            DepartmentID  = departmentID;
            _row = GetDataRow(departmentID);
            DepartmentName  = "DoesNotExist";
            if (_row != null)
            {
                DepartmentName= Convert.ToString(_row["DEPARTMENT_NAME"]);
            }
        }

        public Department(string departmentName)
        {
            DepartmentName = departmentName;
            _row = GetDataRow(departmentName);
            DepartmentID = 0;
            if (_row != null)
            {
                DepartmentID  = Convert.ToInt32(_row["DEPARTMENTID"]);
            }

        }

        
        private DataRow GetDataRow(string departmentName)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_Department1 WHERE DEPARTMENT_NAME=@dName";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@dName", SqlDbType.NVarChar, 20).Value = departmentName;
            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;
        }


        private DataRow GetDataRow(int departmentID)
        {
            DataRow row = null;
            var ConnectionSQL = new SqlConnection(EmployeeConfiguration.ConnectionString);
            string SqlSelectString = "SELECT * FROM FW_Department1 WHERE DEPARTMENTID=@ID";
            var SelectCommand = new SqlCommand(SqlSelectString, ConnectionSQL);
            var adapter = new SqlDataAdapter(SelectCommand);
            var ds2 = new DataSet();
            ds2.Tables.Clear();
            adapter.SelectCommand.Parameters.Add("@ID", SqlDbType.Int).Value = departmentID;
            try
            {
                ConnectionSQL.Open();
                int affectedRows = adapter.Fill(ds2, "t2");
                if (affectedRows != 0)
                {
                    row = ds2.Tables["t2"].Rows[0];
                }
            }
            catch (SqlException ex)
            {
            }
            finally
            {
                ConnectionSQL.Close();
            }
            return row;
        }
    }

    public class MyDepartment
    {
       
                public List<FW_Department1> GetProductionDepartments()
        {
            var dc = new EmployeeDbModelEntities();
                    var depts = dc.FW_Department1.Where(x => x.pSelect == true || x.DEPARTMENTID ==0).ToList();
                    return depts;

        }

                public List<FW_Department1> GetJobCostingDepartments()
                {
                    var dc = new EmployeeDbModelEntities();
                    var depts = dc.FW_Department1.Where(x => x.pSelect == true || x.DEPARTMENTID <= 1).ToList();
                    return depts;

                }
    }

    public static class FsDepartment
    {
        public static readonly int[] DepartmentID = { 2, 3, 4, 6, 7 }; //, 21

        public static readonly string[] DepartmentName = { "Metal", "Painting", "Plastic A", "RA3", "Installation" }; //, "Production"
        public static readonly string[] DepartmentNameInfor = { "W-Metal", "W-Painting", "W-Plastic A", "W-RA3", "Installation", };  // "W-Plastic B", "Workshop"}; 

        public static List<MyKeyValueTriple> MyDepartments()
        {
            var kv2 = new MyKeyValueTriple()
                          {
                              Key = 2,
                              Value1 = "Metal",
                              Value2 = "W-Metal"
                          };

            var kv3 = new MyKeyValueTriple()
                          {
                              Key = 3,
                              Value1 = "Painting",
                              Value2 = "W-Painting"
                          };

            var kv4 = new MyKeyValueTriple()
                          {
                              Key = 4,
                              Value1 = "Plastic A",
                              Value2 = "W-Plastic A"
                          };


            var kv5 = new MyKeyValueTriple()
                          {
                              Key = 5,
                              Value1 = "Plastic B",
                              Value2 = "W-Plastic B"
                          };


            var kv6 = new MyKeyValueTriple()
                          {
                              Key = 6,
                              Value1 = "RA3",
                              Value2 = "W-RA3"
                          };

            var kv7 = new MyKeyValueTriple()
                          {
                              Key = 7,
                              Value1 = "Installation",
                              Value2 = "Installation"
                          };

            var kv21 = new MyKeyValueTriple()
                           {
                               Key = 21,
                               Value1 = "Workshop",
                               Value2 = "Workshop"
                           };

            return new List<MyKeyValueTriple> { kv2, kv3, kv4, kv6, kv7 };
        }
    }

    public static class FsProductionDepartment
    {
        public static readonly int[] DepartmentID = { 2, 3, 4, 6, 7,21 };

        public static readonly string[] DepartmentName = { "Metal", "Painting", "Plastic A", "RA3", "Installation", "Production" }; 
        public static readonly string[] DepartmentNameInfor = { "W-Metal", "W-Painting", "W-Plastic A", "W-RA3", "Installation", "Workshop" };  // "W-Plastic B"}; 

        public static List<MyKeyValueTriple> MyDepartments()
        {
            var kv2 = new MyKeyValueTriple()
            {
                Key = 2,
                Value1 = "Metal",
                Value2 = "W-Metal"
            };

            var kv3 = new MyKeyValueTriple()
            {
                Key = 3,
                Value1 = "Painting",
                Value2 = "W-Painting"
            };

            var kv4 = new MyKeyValueTriple()
            {
                Key = 4,
                Value1 = "Plastic A",
                Value2 = "W-Plastic A"
            };


            var kv5 = new MyKeyValueTriple()
            {
                Key = 5,
                Value1 = "Plastic B",
                Value2 = "W-Plastic B"
            };


            var kv6 = new MyKeyValueTriple()
            {
                Key = 6,
                Value1 = "RA3",
                Value2 = "W-RA3"
            };

            var kv7 = new MyKeyValueTriple()
            {
                Key = 7,
                Value1 = "Installation",
                Value2 = "Installation"
            };

            var kv21 = new MyKeyValueTriple()
            {
                Key = 21,
                Value1 = "Workshop",
                Value2 = "Workshop"
            };

            return new List<MyKeyValueTriple> { kv2, kv3, kv4, kv6, kv7, kv21 };
        }
    }

}