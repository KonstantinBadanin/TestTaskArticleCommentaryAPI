using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    //Copyright Konstantin Badanin.

namespace ArticleCommentary
{
    public class ArticleNode:Article
        //Model-list of ArticleNode. Each has a tree of CommentNodes.
    {
        private CommentNode _rightComment;
        private CommentNode _leftComment;
        public CommentNode RightComment
        {
            get
            {
                return _rightComment;
            }
            private set
            {
                _rightComment = value;
            }
        }
        public CommentNode LeftComment
        {
            get
            {
                return _leftComment;
            }
            private set
            {
                _leftComment = value;
            }
        }

        public void SetRightComment(ref CommentNode comment)
        {
            RightComment = comment;
        }

        public void SetLeftComment(ref CommentNode comment)
        {
            LeftComment = comment;
        }

        public ArticleNode(int id, string title, string text) : base(id, title, text)
        {
            RightComment = LeftComment = null;
        }

        public ArticleNode(Article arg) : base(arg)
        {
            if (arg == null) throw (new NullReferenceException());
            RightComment = LeftComment = null;
        }
    }

    public enum PropertyName
    {
        ComId = 0,
        ComText = 1,
        UserId = 2,
        Article = 3,
        Parent = 4,
        UserId1 = 5,
        Name = 6,
        ArtId = 7,
        Title = 8,
        ArtText = 9
    }

    public class CommentNode : Comment
        //"Node" of comments. Binary tree element.
    {
        private CommentNode derivedLeftCommentNode;
        private CommentNode derivedRightCommentNode;

        public CommentNode DerivedLeftCommentNode
        {
            get
            {
                return derivedLeftCommentNode;
            }
            private set
            {
                derivedLeftCommentNode = value;
            }
        }

        public CommentNode DerivedRightCommentNode
        {
            get
            {
                return derivedRightCommentNode;
            }
            private set
            {
                derivedRightCommentNode = value;
            }
        }

        public void SetDerivedLeftCommentNode(ref CommentNode comment)
        {
            DerivedLeftCommentNode = comment;
        }

        public void SetDerivedRightCommentNode(ref CommentNode comment)
        {
            DerivedRightCommentNode = comment;
        }

        public CommentNode(int id, string text, int user, int article, int? parent) : base(id, text, user, article, parent)
        {
            DerivedRightCommentNode = DerivedLeftCommentNode = null;
        }

        public CommentNode(Comment arg) : base(arg)
        {
            if (arg == null) throw (new NullReferenceException());
            DerivedRightCommentNode = DerivedLeftCommentNode = null;
        }

        public List<CommentNode> GetAllDerivedComments()
            //Returns list of derived comments.
        {
            List<CommentNode> DerivedComments = new List<CommentNode>();
            if (this == null) return DerivedComments;
            if (DerivedLeftCommentNode != null)
            {
                DerivedComments.Add(DerivedLeftCommentNode);
                foreach (CommentNode comment in DerivedLeftCommentNode.GetAllDerivedComments())
                {
                    DerivedComments.Add(comment);
                }
            }
            if (DerivedRightCommentNode != null)
            {
                DerivedComments.Add(DerivedRightCommentNode);
                foreach (CommentNode comment in DerivedRightCommentNode.GetAllDerivedComments())
                {
                    DerivedComments.Add(comment);
                }
            }
            return DerivedComments;
        }

        public void LoadToModel(ref List<CommentNode> lst)
        {
            List<CommentNode> tmp=lst.Where(x => (x.Parent == Id) && (x.Article == Article)).ToList();
            int count = tmp.Count;
            if (count == 0)
            {
                return;
            }
            if (count == 1)
            {
                DerivedLeftCommentNode = tmp[0];
                DerivedRightCommentNode = null;
                DerivedLeftCommentNode.LoadToModel(ref lst);
            }
            if (count == 2)
            {
                DerivedLeftCommentNode = tmp[0];
                DerivedRightCommentNode = tmp[1];
                DerivedLeftCommentNode.LoadToModel(ref lst);
                DerivedRightCommentNode.LoadToModel(ref lst);
            }
            if (count > 2)
            {
                throw (new InvalidOperationException());
            }
        }

        public bool RecInsertByParentId(ref CommentNode arg)
            //Return value: True-element added, false-not added.
        {
            if (arg == null) throw new ArgumentNullException(paramName: nameof(arg));
            if (this == null) return false;
            if (DerivedLeftCommentNode == null)
            {
                if (arg.Parent == Id)
                {
                    SetDerivedLeftCommentNode(ref arg);
                    return true;
                }
            }
            else
            {
                if (DerivedLeftCommentNode.RecInsertByParentId(ref arg)) return true;
            }
            if (DerivedRightCommentNode == null)
            {
                if (arg.Parent == Id)
                {
                    SetDerivedRightCommentNode(ref arg);
                    return true;
                }
            }
            else
            {
                if (DerivedRightCommentNode.RecInsertByParentId(ref arg)) return true;
            }
            return false;
        }
    }
}
