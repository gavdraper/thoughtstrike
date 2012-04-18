using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ideastrike.Helpers;
using DanTup.Web;

namespace Ideastrike.Models.ViewModels
{
    public class IdeaListViewModel
    {
        public int IdeaId { get; set; }
        public int VoteCount { get; set; }
        public bool UserHasVoted { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string ColorClass { get; set; }


        public static implicit operator IdeaListViewModel(Idea i)
        {
            return new IdeaListViewModel
            {
                IdeaId = i.Id,
                VoteCount = i.Votes.Sum(v => v.Value),
                UserHasVoted = i.UserHasVoted,
                Title = i.Title,
                Status = i.Status,
                Time = i.Time.ToFriendly().ToString(),
                Description = MarkdownHelper.Markdown(i.Description.ConvertingLinksToMarkdownUrls()).ToString(),
                ColorClass = StatusColorHelper.ColorClass(i.Status).ToString()
            };
        }

    }
}