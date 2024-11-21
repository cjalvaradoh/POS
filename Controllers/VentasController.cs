using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using UspgPOS.Data;
using UspgPOS.Models;

namespace UspgPOS.Controllers
{
	public class VentasController : Controller
	{
		private readonly AppDbContext _context;

		public VentasController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Ventas
		public async Task<IActionResult> Index()
		{
			var appDbContext = _context.Ventas.Include(v => v.Cliente).Include(v => v.Sucursal);
			return View(await appDbContext.ToListAsync());
		}

		// GET: Ventas/Details/5
		public async Task<IActionResult> Details(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (venta == null)
			{
				return NotFound();
			}

			return View(venta);
		}

		// GET: Ventas/Create
		public IActionResult Create()
		{
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre");
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", HttpContext.Session.GetString("SucursalId"));
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
		{
			if (ModelState.IsValid)
			{
				_context.Add(venta);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// GET: Ventas/Edit/5
		public async Task<IActionResult> Edit(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas.FindAsync(id);
			if (venta == null)
			{
				return NotFound();
			}
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// POST: Ventas/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(long? id, [Bind("Id,Fecha,Total,ClienteId,SucursalId")] Venta venta)
		{
			if (id != venta.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(venta);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!VentaExists(venta.Id))
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
			ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nombre", venta.ClienteId);
			ViewData["SucursalId"] = new SelectList(_context.Sucursales, "Id", "Nombre", venta.SucursalId);
			return View(venta);
		}

		// GET: Ventas/Delete/5
		public async Task<IActionResult> Delete(long? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var venta = await _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (venta == null)
			{
				return NotFound();
			}

			return View(venta);
		}

		// POST: Ventas/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(long? id)
		{
			var venta = await _context.Ventas.FindAsync(id);
			if (venta != null)
			{
				_context.Ventas.Remove(venta);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool VentaExists(long? id)
		{
			return _context.Ventas.Any(e => e.Id == id);
		}

		public IActionResult ImprimirFactura(long id)
		{
			QuestPDF.Settings.License = LicenseType.Community;

			var venta = _context.Ventas
				.Include(v => v.Cliente)
				.Include(v => v.Sucursal)
				.Include(v => v.Detalles)
					.ThenInclude(d => d.Producto)
				.FirstOrDefault(v => v.Id == id);

			if (venta == null)
			{
				return NotFound();
			}

			var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-180x180.png");
			var monedaGuatemala = new System.Globalization.CultureInfo("es-GT");

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Size(PageSizes.A4);
					page.Margin(2, Unit.Centimetre);

					page.Header().Column(header =>
					{
						// Tabla para la fecha
						header.Item().Table(table =>
						{
							table.ColumnsDefinition(columns =>
							{
								columns.ConstantColumn(60); // Primera columna para "Fecha"
								columns.ConstantColumn(40); // Día
								columns.ConstantColumn(40); // Mes
								columns.ConstantColumn(40); // Año
							});

							// Fila de la tabla
							table.Cell().Element(EstiloCelda).Text("FECHA").FontSize(10).Bold();
							table.Cell().Element(EstiloCeldaValorFecha).Text($"{venta.Fecha:dd}").FontSize(10);
							table.Cell().Element(EstiloCeldaValorFecha).Text($"{venta.Fecha:MM}").FontSize(10);
							table.Cell().Element(EstiloCeldaValorFecha).Text($"{venta.Fecha:yyyy}").FontSize(10);

							// Estilo de las celdas
							static IContainer EstiloCelda(IContainer container)
							{
								return container.Background("#e0e0e0")
												 .Border(1)
												 .BorderColor("#c0c0c0")
												 .Padding(5)
												 .AlignCenter()
												 .AlignMiddle();
							}

							static IContainer EstiloCeldaValorFecha(IContainer container)
							{
								return container.Background("#ffffff")
												 .Border(1)
												 .BorderColor("#c0c0c0")
												 .Padding(5)
												 .AlignCenter()
												 .AlignMiddle();
							}
						});


						// Espacio entre la fecha y el título (margen)
						header.Item().PaddingVertical(5, Unit.Millimetre);  // Aquí agregamos el margen
																			// Título y logo
						header.Item().Row(headerRow =>
						{
							// Título en el centro
							headerRow.RelativeItem().Text("FACTURA COMERCIAL").FontSize(40).Bold().AlignCenter();

							// Imagen a la derecha
							headerRow.ConstantItem(100).Image(logo, ImageScaling.FitArea);
						});
					});

					page.Content().Column(column =>
					{
						column.Item().PaddingVertical(3, Unit.Millimetre); 



						column.Item().Row(row =>
						{
							row.RelativeItem().Text($"NOMBRE: {venta.Cliente?.Nombre}");
						});

						column.Item().PaddingVertical(1, Unit.Millimetre);

						column.Item().Row(row =>
						{
							row.RelativeItem().Text($"NIT: {venta.Cliente?.Nit}");
							row.RelativeItem().Text($"SUCURSAL: {venta.Sucursal?.Nombre}");
						});

						column.Item().PaddingVertical(5, Unit.Millimetre);

						column.Item().Table(table =>
						{
							// Definición de columnas
							table.ColumnsDefinition(columns =>
							{
								columns.ConstantColumn(60);  // Cantidad
								columns.RelativeColumn();    // Descripción
								columns.ConstantColumn(100); // Unidad
								columns.ConstantColumn(100); // Total
							});

							// Encabezado de la tabla
							table.Header(header =>
							{
								header.Cell().Element(EstiloCelda).Text("Cantidad").FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Descripción").FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Unidad").FontSize(12).Bold();
								header.Cell().Element(EstiloCelda).Text("Total").FontSize(12).Bold();

								static IContainer EstiloCelda(IContainer container)
								{
									return container.Background("#e0e0e0").Border(1).BorderColor("#e0e0e0").Padding(5).AlignCenter();
								}
							});

							// Cuerpo de la tabla
							foreach (DetalleVenta detalle in venta.Detalles)
							{
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text(detalle.Cantidad.ToString());
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(detalle.Producto?.Nombre ?? "N/A");
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight().Text(detalle.PrecioUnitario.ToString("C", monedaGuatemala));
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight().Text((detalle.Cantidad * detalle.PrecioUnitario).ToString("C", monedaGuatemala));
							}

							// Filas vacías con separaciones
							for (int i = 0; i < 4; i++) // Número de filas vacías
							{
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(""); // Celda "Cantidad"
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(""); // Celda "Descripción"
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(""); // Celda "Unidad"
								table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(""); // Celda "Total"
							}
							// Separador visual: línea o espacio
							column.Item().PaddingVertical(4).Text(""); // Esto añade un espacio entre la tabla y el total.


							// Contenedor para "TOTAL" e información adicional
							column.Item().Row(row =>
							{
								row.RelativeItem(2).Row(innerRow =>
								{
									innerRow.RelativeItem(3).Text(text =>
									{
										text.Span("Dirección: Calle cualquiera 123").FontSize(10).FontColor("#000000");
									});
									innerRow.RelativeItem(3).AlignRight().Text(text =>
									{
										text.Span("Sitio Web: @sitioincreible").FontSize(10).FontColor("#000000");
									});
								});


								// Columna para la tabla "TOTAL"
								row.RelativeItem(3).AlignRight().Table(totalTable =>
								{
									totalTable.ColumnsDefinition(columns =>
									{
										columns.ConstantColumn(80);  // Columna fija para "TOTAL"
										columns.ConstantColumn(100); // Columna fija para el monto
									});

									// Celda del texto "TOTAL"
									totalTable.Cell().Background("#e0e0e0").Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter()
										.Text("TOTAL").FontSize(12).Bold().FontColor("#000000");

									// Celda del monto
									totalTable.Cell().Background("#ffffff").Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight()
										.Text(venta.Total.ToString("C", monedaGuatemala)).FontSize(12).FontColor("#000000");
								});

								column.Item().PaddingVertical(20).Text("");

								column.Item().Row(row =>
								{
									// Primer elemento alineado a la izquierda
									row.RelativeItem(4).AlignLeft().Text(text =>
									{
										text.Span("Correo Electronico: Hola@sitioincreible.com")
											.FontSize(10)
											.FontColor("#000000");
									});

									// Segundo elemento alineado al centro
									row.RelativeItem(3).AlignCenter().Text(text =>
									{
										text.Span("Teléfono: (55) 1234-5678")
											.FontSize(10)
											.FontColor("#000000");
									});

									// Tercer elemento alineado a la derecha con interlineado
									row.RelativeItem(4).AlignRight().Text(text =>
									{
										text.Span("FIRMA CLIENTE")
											.FontSize(10)
											.FontColor("#000000");
										
									});
								});

								column.Item().PaddingVertical(15).Text("");

								column.Item().Row(row =>
								{

									// Primer elemento alineado a la izquierda
									row.RelativeItem(4).AlignLeft().Text(text =>
									{
										text.Span("SERVICIO A DOMICILIO")
														.FontSize(20)
														.FontColor("#000000");
									});

								});

							});
						});

					});

					page.Footer().Text($"USPG POS - 2024 - {venta.Sucursal?.Nombre}").FontSize(10).AlignCenter();
				});
			});


			var stream = new MemoryStream();
			document.GeneratePdf(stream);
			stream.Position = 0;

			return File(stream, "application/pdf", $"Factura_{id}.pdf");
		}

	}
}
