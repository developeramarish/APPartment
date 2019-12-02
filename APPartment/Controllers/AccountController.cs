﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APPartment.Data;
using APPartment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APPartment.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataAccessContext _context;

        public AccountController(DataAccessContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.User.ToList());
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.User.Add(user);
                _context.SaveChanges();

                ModelState.Clear();
                ViewBag.Message = $"{user.Username} successfully registered.";
            }

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var usr = _context.User.Single(u => u.Username == user.Username && u.Password == user.Password);

            if (usr != null)
            {
                HttpContext.Session.SetString("UserId", usr.UserId.ToString());
                HttpContext.Session.SetString("Username", usr.Username.ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Username or password is wrong.");
            }

            return View();
        }

        public IActionResult LoggedIn()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}