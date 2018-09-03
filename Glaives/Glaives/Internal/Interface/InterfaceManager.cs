// MIT License
// 
// Copyright(c) 2018 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;

namespace Glaives.Internal.Interface
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
                throw new GlaivesException("No interface type found for " + interfaceType);
            }

            Interface instance = (Interface)Activator.CreateInstance(type);
            if(instance == null)
            {
                throw new GlaivesException("Failed to create instance of interface for " + type.Name);
            }
            return instance;
        }
    }
}
