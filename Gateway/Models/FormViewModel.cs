using System;
using System.Collections.Generic;

namespace Gateway.Models;

public class FormViewModal
{
    public Guid RequestIdKey { get; set; }
    public string UpstreamPathTemplate { get; set; }
    public List<string> UpstreamHttpMethod { get; set; }
    public string DownstreamScheme { get; set; }
    public string DownstreamPathTemplate { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string swaggerKey { get; set; }
    public string swaggerName { get; set; }
    public string swaggerVersion { get; set; }
    public string swaggerAddress { get; set; }
    
}