using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//copyright Konstantin Badanin

namespace ArticleCommentary.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentaryController : Controller
    {

        [HttpGet]
        public string[] Get()    //нужен для передачи клиенту начальных данных
        {
            List<string> initialData = new List<string>();
            var data = ArticleCommentsTree.GetInstance();
            lock (data.Locker)
            {
                foreach (ArticleNode article in data.Tree)
                {
                    initialData.Add("Article" + " " + article.Id + " " + article.Title + " " + article.ArticleText + " ");
                    if (article.LeftComment != null)
                    {
                        int IdToFind = article.LeftComment.UserId;
                        initialData.Add("Comment" + " " + article.LeftComment.Id + " " + data.Users.Find(x => x.Id == IdToFind).Name+" " + article.LeftComment.ComText + " ");
                        foreach (var item1 in article.LeftComment.GetAllDerivedComments())
                        {
                            initialData.Add(item1);
                        }
                    }
                    if (article.RightComment != null)
                    {
                        int IdToFind = article.RightComment.UserId;
                        initialData.Add("Comment" + " " + article.RightComment.Id + " " + data.Users.Find(x => x.Id == IdToFind).Name + " " + article.RightComment.ComText + " ");
                        foreach (var item1 in article.RightComment.GetAllDerivedComments())
                        {
                            initialData.Add(item1);
                        }
                    }
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
        public void Post(Request arg)   //принимает формы от клиента
        {
            if (arg == null) throw new ArgumentNullException(paramName: nameof(arg));
            var comment = new Comment(arg);
            var Node = new CommentNode(comment);
            var data = ArticleCommentsTree.GetInstance();
            lock (data.Locker)
            {
                try
                {
                    data.Users.Add(new User(comment.UserId, arg.UserName));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            lock (data.Locker)
            {
                if (ArticleCommentsTree.AddByParentId(ref Node))
                {
                    string connectionString = @"Persist Security Info=False; Data Source=(localDB)\mssqllocaldb; AttachDBFilename='C:\Users\kaste\source\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf';Integrated Security=true";
                    DBInteraction Interactor = new DBInteraction(connectionString);
                    try
                    {
                        Interactor.AddNewUser(comment.UserId, arg.UserName);
                        Interactor.AddNewComment(comment);
                    }
                    catch (Exception)
                    {
                        if (ArticleCommentsTree.DeleteCommentById(comment.Id) == true)
                            return;
                        else
                            throw (new Exception());
                    };
                }
                else
                {
                    throw (new Exception());
                    //обработка при неудаче добавления;
                }
            }
        }
    }
}
