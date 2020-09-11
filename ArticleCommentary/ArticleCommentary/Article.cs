using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSinglton
{
    public class Article
        //Article mapping class.
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArtText { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        
        public Article() { }

        public Article(int id, string title, string text, List<Comment> lst)
        {
            if (lst == null)
                throw new ArgumentException("Argument can't be null: ",paramName: nameof(lst));
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Argument can't be null or empty: ", paramName: nameof(title));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Argument can't be null or empty: ", paramName: nameof(text));
            Id = id; Title = title; ArtText = text; Comments = lst;
        }

        public Article(Article arg)
        {
            if (arg == null)
                throw new NullReferenceException();
            Id = arg.Id;
            ArtText = arg.ArtText;
            Title = arg.Title;
            Comments = arg.Comments;
        }

        public override string ToString()
        {
            return "Article " + Id + " " + Title + " " + ArtText;
        }

        public List<string> ToStringCustom(Recursion limit)
        {
            List<string> tmp = new List<string>();
            tmp.Add("Article " + Id + " " + ArtText);
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

    public enum Recursion : int
    {
        //Limit goes down to 0.
        Limit = 500
    }
}
