using CefSharp.WPF.Sample.Util;
using GalaSoft.MvvmLight.Command;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace CefSharp.WPF.Sample.ViewModels
{
    public class CefBrowserViewModel : ReactiveObject, IRequestHandler, ILoadHandler
    {
        static CefBrowserViewModel()
        {
            CefBrowserViewModel.InitCEF();
        }

        public CefBrowserViewModel(IWebBrowser browser, Dispatcher dispatcher)
        {
            if (browser == null)
            {
                throw new ArgumentNullException("browser");
            }
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            this._browser = browser;
            this._dispatcher = dispatcher;

            this._browser.PropertyChanged += OnBrowserPropertyChanged;
            this._browser.RequestHandler = this;

            this.GoCommand = new RelayCommand(this.NavigateTo, () => { return !this.IsLoading && this.IsBrowserInitialized; });
        }


        private void OnBrowserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // sender is not the IWebBrowser injected. I guess this is a defect, but doesn't matter.
            if (this._browser!= null)
            {
                if (string.Compare(e.PropertyName, "IsLoading", true) == 0)
                {
                    this.IsLoading = this._browser.IsLoading;
                }
                else if (string.Compare(e.PropertyName, "IsBrowserInitialized", true) == 0)
                {
                    this.IsBrowserInitialized = this._browser.IsBrowserInitialized;
                    if (this.IsBrowserInitialized)
                    {
                        this.Url = this._homepage;
                        this.NavigateTo();
                    }
                }
            }
        }

        private IWebBrowser _browser;
        private Dispatcher _dispatcher;

        #region Properties
        private string _homepage = "http://github.com/cefsharp/CefSharp";

        public string Homepage
        {
            get { return _homepage; }
            set { _homepage = value; }
        }

        private string _url;

        public string Url
        {
            get { return _url; }
            set { this.RaiseAndSetIfChanged(ref _url, value); }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value);
            (this.GoCommand as RelayCommand).RaiseCanExecuteChanged();
            }
        }

        private bool _isBrowserInitialized;

        public bool IsBrowserInitialized
        {
            get { return _isBrowserInitialized; }
            set { this.RaiseAndSetIfChanged(ref _isBrowserInitialized, value); }
        }

        public ICommand GoCommand { get; private set; }
        #endregion

        private static void InitCEF()
        {
            Settings settings = new Settings()
            {
                // PackLoadingDisabled = true,  // PackLoadingDisabled is to disable developer tools.
                LogSeverity = CefSharp.LogSeverity.Error,
                LogFile = Path.Combine(".\\", "Data", "CefSharp.log"), // Set the log file path.
            };
            if (CEF.Initialize(settings))
            {
                // CEF is initialized
                Console.WriteLine("CEF is initialized");
            }
        }

        private void NavigateTo()
        {
            if (this._dispatcher.CheckAccess())
            {
                this.NavigateToImpl();
            }
            else
            {
                this._dispatcher.BeginInvoke((Action)this.NavigateToImpl, DispatcherPriority.Input);
            }
        }

        private void NavigateToImpl()
        {
            this._browser.Load(this.Url);
        }

        #region IRequestHandler

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browser, bool isProxy, string host, int port, string realm, string scheme, ref string username, ref string password)
        {
            return false;
        }

        bool IRequestHandler.GetDownloadHandler(IWebBrowser browser, string mimeType, string fileName, long contentLength, ref IDownloadHandler handler)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                handler = new SimpleDownloadHandler(fileName);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browser, IRequest request, NavigationType naigationvType, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnBeforeResourceLoad(IWebBrowser browser, IRequestResponse requestResponse)
        {
            return false;
        }

        void IRequestHandler.OnResourceResponse(IWebBrowser browser, string url, int status, string statusText, string mimeType, System.Net.WebHeaderCollection headers)
        {
        }

        #endregion

        #region ILoadHandler

        bool ILoadHandler.OnLoadError(IWebBrowser browser, string url, int errorCode, ref string errorText)
        {
            // log
            return true;
        } 

        #endregion
    }
}
