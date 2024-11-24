using BestStore.Models;
using BestStore.Models.DTO;
using BestStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BestStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDTO productDTO)
        {
            if (productDTO.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDTO);
            }
            string fileName = Guid.NewGuid().ToString() + "_" + productDTO.ImageFile!.FileName;
            string filePath = webHostEnvironment.WebRootPath + "/products/" + fileName;
            using (var stream = System.IO.File.Create(filePath))
            {
                productDTO.ImageFile.CopyTo(stream);
            }
            Product product = new Product()
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                ImageFileName = fileName,
                Brand = productDTO.Brand,
                Category = productDTO.Category,
                CreatedAt = DateTime.Now
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            ProductDTO productDTO = new ProductDTO()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Brand = product.Brand,
                Category = product.Category
            };
            ViewData["id"] = id;
            ViewData["imageName"] = product.ImageFileName;
            ViewData["createdAt"] = product.CreatedAt;
            return View(productDTO);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDTO productDTO)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                ViewData["id"] = id;
                ViewData["imageName"] = product.ImageFileName;
                ViewData["createdAt"] = product.CreatedAt;
                return View(productDTO);
            }
            string fileName = product.ImageFileName;
            if (productDTO.ImageFile != null)
            {
                fileName = Guid.NewGuid().ToString() + "_" + productDTO.ImageFile.FileName;
                string filePath = webHostEnvironment.WebRootPath + "/products/" + fileName;
                using (var stream = System.IO.File.Create(filePath))
                {
                    productDTO.ImageFile.CopyTo(stream);
                }
                string oldFilePath = webHostEnvironment.WebRootPath + "/products/" + product.ImageFileName;
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.ImageFileName = fileName;
            product.Brand = productDTO.Brand;
            product.Category = productDTO.Category;

            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
    }
}
