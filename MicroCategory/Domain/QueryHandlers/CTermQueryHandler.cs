using AutoMapper;
using MediatR;
using MicroCategory.Domain.Common.Queries;
using MicroCategory.Domain.Dtos;
using MicroCategory.Domain.Queries;
using MicroCategory.Domain.Repositories.Interface;
using MicroCategory.Domain.Common.Extension;
using System.Dynamic;

namespace MicroCategory.Domain.QueryHandlers
{
    /// <summary>
    /// Query Handler
    /// </summary>
    public class CTermQueryHandler : IRequestHandler<ListTermByTypeQuery, PagedList<CTermDto>>,
                                     IRequestHandler<ListTermWithTermMetaByTypeQuery, PagedList<CTermWithTermMetaDto>>,
                                     IRequestHandler<GetTermByTermIdQuery, CTermDto>,
                                     IRequestHandler<GetTermWithTermMetaByTermIdQuery, object>,
                                     IRequestHandler<GetDataByParentIdQuery, IList<CTermDto>>,
                                     IRequestHandler<GetFullDataByParentIdQuery, IList<CTermWithTermMetaDto>>
    {

        private readonly ICTermRepository _cTermRepository;
        private readonly ICTermmetumRepository _cTermmetumRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Contructor
        /// </summary>
        public CTermQueryHandler(ICTermRepository cTermRepository,
                                IMapper mapper,
                                ICTermmetumRepository cTermmetumRepository)
        {
            _cTermRepository = cTermRepository;
            _cTermmetumRepository = cTermmetumRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Handle Query ListTermByTypeQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedList<CTermDto>> Handle(ListTermByTypeQuery request, CancellationToken cancellationToken)
        {
            if (request.PageSize == 0)
            {
                request.PageSize = AppConstants.MinPageSize;
            }
            if (request.PageSize == -1)
            {
                request.PageSize = AppConstants.MaxPageSize;
            }

            var query = _cTermRepository.TableNoTracking.Where(a => (string.IsNullOrWhiteSpace(request.Type) || a.Type == request.Type)
                                                                    && (string.IsNullOrWhiteSpace(request.Name) || a.Name == request.Name)
                                                                    && (string.IsNullOrWhiteSpace(request.Code) || a.Code == request.Code)
                                                                    && a.DeletedAt == 0)
                                                                   .Select(a => new CTermDto()
                                                                   {
                                                                       Id = a.Id,
                                                                       Name = a.Name,
                                                                       Slug = a.Slug,
                                                                       Type = a.Type,
                                                                       Code = a.Code,
                                                                       Description = a.Description
                                                                   })
                                                                   .OrderByDescending(item => item.Id);

            var data = new PagedList<CTermDto>(Extensions.GetPagedList(query, request.PageNumber, request.PageSize),
                                                query.Count(),
                                                request.PageNumber,
                                                request.PageSize);

            return await Task.FromResult(data);
        }

        /// <summary>
        /// Handle Query GetDataByParentIdQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<CTermDto>> Handle(GetDataByParentIdQuery request, CancellationToken cancellationToken)
        {
            var query = (from term in _cTermRepository.TableNoTracking
                         join termmeta in _cTermmetumRepository.TableNoTracking on term.Id equals termmeta.TermId
                         where termmeta.MetaValue == request.Id.ToString()
                             && termmeta.MetaKey == "id_parent"
                             && term.DeletedAt == AppConstants.deleted_at
                             && termmeta.DeletedAt == AppConstants.deleted_at
                         select new CTermDto()
                         {
                             Id = term.Id,
                             Name = term.Name,
                             Code = term.Code,
                             Slug = term.Slug,
                             Description = term.Description,
                             Type = term.Type
                         }).ToList();

            return await Task.FromResult(query);
        }

        /// <summary>
        /// Handle Query GetFullDataByParentIdQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<CTermWithTermMetaDto>> Handle(GetFullDataByParentIdQuery request, CancellationToken cancellationToken)
        {
            var query = (from term in _cTermRepository.TableNoTracking
                         join termmeta in _cTermmetumRepository.TableNoTracking on term.Id equals termmeta.TermId
                         where termmeta.MetaValue == request.Id.ToString()
                             && termmeta.MetaKey == "id_parent"
                             && term.DeletedAt == AppConstants.deleted_at
                             && termmeta.DeletedAt == AppConstants.deleted_at
                         select new
                         {
                             Id = term.Id,
                             Name = term.Name,
                             Code = term.Code,
                             Type = term.Type,
                             Description = term.Description,
                             Slug = term.Slug,
                             TermId = termmeta.TermId,
                             TermMetaId = termmeta.Id,
                             MetaKey = termmeta.MetaKey,
                             MetaValue = termmeta.MetaValue
                         }).GroupBy(x => x.Id).Select(x => new CTermWithTermMetaDto()
                         {
                             Id = x.First().Id,
                             Name = x.First().Name,
                             Code = x.First().Code,
                             Type = x.First().Type,
                             Description = x.First().Description,
                             Slug = x.First().Slug,
                             ListTermMeta = x.Select(x => new CTermmetaDto()
                             {
                                 TermId = x.TermId,
                                 TermMetaId = x.TermMetaId,
                                 MetaKey = x.MetaKey,
                                 MetaValue = x.MetaValue
                             }).ToList()
                         }).ToList();

            return await Task.FromResult(query);
        }

        /// <summary>
        /// Handle Query ListTermWithTermMetaByTypeQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagedList<CTermWithTermMetaDto>> Handle(ListTermWithTermMetaByTypeQuery request, CancellationToken cancellationToken)
        {
            if (request.PageSize == 0)
            {
                request.PageSize = AppConstants.MinPageSize;
            }
            if (request.PageSize == -1)
            {
                request.PageSize = AppConstants.MaxPageSize;
            }

            var query = (from term in _cTermRepository.TableNoTracking.Where(a => (string.IsNullOrWhiteSpace(request.Type) || a.Type == request.Type)
                                                                    && (string.IsNullOrWhiteSpace(request.Name) || a.Name == request.Name)
                                                                    && (string.IsNullOrWhiteSpace(request.Code) || a.Code == request.Code)
                                                                    && a.DeletedAt == 0)
                         join termmeta in _cTermmetumRepository.TableNoTracking.Where(a => a.DeletedAt == 0) on term.Id equals termmeta.TermId
                         select new
                         {
                             Id = term.Id,
                             Name = term.Name,
                             Code = term.Code,
                             Type = term.Type,
                             Description = term.Description,
                             Slug = term.Slug,
                             TermId = termmeta.TermId,
                             TermMetaId = termmeta.Id,
                             MetaKey = termmeta.MetaKey,
                             MetaValue = termmeta.MetaValue
                         }).GroupBy(x => x.Id).Select(x => new CTermWithTermMetaDto()
                         {
                             Id = x.First().Id,
                             Name = x.First().Name,
                             Code = x.First().Code,
                             Type = x.First().Type,
                             Description = x.First().Description,
                             Slug = x.First().Slug,
                             ListTermMeta = x.Select(x => new CTermmetaDto()
                             {
                                 TermId = x.TermId,
                                 TermMetaId = x.TermMetaId,
                                 MetaKey = x.MetaKey,
                                 MetaValue = x.MetaValue
                             }).ToList()
                         });

            var data = new PagedList<CTermWithTermMetaDto>(Extensions.GetPagedList(query, request.PageNumber, request.PageSize),
                                                query.Count(),
                                                request.PageNumber,
                                                request.PageSize);

            return await Task.FromResult(data);
        }

        /// <summary>
        /// Handle Query GetTermByTermIdQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CTermDto> Handle(GetTermByTermIdQuery request, CancellationToken cancellationToken)
        {

            var query = await _cTermRepository.SingleOrDefaultAsync(a => a.Id == request.Id && a.DeletedAt == 0);

            var cTermDto = new CTermDto();
            var result = _mapper.Map(query, cTermDto);

            return result;
        }

        /// <summary>
        /// Handle Query GetTermWithTermMetaByTermIdQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object> Handle(GetTermWithTermMetaByTermIdQuery request, CancellationToken cancellationToken)
        {

            var query = (from term in _cTermRepository.TableNoTracking.Where(x => x.Id == request.Id)
                         join termmeta in _cTermmetumRepository.TableNoTracking on term.Id equals termmeta.TermId
                         where term.DeletedAt == AppConstants.deleted_at
                              && termmeta.DeletedAt == AppConstants.deleted_at
                         select new
                         {
                             Id = term.Id,
                             Code = term.Code,
                             Name = term.Name,
                             Type = term.Type,
                             Description = term.Description,
                             MetaKey = termmeta.MetaKey,
                             MetaValue = termmeta.MetaValue
                         }).ToList();

            dynamic expando = new ExpandoObject();
            foreach (var item in query)
            {
                ((IDictionary<string, object>)expando)["id"] = item.Id;
                ((IDictionary<string, object>)expando)["type"] = item.Type;
                ((IDictionary<string, object>)expando)["name"] = item.Name;
                ((IDictionary<string, object>)expando)["code"] = item.Code;
                ((IDictionary<string, object>)expando)["description"] = item.Description;
                ((IDictionary<string, object>)expando)[item.MetaKey] = item.MetaValue;
            }

            return expando;
        }
    }
}
