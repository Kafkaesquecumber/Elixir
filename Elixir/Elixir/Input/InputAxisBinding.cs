
namespace Elixir.Input
{
    /// <summary>
    /// An axis and a scale for the value of that axis
    /// </summary>
    public struct InputAxisScalePair
    {
        /// <summary>
        /// The axis
        /// </summary>
        public InputAxis Axis { get; set; }
        /// <summary>
        /// The value by which to multiply the axis value
        /// </summary>
        public float Scale { get; set; }
    }

    /// <summary>
    /// Binding an id to an axis
    /// </summary>
    public struct InputAxisBinding
    {
        /// <summary>
        /// The identifier for this input axis binding
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The pairs contain the axes and the scales by which to multiply their values
        /// </summary>
        public InputAxisScalePair[] AxisScalePairs { get; set; }
    }
}