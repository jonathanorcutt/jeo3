namespace JEO3.Schema
{
    public class Relation : ObjectBase, IRelation
    {
        // JEO
        public override DatabaseObjectType ObjectType => DatabaseObjectType.ForeignKey;
        public bool IsNullable { get; internal init; }
        public bool IsNotTrusted { get; internal init; }
        public string KeyName { get; internal init; } = string.Empty;

        public long? RelationId { get; internal init; }
        public RelationType RelationType { get; internal init; }
        public ITable? ParentTable { get; internal init; }
        public ITable? ReferencedTable { get; internal init; }
        public long? ParentObjectId { get; internal init; }
        public long? ReferencedObjectId { get; internal init; }
        public IReadOnlyList<IRelationColumnPair> ColumnPairs { get; internal set; } = Array.Empty<IRelationColumnPair>();
        public bool IsDisabled { get; internal init; }
        public bool IsDeferrable { get; internal init; }
        public bool IsInitiallyDeferred { get; internal init; }
        public bool IsEnforced { get; internal init; }
        public override IObject? Parent { get; internal set; }
        public override IEnumerable<IChildGrouping> Children => Array.Empty<IChildGrouping>();
    }
}