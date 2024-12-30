using System.Web.Mvc;
using MIS.Models.ViewModels;
using MIS.Models.Entities;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web;

namespace MIS.Controllers
{
    public class BooksController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MISEntities())
                {
                    User user = null;
                    user = db.User.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        if (webUser.Password == user.Password)
                        {
                            string userRole = "";
                            Guid def = new Guid("2cc40e82-c3c6-4a85-8e8e-4725b455a6f0");
                            if (user.PostId == def)
                            {
                                userRole = "seller";
                            }
                            else
                            {
                                userRole = "admin";
                            }
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                               1,
                               user.Login,
                               DateTime.Now,
                               DateTime.Now.AddDays(1),
                               false,
                               userRole);
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("FirstViewMethod", "Books");
                        }
                    }
                }
            }
            ViewBag.Error = "Польлзователь не найден. Попробуйте еще раз";
            return View(webUser);
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Books");
        }
        public ActionResult FirstViewMethod(string search, string genre, int? year)
        {
            using (var db = new MISEntities())
            {
                var books = db.Book.Include("Genre").AsQueryable();

                if (!string.IsNullOrEmpty(genre))
                {
                    books = books.Where(b => b.Genre.Any(g => g.Name == genre));
                }

                if (year.HasValue)
                {
                    books = books.Where(b => b.Year.Year == year);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    books = books.Where(b => b.Name.Contains(search));
                }

                return View(books.ToList());
            }
        }

        [HttpGet]
        public ActionResult CreateBook()
        {
            using (var db = new MISEntities())
            {
                var model = new BookVM
                {
                    AuthorsList = db.Author.Select(a => new SelectListItem
                    {
                        Value = a.AuthorId.ToString(),
                        Text = a.LastName + " " + a.FirstName
                    }).ToList(),
                    GenresList = db.Genre.Select(g => new SelectListItem
                    {
                        Value = g.GenreId.ToString(),
                        Text = g.Name
                    }).ToList(),
                    ShelfsList = db.Shelf.Select(s => new SelectListItem
                    {
                        Value = s.ShelfId.ToString(),
                        Text = s.Location
                    }).ToList(),
                    PublisherList = db.Publsher.Select(p => new SelectListItem
                    {
                        Value = p.PublisherId.ToString(),
                        Text = p.Name
                    }).ToList()
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBook(BookVM newBook)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MISEntities())
                {
                    var book = new Book
                    {
                        BookId = Guid.NewGuid(),
                        Name = newBook.Name,
                        Year = newBook.Year,
                        Price = newBook.Price,
                        PublisherId = newBook.SelectedPublisherId,
                        Pages = newBook.Pages
                    };

                    foreach (var authorId in newBook.SelectedAuthorIds)
                    {
                        var author = db.Author.Find(authorId);
                        if (author != null)
                        {
                            book.Author.Add(author);
                        }
                    }

                    foreach (var genreId in newBook.SelectedGenreIds)
                    {
                        var genre = db.Genre.Find(genreId);
                        if (genre != null)
                        {
                            book.Genre.Add(genre);
                        }
                    }

                    foreach (var shelfId in newBook.SelectedShelfIds)
                    {
                        var shelf = db.Shelf.Find(shelfId);
                        if (shelf != null)
                        {
                            book.BookShelf.Add(new BookShelf
                            {
                                Book = book,
                                Shelf = shelf
                            });
                        }
                    }

                    db.Book.Add(book);
                    db.SaveChanges();
                }
                return RedirectToAction("FirstViewMethod");
            }

            using (var db = new MISEntities())
            {
                newBook.AuthorsList = db.Author.Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.LastName + " " + a.FirstName
                }).ToList();

                newBook.GenresList = db.Genre.Select(g => new SelectListItem
                {
                    Value = g.GenreId.ToString(),
                    Text = g.Name
                }).ToList();

                newBook.ShelfsList = db.Shelf.Select(s => new SelectListItem
                {
                    Value = s.ShelfId.ToString(),
                    Text = s.Location
                }).ToList();

                newBook.PublisherList = db.Publsher.Select(p => new SelectListItem
                {
                    Value = p.PublisherId.ToString(),
                    Text = p.Name
                }).ToList();
            }

            return View(newBook);
        }


        //[HttpGet]
        //public ActionResult ViewBook1(Guid BookId)
        //{
        //    Book model = new Book();
        //    using (var db = new MISEntities())
        //    {
        //        model = db.Book.Find(BookId);
        //    }
        //    return View(model);
        //}
        [HttpGet]
        public ActionResult ViewBook(Guid BookId)
        {
            BookVM model;

            using (var db = new MISEntities())
            {
                var book = db.Book.Find(BookId);
                if (book == null)
                {
                    return HttpNotFound();
                }

                model = new BookVM
                {
                    BookId = book.BookId,
                    Name = book.Name,
                    Pages = book.Pages,
                    Year = book.Year,
                    Price = book.Price,
                    SelectedAuthorIds = book.Author.Select(a => a.AuthorId).ToList(),
                    SelectedGenreIds = book.Genre.Select(g => g.GenreId).ToList(),
                    SelectedShelfIds = book.BookShelf.Select(bs => bs.Shelf.ShelfId).ToList(),
                    SelectedPublisherId = book.PublisherId,

                    AuthorsList = db.Author
                        .Select(a => new
                        {
                            a.AuthorId,
                            FullName = a.LastName + " " + a.FirstName
                        })
                        .AsEnumerable()
                        .Select(a => new SelectListItem
                        {
                            Value = a.AuthorId.ToString(),
                            Text = a.FullName,
                            Selected = book.Author.Any(ba => ba.AuthorId == a.AuthorId)
                        })
                        .ToList(),

                    GenresList = db.Genre
                        .Select(g => new
                        {
                            g.GenreId,
                            g.Name
                        })
                        .AsEnumerable()
                        .Select(g => new SelectListItem
                        {
                            Value = g.GenreId.ToString(),
                            Text = g.Name,
                            Selected = book.Genre.Any(bg => bg.GenreId == g.GenreId)
                        })
                        .ToList(),

                    ShelfsList = db.Shelf
                        .Select(s => new
                        {
                            s.ShelfId,
                            s.Location
                        })
                        .AsEnumerable()
                        .Select(s => new SelectListItem
                        {
                            Value = s.ShelfId.ToString(),
                            Text = s.Location,
                            Selected = book.BookShelf.Any(bs => bs.Shelf.ShelfId == s.ShelfId)
                        })
                        .ToList(),
                };
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ViewBook(BookVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new MISEntities())
            {
                var book = db.Book.Find(model.BookId);
                if (book == null)
                {
                    return HttpNotFound();
                }

                book.Name = model.Name;
                book.Pages = model.Pages;
                book.Year = model.Year;
                book.Price = model.Price;

                book.Author.Clear();
                var authors = db.Author.Where(a => model.SelectedAuthorIds.Contains(a.AuthorId)).ToList();
                foreach (var author in authors)
                {
                    book.Author.Add(author);
                }

                book.Genre.Clear();
                var genres = db.Genre.Where(g => model.SelectedGenreIds.Contains(g.GenreId)).ToList();
                foreach (var genre in genres)
                {
                    book.Genre.Add(genre);
                }

                book.BookShelf.Clear();
                var shelves = db.Shelf.Where(s => model.SelectedShelfIds.Contains(s.ShelfId)).ToList();
                foreach (var shelf in shelves)
                {
                    book.BookShelf.Add(new BookShelf { BookId = book.BookId, ShelfId = shelf.ShelfId });
                }

                db.SaveChanges();
            }

            return RedirectToAction("FirstViewMethod", "Books");
        }

        [HttpGet]
        public ActionResult DeleteBook(Guid bookId)
        {
            BookVM model;
            using (var db = new MISEntities())
            {
                Book book = db.Book.Find(bookId);
                model = new BookVM
                {
                    BookId = book.BookId,
                    Name = book.Name,
                    Year = book.Year,
                    Pages = book.Pages,
                    Price = book.Price,
                    SelectedAuthorIds = book.Author.Select(a => a.AuthorId).ToList(),
                    SelectedGenreIds = book.Genre.Select(g => g.GenreId).ToList(),
                    SelectedShelfIds = book.BookShelf.Select(bs => bs.Shelf.ShelfId).ToList(),
                    AuthorsList = book.Author.Select(a => new SelectListItem
                    {
                        Value = a.AuthorId.ToString(),
                        Text = $"{a.LastName} {a.FirstName}",
                        Selected = true
                    }).ToList(),
                    GenresList = book.Genre.Select(g => new SelectListItem
                    {
                        Value = g.GenreId.ToString(),
                        Text = g.Name,
                        Selected = true
                    }).ToList(),
                    ShelfsList = book.BookShelf.Select(bs => new SelectListItem
                    {
                        Value = bs.Shelf.ShelfId.ToString(),
                        Text = bs.Shelf.Location,
                        Selected = true
                    }).ToList(),
                };

                return View(model);
            }
        }

        [HttpPost, ActionName("DeleteBook")]
        public ActionResult DeletePost(Guid bookId)
        {
            try
            {
                using (var db = new MISEntities())
                {
                    var bookToDelete = db.Book
                    .Include("Author")            
                    .Include("Genre")           
                    .Include("BookShelf")        
                    .Include("Delivery")         
                    .FirstOrDefault(b => b.BookId == bookId);

                    if (bookToDelete == null)
                    {
                        return HttpNotFound();
                    }

                    bookToDelete.Author.Clear(); 
                    bookToDelete.Genre.Clear();

                    db.BookShelf.RemoveRange(bookToDelete.BookShelf); 
                    db.Delivery.RemoveRange(bookToDelete.Delivery);   

                    db.Book.Remove(bookToDelete);

                    db.SaveChanges();
                }

                return RedirectToAction("FirstViewMethod");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ошибка при удалении книги: " + ex.Message);
                return View();
            }
        }



    }
}
