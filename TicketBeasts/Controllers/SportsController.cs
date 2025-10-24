using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketBeasts.Data;
using TicketBeasts.Models;
using Microsoft.AspNetCore.Hosting;  
using System.IO;

namespace TicketBeasts.Controllers
{
    public class SportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public SportsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Sports.Include(s => s.Category).Include(s => s.Owner);
            return View(await appDbContext.ToListAsync());
        }

      
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Sports
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,EventDateTime,Location,CategoryId,OwnerId,CreatedAt,ImagePath")] Sport sport, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsRoot);

                    var ext = Path.GetExtension(imageFile.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(uploadsRoot, fileName);

                    using var stream = System.IO.File.Create(fullPath);
                    await imageFile.CopyToAsync(stream);

                    sport.ImagePath = $"/uploads/{fileName}";
                }

                _context.Add(sport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sport = await _context.Sports.FindAsync(id);
            if (sport == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }


        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDateTime,Location,CategoryId,OwnerId,CreatedAt,ImagePath")] Sport sport, IFormFile? imageFile)
        {
            if (id != sport.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsRoot);

                        var ext = Path.GetExtension(imageFile.FileName);
                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var fullPath = Path.Combine(uploadsRoot, fileName);

                        using var stream = System.IO.File.Create(fullPath);
                        await imageFile.CopyToAsync(stream);

                        // replace old path with new
                        sport.ImagePath = $"/uploads/{fileName}";
                    }

                    _context.Update(sport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Sports.Any(e => e.Id == sport.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Sports
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Sports.FindAsync(id);
            if (@event != null)
            {
                _context.Sports.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Sports.Any(e => e.Id == id);
        }
    }
}
