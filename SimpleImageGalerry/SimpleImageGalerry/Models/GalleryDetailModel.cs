using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleImageGalerry.Models
{
    public class GalleryDetailModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreateOn { get; set; }
        public string Url { get; set; }
        public List<string> Tags { get; set; }

        public User User { get; set;} 
    }
}
