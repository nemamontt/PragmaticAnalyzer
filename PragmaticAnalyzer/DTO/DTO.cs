namespace PragmaticAnalyzer.DTO
{
    public class DTO<T>
    {
        public required DataType DtoType { get; set; }
        public required T Value { get; set; }
        public required DateTime DateCreation { get; set; }
    }
}
