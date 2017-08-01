using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models.FileUploadModels;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;

namespace AnlabMvc.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileStorageService _fileStorageService;


        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }


        [HttpPut]
        public IActionResult NewFile(IFormFile file)
        {
            

            //var fileToUpload = new FileUpload
            //{
            //    ContentType =  file.
            //};

            //return View();
        }
    }
}