using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
//copyright Konstantin Badanin

namespace DataSingleton.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentaryController : Controller
    {

        [HttpGet]
        public string[] Get()
        //Get method. Done for sending initial data to client.
        {
            List<string> initialData = new List<string>();
            var data = DataSingleton.GetInstance();
            lock (DataSingleton.Locker)
            {
                foreach (Article article in data.Articles)
                {
                    initialData = 
                        initialData.Concat(article.ToStringCustom(Recursion.Limit)).ToList();
                }
                return initialData.ToArray();
            }
        }

        [HttpOptions]
        public string Options()
        {
            return "Allow: POST, GET";
        }

        [HttpPost]
        public void Post(Request arg)
        //Recieves forms from client, writes data to datamodel and database.
        {
            if (arg == null)
                throw new ArgumentNullException(paramName: nameof(arg));
            var comment = new Comment(arg);
            var user = new User(arg);
            var data = DataSingleton.GetInstance();
            lock (DataSingleton.Locker)
            {
                try
                {
                    data.Users.Add(user);
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    if (comment.ParentId == null)
                    {
                        data.Comments.Add(comment);
                    }
                    else
                    {
                        data.Comments.Find(x => x.Article == comment.Article)
                            .AddCommentRecursive(comment,Recursion.Limit);
                    }
                }
                catch (Exception)
                {
                    data.Users.RemoveAll(x => x.Id == user.Id);
                }
                DBInteraction Interactor = new DBInteraction();
                try
                {
                    Interactor.AddNewUserAndHisComment(user.Name, comment);
                }
                catch
                {

                }
            }
        }
    }
}
