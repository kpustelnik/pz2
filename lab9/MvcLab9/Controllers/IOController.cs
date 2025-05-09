using Microsoft.AspNetCore.Mvc;
namespace MvcMovie.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using MvcLab9.ApplicationDbContext;
using MvcLab9.Models;
using MvcLab9.ViewModels;
using System;
using System.Security.Cryptography;
using System.Text;

public class IOController : Controller
{
    private readonly AppDbContext _context;
    public IOController(AppDbContext context) {
        _context = context;
    }
    public IActionResult Logowanie() {
        if (HttpContext.Session.Keys.Contains("user_id")) return this.RedirectToAction("Panel", "IO");
        return View();
    }

    [HttpPost] 
    public async Task<IActionResult> Logowanie(IFormCollection form) {
        string login = form["login"].ToString();
        string password = form["password"].ToString();

        using MD5 hash = MD5.Create();
        byte[] pwdhash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

        StringBuilder clrHash = new StringBuilder();
        foreach (byte b in pwdhash) clrHash.Append(b.ToString("x2"));
        string clearHash = clrHash.ToString();

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.login == login && u.password == clearHash);
        if (user != null) {
            HttpContext.Session.SetInt32("user_id", user.Id);
            ViewData["error"] = "Zalogowano";
            return this.RedirectToAction("Panel", "IO");
        }
        ViewData["error"] = "Użytkownik nie znaleziony";
        return View();
    }

    public async Task<IActionResult> Panel() {
        if (!HttpContext.Session.Keys.Contains("user_id")) return this.RedirectToAction("Logowanie", "IO");

        Int32? userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return this.RedirectToAction("Logowanie", "IO");

        User? user = await _context.Users.FindAsync(userId);
        if (user == null) return this.RedirectToAction("Logowanie", "IO");

        ViewData["login"] = user.login;
        List<Data> dataList = _context.Data.Where(d => d.user.Id == userId).ToList();

        DataViewModel dvm = new DataViewModel(dataList);

        return View("Panel", dvm);
    }

    [HttpPost]
    public IActionResult Wylogowanie(IFormCollection form) {
        HttpContext.Session.Remove("user_id");
        return this.RedirectToAction("Logowanie", "IO");
    }

    [HttpPost]
    public async Task<IActionResult> DodajDane(IFormCollection form) {
        if (!HttpContext.Session.Keys.Contains("user_id")) return this.RedirectToAction("Logowanie", "IO");

        Int32? userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return this.RedirectToAction("Logowanie", "IO");

        User? user = await _context.Users.FindAsync(userId);
        if (user == null) return this.RedirectToAction("Logowanie", "IO");

        string data = form["data"].ToString();
        _context.Data.Add(new Data{ user = user, data = data });
        await _context.SaveChangesAsync();

        return await Panel();
    }

    [HttpPost]
    public async Task<IActionResult> UsunDane(IFormCollection form) {
        if (!HttpContext.Session.Keys.Contains("user_id")) return this.RedirectToAction("Logowanie", "IO");

        Int32? userId = HttpContext.Session.GetInt32("user_id");
        if (userId == null) return this.RedirectToAction("Logowanie", "IO");

        User? user = await _context.Users.FindAsync(userId);
        if (user == null) return this.RedirectToAction("Logowanie", "IO");

        string sid = form["id"].ToString();
        Int32? id = Int32.Parse(sid);
        Data? cdata = await _context.Data.FindAsync(id);
        if (cdata == null) ViewData["error"] = "Nie znaleziono danej";
        else {
            _context.Data.Remove(cdata);
            await _context.SaveChangesAsync();
        }
        return await Panel();
    }
}