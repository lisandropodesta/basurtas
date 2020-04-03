using Microsoft.AspNetCore.Components;
using System;

namespace Basurtas.Model
{
    public class GamePage : ComponentBase, IDisposable
    {
        /// <summary>
        /// Closes the game.
        /// </summary>
        void IDisposable.Dispose()
        {
            DoCloseAll();
        }

        protected virtual void DoCloseAll()
        {
        }
    }
}
