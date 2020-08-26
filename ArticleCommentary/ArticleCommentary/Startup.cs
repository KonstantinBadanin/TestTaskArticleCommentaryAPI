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

namespace ArticleCommentary
{
    public class Startup
    {
        private readonly string connectionString = 
            @"Persist Security Info=False;" + 
            @" Data Source=(localDB)\mssqllocaldb;" + 
            @" AttachDBFilename='C:\Users\kaste\source\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf';" +
            @" Integrated Security=true";

        public List<ArticleNode> InitArticlesOnStartup()
            //Reads database on startup and creates datamodel.
            //This needs to be reworked.
        {
            DBInteraction Interactor = new DBInteraction(connectionString);
            List<ArticleNode> ArticleList = new List<ArticleNode>();
            foreach(ArticleNode item in Interactor.GetArticles())
                //Filling list with articles.
            {
                ArticleList.Add(new ArticleNode(item));
            }
            if (ArticleList.Count == 0)
            {
                return ArticleList;
            }
            foreach (ArticleNode item in ArticleList)
                //Recursively filling comments trees. Max two per level.
            {
                List<Comment> commentsWNoParent = Interactor.FindCommentsByArticleIdWNoParent(item.Id);
                int k = commentsWNoParent.Count;
                if (k == 0)
                { }
                if (k == 1)
                {
                    var tmp = new CommentNode(commentsWNoParent[0]);
                    item.SetLeftComment(ref tmp);
                    item.LeftComment.LoadFromDBToModel(connectionString);
                }
                if (k == 2)
                {
                    var tmp1 = new CommentNode(commentsWNoParent[0]);
                    var tmp2 = new CommentNode(commentsWNoParent[1]);
                    item.SetLeftComment(ref tmp1);
                    item.SetRightComment(ref tmp2);
                    item.LeftComment.LoadFromDBToModel(connectionString);
                    item.RightComment.LoadFromDBToModel(connectionString);
                }
                if (k > 2)
                {
                    throw (new InvalidOperationException());
                }
            }
            return ArticleList;
        }

        public Startup(IConfiguration configuration)
            //DataModel initialization and getting singleton instance.
        {
            Configuration = configuration;
            List<ArticleNode> articles = InitArticlesOnStartup();
            DBInteraction Interactor = new DBInteraction(connectionString);
            List<User> users = Interactor.GetUsers();
            ArticleCommentsTree.GetInstance(ref articles,ref users);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
                //Adding database service.
            services.AddTransient(provider => new DBInteraction(connectionString));
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
