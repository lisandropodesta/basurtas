using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Bazurtas.Model
{
    public class GamePage : ComponentBase, IDisposable
    {
        protected const string CookieUserNickName = "UserNickName";

        [Inject]
        IJSRuntime jsRuntime { get; set; }

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

        private bool isComponentReady;

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            var task = base.OnAfterRenderAsync(firstRender);

            if (!isComponentReady)
            {
                isComponentReady = true;
                OnComponentReady();
            }

            return task;
        }

        protected virtual void OnComponentReady()
        {
        }

        protected async void GetCookie(string name, Action<string> action)
        {
            var cookie = await jsRuntime.InvokeAsync<string>("blazorExtensions.ReadCookie");

            var terms = cookie.Split(';');
            foreach (var term in terms)
            {
                var parts = term.Split('=');
                if (parts.Length == 2)
                {
                    if (parts[0].Trim() == name)
                    {
                        action(parts[1].Trim());
                        return;
                    }
                }
            }
        }

        protected void SetCookie(string name, string value)
        {
            try
            {
                jsRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", name, value);
            }
            catch
            {
            }
        }

        protected void SafeAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception x)
            {
                Alert(x.Message);
            }
        }

        protected void Alert(string message)
        {
            jsRuntime.InvokeAsync<object>("blazorExtensions.ShowAlert", message);
        }
    }
}
