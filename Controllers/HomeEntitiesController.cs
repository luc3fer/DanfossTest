using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DanfossTest.DAL;
using DanfossTest.Models;

namespace DanfossTest.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class HomeEntitiesController : Controller
    {
        private readonly IRepository<HomeEntity> _repository;

        public HomeEntitiesController(IRepository<HomeEntity> repository)
        {
            _repository = repository;
        }

        #region CRUD

        // GET: api/HomeEntities
        [HttpGet]
        public async Task<IEnumerable<HomeEntity>> GetHomeEntitysAsync()
        {
            var test = await _repository.GetItemsAsync(a => a.WaterMeter);
            return test;

            //return await _repository.GetItemsAsync(a => a.WaterMeter);
        }

        // GET: api/HomeEntities/5
        [HttpGet("{id}")]
        public IActionResult GetHomeEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var homeEntity = _repository.GetItem(s => s.Id == id);

            if (homeEntity == null)
            {
                return NotFound();
            }

            return Ok(homeEntity);
        }

        // PUT: api/HomeEntities/5
        [HttpPut("{id}")]
        public IActionResult PutHomeEntity([FromRoute] int id, [FromBody] HomeEntity homeEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != homeEntity.Id)
            {
                return BadRequest();
            }

            if (_repository.GetItem(s => string.Equals(s.Address, homeEntity.Address) && s.Id != homeEntity.Id) != null)
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Content = "Duplicate address"
                };

            _repository.SaveChange(homeEntity);
            return NoContent();
        }

        // POST: api/HomeEntities
        [HttpPost]
        public IActionResult PostHomeEntity([FromBody] HomeEntity homeEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_repository.GetItem(s => string.Equals(s.Address, homeEntity.Address) && s.Id != homeEntity.Id) != null)
                return new ContentResult()
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Content = "Duplicate address"
                };

            _repository.SaveChange(homeEntity);
            return CreatedAtAction("GetHomeEntity", new { id = homeEntity.Id }, homeEntity);
        }

        // DELETE: api/HomeEntities/5
        [HttpDelete("{id}")]
        public IActionResult DeleteHomeEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var homeEntity = _repository.GetItem(s => s.Id == id);
            if (homeEntity == null)
            {
                return NotFound();
            }

            _repository.DeleteItem(id);
            return Ok(homeEntity);
        }

        #endregion CRUD

        #region custome

        // GET: api/HomeEntities/max
        [HttpGet("max")]
        public async Task<IActionResult> GetMaxHomeAsunc()
        {
            var maxItem = await _repository.GetMaxItemAsync();
            if (maxItem != null)
                return Ok(maxItem);
            else
                return BadRequest();
        }

        // GET: api/HomeEntities/min
        [HttpGet("min")]
        public async Task<IActionResult> GetMinHomeAsunc()
        {
            var minItem = await _repository.GetMinItemAsync();
            if (minItem != null)
                return Ok(minItem);
            else
                return BadRequest();
        }

        #endregion custome
    }
}