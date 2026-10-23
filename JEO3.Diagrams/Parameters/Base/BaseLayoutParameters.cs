namespace JEO3.Diagrams
{
    /// <summary>
    /// Base configuration parameters shared by or required dynamically by layout algorithms.
    /// </summary>
    public abstract class BaseLayoutParameters
    {
        // System wide sizing inputs from JSInterop canvas measurements
        public double CanvasWidth { get; set; } = 1920;
        public double CanvasHeight { get; set; } = 1280;
        public bool UseEntityRelationMode { get; set; }

        /// <summary>
        /// Creates a deep-copy preset clone for isolated tweaking.
        /// </summary>
        public abstract BaseLayoutParameters Clone();
    }
}
