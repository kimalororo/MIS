using System.Web.Mvc;
using MIS.Models.ViewModels;
using MIS.Models.Entities;
using System.Linq;
using System;
using System.Collections.Generic;

namespace MIS.Controllers
{
    public class ShelvesController : Controller
    {
        public ActionResult ShowShelves(string search)
        {
            using (var db = new MISEntities())
            {
                var shelves = db.Shelf
                    .Include("BookShelf")
                    .Select(s => new
                    {
                        s.ShelfId,
                        s.Location,
                        TotalBooks = s.BookShelf.Sum(bs => (int?)bs.Counter) ?? 0
                    })
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    shelves = shelves.Where(s => s.Location.Contains(search));
                }

                var shelvesViewModel = shelves
                   .ToList()
                   .Select(s => new ShelfVM
                   {
                       ShelfId = s.ShelfId,
                       Location = s.Location,
                       TotalBooks = s.TotalBooks
                   })
                   .ToList();

                return View(shelvesViewModel);
            }
        }


        [HttpGet]
        public ActionResult EditShelf(Guid id)
        {
            using (var db = new MISEntities())
            {
                var shelf = db.Shelf
                    .Include("BookShelf")
                    .Include("BookShelf.Book")
                    .FirstOrDefault(s => s.ShelfId == id);

                if (shelf == null)
                {
                    return HttpNotFound();
                }

                var model = new EditShelfWithBooksVM
                {
                    ShelfId = shelf.ShelfId,
                    Location = shelf.Location,
                    BooksOnShelf = shelf.BookShelf.Select(bs => new BookOnShelfVM
                    {
                        BookId = bs.BookId,
                        BookName = bs.Book.Name,
                        Counter = bs.Counter
                    }).ToList()
                };

                // Список книг для dropdown
                ViewBag.BooksList = new SelectList(db.Book.Select(b => new { b.BookId, b.Name }).ToList(), "BookId", "Name");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditShelf(EditShelfWithBooksVM model, string action, Guid? NewBookId, int? NewBookCounter)
        {
            using (var db = new MISEntities())
            {
                if (action == "addBook" && NewBookId.HasValue && NewBookCounter.HasValue)
                {                 
                    var book = db.Book.Find(NewBookId.Value);
                    if (book != null)
                    {
                        model.BooksOnShelf.Add(new BookOnShelfVM
                        {
                            BookId = book.BookId,
                            BookName = book.Name,
                            Counter = NewBookCounter.Value
                        });
                    }
                    ViewBag.BooksList = new SelectList(db.Book.Select(b => new { b.BookId, b.Name }).ToList(), "BookId", "Name");
                    return View(model);
                }

                if (action == "saveShelf")
                {
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    var shelf = db.Shelf.Include("BookShelf").FirstOrDefault(s => s.ShelfId == model.ShelfId);

                    if (shelf == null)
                    {
                        return HttpNotFound();
                    }

                    shelf.Location = model.Location;

                    foreach (var bookModel in model.BooksOnShelf)
                    {
                        var bookShelf = db.BookShelf.FirstOrDefault(bs => bs.BookId == bookModel.BookId && bs.ShelfId == shelf.ShelfId);

                        if (bookShelf == null)
                        {
                            db.BookShelf.Add(new BookShelf
                            {
                                BookId = bookModel.BookId,
                                ShelfId = shelf.ShelfId,
                                Counter = bookModel.Counter
                            });
                        }
                        else
                        {
                            bookShelf.Counter = bookModel.Counter;
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("ShowShelves", "Shelves");
                }

                return View(model);
            }
        }



        [HttpGet]
        public ActionResult DeleteShelf(Guid id)
        {
            using (var db = new MISEntities())
            {
                var shelf = db.Shelf.FirstOrDefault(s => s.ShelfId == id);

                if (shelf == null)
                {
                    return HttpNotFound();
                }
                return View(shelf);
            }
        }

        [HttpPost, ActionName("DeleteShelf")]
        public ActionResult DeleteShelfConfirmed(Guid id)
        {
            using (var db = new MISEntities())
            {
                var shelf = db.Shelf.FirstOrDefault(s => s.ShelfId == id);

                if (shelf == null)
                {
                    return HttpNotFound();
                }

                var bookShelves = db.BookShelf.Where(bs => bs.ShelfId == id).ToList();
                db.BookShelf.RemoveRange(bookShelves);
                db.Shelf.Remove(shelf);
                db.SaveChanges();
            }

            return RedirectToAction("ShowShelves", "Shelves");
        }

        [HttpGet]
        public ActionResult CreateShelf()
        {
            using (var db = new MISEntities())
            {
                var books = db.Book
                    .Select(b => new SelectListItem
                    {
                        Value = b.BookId.ToString(),
                        Text = b.Name
                    })
                    .ToList();

                ViewBag.Books = books;

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateShelf(string location, List<Guid> selectedBooks)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                ModelState.AddModelError("Location", "Location is required.");
            }

            if (!ModelState.IsValid)
            {
                using (var db = new MISEntities())
                {                  
                    ViewBag.Books = db.Book
                        .Select(b => new SelectListItem
                        {
                            Value = b.BookId.ToString(),
                            Text = b.Name
                        })
                        .ToList();
                }
                return View();
            }

            using (var db = new MISEntities())
            {
                var newShelf = new Shelf
                {
                    ShelfId = Guid.NewGuid(),
                    Location = location
                };
                db.Shelf.Add(newShelf);

                if (selectedBooks != null && selectedBooks.Count > 0)
                {
                    foreach (var bookId in selectedBooks)
                    {
                        db.BookShelf.Add(new BookShelf
                        {
                            BookId = bookId,
                            ShelfId = newShelf.ShelfId,
                            Counter = 1 
                        });
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("ShowShelves", "Shelves");
        }
    }
}
