using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace JEO3.Monitor
{
    [XmlRoot(ElementName = "ShowPlanXML", Namespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan")]

    public class ShowPlanXmlContainer
    {
        [XmlArray("BatchSequence")]
        [XmlArrayItem("Batch", typeof(PlanBatch))]
        public List<PlanBatch> Batches { get; set; } = new();
    }

    public class PlanBatch
    {
        [XmlArray("Statements")]
        [XmlArrayItem("StmtSimple", typeof(PlanStatement))]
        public List<PlanStatement> Statements { get; set; } = new();
    }

    public class PlanStatement
    {
        [XmlAttribute("StatementText")]
        public string StatementText { get; set; } = string.Empty;

        [XmlElement("QueryPlan")]
        public QueryPlanNode? QueryPlan { get; set; }
    }

    public class QueryPlanNode
    {
        [XmlElement("RelOp")]
        public RelOpNode? RootRelOp { get; set; }
    }

    public class RelOpNode
    {
        [XmlAttribute("PhysicalOp")]
        public string PhysicalOp { get; set; } = string.Empty;

        [XmlAttribute("LogicalOp")]
        public string LogicalOp { get; set; } = string.Empty;

        [XmlAttribute("EstimateRows")]
        public double EstimateRows { get; set; }

        [XmlAttribute("AvgRowSize")]
        public double AvgRowSize { get; set; }

        [XmlAttribute("EstimatedTotalSubtreeCost")]
        public double SubtreeCost { get; set; }

        [XmlArray("OutputList")]
        [XmlArrayItem("ColumnReference")]
        public List<ColumnRef> OutputColumns { get; set; } = new();

        [XmlArray("IndexScan")] // Handles scans/seeks
        [XmlArrayItem("Object")]
        public List<TargetObject> TargetObjects { get; set; } = new();

        [XmlArray("RelOp")]
        public List<RelOpNode> ChildNodes { get; set; } = new();
    }

    public class ColumnRef
    {
        [XmlAttribute("Column")] public string Name { get; set; } = string.Empty;
    }

    public class TargetObject
    {
        [XmlAttribute("Table")] public string TableName { get; set; } = string.Empty;
        [XmlAttribute("Index")] public string IndexName { get; set; } = string.Empty;
    }
}
