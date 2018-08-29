using System;

namespace Elixir.Internal.Interface
{
    internal static class InterfaceManager 
    {
        internal static Type GetInterfaceType(InterfaceType interfaceType)
        {
            // Add new interface types in this function 
            
            switch (interfaceType)
            {
                case InterfaceType.None:
                    break;
                case InterfaceType.Window:
                    return typeof(Implementation.SDLImpl.SdlWindow); 
                case InterfaceType.Texture:
                    return typeof(Implementation.ImageSharpImpl.ImageSharpTexture);
                case InterfaceType.Audio:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(interfaceType), interfaceType, null);
            }

            return null;
        }

        internal static Interface CreateInterface(InterfaceType interfaceType) 
        {
            Type type = GetInterfaceType(interfaceType);
            if(type == null)
            {
                throw new SgeException("No interface type found for " + interfaceType);
            }

            Interface instance = (Interface)Activator.CreateInstance(type);
            if(instance == null)
            {
                throw new SgeException("Failed to create instance of interface for " + type.Name);
            }
            return instance;
        }
    }
}
