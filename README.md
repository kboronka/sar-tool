# sar-tool
search and replace (productivity tool + much more)
  - a bunch of CLI productivity tools useful for C# build scripts
  - everything need to create a light quick and dirty MVC WebApp (http server, http client with JWT, OAuth2 authenticaion, caching, json parsing, etc)
  


## creating a webserver:
```c#
using sar.http;

void main()
{
  var server = new HttpServer(8080);
  ...  
}
```

# sar-cnc
webserver used interface with to [GRBL](https://github.com/grbl/grbl)
- has ability to jog axis from the web interface
- displays current current axis positions
- send g-code commands
- run a g-code job generated by design software such as [Fusion 360](https://academy.autodesk.com/getting-started-fusion-360)
- ui has a pic of the CNC I built
