namespace JEO3.Diagrams
{
    public enum DiagramType
    {
        ERD,
        Node
    }

    public enum LayoutPresetType
    {
        JEO,
        BalancedDefault,
        StrongCouplingClustered,
        LooseSprawlingExploration,
        HighDensityCompact
    }

    public enum GraphLayoutShape
    {
        FruchtermanReingold,
        KamadaKawai,
        EntityRelation,
        CircularStar,
        LinLog,
        ISOM,
        FruchtermanReingoldBounded,
        Sugiyama,
        TreeBallon,
        TreeDouble,
        TreeSimple,
        Random,
        None
    }

    // Specific
    public enum DiagramLayoutDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }
    public enum DiagramCoolingFunction
    {
        Exponential,
        Orthoganal
    }
    public enum DiagramEdgeRoutingFunction
    {
        Traditional,
        Orthoganal
    }
    public enum DiagramSpanTreeGeneration
    {
        //
        // Summary:
        //     BFS (Breadth-First Search).
        BFS,
        //
        // Summary:
        //     DFS (Depth-First Search).
        DFS
    }
}
