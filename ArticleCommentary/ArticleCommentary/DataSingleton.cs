using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//Copyright Konstantin Badanin.

namespace DataSingleton
{
    public class DataSingleton
        //Datamodel object class. Uses singleton.
    {
        private static DataSingleton _instance;

        private DataSingleton(List<Comment> comments, List<User> users, List<Article> articles)
        {
            Users = users;
            Comments = comments;
            Articles = articles;
        }

        public static DataSingleton GetInstance()
        {
            if (_instance == null)
            {
                throw (new Exception());
            }
            else
            {
                return _instance;
            }
        }

        public static DataSingleton GetInstance(List<Comment> comments, List<User> users, List<Article> articles)
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    _instance = new DataSingleton(comments, users, articles);
                }
            }
            return _instance;
        }

        private static readonly object syncRoot = new object();

        public static readonly object Locker = new object();

        public List<User> Users{ get; private set; }

        public List<Comment> Comments { get; private set; }

        public List<Article> Articles { get; private set; }

        public static bool DeleteCommentById(int comId)
        {
            throw new NotImplementedException();
        }
    }
}
