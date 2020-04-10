using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using tut6.Models;

namespace tut6.Services
{
    public class SqlServerStudentDbService : ControllerBase, IStudentServiceDb
    {
        private int idEnrollment = 0;
        private int idStudy = 0;
        private DateTime startDate = DateTime.Now;
        public IActionResult Enrollment(Enrollment enrollment)
        {

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19312;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                com.Transaction = tran;
                com.Connection = con;
                com.Parameters.AddWithValue("Name", enrollment.Studies);
                com.CommandText = $"select idStudy from Studies where Name = @Name";
                tran.Commit();

                var dr = com.ExecuteReader();

                while (dr.Read())
                {
                    idStudy = (int)dr["IdStudy"];
                }
                dr.Close();
                if (idStudy == 0)
                {
                    tran.Rollback();
                    return BadRequest("Studies : " + enrollment.Studies + " does not exist!");
                }
                else
                {

                    com.CommandText = $"select max(startDate) from enrollment where semester = 1 and idStudy = {idStudy} ";
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        com.CommandText = "Select max(idEnrollment) 'idEnrollment' from enrollment";
                        dr = com.ExecuteReader();
                        dr.Read();
                        idEnrollment = (int)dr["IdEnrollment"] + 1;
                        dr.Close();
                        com.Parameters.AddWithValue("idEnrollment", idEnrollment);
                        com.CommandText = "insert into enrollment (idEnrollment,Semester,idStudy,StartDate)" +
                             $"VALUES (@idEnrollment,1,{idStudy} " + ", '" + startDate + "')";
                        com.ExecuteNonQuery();

                    }
                    else
                    {
                        dr.Close();

                    }

                    com.Parameters.AddWithValue("IndexNumber", enrollment.IndexNumber);
                    com.CommandText = "select count(*)'count' from Student Where indexNumber = @indexNumber";
                    dr = com.ExecuteReader();
                    dr.Read();
                    if ((int)dr["count"] > 0)
                    {
                        return BadRequest("IndexNumber : @indexNumber Already exist!");
                    }
                    else
                    {
                        dr.Close();

                        com.Parameters.AddWithValue("FirstName", enrollment.FirstName);
                        com.Parameters.AddWithValue("LastName", enrollment.LastName);
                        com.Parameters.AddWithValue("BirthDate", enrollment.BirthDate);
                        com.CommandText = "insert into Student VALUES (@indexNumber,@firstName,@LastName,@BirthDate,@idEnrollment)";
                        com.ExecuteNonQuery();

                    }
                }
            }
            return Ok("New Student has been enrolled : \n Name = " + enrollment.FirstName + " \n Last Name = " + enrollment.LastName +
                                                            " \n BirthDate = " + enrollment.BirthDate + "\n semester = 1 \n Study = " + enrollment.Studies + " \n StartDate = " + startDate);
        }

        public IActionResult Promote(int semester, string studies)
        {
            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19312;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                con.Open();
                SqlTransaction tran = con.BeginTransaction();
                com.Transaction = tran;
                com.Connection = con;
                com.Parameters.AddWithValue("Semester", semester);
                com.Parameters.AddWithValue("idStudy", studies);
                com.CommandText = "select count() 'count ' from Enrollment where semester = @Semester " +
                                                    "and idStudy = (select idstudy from Studies where idstudy = @idStudy )";
                var dr = com.ExecuteReader();
                dr.Read();
                if ((int)dr["count"] > 0)
                {
                    dr.Close();
                    com.CommandText = "Promote";
                    com.CommandType = System.Data.CommandType.StoredProcedure;
                }
                else
                {
                    return NotFound("Not Found");
                }

            }
            return Ok();
        }
    }
}
