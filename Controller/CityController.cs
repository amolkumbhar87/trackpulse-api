using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class CityController : ControllerBase
{

    IRaceRepositoryBase<City> _cityRepository;
    public CityController(IRaceRepositoryBase<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    [HttpGet("api/cities")]
    public async Task<IActionResult> GetAllCities()
    {
        var cities = await _cityRepository.GetAllAsync();
        return Ok(cities);
    }
    

}