using MapsterMapper;
using MediatR;
using StockFlow.Application.DTOs;
using StockFlow.Application.Interfaces;

namespace StockFlow.Application.StockMovements.Queries;

public class GetAllStockMovementsQueryHandler : IRequestHandler<GetAllStockMovementsQuery, IEnumerable<StockMovementDto>>
{
    private readonly IRepository<Domain.Entities.StockMovement> _stockMovementRepository;
    private readonly IMapper _mapper;

    public GetAllStockMovementsQueryHandler(IRepository<Domain.Entities.StockMovement> stockMovementRepository, IMapper mapper)
    {
        _stockMovementRepository = stockMovementRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StockMovementDto>> Handle(GetAllStockMovementsQuery request, CancellationToken cancellationToken)
    {
        var movements = await _stockMovementRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<StockMovementDto>>(movements);
    }
}
