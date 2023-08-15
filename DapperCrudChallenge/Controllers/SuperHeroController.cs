using Dapper;
using DapperCrudChallenge.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DapperCrudChallenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection _connection;
        public SuperHeroController(IConfiguration config, IDbConnection connection)
        {
            _config = config;
            _connection = connection;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAll() 
        {
            var heroes = await _connection.QueryAsync<SuperHero>("select * from superheroes");
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> GetAll(int heroId)
        {
            var hero = await _connection.QueryFirstAsync<SuperHero>(
                "select * from superheroes where Id= @Id", 
                new { Id = heroId });

            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            var added = await _connection.ExecuteAsync(
                "insert into superheroes (name, firstname, lastname, place) values (@Name, @FirstName, @LastName, @Place)",
                hero);
            if (added <= 0) {
                return BadRequest("Insert Error.");
            }
            return Ok(await SelectAllHeroes(_connection));
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            var updated = await _connection.ExecuteAsync(
                "update superheroes set name = @Name, firstname = @FirstName, lastname = @LastName, place = @Place where Id = @Id",
                hero);
            if (updated <= 0)
            {
                return BadRequest("Update Error.");
            }
            return Ok(await SelectAllHeroes(_connection));
        }


        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeteleHero(int heroId)
        {
            var deleted = await _connection.ExecuteAsync(
                "Delete superheroes where Id = @Id",
                new { Id = heroId });
            if (deleted <= 0)
            {
                return BadRequest("Delete Error.");
            }
            return Ok(await SelectAllHeroes(_connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(IDbConnection con) {
            return await con.QueryAsync<SuperHero>("Select * from superheroes");
        }
    }
}
