using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
    //Copyright Konstantin Badanin.

namespace DataSingleton
{
    //public class Node
    //{
    //    public CommentNode Right { get; protected set; }
    //    public CommentNode Left { get; protected set; }
    //    public void SetRightComment(ref CommentNode comment)
    //    {
    //        Right = comment;
    //    }

    //    public void SetLeftComment(ref CommentNode comment)
    //    {
    //        Left = comment;
    //    }

    //    //public Node(Node arg)
    //    //{
    //    //    if (arg == null)
    //    //        throw new ArgumentNullException(paramName: nameof(arg));
    //    //    Right = arg.Right;
    //    //    Left = arg.Left;
    //    //}

    //    public Node()
    //    {
    //        Right = Left = null;
    //    }

    //    public List<CommentNode> GetAllDerivedCommentsInDeepOrder()
    //    //Returns list of derived comments.
    //    {
    //        List<CommentNode> DerivedComments = new List<CommentNode>();
    //        if (Left != null)
    //        {
    //            DerivedComments.Add(Left);
    //            foreach (CommentNode comment in Left.GetAllDerivedCommentsInDeepOrder())
    //            {
    //                DerivedComments.Add(comment);
    //            }
    //        }
    //        if (Right != null)
    //        {
    //            DerivedComments.Add(Right);
    //            foreach (CommentNode comment in Right.GetAllDerivedCommentsInDeepOrder())
    //            {
    //                DerivedComments.Add(comment);
    //            }
    //        }
    //        return DerivedComments;
    //    }
    //}

    //public class ArticleNode:Node
    //    //Model-list of ArticleNode. Each has a tree of CommentNodes.
    //{
    //    public Article Article { get; private set; }

    //    //public ArticleNode(int id, string title, string text)
    //    //{
    //    //    RightComment = LeftComment = null;
    //    //}

    //    public ArticleNode(Article arg) : base()
    //    {
    //        Article = arg ?? throw new ArgumentNullException(paramName: nameof(arg));
    //    }
    //}

    //public class CommentNode:Node
    //    //"Node" of comments. Binary tree element.
    //{
    //    public Comment Comment { get; private set; }
    //    //public CommentNode(int id, string text, User user, Article article, Comment parent) : base(id, text, user, article, parent)
    //    //{
    //    //    DerivedRightCommentNode = DerivedLeftCommentNode = null;
    //    //}

    //    public CommentNode(Comment arg):base()
    //    {
    //        Comment = arg ?? throw new ArgumentNullException(paramName:nameof(arg));
    //    }

    //    public void LoadToModel(ref List<CommentNode> lst)
    //    {
    //        List<CommentNode> tmp=lst.Where(x => (x.Comment.ParentId == Comment.Id) && (x.Comment.Article == Comment.Article)).ToList();
    //        int count = tmp.Count;
    //        if (count == 0)
    //        {
    //            return;
    //        }
    //        if (count == 1)
    //        {
    //            Left = tmp[0];
    //            Left.LoadToModel(ref lst);
    //            Right = null;
    //        }
    //        if (count == 2)
    //        {
    //            Left = tmp[0];
    //            Left.LoadToModel(ref lst);
    //            Right = tmp[1];
    //            Right.LoadToModel(ref lst);
    //        }
    //        if (count > 2)
    //        {
    //            throw (new InvalidOperationException());
    //        }
    //    }

    //    public bool RecInsertByParentId(ref CommentNode arg)
    //    //Return value: True-element added, false-not added.
    //    {
    //        if (arg == null) throw new ArgumentNullException(paramName: nameof(arg));
    //        if (Left == null)
    //        {
    //            if (arg.Comment.ParentId == Comment.Id)
    //            {
    //                SetLeftComment(ref arg);
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if (Left.RecInsertByParentId(ref arg)) return true;
    //        }
    //        if (Right == null)
    //        {
    //            if (arg.Comment.ParentId == Comment.Id)
    //            {
    //                SetRightComment(ref arg);
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if (Right.RecInsertByParentId(ref arg)) return true;
    //        }
    //        return false;
    //    }
    //}
}
