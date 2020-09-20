using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Dynamic;

namespace ArticleCommentary
{
    public class Startup
    {

        public Tuple<List<Comment>, List<User>, List<Article>> InitOnStartup()
        //Reads database on startup and creates datamodel.
        {
            DBInteraction Interactor = new DBInteraction();
            return Interactor.GetAll();
        }

        public Startup(IConfiguration configuration)
        //Constructing singleton instance.
        {
            Configuration = configuration;
            Tuple<List<Comment>, List<User>, List<Article>> tmp = InitOnStartup();
            DataSingleton.GetInstance(tmp.Item1, tmp.Item2, tmp.Item3);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Adding database service.
            services.AddTransient(provider => new DBInteraction());
            //Enabling CORS.
            services.AddCors(options =>
            {
                options.AddPolicy("NoRestrictions",
                    builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            }); ;
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("NoRestrictions");

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
