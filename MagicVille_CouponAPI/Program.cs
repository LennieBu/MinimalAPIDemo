using AutoMapper;
using FluentValidation;
using MagicVille_CouponAPI;
using MagicVille_CouponAPI.Data;
using MagicVille_CouponAPI.Models;
using MagicVille_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//We use AutoMapper to easily and cleanly map object models to DTO's and other simple objects.
builder.Services.AddAutoMapper(typeof(MappingConfig));
//Minimal API doesn't have built in validation so you can either write your own validation or use a package like FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region API Endpoints

//Get all coupons
app.MapGet("/api/coupon", (ILogger<Program> _logger) => 
{
    APIResponse response = new APIResponse();

    _logger.Log(LogLevel.Information, "Retrieving all coupons...");

    response.Result = CouponStore.CouponList;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

   return Results.Ok(response);
}).WithName("GetAllCoupons").Produces<APIResponse>(200);

//Get coupon by ID
app.MapGet("/api/coupon/{id:int}", (int id, ILogger<Program> _logger) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest};

    if (!CouponStore.CouponList.Any(x => x.Id == id))
    {
        _logger.Log(LogLevel.Error, "Requested coupon ID out of range.");
        return Results.BadRequest(response);
    }

    response.Result = CouponStore.CouponList.First(x => x.Id == id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    _logger.Log(LogLevel.Information, $"Retrieving coupon with ID: {id}...");
    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200).Produces(400);

//Create coupon
app.MapPost("/api/coupon", async (IValidator<CouponCreateDTO> _validation, IMapper _mapper, ILogger<Program> _logger, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        _logger.Log(LogLevel.Error, validationResult.Errors.First().ToString());
        response.ErrorMessages.Add(validationResult.Errors.First().ToString());
        return Results.BadRequest(response);
    }

    if(CouponStore.CouponList.Any(y => y.Name.Equals(coupon_C_DTO.Name, StringComparison.InvariantCultureIgnoreCase)))
    {
        _logger.Log(LogLevel.Error, "Name is not unique.");
        response.ErrorMessages.Add("Name is not unique.");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    coupon.Id = CouponStore.CouponList.OrderByDescending(x => x.Id).First().Id + 1;
    _logger.Log(LogLevel.Information, $"Creating coupon: {coupon.Name}, with Id: {coupon.Id}");
    CouponStore.CouponList.Add(coupon);

    CouponDTO co = _mapper.Map<CouponDTO>(coupon);

    response.Result = co;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

//Update existing coupon
app.MapPut("/api/coupon", async (IValidator<CouponUpdateDTO> _validation, IMapper _mapper, ILogger<Program> _logger, [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        _logger.Log(LogLevel.Error, validationResult.Errors.First().ToString());
        response.ErrorMessages.Add(validationResult.Errors.First().ToString());
        return Results.BadRequest(response);
    }

    _logger.Log(LogLevel.Information, "Updating coupon.");
    Coupon oldCouponData = CouponStore.CouponList.First(x => x.Id == coupon_U_DTO.Id);
    Coupon newCouponData = _mapper.Map<Coupon>(coupon_U_DTO);
    oldCouponData = newCouponData;

    CouponDTO co = _mapper.Map<CouponDTO>(oldCouponData);

    response.Result = co;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

//Delete coupon
app.MapDelete("/api/coupon/{id:int}", (int id, ILogger<Program> _logger) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest};

    if (!CouponStore.CouponList.Any(x => x.Id == id))
    {
        _logger.Log(LogLevel.Error, $"No coupon with Id {id}");
        response.ErrorMessages.Add($"No coupon with Id {id}");
        return Results.BadRequest(response);
    }

    _logger.Log(LogLevel.Information, "Deleting coupon...");
    CouponStore.CouponList.Remove(CouponStore.CouponList.First(x => x.Id == id));

    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    response.Result = "Coupon deleted succesfully.";

    return Results.Ok(response);
}).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400);

#endregion

app.UseHttpsRedirection();

app.Run();

