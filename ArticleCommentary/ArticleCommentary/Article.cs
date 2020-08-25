using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleCommentary
{
    public class Article
        //Article mapping class.
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ArticleText { get; set; }
        
        public Article() { }

        public Article(int id, string title, string text)
        {
            if (String.IsNullOrEmpty(title) || String.IsNullOrEmpty(text))
                throw (new Exception());
            Id = id; Title = title; ArticleText = text;
        }

        public Article(Article arg)
        {
            if (arg == null)
                throw new NullReferenceException();
            Id = arg.Id; ArticleText = arg.ArticleText; Title = arg.Title;
        }

        public override string ToString()
        {
            return "Article " + Id + " " + Title + " " + ArticleText;
        }
    }
}
