using System.Collections.Generic;

namespace JollySamurai.UnrealEngine4.Import
{
    public class ResultSetBuilder
    {
        public bool HasErrors {
            get { return _hasErrors; }
        }

        public bool HasWarnings {
            get { return _hasWarnings; }
        }

        private List<Problem> _problems = new List<Problem>();
        private bool _hasErrors;
        private bool _hasWarnings;

        public void AddProblem(ProblemSeverity severity, string fileName, string message)
        {
            _problems.Add(new Problem(severity, fileName, message));

            if(severity == ProblemSeverity.Fatal) {
                _hasErrors = true;
            } else if(severity == ProblemSeverity.Warning) {
                _hasWarnings = true;
            }
        }

        public void AddProcessorProblems(T3D.Processor.Problem[] resultProblems, string documentFileName)
        {
            foreach (var resultProblem in resultProblems) {
                AddProblem(TranslateSeverity(resultProblem.Severity), documentFileName, resultProblem.Message);
            }
        }

        public ResultSet ToResultSet()
        {
            return new ResultSet(_problems.ToArray(), _hasErrors, _hasWarnings);
        }

        private ProblemSeverity TranslateSeverity(T3D.Processor.ProblemSeverity severity)
        {
            switch (severity) {
                case T3D.Processor.ProblemSeverity.Error:
                    return ProblemSeverity.Fatal;
                case T3D.Processor.ProblemSeverity.Warning:
                    return ProblemSeverity.Warning;
            }

            throw new System.Exception("Unexpected severity");
        }
    }

    public class ResultSet
    {
        public Problem[] Problems { get; }
        public bool HasErrors { get; }
        public bool HasWarnings { get; }

        internal ResultSet(Problem[] problems, bool hasErrors, bool hasWarnings)
        {
            Problems = problems;
            HasErrors = hasErrors;
            HasWarnings = hasWarnings;
        }
    }

    public class Problem
    {
        public ProblemSeverity Severity { get; }
        public string FileName { get; }
        public string Message { get; }

        public Problem(ProblemSeverity severity, string fileName, string message)
        {
            Severity = severity;
            FileName = fileName;
            Message = message;
        }
    }

    public enum ProblemSeverity
    {
        Fatal,
        Warning
    }
}
