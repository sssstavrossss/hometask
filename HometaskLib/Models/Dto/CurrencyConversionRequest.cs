namespace HometaskLib.Models.Dto;

public class CurrencyConversionRequest
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
}