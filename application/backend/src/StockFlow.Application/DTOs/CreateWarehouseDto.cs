namespace StockFlow.Application.DTOs;

public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Capacity { get; set; }
}
