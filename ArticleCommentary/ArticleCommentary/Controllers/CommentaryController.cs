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
        public string[] Get()
            //Get method. Done for sending initial data to client.
        {
            List<string> initialData = new List<string>();
            var data = ArticleCommentsTree.GetInstance();
            lock (data.Locker)
            {
                foreach (ArticleNode article in data.Tree)
                {
                    initialData.Add(article.ToString());
                    if (article.LeftComment != null)
                    {
                        int IdToFind = article.LeftComment.UserId;
                        initialData.Add(data.Users.Find(x => x.Id == IdToFind).Name + " " + 
                            article.LeftComment.ToString());
                        foreach (CommentNode comment in article.LeftComment.GetAllDerivedComments())
                        {
                            IdToFind = comment.UserId;
                            initialData.Add(data.Users.Find(x => x.Id == IdToFind).Name + " " + 
                                comment.ToString());
                        }
                    }
                    if (article.RightComment != null)
                    {
                        int IdToFind = article.RightComment.UserId;
                        initialData.Add(data.Users.Find(x => x.Id == IdToFind).Name + " " + 
                            article.RightComment.ToString());
                        foreach (CommentNode comment in article.RightComment.GetAllDerivedComments())
                        {
                            IdToFind = comment.UserId;
                            initialData.Add(data.Users.Find(x => x.Id == IdToFind).Name + " " + 
                                comment.ToString());
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
        public void Post(Request arg)
        //Recieves forms from client.
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
                if (ArticleCommentsTree.AddByParentId(ref Node))
                {
                    string connectionString = @"Persist Security Info=False; Data Source=(localDB)\mssqllocaldb; AttachDBFilename='C:\Users\kaste\source\repos\ArticleCommentary\ArticleCommentary\App_Data\CommentaryBase.mdf';Integrated Security=true";
                    DBInteraction Interactor = new DBInteraction(connectionString);
                    try
                    {
                        Interactor.AddNewUserAndHisComment(comment.UserId, arg.UserName, comment);
                    }
                    catch (Exception)
                    {
                        data.Users.RemoveAll(x => x.Id == comment.UserId);
                        if (ArticleCommentsTree.DeleteCommentById(comment.Id) == true)
                        {
                            return;
                        }
                        else
                        {
                            throw (new Exception());
                        }
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
