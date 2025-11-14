using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicketBeasts.Data;
using TicketBeasts.Models;

namespace TicketBeasts.Controllers
{
    public class SportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BlobServiceClient _blob;
        private readonly IConfiguration _configuration;

        public SportsController(AppDbContext context, BlobServiceClient blob, IConfiguration configuration)
        {
            _context = context;
            _blob = blob;
            _configuration = configuration;
        }

        // GET: Sports
        public async Task<IActionResult> Index(string? search)
        {
            var q = _context.Sports
                            .Include(s => s.Category)
                            .Include(s => s.Owner)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                q = q.Where(s => s.Title.Contains(search) || s.Location.Contains(search));
            }

            ViewData["Search"] = search;
            return View(await q.ToListAsync());
        }

        // GET: Sports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sport = await _context.Sports
                .Include(s => s.Category)
                .Include(s => s.Owner)
                .Include(s => s.Purchases)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sport == null) return NotFound();

            return View(sport);
        }


        // GET: Sports/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name");
            return View();
        }

        // POST: Sports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,EventDateTime,Location,CategoryId,OwnerId,CreatedAt,ImagePath")] Sport sport, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    sport.ImagePath = await UploadToBlobAsync(imageFile);
                }

                _context.Add(sport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }

        // GET: Sports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sport = await _context.Sports.FindAsync(id);
            if (sport == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }

        // POST: Sports/Edit/5
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
                        await TryDeleteOldBlobAsync(sport.ImagePath);
                        sport.ImagePath = await UploadToBlobAsync(imageFile);
                    }

                    _context.Update(sport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(sport.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", sport.CategoryId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Name", sport.OwnerId);
            return View(sport);
        }

        // GET: Sports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sport = await _context.Sports
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sport == null) return NotFound();
            return View(sport);
        }

        // POST: Sports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sport = await _context.Sports.FindAsync(id);
            if (sport != null)
            {
                // optional: delete blob for this record too
                await TryDeleteOldBlobAsync(sport.ImagePath);
                _context.Sports.Remove(sport);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id) => _context.Sports.Any(e => e.Id == id);

        // ---------- helpers (INSIDE the controller) ----------

        private async Task<string> UploadToBlobAsync(IFormFile imageFile)
        {
            var containerName = _configuration["Blob:Container"] ?? "uploads";
            var container = _blob.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var ext = Path.GetExtension(imageFile.FileName);
            var blobName = $"{Guid.NewGuid()}{ext}";
            var blob = container.GetBlobClient(blobName);

            var headers = new BlobHttpHeaders { ContentType = imageFile.ContentType };
            await blob.UploadAsync(imageFile.OpenReadStream(), new BlobUploadOptions { HttpHeaders = headers });

            return blob.Uri.ToString(); // full https URL
        }

        private async Task TryDeleteOldBlobAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return;

            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)) return;

            var containerName = _configuration["Blob:Container"] ?? "uploads";
            if (!uri.Host.EndsWith(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase)) return;

            var segments = uri.AbsolutePath.Trim('/').Split('/', 2); // [container, blobName]
            if (segments.Length != 2) return;
            if (!string.Equals(segments[0], containerName, StringComparison.OrdinalIgnoreCase)) return;

            var container = _blob.GetBlobContainerClient(containerName);
            var blob = container.GetBlobClient(segments[1]);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
