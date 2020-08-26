using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleCommentary
{
    public class User
        //User mapping class.
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User() { }
        public User(int Id1,string Name1)
        {
            Id = Id1; Name = Name1;
        }
    }
}
