﻿using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers;
[Authorize(Roles = "admin")]
// Defines a route template for Admin area controllers with optional 'id' and default 'action' set to Index
[Route("/Admin/[controller]/{action=Index}/{id?}")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly IWebHostEnvironment environment;
    private readonly int pageSize = 5;
    public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }
    public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
    {
        // Retrieves all products from the database and passes them to the view.
        IQueryable<Product> query = context.Products;

        // search functionality
        if (search != null)
        {
            query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
        }

        // sort functionality
        string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt"};
        string[] validOrderBy = { "desc", "asc" };



        if (!validColumns.Contains(column))
        {
            column = "Id";
        }

        if (!validOrderBy.Contains(orderBy))
        {
            orderBy = "desc";
        }

        if (column == "Name")
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.Name);
            }
            else
            {
                query = query.OrderByDescending(p => p.Name);
            }
        }
        else if (column == "Brand")
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.Brand);
            }
            else
            {
                query = query.OrderByDescending(p => p.Brand);
            }
        }
        else if (column == "Category")
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.Category);
            }
            else
            {
                query = query.OrderByDescending(p => p.Category);
            }
        }
        else if (column == "Price")
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else
            {
                query = query.OrderByDescending(p => p.Price);
            }
        }
        else if (column == "CreatedAt")
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }
        }
        else
        {
            if (orderBy == "asc")
            {
                query = query.OrderBy(p => p.Id);
            }
            else
            {
                query = query.OrderByDescending(p => p.Id);
            }
        }


        // pagination functionality
        if (pageIndex < 1)
        {
            pageIndex = 1;
        }

        if (!validOrderBy.Contains(orderBy))
        {
            orderBy = "desc";
        }

        decimal count = query.Count();
        int totalPages = (int)Math.Ceiling(count / pageSize);
        query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

        var products = query.ToList();

        ViewData["PageIndex"] = pageIndex;
        ViewData["TotalPages"] = totalPages;

        ViewData["Search"] = search ?? "";

        ViewData["Column"] = column;
        ViewData["OrderBy"] = orderBy;
        
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(ProductDto productDto)
    {
        if (productDto.ImageFile == null)
        {
            ModelState.AddModelError("ImageFile", "The image file is required");
        }

        if (!ModelState.IsValid)
        {
            return View(productDto);
        }


        // save the image file
        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

        string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
        using (var stream = System.IO.File.Create(imageFullPath))
        {
            productDto.ImageFile.CopyTo(stream);
        }

        // save the new product in the database
        Product product = new Product()
        {
            Name = productDto.Name,
            Brand = productDto.Brand,
            Category = productDto.Category,
            Price = productDto.Price,
            Description = productDto.Description,
            ImageFileName = newFileName,
            CreatedAt = DateTime.Now,
        };

        context.Products.Add(product);
        context.SaveChanges();

        // redirect user to the list of products
        return RedirectToAction("Index", "Products");
    }


    // Handles GET requests to load the Edit view with data for the product identified by the given ID.
    public IActionResult Edit(int id)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }

        // object of type productDto that is feld using the data of the product object that we obtained from the database
        var productDto = new ProductDto()
        {
            Name = product.Name,
            Brand = product.Brand,
            Category = product.Category,
            Price = product.Price,
            Description = product.Description,
        };

        // Passes product data to the Edit.cshtml view for display (ID, image name, and formatted creation date).
        ViewData["ProductId"] = product.Id;
        ViewData["ImageFileName"] = product.ImageFileName;
        ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

        return View(productDto);
    }


    // Handles POST requests to update an existing product with the given ID using the submitted form data.
    [HttpPost]
    public IActionResult Edit(int id, ProductDto productDto)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }


        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }


        // update the image file if we have a new image file
        string newFileName = product.ImageFileName;
        if (productDto.ImageFile != null)
        {
            newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // delete the old image
            string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(oldImageFullPath);
        }


        // update the product in the database
        product.Name = productDto.Name;
        product.Brand = productDto.Brand;
        product.Category = productDto.Category;
        product.Price = productDto.Price;
        product.Description = productDto.Description;
        product.ImageFileName = newFileName;


        context.SaveChanges();

        return RedirectToAction("Index", "Products");
    }

    public IActionResult Delete(int id)
    {
        var product = context.Products.Find(id);
        if (product == null)
        {
            return RedirectToAction("Index", "Products");
        }

        string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
        System.IO.File.Delete(imageFullPath);

        context.Products.Remove(product);
        context.SaveChanges(true);

        return RedirectToAction("Index", "Products");

    }
}