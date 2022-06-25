using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Gateway.Controllers;

public class ConfigController : Controller
{
    


    public IActionResult Read()
    {
        using var r = new StreamReader("ocelot.json");
        var json = r.ReadToEnd();
        var ocelotInfo = JsonConvert.DeserializeObject<MainRoot>(json);
        return View(ocelotInfo);
    }

 
    public IActionResult New()
    {
        var emptyModel = new FormViewModal()
        {
            RequestIdKey = Guid.Empty
        };
        
        return View(emptyModel);
    }

    [HttpPost]
    public IActionResult Save(FormViewModal model)
    {
        if (!ModelState.IsValid)
        {
            return View("New", model);
        }

        using var r = new StreamReader("ocelot.json");
        var json = r.ReadToEnd();
        var ocelotInfo = JsonConvert.DeserializeObject<MainRoot>(json);

        r.Close();
        
        //find duplicate data

        var findDuplicateHostPort = ocelotInfo.Routes
            .Any(x => x.DownstreamHostAndPorts
                .Any(y => y.Host == model.Host && y.Port == model.Port));

        if (findDuplicateHostPort)
        {
            return Json(new { state = "error", message = "Route is duplicated" });
        }

        if (model.RequestIdKey==Guid.Empty)
        {
            var reqId = Guid.NewGuid();
            var newRoute = new Models.Route()
            {
                RequestIdKey = reqId,
                DownstreamPathTemplate = model.DownstreamPathTemplate,
                DownstreamScheme = model.DownstreamScheme,
                SwaggerKey = model.swaggerKey,
                UpstreamPathTemplate = model.UpstreamPathTemplate,
                UpstreamHttpMethod = model.UpstreamHttpMethod,
                DownstreamHostAndPorts = new List<DownstreamHostAndPort>()
                {
                    new()
                    {
                        Host = model.Host,
                        Port = model.Port
                    }
                }
            };

            var newSwaggerConfig = new Models.SwaggerEndPoint
            {
                RequestIdKey = reqId,
                Config = new List<Config>()
                {
                    new()
                    {
                        Name = model.swaggerName,
                        Url = model.swaggerAddress,
                        Version = model.swaggerVersion
                    }
                },
                Key = model.swaggerKey,
                TakeServersFromDownstreamService = true
            };


            ocelotInfo!.Routes.Add(newRoute);
            ocelotInfo!.SwaggerEndPoints.Add(newSwaggerConfig);

            var jsonFile = JsonConvert.SerializeObject(ocelotInfo);

            System.IO.File.WriteAllText("ocelot.json", jsonFile);
        }
        else
        {
            var findRoute = ocelotInfo.Routes.Find(x => x.RequestIdKey == model.RequestIdKey);
            if (findRoute==null)
            {
                var newRoute = new Models.Route()
                {
                    RequestIdKey = model.RequestIdKey,
                    SwaggerKey = model.swaggerKey,
                    UpstreamPathTemplate = model.UpstreamPathTemplate,
                    UpstreamHttpMethod = model.UpstreamHttpMethod,
                    
                    DownstreamPathTemplate = model.DownstreamPathTemplate,
                    DownstreamScheme = model.DownstreamScheme,
                    DownstreamHostAndPorts = new List<DownstreamHostAndPort>()
                    {
                        new()
                        {
                            Host = model.Host,
                            Port = model.Port
                        }
                    }
                };
                ocelotInfo.Routes.Add(newRoute);
            }
            else
            {
               
                findRoute.UpstreamPathTemplate = model.UpstreamPathTemplate;
                findRoute.UpstreamHttpMethod = model.UpstreamHttpMethod;
                
                findRoute.DownstreamPathTemplate = model.DownstreamPathTemplate;
                findRoute.DownstreamScheme = model.DownstreamScheme;
                findRoute.DownstreamHostAndPorts = new List<DownstreamHostAndPort>()
                {
                    new()
                    {
                        Host = model.Host,
                        Port = model.Port
                    }
                };
                
                
            }

            var swaggerEndpoint = ocelotInfo.SwaggerEndPoints.Find(x => x.Key == model.swaggerKey);
            if (swaggerEndpoint==null)
            {
                var newSwaggerConfig = new Models.SwaggerEndPoint
                {
                    RequestIdKey = model.RequestIdKey,
                    Config = new List<Config>()
                    {
                        new()
                        {
                            Name = model.swaggerName,
                            Url = model.swaggerAddress,
                            Version = model.swaggerVersion
                        }
                    },
                    Key = model.swaggerKey,
                    TakeServersFromDownstreamService = true
                };
                
                ocelotInfo.SwaggerEndPoints.Add(newSwaggerConfig);
            }
            else
            {
                swaggerEndpoint.Config[0].Name = model.swaggerName;
                swaggerEndpoint.Config[0].Url = model.swaggerAddress;
                swaggerEndpoint.Config[0].Version = model.swaggerVersion;
            }
            
            var jsonFile = JsonConvert.SerializeObject(ocelotInfo);

            System.IO.File.WriteAllText("ocelot.json", jsonFile);
        }

        // return RedirectToAction("Read");
        return Json(new { state = "ok" });
    }


    public IActionResult Edit(Guid reqId)
    {
        using var r = new StreamReader("ocelot.json");
        var json = r.ReadToEnd();
        var ocelotInfo = JsonConvert.DeserializeObject<MainRoot>(json);

        var findRoute = ocelotInfo!.Routes.Find(x => x.RequestIdKey == reqId);
        var findConfig = ocelotInfo.SwaggerEndPoints.Find(x => x.RequestIdKey == reqId);
        var emptyModel = new FormViewModal();
        if (findRoute == null)
        {
            return View("New", emptyModel);
        }

        emptyModel.RequestIdKey = reqId;
        emptyModel.Host = findRoute!.DownstreamHostAndPorts[0].Host;
        emptyModel.Port = findRoute!.DownstreamHostAndPorts[0].Port;
        emptyModel.swaggerKey = findRoute.SwaggerKey;
        emptyModel.DownstreamScheme = findRoute.DownstreamScheme;
        emptyModel.DownstreamPathTemplate = findRoute.DownstreamPathTemplate;
        emptyModel.UpstreamHttpMethod = findRoute.UpstreamHttpMethod;
        emptyModel.UpstreamPathTemplate = findRoute.UpstreamPathTemplate;

        emptyModel.swaggerAddress = findConfig != null ? findConfig.Config[0].Url : "";
        emptyModel.swaggerName = findConfig != null ? findConfig.Config[0].Name : "";
        emptyModel.swaggerVersion = findConfig != null ? findConfig.Config[0].Version : "";

        return View("New", emptyModel);
    }
}