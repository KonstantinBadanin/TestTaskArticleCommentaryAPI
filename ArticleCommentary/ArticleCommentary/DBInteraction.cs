using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Dapper;
using Microsoft.AspNetCore.Identity;
//Copyright Konstantin Badanin.

namespace DataSinglton
{
    public class Request
        //Class for transporting data by post request.
    {
        public string UserName { get; private set; }
        public string Id { get; private set; }
        public string ComText { get; private set; }
        public string UserId { get; private set; }
        public string Article { get; private set; }
        public string Parent { get; private set; }
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

        public User GetUserById(int id)
        {
            User res;
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                res = db.Query<User>("GetUserById", new { id },
                commandType: CommandType.StoredProcedure).ToList().First();
            }
            return res;
        }

        public void AddNewUserAndHisComment(string username, Comment arg)
        {
            if (arg == null)
                throw new ArgumentNullException(paramName: nameof(arg));
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
                    arg.ParentId
                }, tran, commandType: CommandType.StoredProcedure);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public Tuple<List<Comment>, List<User>, List<Article>> GetAll()
        {
            var res = new List<Comment>();
            var usr = new List<User>();
            var art = new List<Article>();
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = db.QueryMultiple("GetAll", commandType: CommandType.StoredProcedure);
                var com = result.Read<Comment>().ToList();
                usr = result.Read<User>().ToList();
                art = result.Read<Article>().ToList();
                res = BuildTree(com).ToList();
            }
            return new Tuple<List<Comment>, List<User>, List<Article>>(res, usr, art);
        }

        private static IEnumerable<Comment> BuildTree(List<Comment> items)
        {
            items.ForEach(i => i.Comments = items.Where(ch => ch.ParentId == i.Id).ToList());
            return items.Where(i => i.ParentId == null).ToList();
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
