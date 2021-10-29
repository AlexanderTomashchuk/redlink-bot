namespace Domain.Entities
{
    public class Country
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Flag { get; set; }

        public Language DefaultLanguage { get; set; }

        public Currency DefaultCurrency { get; set; }
    }
}