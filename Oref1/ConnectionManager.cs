using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using DxCK.Utils.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Oref1
{
    public class ConnectionManager
    {
        //private static readonly Uri[] _uris = Settings.Default.AlertsUrls.Cast<string>().Select(url => new Uri(url)).ToArray();

        private static readonly string _initialRequestFormat = "GET {0} HTTP/1.1\r\n" +
        "Host: {1}\r\n" +
        "Connection: keep-alive\r\n" +
        "\r\n";

        //private static readonly string _nextRequestFormat = "GET {0} HTTP/1.1\r\n" +
        //"Host: {1}\r\n" +
        //"\r\n";

        private static readonly char[] _spaceCharArray = new char[] { ' ' };

        private Uri _uri;
        private TcpClient _currentTcpClient;
        private NetworkStream _currentStream;
        private byte[] _readyNextRequest;
        private FastList<byte> _responseBuffer = new FastList<byte>(64 * 1024);
        private byte[] _buffer = new byte[64 * 1024];
        private bool _keepAlive;
        private bool _errored = true;
        private IPAddress _ip;

        public ConnectionManager(Uri uri, IPAddress ip)
        {
            _uri = uri;
            _ip = ip;
        }

        public byte[] DownloadData()
        {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                if (_currentStream == null)
                {
                    Trace.WriteLine("Connecting to " + _ip + " for " + _uri);

                    _errored = false;
                    _currentTcpClient = new TcpClient();
                    _currentTcpClient.NoDelay = true;
                    _currentTcpClient.ReceiveTimeout = 500;
                    _currentTcpClient.SendTimeout = 500;
                    _currentTcpClient.Connect(_ip, _uri.Port);
                    _currentStream = _currentTcpClient.GetStream();
                    _currentStream.ReadTimeout = 500;
                    _currentStream.WriteTimeout = 500;

                    byte[] initialBuffer = Encoding.ASCII.GetBytes(string.Format(_initialRequestFormat, _uri.ToString(), _uri.Host));
                    _currentStream.Write(initialBuffer, 0, initialBuffer.Length);

                    //_readyNextRequest = Encoding.ASCII.GetBytes(string.Format(_nextRequestFormat, uri.ToString(), uri.Host));
                    _readyNextRequest = initialBuffer;
                }
                else
                {
                    _currentStream.Write(_readyNextRequest, 0, _readyNextRequest.Length);
                }

                _responseBuffer.Clear();

                int bytesRead;

                int bytesAnalyzed;
                bool endOfHeadersReached = false;

                int contentLength = -1;
                string httpVersion = null;

                do
                {
                    bytesRead = _currentStream.Read(_buffer, 0, _buffer.Length);

                    _responseBuffer.AddRange(_buffer, 0, bytesRead);
                    IList<string> newHeaders = AnalyzeHeaders(_responseBuffer.GetInternalBuffer(), _responseBuffer.Count, out bytesAnalyzed, out endOfHeadersReached);

                    _responseBuffer.RemoveRange(0, bytesAnalyzed);

                    foreach (string header in newHeaders)
                    {
                        Trace.WriteLine(header);

                        if (header.StartsWith("HTTP/1"))
                        {
                            string[] splitted = header.Split(_spaceCharArray);
                            string httpResponseCode = splitted[1];

                            if (httpResponseCode != "200")
                            {
                                throw new InvalidDataException("httpResponseCode is " + httpResponseCode);
                            }

                            httpVersion = splitted[0];
                        }
                        else if (header.StartsWith("Content-Length:"))
                        {
                            contentLength = int.Parse(header.Split(_spaceCharArray)[1]);
                        }
                        else if (header == "Connection: keep-alive")
                        {
                            _keepAlive = true;
                        }
                        else if (header == "Connection: close")
                        {
                            _keepAlive = false;
                        }
                    }
                }
                while (!endOfHeadersReached);

                if (contentLength < 0 && httpVersion != "HTTP/1.0")
                {
                    throw new InvalidDataException("Content-Length is " + contentLength);
                }

                Trace.WriteLine("HTTP Version is " + httpVersion);

                if (httpVersion != "HTTP/1.0")
                {
                    while (_responseBuffer.Count < contentLength)
                    {
                        bytesRead = _currentStream.Read(_buffer, 0, _buffer.Length);
                        _responseBuffer.AddRange(_buffer, 0, bytesRead);
                    }
                }
                else
                {
                    _keepAlive = false;

                    try
                    {
                        while ((bytesRead = _currentStream.Read(_buffer, 0, _buffer.Length)) > 0)
                        {
                            _responseBuffer.AddRange(_buffer, 0, bytesRead);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                byte[] result = _responseBuffer.ToArray();
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Connecting to " + _ip + " for " + _uri);
                Trace.WriteLine("Exception from " + _ip + " for " + _uri + ": " + ex.ToString());

                _errored = true;
                _keepAlive = false;

                if (_currentStream != null)
                {
                    _currentStream.Close();
                    _currentStream = null;
                }

                if (_currentTcpClient != null)
                {
                    _currentTcpClient.Close();
                    _currentTcpClient = null;
                }

                throw;
            }
            finally
            {
                if (!_keepAlive)
                {
                    if (_currentStream != null)
                    {
                        _currentStream.Close();
                        _currentStream = null;
                    }

                    if (_currentTcpClient != null)
                    {
                        _currentTcpClient.Close();
                        _currentTcpClient = null;
                    }
                }

                Trace.WriteLine("DownloadData " + sw.Elapsed);
            }

            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            ////request.KeepAlive = true;
            ////request.IfModifiedSince = _lastModified;

            ////request.Host = "www.oref.org.il";

            //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //{
            //    //_lastModified = response.LastModified;

            //    using (Stream responseStream = response.GetResponseStream())
            //    using (StreamReader reader = new StreamReader(responseStream, Encoding.Unicode))
            //    {
            //        return reader.ReadToEnd();
            //    }
            //}
        }

        private IList<string> AnalyzeHeaders(byte[] buffer, int count, out int bytesAnalyzed, out bool endOfHeadersReached)
        {
            endOfHeadersReached = false;
            bytesAnalyzed = 0;
            List<string> headers = new List<string>();

            for (int i = 0; i < count - 1; i++)
            {
                if (buffer[i] == '\r' && buffer[i + 1] == '\n')
                {
                    string header = Encoding.ASCII.GetString(buffer, bytesAnalyzed, i - bytesAnalyzed);

                    if (!string.IsNullOrEmpty(header))
                    {
                        headers.Add(header);

                        bytesAnalyzed = i + 2;
                    }
                    else
                    {
                        bytesAnalyzed = i + 2;
                        endOfHeadersReached = true;
                        break;
                    }
                }
            }

            return headers;
        }
    }
}
