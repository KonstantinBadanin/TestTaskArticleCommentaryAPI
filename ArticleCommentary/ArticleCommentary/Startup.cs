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
            @"Persist Security Info=False; Data Source=(localDB)\mssqllocaldb;
            AttachDBFilename=
            'C:\Users\kaste\source\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf';
            Integrated Security=true";
        public List<User> InitUsersOnStartup()//создает список юзеров
        {
            DBInteraction Interactor = new DBInteraction(connectionString);
            List<User> UserList = Interactor.GetUsers();
            return UserList;
        }
        public List<ArticleNode> InitOnStartup() //Считывает базу при старте программы и создает модель данных
        {
            DBInteraction Interactor = new DBInteraction(connectionString);
            List<ArticleNode> DataTree = new List<ArticleNode>();
            foreach(var item in Interactor.GetArticles())  //заполнение списка статьями
            {
                DataTree.Add(new ArticleNode(item));
            }
            if (DataTree.Count == 0) return DataTree;
            foreach (var item in DataTree) //рекурсивно заполняем деревья комментов, больше 2 на одном уровне нельзя
            {
                List<Comment> commentsWNoParent = Interactor.FindCommentsByArticleIdWNoParent(item.Id);
                int k = commentsWNoParent.Count;
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
            }
            return DataTree;
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            List<ArticleNode> articles = InitOnStartup();
            List<User> users = InitUsersOnStartup();
            ArticleCommentsTree.GetInstance(ref articles,ref users);
            //Инициализация модели данных
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(provider => new DBInteraction(connectionString));   //внедрение зависимости
            services.AddCors(options =>                 //включение cors
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
