using Microsoft.AspNetCore.Mvc;
using MicroCategory.Domain.Models;
using MediatR;
using MicroCategory.Domain.Notification;
using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Queries;
using System.Net;
using MicroCategory.Domain.Dtos;
using MicroCategory.Domain.Commands;

namespace MicroCategory.Controllers
{
    [Route("api/term")]
    [ApiController]
    public class CTermsController : BaseController
    {


        public CTermsController(IMediator mediator,
                  INotificationHandler<DomainNotification> notificationHandler)
                : base(mediator, notificationHandler) { }

        /// <summary>
        /// Lấy danh sách danh mục
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedList<CTermDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<PagedList<CTermDto>>> GetAllCTerms([FromQuery] ListTermByTypeQuery request)
        {
            var query = await QueryAsync(request);
            if (query == null)
                return NoContent();

            var data = new
            {
                Paging = query.GetHeader(),
                Items = query.List
            };

            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách danh mục và meta data
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("get-with-term-meta")]
        [ProducesResponseType(typeof(PagedList<CTermWithTermMetaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<PagedList<CTerm>>> GetAllCTermsWithTermMeta([FromQuery] ListTermWithTermMetaByTypeQuery request)
        {
            var query = await QueryAsync(request);
            if (query == null)
                return NoContent();

            var data = new
            {
                Paging = query.GetHeader(),
                Items = query.List
            };

            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin của danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CTermDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CTermDto>> GetById(int id)
        {
            if (id == 0)
                return BadRequest("Bạn phải nhập ID!");

            var data = await QueryAsync(new GetTermByTermIdQuery(id));

            if (data == null)
                return NoContent();

            return data;

        }


        /// <summary>
        /// Lấy danh sách danh mục theo id_parent
        /// </summary>
        /// <param name="idParent"></param>
        /// <returns></returns>
        [HttpGet("get-by-id-parent/{idParent:int}")]
        [ProducesResponseType(typeof(PagedList<CTermWithTermMetaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<CTerm>>> GetByIdParent(int idParent)
        {
            if (idParent == 0)
                return BadRequest("Bạn phải nhập ID!");

            var data = await QueryAsync(new GetDataByParentIdQuery(idParent));

            if (data == null)
                return NoContent();

            return Ok(data);
        }

        /// <summary>
        /// Lấy danh sách danh mục theo id_parent
        /// </summary>
        /// <param name="idParent"></param>
        /// <returns></returns>
        [HttpGet("get-full-data-by-id-parent/{idParent:int}")]
        [ProducesResponseType(typeof(PagedList<CTermWithTermMetaDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<CTerm>>> GetFullByIdParent(int idParent)
        {
            if (idParent == 0)
                return BadRequest("Bạn phải nhập ID!");

            var data = await QueryAsync(new GetFullDataByParentIdQuery(idParent));

            if (data == null)
                return NoContent();

            return Ok(data);
        }

        /// <summary>
        /// Lấy thông tin của danh mục và meta data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-with-term-meta/{id:int}")]
        [ProducesResponseType(typeof(CTermDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<object>> GetWithMetaDataById(int id)
        {
            if (id == 0)
                return BadRequest("Bạn phải nhập ID!");

            var data = await QueryAsync(new GetTermWithTermMetaByTermIdQuery(id));

            if (data == null)
                return NoContent();

            return data;

        }

        /// <summary>
        /// Thêm mới danh mục
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(CreateTermCommand request)
        {
            var result = await CommandAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Xoá danh mục theo ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(DeleteTermCommand request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState });

            // Hanlder
            await CommandAsync(request);
            if (Errors.Any())
                return BadRequest(new { Errors });

            return Ok();
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateTermCommand request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = ModelState });

            // Handler
            await CommandAsync(request);
            if (Errors.Any())
                return BadRequest(new { Errors });

            return Ok();
        }

    }
}
