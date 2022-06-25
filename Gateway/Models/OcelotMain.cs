using System;
using System.Collections.Generic;

namespace Gateway.Models;


public class Config
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string Url { get; set; }
}

public class DownstreamHostAndPort
{
    public string Host { get; set; }
    public int Port { get; set; }
}

public class MainRoot
{
   
    public List<Route> Routes { get; set; } = new();
    public List<SwaggerEndPoint> SwaggerEndPoints { get; set; } = new();
}

public class Route
{
    public Guid RequestIdKey { get; set; }
    public string UpstreamPathTemplate { get; set; }
    public List<string> UpstreamHttpMethod { get; set; } = new();
    public string DownstreamScheme { get; set; }
    public string DownstreamPathTemplate { get; set; }
    public List<DownstreamHostAndPort> DownstreamHostAndPorts { get; set; } = new();
    public string SwaggerKey { get; set; }
}

public class SwaggerEndPoint
{
  
    public Guid RequestIdKey { get; set; }
    public string Key { get; set; }
    public bool TakeServersFromDownstreamService { get; set; }
    public List<Config> Config { get; set; } = new();
}