using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Configuration;
using SimpleImageGalerry.Models;
using System.IO;
using SimpleImageGallery.Data;
using SimpleImageGallery.Servises;
using Microsoft.AspNetCore.Authorization;

namespace SimpleImageGalerry.Controllers
{
   
    public class ImageController : Controller
    {
        private IConfiguration _config;

        private IImage _imageService;
        private string AzureConnectionString { get; }

        public ImageController(IConfiguration config, IImage imageService)
        {
            _config = config;
            _imageService = imageService;
            AzureConnectionString = _config["AzureStorageConnectionString"];
        }
        public IActionResult Upload()
        {
            var model = new UploadImageModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UploadNewImage(IFormFile file, string tags, string title)
        {
            var container = _imageService.GetBlobContainer(AzureConnectionString, "images");
            var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
            var fileName = content.FileName.Trim('"');

            
            { 
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(file.OpenReadStream());

            await _imageService.SetImage(title, tags, blockBlob.Uri,User.Identity.Name);
            }
    
            return RedirectToAction("Index", "Gallery");
        }



        private object GetContainer()
        {
            return _imageService.GetBlobContainer(AzureConnectionString, "images");
        }
    }
}