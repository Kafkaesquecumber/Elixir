using System;

namespace Elixir.GameFramework
{
    public interface ILoadableContent : IDisposable
    {
        void Dispose();
        bool IsDisposed();
    }
}