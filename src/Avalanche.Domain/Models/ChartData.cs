namespace Avalanche.Domain.Models
{
    public class ChartData
    {
        public string Name { get; set; }

        public IEnumerable<ChartDataRow> Data { get; set; }
    }
}
