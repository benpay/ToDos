using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ToDos.Models;

namespace ToDos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region Category
        public IActionResult ListCategories()
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            using (var db = new Context())
            {
                categories = db.Category.ToList();
            }

            TempData["categories"] = categories;

            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(CategoryModel category)
        {
            using (var db = new Context())
            {
                db.Add(category);
                db.SaveChanges();
            }
            return View("ListCategories");
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DelCategory(CategoryModel category)
        {
            using (var db = new Context())
            {
                var item = db.Category.ToList().Find(x => x.Name.ToLower().Contains(category.Name.ToLower()));
                if (item is not null)
                {
                    db.Remove(item);
                    db.SaveChanges();
                }
            }
            return View("ListCategories");
        }

        public IActionResult DelCategory()
        {
            return View();
        }

        public IActionResult GetCategoryByName(string name)
        {
            List<CategoryModel> category = new List<CategoryModel>();

            using (var db = new Context())
            {
                category = db.Category.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            return View();
        }

        [HttpPost]
        public IActionResult UpdateCategory(CategoryModel category)
        {
            using (var db = new Context())
            {
                var categoryTemp = db.Category.Where(x => x.Id == category.Id).FirstOrDefault();

                TempData["categoryTemp"] = categoryTemp;
            }

            return View();
        }

        public IActionResult UpdateCategoryFinal(CategoryModel category)
        {

            using (var db = new Context())
            {
                var updateCategory = db.Category.Where(x => x.Id == category.Id).FirstOrDefault();

                updateCategory.Name = category.Name;

                db.SaveChanges();
            }

            return View("ListCategories");
        }


        public IActionResult SearchCategories(CategoryModel category)
        {
            List<CategoryModel> categories = new List<CategoryModel>();

            using (var db = new Context())
            {
                categories = db.Category.Where(x => x.Name.ToLower().Contains(category.Name.ToLower())).ToList();
            }

            TempData["categories"] = categories;

            return View("ListCategories");
        }
        #endregion

        #region ToDo
        [HttpPost]
        public IActionResult AddToDo(ToDoModel todo)
        {
            using (var db = new Context())
            {
                if (todo.CategoryName != null)
                {
                    CategoryModel categorySection;
                    using (var dbCategory = new Context())
                    {
                        categorySection = dbCategory.Category.Where(x => x.Name.ToLower() == todo.CategoryName.ToLower()).FirstOrDefault();
                    }

                    if (categorySection != null && categorySection.Name != string.Empty)
                    {
                        db.Add(todo);
                        db.SaveChanges();
                    }
                    else
                    {
                        return View("WrongCategory");
                    }
                }
            }
            return View("ListTodos");
        }

        public IActionResult AddToDo()
        {
            return View();
        }

        public IActionResult ListToDos()
        {
            List<ToDoModel> todos = new List<ToDoModel>();

            using (var db = new Context())
            {
                todos = db.ToDo.ToList();
            }

            TempData["todos"] = todos;

            return View();
        }


        [HttpPost]
        public IActionResult UpdateToDo(ToDoModel todo)
        {
            using (var db = new Context())
            {
                var todoTemp = db.ToDo.Where(x => x.Id == todo.Id).FirstOrDefault();

                TempData["todoTemp"] = todoTemp;
            }

            return View();
        }

        public IActionResult UpdateToDoFinal(ToDoModel todo)
        {

            using (var db = new Context())
            {
                var updateToDo = db.ToDo.Where(x => x.Id == todo.Id).FirstOrDefault();

                CategoryModel categorySection;
                using (var dbCategory = new Context())
                {
                    categorySection = dbCategory.Category.Where(x => x.Name.ToLower() == todo.CategoryName.ToLower()).FirstOrDefault();
                }

                if (categorySection != null && categorySection.Name != string.Empty)
                {
                    updateToDo.Name = todo.Name;
                    updateToDo.Description = todo.Description;
                    updateToDo.CategoryName = todo.CategoryName;
                    db.SaveChanges();
                }
                else
                {
                    return View("WrongCategory");
                }
            }

            return View("ListToDos");
        }

        [HttpPost]
        public IActionResult DelToDo(ToDoModel todo)
        {
            using (var db = new Context())
            {
                var item = db.ToDo.ToList().Find(x => x.Name.ToLower().Contains(todo.Name.ToLower()));
                if (item is not null)
                {
                    db.Remove(item);
                    db.SaveChanges();
                }
            }
            return View("ListTodos");
        }

        public IActionResult DelToDo()
        {
            return View();
        }

        public IActionResult SearchTDo(ToDoModel todo)
        {
            List<ToDoModel> todos = new List<ToDoModel>();

            using (var db = new Context())
            {
                todos = ApplyFilterCriteria(todo, db);
            }

            TempData["todos"] = todos;

            return View("ListToDos");
        }

        private static List<ToDoModel> ApplyFilterCriteria(ToDoModel todo, Context db)
        {
            List<ToDoModel> todos;
            if (todo.Name != null)
            {
                if (todo.Description != null)
                {
                    todos = todo.CategoryName != null
                        // If we have Name, Description and Category to filter
                        ? db.ToDo.ToList().Where(x => x.Name.ToLower().Contains(todo.Name.ToLower())
                                                 && x.Description.ToLower().Contains(todo.Description.ToLower())
                                                 && x.CategoryName.ToLower().Contains(todo.CategoryName.ToLower())).ToList()
                        // If we have Name and Description to filter
                        : db.ToDo.ToList().Where(x => x.Name.ToLower().Contains(todo.Name.ToLower())
                                                 && x.Description.ToLower().Contains(todo.Description.ToLower())).ToList();
                }
                else
                {
                    todos = todo.CategoryName != null
                        // If we have Name and Category to filter
                        ? db.ToDo.ToList().Where(x => x.Name.ToLower().Contains(todo.Name.ToLower())
                                                 && x.CategoryName.ToLower().Contains(todo.CategoryName.ToLower())).ToList()
                        // If we have Name to filter
                        : db.ToDo.ToList().Where(x => x.Name.ToLower().Contains(todo.Name.ToLower())).ToList();
                }
            }
            else
            {
                if (todo.Description != null)
                {
                    todos = todo.CategoryName != null
                        // If we have Description and Category to filter
                        ? db.ToDo.ToList().Where(x => x.Description.ToLower().Contains(todo.Description.ToLower())
                                                 && x.CategoryName.ToLower().Contains(todo.CategoryName.ToLower())).ToList()
                        // If we have Name, Description and NOT Category to filter
                        : db.ToDo.ToList().Where(x => x.Description.ToLower().Contains(todo.Description.ToLower())).ToList();
                }
                else
                {
                    todos = db.ToDo.ToList().Where(x => x.CategoryName.ToLower().Contains(todo.CategoryName.ToLower())).ToList();
                }
            }

            return todos;
        }
        #endregion
    }
}
