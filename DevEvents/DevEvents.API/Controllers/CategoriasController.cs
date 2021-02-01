using DevEvents.API.Entidades;
using DevEvents.API.Persistencia;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEvents.API.Controllers
{
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly DevEventsDbContext _dbContext;

        public CategoriasController(DevEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ObterTodas()
        {
            var categorias = _dbContext.Categorias.ToList();
            return Ok(categorias);
        }
    }
}
