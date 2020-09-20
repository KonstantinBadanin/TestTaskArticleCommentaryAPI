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
using System.Text;
//Copyright Konstantin Badanin.

namespace ArticleCommentary
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
        public string DatabaseLocation { get; set; } = @"C:\Users\kaste\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf";
        private string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    ["Data Source"] = @"(localDB)\mssqllocaldb",
                    ["Persist Security Info"] = false,
                    ["AttachDBFilename"] = DatabaseLocation,
                    ["Integrated Security"] = true
                };
                return builder.ConnectionString;
            }
        }
        public DBInteraction() { }

        public void AddNewUserAndHisComment(string username, Comment arg)
        {
            if (arg == null)
                throw new ArgumentNullException(paramName: nameof(arg));
            using IDbConnection db = new SqlConnection(ConnectionString);
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
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                var result = db.QueryMultiple("GetAll", commandType: CommandType.StoredProcedure);
                List<Comment> com = result.Read<Comment>().ToList();
                List<User> tmp1 = result.Read<User>().ToList();
                List<Article> tmp2 = result.Read<Article>().ToList();
                res = BuildTree(com).ToList();
                usr = BuildList(tmp1, com).ToList();
                art = BuildList(tmp2, com).ToList();
            }
            return new Tuple<List<Comment>, List<User>, List<Article>>(res, usr, art);
        }

        private static IEnumerable<Comment> BuildTree(List<Comment> items)
        {
            items.ForEach(i => i.Comments = items.Where(ch => ch.ParentId == i.Id).ToList());
            return items.Where(i => i.ParentId == null).ToList();
        }

        private static IEnumerable<User> BuildList(List<User> arg, List<Comment> items)
        {
            arg.ForEach(i => i.Comments = items.Where(x => x.UserId == i.Id).ToList());
            return arg;
        }

        private static IEnumerable<Article> BuildList(List<Article> arg, List<Comment> items)
        {
            arg.ForEach(i => i.Comments = 
            items.Where(x => x.Article == i.Id).ToList());
            return arg;
        }
    }
}
