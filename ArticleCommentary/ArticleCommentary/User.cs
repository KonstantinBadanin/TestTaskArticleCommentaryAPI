using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleCommentary
{
    public class User
        //User mapping class.
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public User() { }

        public User(int id, string name, List<Comment> lst)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Argument can't be null or empty", paramName: nameof(name));
            Comments = lst ?? throw new ArgumentException("Argument can't be null: ", paramName: nameof(lst));
            Id = id; Name = name;
        }

        public override string ToString()
        {
            return Id + " " + Name;
        }

        public User(Request arg)
        {
            if (arg == null)
                throw new ArgumentException("Argument can't be null: ", paramName: nameof(arg));
            Id = Int32.Parse(arg.UserId,CultureInfo.CurrentCulture.NumberFormat);
            Name = arg.UserName;
            Comments.Add(new Comment(arg));
        }
    }
}
