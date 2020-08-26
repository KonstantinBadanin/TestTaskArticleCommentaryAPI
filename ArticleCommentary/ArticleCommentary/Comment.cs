using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace ArticleCommentary
{
    public class Comment
        //Class for comments mapping.
    {
        public int Id { get; set; }
        public string ComText { get; set; }
        public int UserId { get; set; }
        public int Article { get; set; }
        public int? Parent { get; set; }

        public Comment(Request arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName: nameof(arg));
            }
            else if ((arg.Id == null) || (arg.ComText == null) || (arg.UserId == null) || (arg.Article == null))
            {
                throw new NullReferenceException();
            }
            else
            {
                Id = Int32.Parse(arg.Id, CultureInfo.CurrentCulture.NumberFormat);
                ComText = arg.ComText; UserId = Int32.Parse(arg.UserId,
                    CultureInfo.CurrentCulture.NumberFormat);
                Article = Int32.Parse(arg.Article, CultureInfo.CurrentCulture.NumberFormat);
                Parent = (arg.Parent.Equals("null",StringComparison.Ordinal)) ?
                    null : (int?)Int32.Parse(arg.Parent, CultureInfo.CurrentCulture.NumberFormat);
            }
        }
        
        public Comment() { }
        
        public Comment(int id, string text, int user, int article, int? parent)
        {
            Id = id; ComText = text; UserId = user; Article = article; Parent = parent;
        }

        public Comment(int id, string text, int user, int article, string parent)
        {
            if (text == null)
            {
                throw new ArgumentNullException(paramName: nameof(text));
            }
            else if (parent == null)
            {
                throw new ArgumentNullException(paramName: nameof(parent));
            }
            Id = id;
            ComText = text;
            UserId = user;
            Article = article;
            Parent = (parent.Equals("null",StringComparison.Ordinal))?
                null:(int?)Int32.Parse(parent, CultureInfo.CurrentCulture.NumberFormat);
        }

        public Comment(Comment arg)
        {
            if (arg == null) throw (new NullReferenceException());
            Id = arg.Id; ComText = arg.ComText; UserId = arg.UserId; Article = arg.Article; Parent = arg.Parent;
        }

        public override string ToString()
        {
            return "Comment " + Id + " " + ComText;
        }
    }
}
