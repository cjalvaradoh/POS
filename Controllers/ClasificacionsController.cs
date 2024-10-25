using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
    public class ClasificacionsController : Controller
    {
        private readonly AppDbContext _context;

        public ClasificacionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Clasificacions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clasificacion.ToListAsync());
        }

        // GET: Clasificacions/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = await _context.Clasificacion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }

        // GET: Clasificacions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clasificacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Clasificacion clasificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clasificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clasificacion);
        }

        // GET: Clasificacions/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = await _context.Clasificacion.FindAsync(id);
            if (clasificacion == null)
            {
                return NotFound();
            }
            return View(clasificacion);
        }

        // POST: Clasificacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Nombre")] Clasificacion clasificacion)
        {
            if (id != clasificacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clasificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClasificacionExists(clasificacion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(clasificacion);
        }

        // GET: Clasificacions/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = await _context.Clasificacion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }

        // POST: Clasificacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var clasificacion = await _context.Clasificacion.FindAsync(id);
            if (clasificacion != null)
            {
                _context.Clasificacion.Remove(clasificacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClasificacionExists(long? id)
        {
            return _context.Clasificacion.Any(e => e.Id == id);
        }
    }
}
