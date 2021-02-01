using Dapper;
using DevEvents.API.Entidades;
using DevEvents.API.Persistencia;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevEvents.API.Controllers
{
    [Route("api/eventos")]
    public class EventosController : ControllerBase
    {
        private readonly DevEventsDbContext _dbContext;
        public EventosController(DevEventsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult ObterEventos()
        {
            var eventos = _dbContext.Eventos.ToList();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public IActionResult ObterEvento(int id)
        {
            var evento = _dbContext.Eventos
                .Include(e => e.Categoria)
                .Include(e => e.Usuario)
                .Include(e => e.Inscricoes)
                .SingleOrDefault(e => e.Id == id);
            if(evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Evento evento)
        {
            _dbContext.Eventos.Add(evento);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPost("idEvento/usuarios/{idUsuario}/inscrever")]
        public IActionResult inscrever(int idEvento, int idUsuario)
        {

            var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == idEvento);

            if (!evento.Ativo)
            {
                return BadRequest();
            }

            Inscricao inscricao = new Inscricao();
            inscricao.IdEvento = idEvento;
            inscricao.IdUsuario = idUsuario;
            _dbContext.Inscricoes.Add(inscricao);
            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Evento evento)
        {
            _dbContext.Eventos.Update(evento);

            _dbContext.Entry(evento).Property(e => e.DataCadastro).IsModified = false;
            _dbContext.Entry(evento).Property(e => e.IdUsuario).IsModified = false;
            _dbContext.Entry(evento).Property(e => e.Ativo).IsModified = false;

            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Cancelar(int id)
        {
            //usando dapper
            var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                var script = "UPDATE Eventos set Ativo = 0 where Id = @id";

                sqlConnection.Execute(script, new { id = id });
            }

                /*
                //usando entity
                var evento = _dbContext.Eventos.SingleOrDefault(e => e.Id == id);

                if(evento == null)
                {
                    return NotFound();
                }

                evento.Ativo = false;

                _dbContext.SaveChanges();

                */
                return NoContent();
        }

        [HttpPost("popular")]
        public IActionResult Popular()
        {
            var usuario = new Usuario
            {
                NomeCompleto = "Luth Sanches",
                Email = "luth@gmail.com",
                Ativo = true,
                DataCadastro = new DateTime(),
                DataNascimento = new DateTime(2012, 01, 02)
            };

            var categorias = new List<Categoria>
            {
                new Categoria() {Descricao = "C#"},
                new Categoria() {Descricao = "Flutter"},
                new Categoria() {Descricao = "Xamarin"}
            };

            _dbContext.Usuarios.Add(usuario);
            _dbContext.Categorias.AddRange(categorias);

            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
