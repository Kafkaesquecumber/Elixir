using System;
using Glaives.Core;

namespace Glaives.LevelEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Get.Initialize<LevelEditorInstance, MainLevel>();
        }
    }
}
