using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using PersonProfile_DF.Utilities;
using PersonProfile_DF.Business.Data.Models;
using PersonProfile_DF.Business;
using PersonProfile_DF.Business.Contracts;
using PersonProfile_DF.Api;
using PersonProfile_DF.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using PersonProfile_DF.Api.Caching;
using PersonProfile_DF.Api.DataShaping;

namespace PersonProfile_DF.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ApiVersion("1.0")]
	public class PhoneController : AuthorizedController
	{
		private readonly IPhoneProcessor _phoneProcessor;
		private readonly IMapper _mapper;
		private readonly ApiProjectConfig _apiProjectConfig;
		private ICacheService _cache = null;
		private const string _cacheKeyPrefix = "phone";

		public PhoneController(IMapper mapper, IOptions<ApiProjectConfig> options, ICacheService cache, IPhoneProcessor phoneProcessor)
		{
			_phoneProcessor = phoneProcessor;
			_mapper = mapper;
			_apiProjectConfig = options.Value;
			_cache = cache;
		}

		[HttpGet("{id}", Name = nameof(GetPhoneById))]
		public async Task<IActionResult> GetPhoneById(int id, string requestedFields)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var phone = await _cache.RetrieveOrCreateAsync($"{_cacheKeyPrefix}{id}", async entry =>
			{
				return await _phoneProcessor.GetPhoneAsync(CurrentUser.UserToken, id);
			});

			if (phone == null)
			{
				return BadRequest("No Data Found");
			}

			// Translating business model to API model
			var result = _mapper.Map<Phone, PhoneDtl>(phone);

			// Data-shaping
			return Ok(string.IsNullOrEmpty(requestedFields)
				? result
				: GetShapedEntity(result, requestedFields));
		}

		[HttpGet(Name = nameof(GetPhones))]
		public async Task<IActionResult> GetPhones([FromQuery] QueryStringParameters queryStringParameters)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var phones = await _cache.RetrieveOrCreateAsync($"{ApiUtilities.GenerateCacheKey(_cacheKeyPrefix, queryStringParameters)}", async entry =>
			{
				return await _phoneProcessor.GetAllPhonesAsync(CurrentUser.UserToken, queryStringParameters.SortColumns, queryStringParameters.PageNumber, queryStringParameters.PageSize);
			});

			if (phones == null)
			{
				return Ok("No Data Found");
			}

			// Translate the business model to API model
			var result = _mapper.Map<IEnumerable<Phone>, IEnumerable<PhoneCore>>(phones.Item1);

			// Pagination
			Pagination pagination = new Pagination { PageNumber = queryStringParameters.PageNumber, PageSize = queryStringParameters.PageSize, TotalRecords = phones.Item2 };
			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

			// Data-shaping
			return Ok(string.IsNullOrEmpty(queryStringParameters.RequestedFields)
				? result
				: GetShapedEntities(result.ToList(), queryStringParameters.RequestedFields));
		}

		[HttpGet("Search", Name = nameof(SearchPhones))]
		public async Task<IActionResult> SearchPhones([FromQuery] PhoneSearchParameters searchParameters, [FromQuery] QueryStringParameters queryStringParameters)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var filteredPhones = await _cache.RetrieveOrCreateAsync($"{ApiUtilities.GenerateCacheKey(_cacheKeyPrefix, queryStringParameters, searchParameters)}", async entry =>
			{
				return await _phoneProcessor.SearchPhonesAsync(CurrentUser.UserToken, x =>
				
					(x.PhoneId == searchParameters.PhoneId || searchParameters.PhoneId == null) && 
					(x.PersonId == searchParameters.PersonId || searchParameters.PersonId == null) && 
					(x.PhoneNumber == searchParameters.PhoneNumber || string.IsNullOrEmpty(searchParameters.PhoneNumber))
					, queryStringParameters.SortColumns, queryStringParameters.PageNumber, queryStringParameters.PageSize, true, false);
			});

			if (filteredPhones == null)
			{
				return Ok("Search did not yield any results");
			}

			// Translate the business model to API model
			var result = _mapper.Map<IEnumerable<Phone>, IEnumerable<PhoneSearchResult>>(filteredPhones.Item1);

			// Pagination
			Pagination pagination = new Pagination { PageNumber = queryStringParameters.PageNumber, PageSize = queryStringParameters.PageSize, TotalRecords = filteredPhones.Item2 };
			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

			// Data-shaping
			return Ok(string.IsNullOrEmpty(queryStringParameters.RequestedFields)
				? result
				: GetShapedEntities(result.ToList(), queryStringParameters.RequestedFields));
		}

		[HttpPost(Name = nameof(CreatePhone))]
		public async Task<IActionResult> CreatePhone(CreatePhone createPhone)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Errors found in request");
			}

			var phone = _mapper.Map<CreatePhone, Phone>(createPhone);

			await _phoneProcessor.AddPhoneAsync(CurrentUser.UserToken, phone);

			_cache.RemoveAll();

			return Ok();
		}

		[HttpPut(Name = nameof(UpdatePhone))]
		public async Task<IActionResult> UpdatePhone(UpdatePhone updatePhone)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Errors in request");
			}

			var phone = _mapper.Map<UpdatePhone, Phone>(updatePhone);

			await _phoneProcessor.UpdatePhoneAsync(CurrentUser.UserToken, phone, true);

			_cache.RemoveAll();

			return Ok();
		}

		[HttpDelete("{id}", Name = nameof(DeletePhone))]
		public async Task<IActionResult> DeletePhone(int id)
		{
			var phone = _phoneProcessor.GetPhoneAsync(CurrentUser.UserToken, id);

			if (phone == null)
			{
				return BadRequest("No Data Found");
			}

			await _phoneProcessor.DeletePhoneAsync(CurrentUser.UserToken, id);

			_cache.RemoveAll();

			return Ok();
		}

		private Entity GetShapedEntity(PhoneDtl record, string requestedFields)
		{
			IDataShaper<PhoneDtl> dataShaper = new DataShaper<PhoneDtl>();
			var shapedData = dataShaper.ShapeData(record, requestedFields);
			return shapedData;
		}

		private IEnumerable<Entity> GetShapedEntities(IEnumerable<PhoneCore> records, string requestedFields)
		{
			IDataShaper<PhoneCore> dataShaper = new DataShaper<PhoneCore>();
			var shapedData = dataShaper.ShapeData(records, requestedFields);
			return shapedData;
		}

		private IEnumerable<Entity> GetShapedEntities(IEnumerable<PhoneSearchResult> records, string requestedFields)
		{
			IDataShaper<PhoneSearchResult> dataShaper = new DataShaper<PhoneSearchResult>();
			var shapedData = dataShaper.ShapeData(records, requestedFields);
			return shapedData;
		}

	}
}

