using JEO3.Schema;

namespace JEO3.Generation.Models
{
    /// <summary>
    /// One row in the heatmap: a table plus its four normalized (0-100, higher = worse) severity
    /// scores. Raw values are also kept so the UI can show real numbers in tooltips.
    /// </summary>
    public sealed class TableHealthMetric
    {
        public required int? ObjectId { get; init; }
        public required string TablePath { get; init; }
        public required string SchemaName { get; init; }
        public required string Name { get; init; }

        public double RawFragmentationPercent { get; init; }
        public double RawUnindexedFkCount { get; init; }
        public double RawCoveragePercent { get; init; }
        public int RawMissingIndexCount { get; init; }

        public double FragmentationSeverity { get; init; }
        public double UnindexedFkSeverity { get; init; }
        public double CoverageSeverity { get; init; }
        public double MissingIndexSeverity { get; init; }
    }
}
