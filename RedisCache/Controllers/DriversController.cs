using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisCache.Data;
using RedisCache.Models;

namespace RedisCache.Controllers
{
    public class DriversController : Controller
    {
        private readonly ILogger<DriversController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _appDbContext;
        public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDbContext appDbContext)
        {
            _logger = logger;
            _cacheService = cacheService;
            _appDbContext = appDbContext;
        }

        [HttpGet("drivers")]
        public async Task<IActionResult> Get() 
        {
            var cacheData =  _cacheService.GetData<IEnumerable<Driver>>("drivers");
            if (cacheData != null && cacheData.Count() > 0)
                return Ok(cacheData);

            cacheData = await _appDbContext.Drivers.ToListAsync();
            _cacheService.SetData<IEnumerable<Driver>>("drivers", cacheData, setExpiryTime());

            return Ok(cacheData);
        }

        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post(Driver value) 
        {
            var addObj = await _appDbContext.Drivers.AddAsync(new Driver
            {
                Name = value.Name,
                DriverNumber = value.DriverNumber
            });
            _cacheService.SetData<Driver>($"divers{addObj.Entity.Id}", addObj.Entity, setExpiryTime());
            await _appDbContext.SaveChangesAsync();

            return Ok(addObj.Entity);
        }


        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int id) 
        {
            var exist = _appDbContext.Drivers.FirstOrDefaultAsync(z=>z.Id ==id);

            if(exist != null) 
            {
                _appDbContext.Remove(exist);
                _cacheService.RemoveData($"driver{id}");
                await _appDbContext.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }
        private DateTimeOffset setExpiryTime() 
        {
            return DateTimeOffset.Now.AddSeconds(30);
        }
    }
}
