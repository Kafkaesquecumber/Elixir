namespace Elixir.Graphics
{
    /// <summary>
    /// The way the texture is sampled outside the coordinate range of 0 to 1
    /// </summary>
    public enum TextureWrapMode
    {
        /// <summary>
        /// The integer part of the coordinate is ignored and a repeated pattern is formed
        /// </summary>
        Repeat,
        /// <summary>
        /// Same as Repeat but it will be mirrored when the integer part of the coordinate is odd
        /// </summary>
        MirroredRepeat,
        /// <summary>
        /// The coordinate will be clamped between 0 and 1
        /// </summary>
        ClampToEdge,
        /// <summary>
        /// The coordinates that fall outside the range will be given a border
        /// </summary>
        ClampToBorder
    }
}