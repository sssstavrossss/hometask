namespace HometaskLib.Models.Dto;

public class CurrencyConversionResponse
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal ConvertedAmount { get; set; }
}