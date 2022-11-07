﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PWEB_P2.Data;
using PWEB_P2.Models;
using PWEB_P2.ViewModels;

namespace PWEB_P2.Controllers
{
    public class CursosController : Controller
    {
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

            return View(curso);
        }

        // GET: Cursos/Create
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

            return View(curso);
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Disponivel,Descricao,DescricaoResumida,Requisitos,IdadeMinima,Preco,CategoriaId")] Curso curso)
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
            return View(curso);
        }

        // GET: Cursos/Delete/5
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

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
