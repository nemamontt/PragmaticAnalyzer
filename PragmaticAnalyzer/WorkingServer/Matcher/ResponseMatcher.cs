using System.Collections.ObjectModel;

namespace PragmaticAnalyzer.WorkingServer.Matcher
{
    public class ResponseMatcher
    {   
        public ObservableCollection<MatcherObject> MatcherObjects { get; set; }

        public class MatcherObject
        {
            public float Coefficient { get; set; }
            public Dictionary<Guid, string> Sources { get; set; }
        }
    }
}