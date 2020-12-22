using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SimpleImageGallery.Servises
{
    public class ImageService : IImage

    {
        private readonly SimpleImageGalleryDBContext _ctx; 
        public ImageService(SimpleImageGalleryDBContext ctx)
        {
            _ctx = ctx;
        }
        public IEnumerable<GalleryImage> GetAll()
        {
            return _ctx.GalleryImages.Include(img => img.Tags).Include(img => img.User);
        }

        public GalleryImage GetById(int Id)
        {
            return GetAll().Where(img => img.Id ==Id).First();
        }

        public IEnumerable<GalleryImage> GetWithTags(string tag)
        {
            return GetAll().Where(img => img.Tags.Any(t => t.Description == tag));
        }

        public CloudBlobContainer GetBlobContainer(string azureConnectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);

        }

        public async Task SetImage(string title, string tags, Uri uri, string UserName)
        {

            var image = new GalleryImage
            {
                Title = title,
                Url = uri.AbsoluteUri,
                Created = DateTime.Now,
                User = _ctx.User.FirstOrDefault(p=>p.Login == UserName) 
            };

            _ctx.Add(image);
             await _ctx.SaveChangesAsync();

            if (!string.IsNullOrEmpty(tags)) 
            {
                image.Tags = ParseTags(tags);
             }
        }
        public  void DeleteImage(int Id)
        {
            var a = _ctx.GalleryImages.FirstOrDefault(p => p.Id == Id);
            if (a.Tags!=null)
            _ctx.ImageTags.RemoveRange(a.Tags);
            _ctx.Remove(a);
             _ctx.SaveChanges();
        }

        public List<ImageTag> ParseTags(string tags)
        {
            return tags.Split(",").Select(tag => new ImageTag { Description = tag}).ToList();
        }

        public CloudBlobContainer GetCloudBlobContainer(string connectingString, string containerName)
        {
            throw new NotImplementedException();
        }
    }
}
