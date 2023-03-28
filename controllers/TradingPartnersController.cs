namespace TradingPartnerManagement.Controllers.v1;

using TradingPartnerManagement.Domain.TradingPartners.Features;
using TradingPartnerManagement.Domain.TradingPartners.Dtos;
using TradingPartnerManagement.Wrappers;
using TradingPartnerManagement.Domain;
using SharedKernel.Domain;
using System.Text.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Threading;
using MediatR;

[ApiController]
[Route("api/tradingpartners")]
[ApiVersion("1.0")]
public class TradingPartnersController: ControllerBase
{
    private readonly IMediator _mediator;

    public TradingPartnersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(TradingPartnerDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanAddTradingPartnerRecord")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "AddTradingPartner")]
    public async Task<ActionResult<TradingPartnerDto>> AddTradingPartner([FromBody]TradingPartnerForCreationDto tradingPartnerForCreation)
    {
        var command = new AddTradingPartner.AddTradingPartnerCommand(tradingPartnerForCreation);
        var commandResponse = await _mediator.Send(command);

        return CreatedAtRoute("GetTradingPartner",
            new { commandResponse.Id },
            commandResponse);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(TradingPartnerDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerRecord")]
    [Produces("application/json")]
    [HttpGet("{id:guid}", Name = "GetTradingPartner")]
    public async Task<ActionResult<TradingPartnerDto>> GetTradingPartner(Guid id)
    {
        var query = new GetTradingPartner.TradingPartnerQuery(id);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a list of all TradingPartners.
    /// </summary>
    /// <response code="200">TradingPartner list returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    /// <remarks>
    /// Requests can be narrowed down with a variety of query string values:
    /// ## Query String Parameters
    /// - **PageNumber**: An integer value that designates the page of records that should be returned.
    /// - **PageSize**: An integer value that designates the number of records returned on the given page that you would like to return. This value is capped by the internal MaxPageSize.
    /// - **SortOrder**: A comma delimited ordered list of property names to sort by. Adding a `-` before the name switches to sorting descendingly.
    /// - **Filters**: A comma delimited list of fields to filter by formatted as `{Name}{Operator}{Value}` where
    ///     - {Name} is the name of a filterable property. You can also have multiple names (for OR logic) by enclosing them in brackets and using a pipe delimiter, eg. `(LikeCount|CommentCount)>10` asks if LikeCount or CommentCount is >10
    ///     - {Operator} is one of the Operators below
    ///     - {Value} is the value to use for filtering. You can also have multiple values (for OR logic) by using a pipe delimiter, eg.`Title@= new|hot` will return posts with titles that contain the text "new" or "hot"
    ///
    ///    | Operator | Meaning                       | Operator  | Meaning                                      |
    ///    | -------- | ----------------------------- | --------- | -------------------------------------------- |
    ///    | `==`     | Equals                        |  `!@=`    | Does not Contains                            |
    ///    | `!=`     | Not equals                    |  `!_=`    | Does not Starts with                         |
    ///    | `>`      | Greater than                  |  `@=*`    | Case-insensitive string Contains             |
    ///    | `&lt;`   | Less than                     |  `_=*`    | Case-insensitive string Starts with          |
    ///    | `>=`     | Greater than or equal to      |  `==*`    | Case-insensitive string Equals               |
    ///    | `&lt;=`  | Less than or equal to         |  `!=*`    | Case-insensitive string Not equals           |
    ///    | `@=`     | Contains                      |  `!@=*`   | Case-insensitive string does not Contains    |
    ///    | `_=`     | Starts with                   |  `!_=*`   | Case-insensitive string does not Starts with |
    /// </remarks>
    [ProducesResponseType(typeof(IEnumerable<TradingPartnerDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerList")]
    [Produces("application/json")]
    [HttpGet(Name = "GetTradingPartners")]
    public async Task<IActionResult> GetTradingPartners([FromQuery] TradingPartnerParametersDto tradingPartnerParametersDto)
    {
        var query = new GetTradingPartnerList.TradingPartnerListQuery(tradingPartnerParametersDto);
        var queryResponse = await _mediator.Send(query);

        var paginationMetadata = new
        {
            totalCount = queryResponse.TotalCount,
            pageSize = queryResponse.PageSize,
            currentPageSize = queryResponse.CurrentPageSize,
            currentStartIndex = queryResponse.CurrentStartIndex,
            currentEndIndex = queryResponse.CurrentEndIndex,
            pageNumber = queryResponse.PageNumber,
            totalPages = queryResponse.TotalPages,
            hasPrevious = queryResponse.HasPrevious,
            hasNext = queryResponse.HasNext
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerRecord")]
    [Produces("application/json")]
    [HttpPut("{id:guid}", Name = "UpdateTradingPartner")]
    public async Task<IActionResult> UpdateTradingPartner(Guid id, TradingPartnerForUpdateDto tradingPartner)
    {
       if (tradingPartner.Id == Guid.Empty) 
         tradingPartner.Id = id;
        var command = new UpdateTradingPartner.UpdateTradingPartnerCommand(id, tradingPartner);
        await _mediator.Send(command);

        return NoContent();
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerRecord")]
    [Produces("application/json")]
    [HttpDelete("{id:guid}", Name = "DeleteTradingPartner")]
    public async Task<ActionResult> DeleteTradingPartner(Guid id)
    {
        var command = new DeleteTradingPartner.DeleteTradingPartnerCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }


    /// <summary>
    /// Deletes an set of existing TradingPartner records.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnersRecord")]
    [Produces("application/json")]
    [HttpDelete]
    public async Task<ActionResult> DeleteTradingPartners(Guid[] ids)
    {
        var command = new DeleteTradingPartners.DeleteTradingPartnersCommand(ids);
        await _mediator.Send(command);

        return NoContent();
    }


    /// <summary>
    /// Updates specific properties on an existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanPatchTradingPartnerRecord")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPatch("{id}", Name = "PartiallyUpdateTradingPartner")]
    public async Task<IActionResult> PartiallyUpdateTradingPartner(Guid id, JsonPatchDocument<TradingPartnerForUpdateDto> patchDoc)
    {
        var command = new PatchTradingPartner.PatchTradingPartnerCommand(id, patchDoc);
        await _mediator.Send(command);

        return NoContent();
    }


    /// <summary>
    /// Gets a list of all TradingPartners.
    /// </summary>
    /// <response code="200">TradingPartner list returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    /// <remarks>
    /// Requests can be narrowed down with a variety of query string values:
    /// ## Query String Parameters
    /// - **PageNumber**: An integer value that designates the page of records that should be returned.
    /// - **PageSize**: An integer value that designates the number of records returned on the given page that you would like to return. This value is capped by the internal MaxPageSize.
    /// - **SortOrder**: A comma delimited ordered list of property names to sort by. Adding a `-` before the name switches to sorting descendingly.
    /// - **Filters**: A comma delimited list of fields to filter by formatted as `{Name}{Operator}{Value}` where
    ///     - {Name} is the name of a filterable property. You can also have multiple names (for OR logic) by enclosing them in brackets and using a pipe delimiter, eg. `(LikeCount|CommentCount)>10` asks if LikeCount or CommentCount is >10
    ///     - {Operator} is one of the Operators below
    ///     - {Value} is the value to use for filtering. You can also have multiple values (for OR logic) by using a pipe delimiter, eg.`Title@= new|hot` will return posts with titles that contain the text "new" or "hot"
    ///
    ///    | Operator | Meaning                       | Operator  | Meaning                                      |
    ///    | -------- | ----------------------------- | --------- | -------------------------------------------- |
    ///    | `==`     | Equals                        |  `!@=`    | Does not Contains                            |
    ///    | `!=`     | Not equals                    |  `!_=`    | Does not Starts with                         |
    ///    | `>`      | Greater than                  |  `@=*`    | Case-insensitive string Contains             |
    ///    | `&lt;`   | Less than                     |  `_=*`    | Case-insensitive string Starts with          |
    ///    | `>=`     | Greater than or equal to      |  `==*`    | Case-insensitive string Equals               |
    ///    | `&lt;=`  | Less than or equal to         |  `!=*`    | Case-insensitive string Not equals           |
    ///    | `@=`     | Contains                      |  `!@=*`   | Case-insensitive string does not Contains    |
    ///    | `_=`     | Starts with                   |  `!_=*`   | Case-insensitive string does not Starts with |
    /// </remarks>
    [ProducesResponseType(typeof(IEnumerable<TradingPartnerDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetLogicalTradingPartnerList")]
    [Produces("application/json")]
    [Route("GetLogicalTradingPartners")]
    [HttpGet]
    public async Task<IActionResult> GetLogicalTradingPartners([FromQuery] TradingPartnerParametersDto tradingPartnerParametersDto)
    {
        var query = new GetTradingPartnerLogicalList.TradingPartnerLogicalListQuery(tradingPartnerParametersDto);
        var queryResponse = await _mediator.Send(query);

        var paginationMetadata = new
        {
            totalCount = queryResponse.TotalCount,
            pageSize = queryResponse.PageSize,
            currentPageSize = queryResponse.CurrentPageSize,
            currentStartIndex = queryResponse.CurrentStartIndex,
            currentEndIndex = queryResponse.CurrentEndIndex,
            pageNumber = queryResponse.PageNumber,
            totalPages = queryResponse.TotalPages,
            hasPrevious = queryResponse.HasPrevious,
            hasNext = queryResponse.HasNext
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetadata));

        return Ok(queryResponse);
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerRoleStatusById")]
    [Produces("application/json")]
    [Route("DeleteTradingPartnerRoleStatusById")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTradingPartnerRoleStatusById(Guid tradingPartnerId, Guid roleId, Guid statusId)
    {
        var query = new DeleteTradingPartnerRoleStatusById.DeleteTradingPartnerRoleStatusByIdQuery(tradingPartnerId, roleId, statusId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerRoleStatusById")]
    [Produces("application/json")]
    [Route("UpdateTradingPartnerRoleStatusById")]
    [HttpPut]
    public async Task<IActionResult> UpdateTradingPartnerRoleStatusById(Guid tradingPartnerId, Guid roleId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Statuses statuses)
    {
        var query = new UpdateTradingPartnerRoleStatusById.UpdateTradingPartnerRoleStatusByIdQuery(tradingPartnerId, roleId, statuses);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanCreateTradingPartnerRoleStatusByRoleId")]
    [Produces("application/json")]
    [Route("CreateTradingPartnerRoleStatusByRoleId")]
    [HttpPost]
    public async Task<IActionResult> CreateTradingPartnerRoleStatusByRoleId(Guid tradingPartnerId, Guid roleId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Statuses statuses)
    {
        var query = new CreateTradingPartnerRoleStatusByRoleId.CreateTradingPartnerRoleStatusByRoleIdQuery(tradingPartnerId, roleId, statuses);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUnAssignTradingPartnerRoleFromTradingPartner")]
    [Produces("application/json")]
    [Route("UnAssignTradingPartnerRoleFromTradingPartner")]
    [HttpDelete]
    public async Task<IActionResult> UnAssignTradingPartnerRoleFromTradingPartner(Guid tradingPartnerId, Guid roleId)
    {
        var query = new UnAssignTradingPartnerRoleFromTradingPartner.UnAssignTradingPartnerRoleFromTradingPartnerQuery(tradingPartnerId, roleId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanAssignTradingPartnerRoleToTradingPartner")]
    [Produces("application/json")]
    [Route("AssignTradingPartnerRoleToTradingPartner")]
    [HttpPost]
    public async Task<IActionResult> AssignTradingPartnerRoleToTradingPartner(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Roles roles)
    {
        var query = new AssignTradingPartnerRoleToTradingPartner.AssignTradingPartnerRoleToTradingPartnerQuery(tradingPartnerId, roles);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerElectronicAddressByEmailId")]
    [Produces("application/json")]
    [Route("UpdateTradingPartnerElectronicAddressByEmailId")]
    [HttpPut]
    public async Task<IActionResult> UpdateTradingPartnerElectronicAddressByEmailId(Guid tradingPartnerId, Guid emailId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_email_addresses electronicemailaddresses)
    {
        var query = new UpdateTradingPartnerElectronicAddressByEmailId.UpdateTradingPartnerElectronicAddressByEmailIdQuery(tradingPartnerId, emailId, electronicemailaddresses);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerElectronicAddressByWebsiteId")]
    [Produces("application/json")]
    [Route("UpdateTradingPartnerElectronicAddressByWebsiteId")]
    [HttpPut]
    public async Task<IActionResult> UpdateTradingPartnerElectronicAddressByWebsiteId(Guid tradingPartnerId, Guid webSiteId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_website_addresses electronicwebsiteaddresses)
    {
        var query = new UpdateTradingPartnerElectronicAddressByWebsiteId.UpdateTradingPartnerElectronicAddressByWebsiteIdQuery(tradingPartnerId, webSiteId, electronicwebsiteaddresses);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerElectronicAddressByEmailId")]
    [Produces("application/json")]
    [Route("DeleteTradingPartnerElectronicAddressByEmailId")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTradingPartnerElectronicAddressByEmailId(Guid tradingPartnerId, Guid emailId)
    {
        var query = new DeleteTradingPartnerElectronicAddressByEmailId.DeleteTradingPartnerElectronicAddressByEmailIdQuery(tradingPartnerId, emailId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerElectronicAddressByWebsiteId")]
    [Produces("application/json")]
    [Route("DeleteTradingPartnerElectronicAddressByWebsiteId")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTradingPartnerElectronicAddressByWebsiteId(Guid tradingPartnerId, Guid webSiteId)
    {
        var query = new DeleteTradingPartnerElectronicAddressByWebsiteId.DeleteTradingPartnerElectronicAddressByWebsiteIdQuery(tradingPartnerId, webSiteId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanCreateTradingPartnerElectronicEmailAddress")]
    [Produces("application/json")]
    [Route("CreateTradingPartnerElectronicEmailAddress")]
    [HttpPost]
    public async Task<IActionResult> CreateTradingPartnerElectronicEmailAddress(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_email_addresses electronicemailaddresses)
    {
        var query = new CreateTradingPartnerElectronicEmailAddress.CreateTradingPartnerElectronicEmailAddressQuery(tradingPartnerId, electronicemailaddresses);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanCreateTradingPartnerElectronicWebsiteAddress")]
    [Produces("application/json")]
    [Route("CreateTradingPartnerElectronicWebsiteAddress")]
    [HttpPost]
    public async Task<IActionResult> CreateTradingPartnerElectronicWebsiteAddress(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Electronic_website_addresses electronicwebsiteaddresses)
    {
        var query = new CreateTradingPartnerElectronicWebsiteAddress.CreateTradingPartnerElectronicWebsiteAddressQuery(tradingPartnerId, electronicwebsiteaddresses);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(Electronic_email_addresses), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerElectronicAddressByEmailId")]
    [Produces("application/json")]
    [Route("GetTradingPartnerElectronicAddressByEmailId")]
    [HttpGet]
    public async Task<ActionResult<Electronic_email_addresses>> GetTradingPartnerElectronicAddressByEmailId(Guid emailId)
    {
        var query = new GetTradingPartnerElectronicAddressByEmailId.GetTradingPartnerElectronicAddressByEmailIdQuery(emailId);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(Electronic_website_addresses), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerElectronicAddressByWebsiteId")]
    [Produces("application/json")]
    [Route("GetTradingPartnerElectronicAddressByWebsiteId")]
    [HttpGet]
    public async Task<ActionResult<Electronic_website_addresses>> GetTradingPartnerElectronicAddressByWebsiteId(Guid websiteId)
    {
        var query = new GetTradingPartnerElectronicAddressByWebsiteId.GetTradingPartnerElectronicAddressByWebsiteIdQuery(websiteId);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerPhoneAddressById")]
    [Produces("application/json")]
    [Route("DeleteTradingPartnerPhoneAddressById")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTradingPartnerPhoneAddressById(Guid tradingPartnerId, Guid phoneAddressId)
    {
        var query = new DeleteTradingPartnerPhoneAddressById.DeleteTradingPartnerPhoneAddressByIdQuery(tradingPartnerId, phoneAddressId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerPhoneAddressByPhoneId")]
    [Produces("application/json")]
    [Route("UpdateTradingPartnerPhoneAddressByPhoneId")]
    [HttpPut]
    public async Task<IActionResult> UpdateTradingPartnerPhoneAddressByPhoneId(Guid tradingPartnerId, Guid phoneAddressId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Phone_addresses phoneaddresses)
    {
        var query = new UpdateTradingPartnerPhoneAddressByPhoneId.UpdateTradingPartnerPhoneAddressByPhoneIdQuery(tradingPartnerId, phoneAddressId, phoneaddresses);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanCreateTradingPartnerPhoneAddress")]
    [Produces("application/json")]
    [Route("CreateTradingPartnerPhoneAddress")]
    [HttpPost]
    public async Task<IActionResult> CreateTradingPartnerPhoneAddress(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Phone_addresses phoneaddresses)
    {
        var query = new CreateTradingPartnerPhoneAddress.CreateTradingPartnerPhoneAddressQuery(tradingPartnerId, phoneaddresses);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(Phone_addresses), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerPhoneAddressById")]
    [Produces("application/json")]
    [Route("GetTradingPartnerPhoneAddressById")]
    [HttpGet]
    public async Task<ActionResult<Phone_addresses>> GetTradingPartnerPhoneAddressById(Guid phoneAddressId)
    {
        var query = new GetTradingPartnerPhoneAddressById.GetTradingPartnerPhoneAddressByIdQuery(phoneAddressId);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Deletes an existing TradingPartner record.
    /// </summary>
    /// <response code="204">TradingPartner deleted.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanDeleteTradingPartnerPostalAddressById")]
    [Produces("application/json")]
    [Route("DeleteTradingPartnerPostalAddressById")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTradingPartnerPostalAddressById(Guid tradingPartnerId, Guid postalAddressId)
    {
        var query = new DeleteTradingPartnerPostalAddressById.DeleteTradingPartnerPostalAddressByIdQuery(tradingPartnerId, postalAddressId);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Updates an entire existing TradingPartner.
    /// </summary>
    /// <response code="204">TradingPartner updated.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanUpdateTradingPartnerPostalAddressById")]
    [Produces("application/json")]
    [Route("UpdateTradingPartnerPostalAddressById")]
    [HttpPut]
    public async Task<IActionResult> UpdateTradingPartnerPostalAddressById(Guid tradingPartnerId, Guid postalAddressId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Postal_addresses postaladdresses)
    {
        var query = new UpdateTradingPartnerPostalAddressById.UpdateTradingPartnerPostalAddressByIdQuery(tradingPartnerId, postalAddressId, postaladdresses);
        var queryResponse = await _mediator.Send(query);

        return NoContent();
    }


    /// <summary>
    /// Creates a new TradingPartner record.
    /// </summary>
    /// <response code="201">TradingPartner created.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(bool), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanCreateTradingPartnerPostalAddress")]
    [Produces("application/json")]
    [Route("CreateTradingPartnerPostalAddress")]
    [HttpPost]
    public async Task<IActionResult> CreateTradingPartnerPostalAddress(Guid tradingPartnerId, TradingPartnerManagement.Domain.TradingPartners.Dtos.Postal_addresses postaladdresses)
    {
        var query = new CreateTradingPartnerPostalAddress.CreateTradingPartnerPostalAddressQuery(tradingPartnerId, postaladdresses);
        var queryResponse = await _mediator.Send(query);

        return StatusCode(201);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(Postal_addresses), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanGetTradingPartnerPostalAddressById")]
    [Produces("application/json")]
    [Route("GetTradingPartnerPostalAddressById")]
    [HttpGet]
    public async Task<ActionResult<Postal_addresses>> GetTradingPartnerPostalAddressById(Guid postalAddressId)
    {
        var query = new GetTradingPartnerPostalAddressById.GetTradingPartnerPostalAddressByIdQuery(postalAddressId);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(Roles), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanReadTradingPartnerRoleByRoleId")]
    [Produces("application/json")]
    [Route("ReadTradingPartnerRoleByRoleId")]
    [HttpGet]
    public async Task<ActionResult<Roles>> ReadTradingPartnerRoleByRoleId(Guid roleId)
    {
        var query = new ReadTradingPartnerRoleByRoleId.ReadTradingPartnerRoleByRoleIdQuery(roleId);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }


    /// <summary>
    /// Gets a single TradingPartner by ID.
    /// </summary>
    /// <response code="200">TradingPartner record returned successfully.</response>
    /// <response code="400">TradingPartner has missing/invalid values.</response>
    /// <response code="401">This request was not able to be authenticated.</response>
    /// <response code="403">The required permissions to access this resource were not present in the given request.</response>
    /// <response code="500">There was an error on the server while creating the TradingPartner.</response>
    [ProducesResponseType(typeof(List<TradingPartnerDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(500)]
    [Authorize(Policy = "CanReadTradingPartnerListByRoleType")]
    [Produces("application/json")]
    [Route("ReadTradingPartnerListByRoleType")]
    [HttpGet]
    public async Task<ActionResult<List<TradingPartnerDto>>> ReadTradingPartnerListByRoleType(string type)
    {
        var query = new ReadTradingPartnerListByRoleType.ReadTradingPartnerListByRoleTypeQuery(type);
        var queryResponse = await _mediator.Send(query);

        return Ok(queryResponse);
    }

    // endpoint marker - do not delete this comment
}
