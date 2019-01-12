using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ResourcePresentationInSearch
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string KeyWords { get; set; }
        public DateTime Created { get; set; }

        public ResourcePresentationInSearch() { }

        public ResourcePresentationInSearch(int id, string title, string keyWords, DateTime created)
        {
            this.Id = id;
            this.Title = title;
            this.KeyWords = keyWords;
            this.Created = created;
        }
    }
}