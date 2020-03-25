﻿using APPartment.Data;
using APPartment.Models;
using System.Collections.Generic;
using System.Linq;

namespace APPartment.Utilities
{
    public class HtmlRenderHelper
    {
        public List<string> BuildMessagesForChat(List<Message> messages, long currentHouseId)
        {
            return messages.Where(x => x.HouseId == currentHouseId).OrderByDescending(x => x.Id).Take(10).OrderBy(x => x.Id).Select(x => $"{x.Username}: {x.Text}").ToList();
        }

        public List<string> BuildComments(List<Comment> comments, long targetId, DataAccessContext context)
        {
            return comments.Where(x => x.TargetId == targetId)
                .OrderByDescending(x => x.Id).Take(20).Select(x => $"<strong>{x.Username}</strong>: {x.Text} <br/> <strong><span style=\"font-size: x-small;\">{context.Objects.Where(y => y.ObjectId == x.ObjectId).FirstOrDefault().CreatedDate.ToString("dd'/'MM'/'yyyy HH:mm:ss")}</span></strong>").ToList();
        }

        public string BuildPostComment(Comment comment, DataAccessContext context)
        {
            return $"<strong>{comment.Username}</strong>: {comment.Text} <br/> <strong><span style=\"font-size: x-small;\">{context.Objects.Where(y => y.ObjectId == comment.ObjectId).FirstOrDefault().CreatedDate.ToString("dd'/'MM'/'yyyy HH:mm:ss")}</span></strong>";
        }
    }
}
