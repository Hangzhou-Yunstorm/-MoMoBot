using Nest;
using System.Collections.Generic;

namespace MoMoBot.ElasticSearch.Models
{
    public class Question
    {
        public Question(string id, string content, string intent) : this(id, content, intent, intent)
        {

        }

        public Question(string id, string content, string intent, string text)
        {
            Id = id;
            Content = content;
            Text = text;
            Intent = intent;
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public string Content { get; set; }

        public string Intent { get; set; }
    }
}
