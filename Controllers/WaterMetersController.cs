using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DanfossTest.DAL;
using DanfossTest.Models;
using System.Linq.Expressions;

namespace DanfossTest.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WaterMetersController : Controller
    {
        private readonly IRepository<WaterMeter> _repository;

        public WaterMetersController(IRepository<WaterMeter> repository) => _repository = repository;

        #region CRUD

        // GET: api/WaterMeters
        [HttpGet]
        public IEnumerable<WaterMeter> GetWaterMeters() => _repository.GetItems();

        // GET: api/WaterMeters/5
        [HttpGet("{id}")]
        public IActionResult GetWaterMeter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterMeter = _repository.GetItem(s => s.Id == id);
            if (waterMeter == null)
            {
                return NotFound();
            }

            return Ok(waterMeter);
        }

        // PUT: api/WaterMeters/5
        [HttpPut("{id}")]
        public IActionResult PutWaterMeter([FromRoute] int id, [FromBody] WaterMeter waterMeter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != waterMeter.Id)
            {
                return BadRequest();
            }

            _repository.SaveChange(waterMeter);
            return NoContent();
        }

        // POST: api/WaterMeters
        [HttpPost]
        public IActionResult PostWaterMeter([FromBody] WaterMeter waterMeter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.SaveChange(waterMeter);
            return CreatedAtAction("GetWaterMeter", new { id = waterMeter.Id }, waterMeter);
        }

        // DELETE: api/WaterMeters/5
        [HttpDelete("{id}")]
        public IActionResult DeleteWaterMeter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterMeter = _repository.GetItem(s => s.Id == id);
            if (waterMeter == null)
            {
                return NotFound();
            }

            _repository.LoadRelatedEntiti(waterMeter);
            _repository.DeleteItem(id);

            return Ok(waterMeter.HomeEntity);
        }

        #endregion CRUD

        #region custom

        // POST: api/WaterMeters/reg
        [HttpPost("reg")]
        public IActionResult RegWaterMeter([FromBody] WaterMeter waterMeter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterMeter.Meter = 0;
            _repository.SaveChange(waterMeter);

            _repository.LoadRelatedEntiti(waterMeter);
            return Ok(waterMeter.HomeEntity);
        }

        // PUT: api/WaterMeters/SubmitMeter/by_home_id?home_id=5&meter=7
        [HttpPut("[action]/by_home_id")]
        public IActionResult SubmitMeter([FromQuery(Name = "meter")] int meter, [FromQuery(Name = "home_id")] int homeEntityId)
        {
            if (homeEntityId == 0 || meter == 0)
            {
                return BadRequest();
            }

            return Ok(SubmitMeter(meter, s => s.HomeEntityId == homeEntityId).HomeEntity);
        }

        // PUT: api/WaterMeters/SubmitMeter/by_serial_number?serial_number='asd'&meter=7
        [HttpPut("[action]/by_serial_number")]
        public IActionResult SubmitMeter([FromQuery(Name = "meter")] int meter, [FromQuery(Name = "serial_number")] string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber) || meter == 0)
            {
                return BadRequest();
            }

            return Ok(SubmitMeter(meter, s => string.Equals(serialNumber, s.SerialNumber)));
        }

        #endregion custom

        private WaterMeter SubmitMeter(int meter, Expression<Func<WaterMeter, bool>> predicate)
        {
            var waterMeters = _repository.GetItem(predicate);
            if (waterMeters == null)
            {
                return null;
            }

            waterMeters.Meter = meter;
            _repository.SaveChange(waterMeters);
            _repository.LoadRelatedEntiti(waterMeters);

            return waterMeters;
        }
    }
}