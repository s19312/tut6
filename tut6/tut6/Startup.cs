using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tut6.MIddlewares;
using tut6.Models;
using tut6.Services;

namespace tut6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IStudentServiceDb, SqlServerStudentDbService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {

                if (context.Request.Headers.ContainsKey("Index"))
                {
                    string indexNumber = context.Response.Headers["Index"].ToString();
                    using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19312;Integrated Security=True"))
                    using (var com = new SqlCommand())
                    {
                        con.Open();
                        SqlTransaction tran = con.BeginTransaction();
                        com.Transaction = tran;
                        com.Connection = con;
                        Student student = new Student();
                        com.Parameters.AddWithValue("IndexNumber", indexNumber);
                        com.CommandText = "Select count()'count' from Student where IndexNumber = @IndexNumber)";
                        var dr = com.ExecuteReader();
                        dr.Read();
                        if ((int)dr["count"] == 1)
                        {
                            await context.Response.WriteAsync("indexNumber Exist : @indexNumber");
                            return;
                        }
                        else {
                            context.Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("indexNumber does not Exist");
                            return;
                        }
                    }
                }
                
                await next();
            });

            app.UseMiddleware<LoggingMiddleware>();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
