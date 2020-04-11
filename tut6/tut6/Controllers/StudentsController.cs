using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using tut6.Models;

namespace tut6.Controllers
{
        [ApiController]
        [Route("api/students")]
        public class StudentsController : ControllerBase
        {

            [HttpGet]
            public IActionResult GetStudents()
            {
                var result = new List<Student>();
                using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19312;Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select * from Student";

                    con.Open();
                    var dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        var st = new Student();
                        st.FirstName = dr["FirstName"].ToString();
                        st.LastName = dr["LastName"].ToString();
                        st.IdEnrollment = (int)dr["IdEnrollment"];
                        st.IndexNumber = dr["IndexNumber"].ToString();
                        st.BirthDate = (DateTime)dr["BirthDate"];
                        result.Add(st);
                    }
                }
                return Ok(result);
            }


            [HttpGet("entries/{id}")]
            public IActionResult GetSemesterEntries(int id)
            {
                var result = new List<string>();
                using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19312;Integrated Security=True"))
                using (var com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "SELECT e.semester " +
                                      "FROM enrollment e " +
                                      "WHERE e.idenrollment = (SELECT s.idenrollment FROM student s " +
                                      $"WHERE s.indexnumber=@id)";
                    com.Parameters.AddWithValue("id", id);
                    con.Open();
                    var dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        string semester = dr["Semester"].ToString();
                        result.Add(semester);
                    }
                }
                return Ok(result);
            }

        }
}
