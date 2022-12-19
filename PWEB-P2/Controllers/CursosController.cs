using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEB_P2.Data;
using PWEB_P2.Helpers;
using PWEB_P2.Models;
using PWEB_P2.ViewModels;

namespace PWEB_P2.Controllers
{
    public class CursosController : Controller
    {
        private readonly string _cursosPath = "wwwroot/ficheiros/Cursos/";
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index(bool? disponivel)
        {
            ViewData["ListaDeCategorias"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            if (disponivel == null)
            {
                ViewData["Title"] = "Lista de Todos os Cursos";
                return View(await _context.Cursos.Include("Categoria").ToListAsync());
            }

            if (disponivel == true)
            {
                ViewData["Title"] = "Lista de Cursos Activos";
            }
            else
            {
                ViewData["Title"] = "Lista de Cursos Inactivos";
            }

            return View(await _context.Cursos.Include("Categoria")
                .Where(c => c.Disponivel == disponivel).ToListAsync()
            );
        }

        [HttpPost]
        // POST: Cursos
        public async Task<IActionResult> Index(string TextoAPesquisar, int? CategoriaId)
        {
            ViewData["Title"] = "Lista de Cursos com '" + TextoAPesquisar + "'";
            ViewData["ListaDeCategorias"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            if (CategoriaId == null)
            {
                return View(await _context.Cursos.Include("Categoria")
                    .Where(c => c.Nome.Contains(TextoAPesquisar) || c.Descricao.Contains(TextoAPesquisar)).ToListAsync());
            }

            return View(await _context.Cursos.Include("Categoria")
                .Where(c => (c.Nome.Contains(TextoAPesquisar) || c.Descricao.Contains(TextoAPesquisar)) && c.CategoriaId == CategoriaId).ToListAsync());
        }

        // GET: Cursos/Search
        public async Task<IActionResult> Search(string? TextoAPesquisar)
        {
            ViewData["Title"] = "Lista de Cursos com '" + TextoAPesquisar + "'";

            var pesquisaVM = new PesquisaCursoViewModel();
            pesquisaVM.TextoAPesquisar = TextoAPesquisar;

            if (string.IsNullOrWhiteSpace(TextoAPesquisar))
            {
                pesquisaVM.ListaDeCursos = await _context.Cursos.ToListAsync();
                pesquisaVM.NumResultados = pesquisaVM.ListaDeCursos.Count;
                return View(pesquisaVM);
            }

            pesquisaVM.ListaDeCursos = await _context.Cursos.Where(c => c.Nome.Contains(TextoAPesquisar) || c.Descricao.Contains(TextoAPesquisar)).ToListAsync();
            pesquisaVM.NumResultados = pesquisaVM.ListaDeCursos.Count;

            return View(pesquisaVM);
        }

        // POST: Cursos/Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("TextoAPesquisar")] PesquisaCursoViewModel pesquisaCurso)
        {
            ViewData["Title"] = "Lista de Cursos com '" + pesquisaCurso.TextoAPesquisar + "'";

            if (string.IsNullOrWhiteSpace(pesquisaCurso.TextoAPesquisar))
            {
                pesquisaCurso.ListaDeCursos = await _context.Cursos.ToListAsync();
                pesquisaCurso.NumResultados = pesquisaCurso.ListaDeCursos.Count;
                return View(pesquisaCurso);
            }

            pesquisaCurso.ListaDeCursos = await _context.Cursos.Where(c => c.Nome.Contains(pesquisaCurso.TextoAPesquisar) || c.Descricao.Contains(pesquisaCurso.TextoAPesquisar)).ToListAsync();

            pesquisaCurso.NumResultados = pesquisaCurso.ListaDeCursos.Count;

            return View(pesquisaCurso);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.Include("Categoria")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            var coursePath = Path.Combine(Directory.GetCurrentDirectory(), _cursosPath + id.ToString());

            var files = new List<string>();

            if (Directory.Exists(coursePath))
                files = (from file in Directory.EnumerateFiles(coursePath) select $"{_cursosPath[7..]}{id}/{Path.GetFileName(file)}").ToList();

            ViewData["Ficheiros"] = files;

            return View(curso);
        }

        // GET: Cursos/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["ListaDeCategorias"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");
            return View();
        }

        // POST: Cursos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Nome,Disponivel,Descricao,DescricaoResumida,Requisitos,IdadeMinima,Preco,CategoriaId")] Curso curso)
        {
            ModelState.Remove(nameof(curso.Categoria));

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(curso);
        }

        // GET: Cursos/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            ViewData["ListaDeCategorias"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            var coursePath = Path.Combine(Directory.GetCurrentDirectory(), _cursosPath + id.ToString());

            var files = new List<string>();

            if (Directory.Exists(coursePath))
                files = (from file in Directory.EnumerateFiles(coursePath) select $"{_cursosPath[7..]}{id}/{Path.GetFileName(file)}").ToList();

            ViewData["Ficheiros"] = files;

            return View(curso);
        }

        public async Task<IActionResult> DeleteImage(int? id, string? image)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/{image}");

            try
            {
                System.IO.File.Delete(filePath);
            }
            catch
            {
                return NotFound();
            }

            return RedirectToAction("Edit", new { Id = id });
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Disponivel,Descricao,DescricaoResumida,Requisitos,IdadeMinima,Preco,CategoriaId")] Curso curso, [FromForm] List<IFormFile> ficheiros)
        {
            if (id != curso.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(curso.Categoria));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();

                    var path = Path.Combine(Directory.GetCurrentDirectory(), _cursosPath);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var coursePath = Path.Combine(Directory.GetCurrentDirectory(), $"{_cursosPath}{id}");
                    if (!Directory.Exists(coursePath))
                        Directory.CreateDirectory(coursePath);

                    foreach (var formFile in ficheiros)
                    {
                        if (formFile.Length <= 0) continue;
                        
                        var filePath = Path.Combine(coursePath, $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}");
                        while (System.IO.File.Exists(filePath))
                        {
                            filePath = Path.Combine(coursePath, $"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}");
                        }

                        await using var stream = System.IO.File.Create(filePath);
                        await formFile.CopyToAsync(stream);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id))
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
            return RedirectToAction(nameof(Edit), new { Id = id });
        }

        // GET: Cursos/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        public async Task<IActionResult> Comprar(int? id)
        {
            if (id == null || _context.Cursos == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .Include("Categoria")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            var carrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();
            carrinhoDeCompras.AddItem(curso, 1);
            HttpContext.Session.SetJson("CarrinhoDeCompras", carrinhoDeCompras);

            return RedirectToAction(nameof(Carrinho));
        }

        public async Task<IActionResult> Carrinho()
        {
            var carrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();
            return View(carrinhoDeCompras);
        }

        public async Task<IActionResult> AlterarQuantidadeCarrinhoItem(int cursoId, int quantidade)
        {
            var carrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();

            var item = carrinhoDeCompras.items.First(i => i.CursoId == cursoId);
            if (item == null)
            {
                return NotFound();
            }

            item.Quantidade += quantidade;
            if (item.Quantidade <= 0)
            {
                return await RemoverCarrinhoItem(cursoId);
            }

            HttpContext.Session.SetJson("CarrinhoDeCompras", carrinhoDeCompras);
            return RedirectToAction(nameof(Carrinho));
        }

        public async Task<IActionResult> RemoverCarrinhoItem(int cursoId)
        {
            var carrinhoDeCompras = HttpContext.Session.GetJson<Carrinho>("CarrinhoDeCompras") ?? new Carrinho();

            carrinhoDeCompras.RemoveItem(cursoId);

            HttpContext.Session.SetJson("CarrinhoDeCompras", carrinhoDeCompras);
            return RedirectToAction(nameof(Carrinho));
        }

        // GET: Cursos/GraficoVendas
        public async Task<IActionResult> GraficoVendas()
        {
            return View();
        }

        [HttpPost]
        // POST: Cursos/GraficoVendas
        public async Task<IActionResult> GetDadosVendas()
        {
            //dados de exemplo
            List<object> dados = new List<object>();
            DataTable dt = new DataTable();
            dt.Columns.Add("Cursos", System.Type.GetType("System.String"));
            dt.Columns.Add("Quantidade", System.Type.GetType("System.Int32"));
            DataRow dr = dt.NewRow();
            dr["Cursos"] = "CATEGORIA AM (Ciclomotor)";
            dr["Quantidade"] = 12;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Cursos"] = "CATEGORIA A1 (Motociclo - 11kw/125cc)";
            dr["Quantidade"] = 96;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Cursos"] = "CATEGORIA A2\r\n(Motociclo - 35kw)";
            dr["Quantidade"] = 87;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Cursos"] = "CATEGORIA B1\r\n(Quadriciclo)";
            dr["Quantidade"] = 67;
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr["Cursos"] = "CATEGORIA B\r\n(Ligeiro Caixa Automática)";
            dr["Quantidade"] = 63;
            dt.Rows.Add(dr);

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                dados.Add(x);
            }

            return Json(dados);
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cursos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Cursos'  is null.");
            }
            var curso = await _context.Cursos.FindAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
          return _context.Cursos.Any(e => e.Id == id);
        }
    }
}
