using Microsoft.WindowsAzure.Storage.Blob;
using SimpleImageGallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageGallery.Data
{
    public interface IImage
    {
        IEnumerable<GalleryImage> GetAll();
        IEnumerable<GalleryImage> GetWithTags(string tag);
        GalleryImage GetById( int Id);
        CloudBlobContainer GetBlobContainer(string connectingString, string containerName);
        Task SetImage(string title, string tags, Uri uri, string UserName);
        List<ImageTag> ParseTags(string tags);
        void DeleteImage(int Id);
    }
}
