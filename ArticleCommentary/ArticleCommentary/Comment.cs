using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using DataSingleton.Controllers;

namespace DataSingleton
{
    public class Comment
    //Class for comments mapping.
    {
        public int Id { get; set; }
        public string ComText { get; set; }
        public int UserId { get; set; }
        public int Article { get; set; }
        public int? ParentId { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public Comment() { }

        public Comment(int id, string text, int userId, int articleId, int? parId, List<Comment> childs)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Argument can't be null or empty: ", paramName: nameof(text));
            Comments = childs ?? throw new ArgumentException("Argument can't be null: ", paramName: nameof(childs));
            Id = id; ComText = text; UserId = userId; Article = articleId;
            ParentId = parId;
        }

        public Comment(Comment arg)
        {
            if (arg == null)
                throw new ArgumentNullException(paramName: nameof(arg));
            Id = arg.Id; ComText = arg.ComText; UserId = arg.UserId; Article = arg.Article; Comments = arg.Comments;
        }

        public Comment(Request arg)
        {
            if (arg == null)
                throw new ArgumentNullException(paramName: nameof(arg));
            Id = Int32.Parse(arg.Id, CultureInfo.CurrentCulture.NumberFormat);
            ComText = arg.ComText;
            UserId = Int32.Parse(arg.UserId, CultureInfo.CurrentCulture.NumberFormat);
            Article = Int32.Parse(arg.Article, CultureInfo.CurrentCulture.NumberFormat);
            ParentId = Int32.Parse(arg.Parent, CultureInfo.CurrentCulture.NumberFormat);
        }

        public bool AddCommentRecursive(Comment arg, Recursion limit)
        {
            bool result = false;
            if ((arg == null)||(limit==0))
                throw new ArgumentNullException(paramName: nameof(arg));
            if (Id == arg.ParentId)
            {
                Comments.Add(arg);
                result = true;
            }
            else
            {
                foreach (Comment item in Comments)
                {
                    if (item.AddCommentRecursive(arg,limit--))
                        break;
                }
            }
            return result;
        }

        public List<string> ToStringCustom(Recursion limit)
        {
            var data = DataSingleton.GetInstance();
            List<string> tmp = new List<string>();
            tmp.Add("Comment " + data.Users.Find(x => x.Id == UserId).ToString() + 
                " " + Id + " " + ComText);
            if ((Comments.Count == 0) || (limit == 0))
            {
                return tmp;
            }
            else
            {
                foreach (var item in Comments)
                {
                    tmp = tmp.Concat(item.ToStringCustom(limit--)).ToList();
                }
                return tmp;
            }
        }
    }
}
