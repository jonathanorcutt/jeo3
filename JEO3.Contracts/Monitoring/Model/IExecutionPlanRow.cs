namespace JEO3.Contracts
{
    public interface IExecutionPlanRow
    {
        string FullBatchPlanXml { get; set; }
        string SpecificStatementPlanXml { get; set; }
    }
}
