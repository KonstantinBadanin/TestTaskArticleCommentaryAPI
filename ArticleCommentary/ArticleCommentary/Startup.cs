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

        public Tuple<List<ArticleNode>, List<User>> InitOnStartup()
            //Reads database on startup and creates datamodel.
            //This needs to be reworked.
        {
            List<ArticleNode> ArticleList = new List<ArticleNode>();
            List<CommentNode> CommentList = new List<CommentNode>();
            List<User> UserList = new List<User>();
            DBInteraction Interactor = new DBInteraction();
            foreach (Comment comment in Interactor.GetAll())
            {
                
            }
            foreach (ArticleNode art in ArticleList.Distinct())
            {
                    //Recursively filling comments trees. Max two per level.
                List<CommentNode> lst = CommentList.Distinct().Where(x => (x.Parent == null) && (x.Article == art.Id)).ToList();
                int k = lst.Count;
                if (k == 0)
                { }
                if (k == 1)
                {
                    CommentNode tmp = lst[0];
                    art.SetLeftComment(ref tmp);
                    art.LeftComment.LoadToModel(ref lst);
                }
                if (k == 2)
                {
                    CommentNode tmp1 = lst[0];
                    CommentNode tmp2 = lst[1];
                    art.SetLeftComment(ref tmp1);
                    art.SetRightComment(ref tmp2);
                    art.LeftComment.LoadToModel(ref lst);
                    art.RightComment.LoadToModel(ref lst);
                }
                if (k > 2)
                {
                    throw (new InvalidOperationException());
                }
            }
            return new Tuple<List<ArticleNode>, List<User>>(ArticleList, UserList.Distinct().ToList());
            //List<ArticleNode> ArticleList = new List<ArticleNode>();
            //foreach(ArticleNode item in Interactor.GetArticles())
            //    //Filling list with articles.
            //{
            //    ArticleList.Add(new ArticleNode(item));
            //}
            //if (ArticleList.Count == 0)
            //{
            //    return ArticleList;
            //}
            //foreach (ArticleNode item in ArticleList)
            //    //Recursively filling comments trees. Max two per level.
            //{
            //    List<Comment> commentsWNoParent = Interactor.FindCommentsByArticleIdWNoParent(item.Id);
            //    int k = commentsWNoParent.Count;
            //    if (k == 0)
            //    { }
            //    if (k == 1)
            //    {
            //        var tmp = new CommentNode(commentsWNoParent[0]);
            //        item.SetLeftComment(ref tmp);
            //        item.LeftComment.LoadFromDBToModel(_connectionString);
            //    }
            //    if (k == 2)
            //    {
            //        var tmp1 = new CommentNode(commentsWNoParent[0]);
            //        var tmp2 = new CommentNode(commentsWNoParent[1]);
            //        item.SetLeftComment(ref tmp1);
            //        item.SetRightComment(ref tmp2);
            //        item.LeftComment.LoadFromDBToModel(_connectionString);
            //        item.RightComment.LoadFromDBToModel(_connectionString);
            //    }
            //    if (k > 2)
            //    {
            //        throw (new InvalidOperationException());
            //    }
            //}
            //return ArticleList;
        }

        public Startup(IConfiguration configuration)
            //DataModel initialization and getting singleton instance.
        {
            Configuration = configuration;
            Tuple<List<ArticleNode>,List<User>> initialData = InitOnStartup();
            List<ArticleNode> articles = initialData.Item1;
            List<User> users = initialData.Item2;
            ArticleCommentsTree.GetInstance(ref articles,ref users);
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
