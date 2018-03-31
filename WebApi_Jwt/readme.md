# 說明

# 需安裝套件
本專案使用 Json Web Token 搭配 ASP.NET 驗證機制，進行 Controller 存取授權限制，專案需安裝以下套件
<pre>Install-Package Microsoft.AspNet.WebApi<!-- -Version 5.2.4-->
Install-Package Microsoft.AspNet.WebApi.Owin<!-- -Version 5.2.4-->
Install-Package Microsoft.Owin.Host.SystemWeb<!-- -Version 4.0.0-->
Install-Package Microsoft.Owin.Cors <!-- -Version 4.0.0-->
Install-Package Microsoft.Owin.Security.OAuth<!-- -Version 4.0.0-->
Install-Package Microsoft.Owin.Security.Jwt<!-- -Version 4.0.0--></pre>

※套件安裝完畢後，請務必執行<code>Update-Package</code> 指令以更新套件版本


# 參考
- Json Web Token 產出與驗證實作:<br />[JSON Web Token in ASP.NET Web API 2 using Owin](
http://bitoftech.net/2014/10/27/json-web-token-asp-net-web-api-2-jwt-owin-authorization-server/)