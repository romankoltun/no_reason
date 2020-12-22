using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.Compute.Models;
using SimpleImageGalerry.Models;
using SimpleImageGallery.Data;
using SimpleImageGallery.Data.Models;
using GalleryImage = SimpleImageGallery.Data.Models.GalleryImage;


namespace SimpleImageGalerry.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImage _imageService;
        public GalleryController(IImage imageService)
        {
            _imageService = imageService;
        }

        public IActionResult Index()
        {
           
            IEnumerable<GalleryImage> imageList = _imageService.GetAll().Where(p => p.User.Login == User.Identity.Name);
            var model = new GalleryIndexModel()
            {
                Images = imageList,
                SearchQuery = ""
            };

            return View(model);
        }
        public IActionResult Detail(int Id)
        {
            var image = _imageService.GetById(Id);
            var model = new GalleryDetailModel()
            {
                Id = image.Id,
                Title = image.Title,
                CreateOn = image.Created,
                Url = image.Url,
                Tags = image.Tags.Select(t => t.Description).ToList()
                
            };
            return View(model);
        }
        public RedirectToActionResult Delete(int id) 
        {
            _imageService.DeleteImage(id);
            return RedirectToAction("Index");
        }
    }

}