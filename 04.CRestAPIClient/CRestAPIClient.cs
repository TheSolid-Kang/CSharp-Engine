using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Engine._04.CRestAPIClient
{

  public class CRestAPIClient : IDisposable
  {
    //defualt 생성자
    public CRestAPIClient(string _send_url) : this(_send_url, "xml", "GET") { }
    public CRestAPIClient(string _send_url, string _content_type) : this(_send_url, _content_type, "GET") { }
    public CRestAPIClient(string _send_url, string _content_type, string _method)
    {
      this._send_url = _send_url ?? throw new ArgumentNullException(nameof(_send_url));
      this._content_type = _content_type ?? throw new ArgumentNullException(nameof(_content_type));
      this._method = _method ?? throw new ArgumentNullException(nameof(_method));
      this._content_type = _content_type.ToLower();
      _web_client = null;
    }

    ~CRestAPIClient() => this?.Dispose();
    #region Member Variable
    //private WebRequest _web_request;
    private WebClient _web_client;

    private string _send_url;
    private string _content_type;
    private string _method;
    #endregion

    public void Dispose()
    {
      _web_client?.Dispose();
      _web_client = null;
      //_web_request = null;
    }

    private string _GetRequestResult()
    {
      string response_from_Server = string.Empty;
      try
      {
        _web_client = new WebClient() { Encoding = Encoding.UTF8 };
        response_from_Server = _web_client.DownloadString(new Uri(_send_url));
      }
      catch (Exception _e)
      {
        System.Diagnostics.Debug.WriteLine(_e.Message);
      }
      finally
      {
        _web_client?.Dispose();
        _web_client = null;
        GC.SuppressFinalize(this);
      }
      return response_from_Server;
    }

    private string _GetOriResponse() => _GetRequestResult();
    public JObject GetJsonResponse()
    {
      if (!_content_type.Equals("json")) throw new ArgumentException("contents type이 json이 아닙니다.");
      return JObject.Parse(_GetOriResponse());
    }
    public XmlDocument GetXmlResponse()
    {
      if (!_content_type.Equals("xml")) throw new ArgumentException("contents type이 xml이 아닙니다.");
      XmlDocument xml_document = new XmlDocument();
      xml_document.LoadXml(_GetOriResponse());
      return xml_document;
    }
  }

}
