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
