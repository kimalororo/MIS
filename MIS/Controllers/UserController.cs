using MIS.Models.Entities;
using MIS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIS.Views
{
    public class UsersController : Controller
    {
        
        public ActionResult ShowUsers()
        {
            using (var db = new MISEntities())
            {
                var users = db.User.ToList();           
                return View(users);
            }
        }

        [HttpGet]
        public ActionResult UserDetails(Guid id)
        {
            EditUserVM model;

            using (var db = new MISEntities())
            {
                var user = db.User.Find(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                model = new EditUserVM
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Patronymic = user.Patronymic,
                    Login = user.Login,
                    SelectedPostId = user.PostId,

                    Posts = db.Post
                        .Select(a => new
                        {
                            a.PostId,
                            Name = a.Name
                        })
                        .AsEnumerable()
                        .Select(p => new SelectListItem
                        {
                            Value = p.PostId.ToString(),
                            Text = p.Name,
                            Selected = true
                        })
                        .ToList(),
                };
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDetails(EditUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var db = new MISEntities())
            {
                var user = db.User.Find(model.UserId);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.Login = model.Login;
                user.Patronymic = model.Patronymic;
                user.PostId = model.SelectedPostId;

                db.SaveChanges();
            }

            return RedirectToAction("ShowUsers", "Users");
        }

        public ActionResult CreateUser()
        {
            using (var db = new MISEntities())
            {
                var model = new EditUserVM
                {
                    Posts = db.Post.Select(p => new SelectListItem
                    {
                        Value = p.PostId.ToString(),
                        Text = p.Name
                    }).ToList()
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(EditUserVM model)
        {
            using (var db = new MISEntities())
            {
                if (ModelState.IsValid)
                {
                    var newUser = new User
                    {
                        UserId = Guid.NewGuid(),
                        Login = model.Login,
                        Password = model.Password,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Patronymic = model.Patronymic,
                        PostId = model.SelectedPostId
                    };

                    db.User.Add(newUser);
                    db.SaveChanges();

                    return RedirectToAction("ShowUsers");
                }

                model.Posts = db.Post.Select(p => new SelectListItem
                {
                    Value = p.PostId.ToString(),
                    Text = p.Name
                }).ToList();

                return View(model);
            }
        }

        public ActionResult DeleteUser(Guid id)
        {
            using (var db = new MISEntities())
            {
                var user = db.User.FirstOrDefault(u => u.UserId == id);
                if (user == null)
                {
                    return HttpNotFound("Пользователь не найден.");
                }
                return View(user);
            }
        }

        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            using (var db = new MISEntities())
            {
                var user = db.User.FirstOrDefault(u => u.UserId == id);
                if (user != null)
                {
                    db.User.Remove(user);
                    db.SaveChanges();

                    return RedirectToAction("ShowUsers");
                }

                return HttpNotFound("Пользователь не найден.");
            }
        }
    }
}