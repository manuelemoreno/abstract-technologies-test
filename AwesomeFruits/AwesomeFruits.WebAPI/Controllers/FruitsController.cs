using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.WebAPI.Exceptions;
using AwesomeFruits.WebAPI.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeFruits.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class FruitsController : ControllerBase
{
    private readonly IFruitService _fruitService;

    public FruitsController(IFruitService fruitService)
    {
        _fruitService = fruitService;
    }

    // GET: api/Fruits
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _fruitService.FindAllFruitsAsync();

        return Ok(result);
    }

    // GET: api/Fruits/{id}
    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _fruitService.FindFruitByIdAsync(id);

        return Ok(result);
    }

    // POST: api/Fruits
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SaveFruitDto request)
    {
        ValidateSaveFruitDtoRequest(request);

        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userNameId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        request.CreatedByUserId = userNameId;

        return Created("", await _fruitService.SaveFruitAsync(request));
    }

    // PUT: api/Fruits
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateFruitDto request)
    {
        ValidateUpdateFruitDtoRequest(request);

        await _fruitService.UpdateFruitAsync(request);

        return NoContent();
    }

    // DELETE: api/Fruits
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _fruitService.DeleteFruitAsync(id);

        return NoContent();
    }

    private static void ValidateSaveFruitDtoRequest(SaveFruitDto request)
    {
        var errors = new SaveFruitDtoValidator().ValidateSaveFruitDto(request);

        if (errors.Count > 0) throw new ValidationErrorsException(errors);
    }

    private static void ValidateUpdateFruitDtoRequest(UpdateFruitDto request)
    {
        var errors = new UpdateFruitDtoValidator().ValidateUpdateFruitDto(request);

        if (errors.Count > 0) throw new ValidationErrorsException(errors);
    }
}