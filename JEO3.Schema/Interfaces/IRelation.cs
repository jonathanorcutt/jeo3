namespace JEO3.Schema
{
    public interface IRelation : IObject
    {
        string KeyName { get; }

        long? RelationId { get; }
        RelationType RelationType { get; }
        long? ParentObjectId { get; }
        long? ReferencedObjectId { get; }
        bool IsDisabled { get; }
        bool IsDeferrable { get; }
        bool IsInitiallyDeferred { get; }
        bool IsEnforced { get; }
        bool IsNullable { get; }
        public bool IsNotTrusted { get; }

        IReadOnlyList<IRelationColumnPair> ColumnPairs { get; }
        ITable? ParentTable { get; }
        ITable? ReferencedTable { get; }
    }
}
