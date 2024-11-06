using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UspgPOS.Data;
using UspgPOS.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ExcelDataReader;

namespace UspgPOS.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ProductosController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Productos.Include(p => p.Clasificacion).Include(p => p.Marca);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .Include(p => p.Clasificacion)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["ClasificacionId"] = new SelectList(_context.Clasificacion, "Id", "Nombre");
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,MarcaId,ClasificacionId,Precio,Cantidad")] Productos producto, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    producto.img_url = uploadResult.SecureUrl.ToString();

                    var thumbnailParams = new Transformation().Width(150).Height(150).Crop("thumb");
                    producto.thumbnail_url = _cloudinary.Api.UrlImgUp.Transform(thumbnailParams).BuildUrl(uploadResult.PublicId);
                }
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClasificacionId"] = new SelectList(_context.Set<Clasificacion>(), "Id", "Nombre", producto.ClasificacionId);
            ViewData["MarcaId"] = new SelectList(_context.Set<Marca>(), "Id", "Nombre", producto.MarcaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos.FindAsync(id);
            if (productos == null)
            {
                return NotFound();
            }
            ViewData["ClasificacionId"] = new SelectList(_context.Clasificacion, "Id", "Nombre", productos.ClasificacionId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", productos.MarcaId);
            return View(productos);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long? id, [Bind("Id,Nombre,MarcaId,ClasificacionId,Precio,Cantidad")] Productos productos)
        {
            if (id != productos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductosExists(productos.Id))
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
            ViewData["ClasificacionId"] = new SelectList(_context.Clasificacion, "Id", "Nombre", productos.ClasificacionId);
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", productos.MarcaId);
            return View(productos);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productos = await _context.Productos
                .Include(p => p.Clasificacion)
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productos == null)
            {
                return NotFound();
            }

            return View(productos);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long? id)
        {
            var productos = await _context.Productos.FindAsync(id);
            if (productos != null)
            {
                _context.Productos.Remove(productos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductosExists(long? id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        [HttpPost]
		public async Task<IActionResult> CargarArchivo(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("Por favor, seleccione un archivo de Excel");
			}

			if (!file.FileName.EndsWith(".xlsx"))
			{
				return BadRequest("Por favor, seleccione un archivo de Excel");
			}

			using (var stream = file.OpenReadStream())
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					var dataTable = reader.AsDataSet().Tables[0];

					for (int fila = 1; fila < dataTable.Rows.Count; fila++)
					{
						string codigo = dataTable.Rows[fila][0].ToString();
						string nombre = dataTable.Rows[fila][1].ToString();
						string nombreMarca = dataTable.Rows[fila][2].ToString();
						string nombreClasificacion = dataTable.Rows[fila][3].ToString();
						decimal precio = decimal.Parse(dataTable.Rows[fila][4].ToString());
						int cantidad = int.Parse(dataTable.Rows[fila][5].ToString());

						Productos? producto = await _context.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);

						Marca? marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Nombre == nombreMarca);

						Clasificacion? clasificacion = await _context.Clasificacion.FirstOrDefaultAsync(c => c.Nombre == nombreClasificacion);

						if (producto == null)
						{
							//Crear el producto
							Productos nuevoProducto = new Productos
							{
								Codigo = codigo,
								Nombre = nombre,
								Precio = precio,
								Cantidad = cantidad,
								MarcaId = marca!= null ? marca.Id : -1,
								ClasificacionId = clasificacion != null ?  clasificacion.Id : -1,
							};

							_context.Productos.Add(nuevoProducto);
						}
						else
						{
							//Editar el producto
							producto.Nombre = nombre;
							producto.MarcaId = marca != null ? marca.Id : -1;
							producto.ClasificacionId = clasificacion != null ? clasificacion.Id : -1;
							producto.Precio = precio;
							producto.Cantidad = cantidad;

							_context.Productos.Update(producto);
						}
					}
				}
			}

			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}
    }
}
