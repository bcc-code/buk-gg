using Buk.Gaming.Models;
using Sanity.Linq;
using Sanity.Linq.BlockContent;
using Sanity.Linq.CommonTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buk.Gaming.Sanity.Models
{
    public class SanityEvent : SanityDocument
    {
        public SanityEvent()
        {
            SanityType = "event";
        }

        public SanityLocaleString Title { get; set;}

        public SanitySlug Slug { get; set; }

        [Include]
        public SanityImage Image { get; set; }

        public SanityReference<SanityCategory> Category { get; set; }

        [Include]
        public SanityReference<Player> Responsible { get; set; }

        public DateTime Date { get; set; }

        public SanityLocaleBlock Description { get; set; }

        public bool LiveChat { get; set; }

        public EventInfo ToEventInfo(SanityHtmlBuilder htmlBuilder)
        {
            var responsible = new Player();
            if (this.Responsible != null) {
                responsible.Name = this.Responsible.Value.Name;
                responsible.Id = this.Responsible.Value.Id;
                responsible.Email = this.Responsible.Value.Id;
                responsible.Nickname = this.Responsible.Value.Nickname;
            }

            return new EventInfo {
                Id = this.Id,
                Title = this.Title?.GetForCurrentCulture(),
                Image = this.Image?.Asset?.Value?.Url,
                Responsible = this.Responsible?.Value,
                Date = this.Date,
                Description = this.Description?.GetForCurrentCulture(htmlBuilder),
                CategoryId = this.Category?.Ref,
            };
        }

    }
}
