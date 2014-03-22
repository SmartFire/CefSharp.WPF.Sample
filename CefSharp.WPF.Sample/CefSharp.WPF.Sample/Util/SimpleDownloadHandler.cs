using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefSharp.WPF.Sample.Util
{
    public class SimpleDownloadHandler : IDownloadHandler
    {
        private Stream _stream;

        public SimpleDownloadHandler(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    _stream = File.Create(fileName);
                }
                catch (Exception e)
                {
                    // log
                }
            }
            else
            {
                throw new ArgumentException("fileName");
            }
        }

        #region IDownloadHandler
        public bool ReceivedData(byte[] data)
        {
            try
            {
                if (_stream != null)
                {
                    _stream.Write(data, 0, data.Length);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void Complete()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream = null;
                this.OnDownloadCompleted();
            }
        }

        private void OnDownloadCompleted()
        {
        } 
        #endregion
    }
}
