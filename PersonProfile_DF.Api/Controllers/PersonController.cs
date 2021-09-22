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
	public class PersonController : AuthorizedController
	{
		private readonly IPersonProcessor _personProcessor;
		private readonly IMapper _mapper;
		private readonly ApiProjectConfig _apiProjectConfig;
		private ICacheService _cache = null;
		private const string _cacheKeyPrefix = "person";

		public PersonController(IMapper mapper, IOptions<ApiProjectConfig> options, ICacheService cache, IPersonProcessor personProcessor)
		{
			_personProcessor = personProcessor;
			_mapper = mapper;
			_apiProjectConfig = options.Value;
			_cache = cache;
		}

		[HttpGet("{id}", Name = nameof(GetPersonById))]
		public async Task<IActionResult> GetPersonById(int id, string requestedFields)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var person = await _cache.RetrieveOrCreateAsync($"{_cacheKeyPrefix}{id}", async entry =>
			{
				return await _personProcessor.GetPersonAsync(CurrentUser.UserToken, id);
			});

			if (person == null)
			{
				return BadRequest("No Data Found");
			}

			// Translating business model to API model
			var result = _mapper.Map<Person, PersonDtl>(person);

			// Data-shaping
			return Ok(string.IsNullOrEmpty(requestedFields)
				? result
				: GetShapedEntity(result, requestedFields));
		}

		[HttpGet(Name = nameof(GetPeople))]
		public async Task<IActionResult> GetPeople([FromQuery] QueryStringParameters queryStringParameters)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var people = await _cache.RetrieveOrCreateAsync($"{ApiUtilities.GenerateCacheKey(_cacheKeyPrefix, queryStringParameters)}", async entry =>
			{
				return await _personProcessor.GetAllPeopleAsync(CurrentUser.UserToken, queryStringParameters.SortColumns, queryStringParameters.PageNumber, queryStringParameters.PageSize);
			});

			if (people == null)
			{
				return Ok("No Data Found");
			}

			// Translate the business model to API model
			var result = _mapper.Map<IEnumerable<Person>, IEnumerable<PersonCore>>(people.Item1);

			// Pagination
			Pagination pagination = new Pagination { PageNumber = queryStringParameters.PageNumber, PageSize = queryStringParameters.PageSize, TotalRecords = people.Item2 };
			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

			// Data-shaping
			return Ok(string.IsNullOrEmpty(queryStringParameters.RequestedFields)
				? result
				: GetShapedEntities(result.ToList(), queryStringParameters.RequestedFields));
		}

		[HttpGet("Search", Name = nameof(SearchPeople))]
		public async Task<IActionResult> SearchPeople([FromQuery] PersonSearchParameters searchParameters, [FromQuery] QueryStringParameters queryStringParameters)
		{
			// Try to fetch the records from Cache; if not, then fetch it from business layer
			var filteredPeople = await _cache.RetrieveOrCreateAsync($"{ApiUtilities.GenerateCacheKey(_cacheKeyPrefix, queryStringParameters, searchParameters)}", async entry =>
			{
				return await _personProcessor.SearchPeopleAsync(CurrentUser.UserToken, x =>
				
					(x.PersonId == searchParameters.PersonId || searchParameters.PersonId == null) && 
					(x.Name == searchParameters.Name || string.IsNullOrEmpty(searchParameters.Name)) && 
					(x.EmailAddress == searchParameters.EmailAddress || string.IsNullOrEmpty(searchParameters.EmailAddress)) && 
					(x.MailingAddress == searchParameters.MailingAddress || string.IsNullOrEmpty(searchParameters.MailingAddress)) && 
					(x.PhoneNumber == searchParameters.PhoneNumber || string.IsNullOrEmpty(searchParameters.PhoneNumber)) && 
					(x.LastUpdateDateTimeStamp == searchParameters.LastUpdateDateTimeStamp || searchParameters.LastUpdateDateTimeStamp == null)
					, queryStringParameters.SortColumns, queryStringParameters.PageNumber, queryStringParameters.PageSize, true, false);
			});

			if (filteredPeople == null)
			{
				return Ok("Search did not yield any results");
			}

			// Translate the business model to API model
			var result = _mapper.Map<IEnumerable<Person>, IEnumerable<PersonSearchResult>>(filteredPeople.Item1);

			// Pagination
			Pagination pagination = new Pagination { PageNumber = queryStringParameters.PageNumber, PageSize = queryStringParameters.PageSize, TotalRecords = filteredPeople.Item2 };
			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

			// Data-shaping
			return Ok(string.IsNullOrEmpty(queryStringParameters.RequestedFields)
				? result
				: GetShapedEntities(result.ToList(), queryStringParameters.RequestedFields));
		}

		[HttpGet("{personId}/Phones", Name = nameof(GetPersonPhones))]
		public async Task<IActionResult> GetPersonPhones(int personId, [FromQuery] QueryStringParameters queryStringParameters)
		{
			var phones = await _cache.RetrieveOrCreateAsync($"{ApiUtilities.GenerateCacheKey(_cacheKeyPrefix + "phones", queryStringParameters)}", async entry =>
			{
				return await _personProcessor.GetPhonesAsync(CurrentUser.UserToken, personId, queryStringParameters.SortColumns, queryStringParameters.PageNumber, queryStringParameters.PageSize, false, false);
			});

			if (phones == null)
			{
				return Ok("No Data Found");
			}

			// Translating business model to API model
			var result = _mapper.Map<IEnumerable<Phone>, IEnumerable<PhoneCore>>(phones.Item1);

			// Pagination
			Pagination pagination = new Pagination { PageNumber = queryStringParameters.PageNumber, PageSize = queryStringParameters.PageSize, TotalRecords = phones.Item2 };
			Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagination));

			return Ok(result);
		}

		[HttpPost(Name = nameof(CreatePerson))]
		public async Task<IActionResult> CreatePerson(CreatePerson createPerson)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Errors found in request");
			}

			var person = _mapper.Map<CreatePerson, Person>(createPerson);

			await _personProcessor.AddPersonAsync(CurrentUser.UserToken, person);

			_cache.RemoveAll();

			return Ok();
		}

		[HttpPut(Name = nameof(UpdatePerson))]
		public async Task<IActionResult> UpdatePerson(UpdatePerson updatePerson)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest("Errors in request");
			}

			var person = _mapper.Map<UpdatePerson, Person>(updatePerson);

			await _personProcessor.UpdatePersonAsync(CurrentUser.UserToken, person, true);

			_cache.RemoveAll();

			return Ok();
		}

		[HttpDelete("{id}", Name = nameof(DeletePerson))]
		public async Task<IActionResult> DeletePerson(int id)
		{
			var person = _personProcessor.GetPersonAsync(CurrentUser.UserToken, id);

			if (person == null)
			{
				return BadRequest("No Data Found");
			}

			await _personProcessor.DeletePersonAsync(CurrentUser.UserToken, id);

			_cache.RemoveAll();

			return Ok();
		}

		private Entity GetShapedEntity(PersonDtl record, string requestedFields)
		{
			IDataShaper<PersonDtl> dataShaper = new DataShaper<PersonDtl>();
			var shapedData = dataShaper.ShapeData(record, requestedFields);
			return shapedData;
		}

		private IEnumerable<Entity> GetShapedEntities(IEnumerable<PersonCore> records, string requestedFields)
		{
			IDataShaper<PersonCore> dataShaper = new DataShaper<PersonCore>();
			var shapedData = dataShaper.ShapeData(records, requestedFields);
			return shapedData;
		}

		private IEnumerable<Entity> GetShapedEntities(IEnumerable<PersonSearchResult> records, string requestedFields)
		{
			IDataShaper<PersonSearchResult> dataShaper = new DataShaper<PersonSearchResult>();
			var shapedData = dataShaper.ShapeData(records, requestedFields);
			return shapedData;
		}

	}
}

