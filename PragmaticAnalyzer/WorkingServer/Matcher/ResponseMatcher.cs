namespace PragmaticAnalyzer.WorkingServer.Matcher
{
    public class ResponseMatcher
    {
        public float Coefficient { get; set; }
        public Guid ExploitId { get; set; }
        public Guid TechnologyId { get; set; }
        public Guid ConsequenceIde { get; set; }
        public Guid ProtectionMeasureId { get; set; }
        public Guid TacticId { get; set; }
        public Guid ThreadId { get; set; }
        public Guid VulnerabilitieId { get; set; }
        public Guid ViolatorId { get; set; }
    }
}