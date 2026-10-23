using BankingApi.DTOs;
using BankingApi.Models;
using BankingApi.Security;
using BankingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<LoanProduct>>> GetAll(
        [FromQuery] string? category, [FromQuery] bool includeInactive = false)
    {
        // Only admins are allowed to see switched-off products
        bool showInactive = includeInactive && User.IsAdmin();
        return Ok(await _products.GetAllAsync(showInactive, category));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanProduct>> GetById(int id) =>
        Ok(await _products.GetByIdAsync(id));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(LoanProduct), StatusCodes.Status201Created)]
    public async Task<ActionResult<LoanProduct>> Add(ProductRequest request)
    {
        LoanProduct product = await _products.AddAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LoanProduct>> Update(int id, ProductRequest request) =>
        Ok(await _products.UpdateAsync(id, request));

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        string result = await _products.DeleteAsync(id);
        return Ok(new { productId = id, result });
    }
}
