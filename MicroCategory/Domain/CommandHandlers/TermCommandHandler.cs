using MediatR;
using MicroCategory.Domain.Commands;
using MicroCategory.Domain.Models;
using MicroCategory.Domain.Notification;
using MicroCategory.Domain.Repositories.Interface;
using MicroCategory.Domain.UnitOfWork;

namespace MicroCategory.Domain.CommandHandlers
{
    public class TermCommandHandler : IRequestHandler<CreateTermCommand>,
                                         IRequestHandler<DeleteTermCommand>,
                                         IRequestHandler<UpdateTermCommand>
    {

        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICTermRepository _cTermRepository;
        private readonly ICTermmetumRepository _cTermmetumRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TermCommandHandler(IEventDispatcher eventDispatcher,
                                     ICTermRepository cTermRepository,
                                     ICTermmetumRepository cTermmetumRepository,
                                     IUnitOfWork unitOfWork)
        {
            _eventDispatcher = eventDispatcher;
            _cTermRepository = cTermRepository;
            _cTermmetumRepository = cTermmetumRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// CreateDanhMucCommand
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task<Unit> Handle(CreateTermCommand request, CancellationToken cancellationToken)
        {
            // request isValid
            if (!request.IsValid())
            {
                foreach (var error in request.ValidationResult.Errors)
                    await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, error.ErrorMessage));

                return Unit.Value;
            }

            await using (var transaction = _unitOfWork.BeginTransaction())
            {

                try
                {
                    var model = new CTerm
                    {
                        // map model
                        Name = request.Name,
                        Type = request.Type,
                        Code = request.Code,

                        CreatedAt = DateTime.Now,
                        Slug = Guid.NewGuid().ToString()
                    };
                    _cTermRepository.Insert(model);
                    await _unitOfWork.Commit();

                    if (request.TermMetas is not null)
                    {
                        CTermmetum cTermmetum;
                        foreach (var item in request.TermMetas)
                        {
                            cTermmetum = new CTermmetum()
                            {
                                CreatedAt = DateTime.Now,
                                TermId = model.Id,
                                MetaKey = item.MetaKey,
                                MetaValue = item.MetaValue,
                            };
                            _cTermmetumRepository.Insert(cTermmetum);
                            await _unitOfWork.Commit();
                        }
                    }
                    await _unitOfWork.Commit();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, "Không thành công"));
                    return Unit.Value;

                }
                return Unit.Value;
            }
        }

        /// <summary>
        /// DeleteDanhMucCommand
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Unit> Handle(DeleteTermCommand request, CancellationToken cancellationToken)
        {

            await using (var transaction = _unitOfWork.BeginTransaction())
            {

                try
                {
                    var cterm = await _cTermRepository.SingleOrDefaultAsync(_ => _.Id == request.Id
                                                                                     && _.DeletedAt == AppConstants.deleted_at);
                    if (cterm is null)
                    {
                        await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, $"Danh mục có mã có mã: {request.Id} không tồn tại"));
                        return Unit.Value;
                    }
                    IEnumerable<CTermmetum> termMetas = _cTermmetumRepository.TableNoTracking.Where(_ => _.TermId == request.Id
                                                                                            && _.DeletedAt == AppConstants.deleted_at).ToList();
                    if (termMetas is not null)
                    {
                        foreach (var term in termMetas)
                        {
                            foreach (var item in request.TermMetas)
                            {
                                if (term.Id == item.TermId)
                                {
                                    term.DeletedAt = 1;
                                    term.UpdatedAt = DateTime.Now;
                                }
                            }
                        }
                        _cTermmetumRepository.UpdateRange(termMetas);
                        await _unitOfWork.Commit();
                    }

                    cterm.DeletedAt = 1;
                    cterm.UpdatedAt = DateTime.Now;
                    _cTermRepository.Update(cterm);
                    await _unitOfWork.Commit();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, "Không thành công"));
                    return Unit.Value;

                }
                return Unit.Value;
            }
        }

        /// <summary>
        /// UpdateDanhMucCommand
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Unit> Handle(UpdateTermCommand request, CancellationToken cancellationToken)
        {
            await using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var cterm = await _cTermRepository.SingleOrDefaultAsync(_ => _.Id == request.Id
                                                                              && _.DeletedAt == AppConstants.deleted_at);
                    if (cterm is null)
                    {
                        await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, $"Danh mục có mã có mã: {request.Id} không tồn tại"));
                        var a = Unit.Value;
                        return Unit.Value;
                    }
                    IEnumerable<CTermmetum> termMetas = _cTermmetumRepository.TableNoTracking.Where(_ => _.TermId == request.Id);

                    if (request.Type is not null) cterm.Type = request.Type;
                    if (request.Name is not null) cterm.Name = request.Name;
                    _cTermRepository.Update(cterm);
                    await _unitOfWork.Commit();

                    if (request.TermMetas is not null)
                    {

                        foreach (var term in termMetas)
                        {
                            foreach (var item in request.TermMetas)
                            {
                                if (term.Id == item.TermId)
                                {
                                    if (item.MetaKey is not null) term.MetaKey = item.MetaKey;
                                    if (item.MetaValue is not null) term.MetaValue = item.MetaValue;
                                    term.UpdatedAt = DateTime.Now;
                                }
                            }
                        }
                        _cTermmetumRepository.UpdateRange(termMetas);
                        await _unitOfWork.Commit();
                    }
                    await _unitOfWork.Commit();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    await _eventDispatcher.RaiseEvent(new DomainNotification(request.MessageType, "Không thành công"));
                    return Unit.Value;

                }
                return Unit.Value;
            }
        }
    }
}
