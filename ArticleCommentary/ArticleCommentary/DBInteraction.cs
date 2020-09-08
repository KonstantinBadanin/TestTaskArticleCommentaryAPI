using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
    //Copyright Konstantin Badanin.

namespace ArticleCommentary
{
    public class Request
        //Class for transporting data by post request.
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string ComText { get; set; }
        public string UserId { get; set; }
        public string Article { get; set; }
        public string Parent { get; set; }
        public Request() { }
    }
    public class DBInteraction 
        //Database interaction class.
    {
        private readonly string _connectionString=
            @"Persist Security Info=False;" +
            @" Data Source=(localDB)\mssqllocaldb;" +
            @" AttachDBFilename='C:\Users\kaste\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf';" +
            @" Integrated Security=true";
        public DBInteraction() { }

        public void AddNewUserAndHisComment(string username, Comment arg)
        {
            if (arg == null) throw new ArgumentNullException(paramName: nameof(arg));
            using IDbConnection db = new SqlConnection(_connectionString);
            db.Open();
            using IDbTransaction tran = db.BeginTransaction();
            try
            {
                db.Execute("AddNewUserAndHisComment", new
                {
                    username,
                    arg.Id,
                    arg.ComText,
                    arg.Article,
                    arg.UserId,
                    arg.Parent
                }, tran, commandType: CommandType.StoredProcedure);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public List<Comment> GetAll()
        {
            var result = new List<Comment>();
            using(IDbConnection db=new SqlConnection(_connectionString))
            {
                result = db.Query<Comment,User,Article,Comment>("GetAll", map:(c,u,a)=>
                {
                    c.Article = a.Id;
                    c.UserId = u.Id;
                    return c;
                },
                splitOn:"Id,Id",
                commandType: CommandType.StoredProcedure).Distinct().ToList();
            }
            return result;
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                users = db.Query<User>("GetUsers", commandType: CommandType.StoredProcedure).ToList();
            }
            return users;
        }

        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                articles = db.Query<Article>("GetArticles", commandType: CommandType.StoredProcedure).ToList();
            }
            return articles;
        }

        public List<Comment> FindCommentsByParentId(int parId)
        {
            List<Comment> comments = null;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                comments = db.Query<Comment>("FindCommentsByParentId", new { parId },
                    commandType: CommandType.StoredProcedure).ToList();
            }
            return comments;
        }

        public List<Comment> FindCommentsByArticleIdWNoParent(int artId)
        {
            List<Comment> comment = null;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                comment = db.Query<Comment>("FindCommentsByArticleIdWNoParent", new { artId },
                    commandType: CommandType.StoredProcedure).ToList();
            }
            return comment;
        }

        public List<Comment> FindCommentsByArticleId(int artId)
        {
            List<Comment> comment = null;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                comment = db.Query<Comment>("FindCommentsByArticleId", new { artId },
                    commandType: CommandType.StoredProcedure).ToList();
            }
            return comment;
        }
    }
}
