using Microsoft.EntityFrameworkCore;
using SimpleImageGallery.Data.Models;
using System;

namespace SimpleImageGallery.Data
{
    public class SimpleImageGalleryDBContext : DbContext
    {
        public SimpleImageGalleryDBContext(DbContextOptions options) : base(options)
        {
            
        }
        public virtual DbSet<GalleryImage> GalleryImages { get; set; }
        public virtual DbSet<ImageTag> ImageTags { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
