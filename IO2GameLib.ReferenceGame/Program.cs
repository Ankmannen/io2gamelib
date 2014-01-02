using System;

namespace IO2GameLib.ReferenceGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var factory = new MonoGame.Framework.GameFrameworkViewSource<TheReferenceGame>();
            Windows.ApplicationModel.Core.CoreApplication.Run(factory);
        }
    }
}
