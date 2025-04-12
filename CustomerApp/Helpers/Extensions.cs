using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerApp.Helpers
{
    public enum HttpExceptionCause
    {
        NotAnHttpException,
        Unknown,
        NoInternet,
        RequestLost,
    }
    public static class Extensions
    {
        public static HttpExceptionCause GetHttpExceptionCause(this Exception e)
        {
            if (e is not HttpRequestException httpExc)
            {
                return HttpExceptionCause.NotAnHttpException;
            }
            if (httpExc.InnerException is Exception innerEx)
            {

                switch (innerEx)
                {
                    case SocketException socketEx:
                        {
                            switch (socketEx.SocketErrorCode)
                            {
                                case SocketError.NotSocket:
                                case SocketError.AddressNotAvailable:
                                case SocketError.NetworkDown:
                                case SocketError.NetworkUnreachable:
                                case SocketError.NetworkReset:
                                case SocketError.ConnectionAborted:
                                case SocketError.ConnectionReset:
                                case SocketError.NotConnected:
                                case SocketError.Shutdown:
                                case SocketError.ConnectionRefused:
                                case SocketError.HostDown:
                                case SocketError.HostUnreachable:
                                case SocketError.HostNotFound:
                                case SocketError.SystemNotReady:
                                    return HttpExceptionCause.NoInternet;
                                case SocketError.TimedOut:
                                case SocketError.NoData:
                                    return HttpExceptionCause.RequestLost;
                            }
                        }
                        break;
                    case WebException webEx:
                        {
                            switch (webEx.Status)
                            {
                                case WebExceptionStatus.NameResolutionFailure:
                                case WebExceptionStatus.ConnectFailure:
                                case WebExceptionStatus.ReceiveFailure:
                                case WebExceptionStatus.SendFailure:
                                case WebExceptionStatus.PipelineFailure:
                                case WebExceptionStatus.RequestCanceled:
                                case WebExceptionStatus.ConnectionClosed:
                                case WebExceptionStatus.TrustFailure:
                                case WebExceptionStatus.SecureChannelFailure:
                                case WebExceptionStatus.ServerProtocolViolation:
                                case WebExceptionStatus.KeepAliveFailure:
                                case WebExceptionStatus.ProxyNameResolutionFailure:
                                    return HttpExceptionCause.NoInternet;
                                case WebExceptionStatus.Timeout:
                                    return HttpExceptionCause.RequestLost;
                            }
                        }
                        break;
                    case IOException IOEx:
                        {
                            Console.WriteLine(JsonSerializer.Serialize(IOEx.Data));
                        }
                        break;
                    case TimeoutException _:
                    case OperationCanceledException _:
                        return HttpExceptionCause.RequestLost;
                }
                var name = innerEx.GetType().Name;
                switch (name)
                {
                    case nameof(Java.Net.UnknownHostException):
                    case nameof(Java.Net.BindException):
                    case nameof(Java.Net.ConnectException):
                    case nameof(Java.Net.NoRouteToHostException):
                    case nameof(Java.Net.PortUnreachableException):
                        return HttpExceptionCause.NoInternet;
                    case nameof(Java.Net.SocketTimeoutException):
                    case nameof(Java.Util.Concurrent.TimeoutException):
                    case nameof(Java.Util.Concurrent.CancellationException):
                        return HttpExceptionCause.NoInternet;
                }
            }

            return HttpExceptionCause.Unknown;
        }
        public static ImageSource? ToImageSourceFromUrl(this string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                return null;
            }
            try
            {
                return new UriImageSource()
                {
                    Uri = new(url),
                    CachingEnabled = true,
                    CacheValidity = TimeSpan.FromDays(1)
                };
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
                return null;
            }
        }
    }
}
