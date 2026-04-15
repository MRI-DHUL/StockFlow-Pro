using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.StockMovements.Queries;

public class GetStockMovementByIdQueryHandler : IRequestHandler<GetStockMovementByIdQuery, StockMovementDto?>
{
    private readonly IRepository<Domain.Entities.StockMovement> _stockMovementRepository;
    private readonly IMapper _mapper;

    public GetStockMovementByIdQueryHandler(IRepository<Domain.Entities.StockMovement> stockMovementRepository, IMapper mapper)
    {
        _stockMovementRepository = stockMovementRepository;
        _mapper = mapper;
    }

    public async Task<StockMovementDto?> Handle(GetStockMovementByIdQuery request, CancellationToken cancellationToken)
    {
        var movement = await _stockMovementRepository.GetByIdAsync(request.Id, cancellationToken);
        return movement != null ? _mapper.Map<StockMovementDto>(movement) : null;
    }
}
