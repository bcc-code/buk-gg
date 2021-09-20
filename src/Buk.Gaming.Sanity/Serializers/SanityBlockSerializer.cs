using Buk.Gaming.Providers;
using Buk.Gaming.Sanity.Models;
using Newtonsoft.Json.Linq;
using Sanity.Linq;
using Sanity.Linq.CommonTypes;
using Sanity.Linq.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Buk.Gaming.Sanity.Serializers
{

    public static class SanityBlockSerializerExtensions
    {
        public static SanityDataContext UseSanityBlockSerializer(this SanityDataContext sanity, SanityOptions options)
        {
            var serializer = new SanityBlockSerializer(options);
            serializer.Initialize(sanity);
            return sanity;

        }
    }


    internal class SanityBlockSerializer
    {
        private SanityDataContext _sanity { get; set; }
        public SanityOptions _opts { get; set; }

        internal SanityBlockSerializer(SanityOptions options)
        {
            _opts = options;
        }     
        
        internal void Initialize(SanityDataContext sanity)
        {
            _sanity = sanity;
            _sanity.AddHtmlSerializer("column2", SerializeDoubleColumns());
            _sanity.AddHtmlSerializer("column3", SerializeTrippleColumns());
            _sanity.AddHtmlSerializer("video", SerializeVideo());
            _sanity.AddHtmlSerializer("tableItem", SerializeTable());
            _sanity.AddHtmlSerializer("modal", SerializeModal());
        }

        public Func<JToken, SanityOptions, Task<string>> SerializeDoubleColumns()
        {
            return (JToken input, SanityOptions options) =>
            {
                //column1
                var col1val = input["column1"]?.ToObject<SanityBlock>().ToHtmlAsync(_sanity);

                //column2
                var col2val = input["column2"]?.ToObject<SanityBlock>().ToHtmlAsync(_sanity);

                var parameters = new StringBuilder();

                parameters.Append(@"<div class=""row"">");
                parameters.Append(@"<div class=""col col - md - 6 col - 12"">");
                parameters.Append(col1val);
                parameters.Append(@"</div>");
                parameters.Append(@"<div class=""col col - md - 6 col - 12"">");
                parameters.Append(col2val);
                parameters.Append(@"</div>");
                parameters.Append(@"</div>");

                return Task.FromResult(parameters.ToString());
            };
        }

        public Func<JToken, SanityOptions, Task<string>> SerializeTrippleColumns()
        {
            return (JToken input, SanityOptions options) =>
            {
                //column1
                var col1val = input["column1"]?.ToObject<SanityBlock>().ToHtml(_sanity);

                //column2
                var col2val = input["column2"]?.ToObject<SanityBlock>().ToHtml(_sanity);

                //column3
                var col3val = input["column3"]?.ToObject<SanityBlock>().ToHtml(_sanity);

                var parameters = new StringBuilder();

                parameters.Append(@"<div class=""row"">");
                parameters.Append(@"<div class=""col col - md - 4 col - 12"">");
                parameters.Append(col1val);
                parameters.Append(@"</div>");
                parameters.Append(@"<div class=""col col - md - 4 col - 12"">");
                parameters.Append(col2val);
                parameters.Append(@"</div>");
                parameters.Append(@"<div class=""col col - md - 4 col - 12"">");
                parameters.Append(col3val);
                parameters.Append(@"</div>");
                parameters.Append(@"</div>");

                return Task.FromResult(parameters.ToString());
            };
        }

        public Func<JToken, SanityOptions, Task<string>> SerializeVideo()
        {
            return (JToken input, SanityOptions options) =>
            {
                var videoid = input["videoid"]?.ToObject<string>() ?? "";
                var layout = input["layout"]?.ToObject<string>() ?? "";

                var _url = new StringBuilder();
                _url.Append("<div class=\"video-wrapper\">");
                if (layout == "youtube")
                {
                    _url.Append("<iframe class=\"gp-video youtube\" width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/" + videoid + "\" frameborder=\"0\" allow=\"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");
                }
                else if (layout == "vimeo")
                {
                    _url.Append("<iframe class=\"gp-video vimeo\" src=\"https://player.vimeo.com/video/" + videoid + "\" width=\"640\" height=\"320\" frameborder=\"0\" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>");
                }
                else
                {
                    _url.Append("");
                }
                _url.Append("</div>");
                return Task.FromResult(_url.ToString());
            };

            // gp-video  


            //YOUTUBE 
            //VIMEO 
        }

         public Func<JToken, SanityOptions, Task<string>> SerializeModal()
        {
            return (JToken input, SanityOptions options) =>
            {
                var html = new StringBuilder();
                var src = input["src"]?.ToObject<string>() ?? "";
                var buttonText = input["buttonText"]?.ToObject<string>() ?? "";
                var title = input["title"]?.ToObject<string>() ?? "";
  
                html.Append($"<button type=\"button\" class=\"btn btn-primary\" onclick=\"window.showModal('{src}')\">{buttonText}</button>");
                
                return Task.Run(() => html.ToString());

            };
        }


        public Func<JToken, SanityOptions, Task<string>> SerializeTable()
        {
            return (JToken input, SanityOptions options) =>
            {
                var table = input?.ToObject<SanityTable>();
                var html = new StringBuilder();
                if (table.Table == null)
                {
                    return Task.Run(() => "");
                }

                if (table.Bootstrap)
                {
                    var classes = new StringBuilder();

                    if (table.Responsive)
                    {
                        html.Append("<div class=\"table-responsive\">");
                    }
                    if (table.Striped)
                    {
                        classes.Append(" table-striped");
                    }
                    if (table.Bordered)
                    {
                        classes.Append(" table-bordered");
                    }
                    if (table.Borderless)
                    {
                        classes.Append(" table-borderless");
                    }
                    if (table.Hover)
                    {
                        classes.Append(" table-hover");
                    }
                    if (table.Condesed)
                    {
                        classes.Append(" table-sm");
                    }
                    if (!string.IsNullOrEmpty(table.Class))
                    {
                        classes.Append(" " + table.Class);
                    }

                    html.Append("<table class=\"table" + classes + "\">");
                    if (!string.IsNullOrEmpty(table.Caption))
                    {
                        html.Append("<caption>" + table.Caption + "</caption>");
                    }

                    if (table.Headers)
                    {
                        html.Append("<thead>");
                        html.Append("<tr>");
                        foreach (var cell in table.Table?.Rows[0].Cells)
                        {
                            html.Append("<th scope=\"col\">");
                            html.Append(cell);
                            html.Append("</th>");
                        }
                        html.Append("</tr>");
                        html.Append("</thead>");
                    }
                    Boolean first = true;
                    html.Append("<tbody>");
                    foreach (var row in table.Table?.Rows)
                    {
                        if (table.Headers && first)
                        {
                            // first row and has headers.. do nothing
                        }
                        else
                        {
                            html.Append("<tr>");
                            foreach (var cell in row.Cells)
                            {
                                html.Append("<td>");
                                html.Append(cell);
                                html.Append("</td>");
                            }
                            html.Append("</tr>");
                        }
                        first = false;
                    }
                    html.Append("</tbody>");
                    html.Append("</table>");
                    if (table.Responsive)
                    {
                        html.Append("</div>");
                    }
                    return Task.Run(() => html.ToString());
                }
                else
                {
                    //pure html
                    var classes = new StringBuilder();
                    var styles = new StringBuilder();

                    if (table.Responsive)
                    {
                        html.Append("<div style=\"width: 100%;\">");
                        styles.Append(" width:100%;");
                    }
                    if (!string.IsNullOrEmpty(table.Class))
                    {
                        classes.Append(table.Class);
                    }

                    html.Append("<table class=\"" + classes.ToString() + "\" style=\"" + styles.ToString() + "\">");
                    if (!string.IsNullOrEmpty(table.Caption))
                    {
                        html.Append("<caption>" + table.Caption + "</caption>");
                    }
                    if (table.Headers)
                    {
                        html.Append("<tr>");
                        foreach (var cell in table.Table?.Rows[0].Cells)
                        {
                            html.Append("<th>");
                            html.Append(cell);
                            html.Append("</th>");
                        }
                        html.Append("</tr>");
                    }
                    Boolean first = true;
                    foreach (var row in table.Table?.Rows)
                    {
                        if (table.Headers && first)
                        {
                            // first row and has headers.. do nothing
                        }
                        else
                        {
                            html.Append("<tr>");
                            foreach (var cell in row.Cells)
                            {
                                html.Append("<td>");
                                html.Append(cell);
                                html.Append("</td>");
                            }
                            html.Append("</tr>");
                        }
                        first = false;
                    }
                    html.Append("</table>");

                    if (table.Responsive)
                    {
                        html.Append("</div>");
                    }
                    return Task.Run(() =>html.ToString());
                }
            };
        }
    }
}
