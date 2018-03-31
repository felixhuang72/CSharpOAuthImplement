using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiNetCore_Jwt.Models;

namespace WebApiNetCoreJwt
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //站台設定檔轉換為強型別
            services.Configure<SysConfig>(Configuration);

            //regist HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Json Web Token (JWT) 設定

            //取得 Jwt:Audiences 清單
            Database db = new Database();
            IEnumerable<string> _audience = db.Audiences.Select(x => x.AudienceId);


            //從設定文件中獲取 Jwt 相關設定資訊            
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtConfig));

            //設定 Jwt 密鑰及簽發憑證
            //type1: secret 為未編碼字串
            //var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingOptions[nameof(JwtOption.JwtPlainSecret)]));

            //type2: secret 已透過 Base64 編碼
            byte[] decodedBytes = Convert.FromBase64String(jwtAppSettingOptions[nameof(JwtConfig.JwtBase64Secret)]);
            var secret = new SymmetricSecurityKey(decodedBytes);

            //使用 secret，建立簽發憑證
            var creds = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            //將發行者、受眾者、簽發憑證指定至 Jwt 保留項 (JwtOptions)，之後建立 Token 時可直接使用
            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtConfig.JwtIssuer)];
                options.Audience = _audience == null || _audience.Count() == 0 ? null : string.Join(',', _audience);
                options.SigningCredentials = creds;
                options.ValidFor = TimeSpan.FromMinutes(int.Parse(jwtAppSettingOptions[nameof(JwtConfig.JwtExpiredTimer)]));
            });

            //自服務中添加 Jwt 使用者驗證機制            
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            } /*Uncomment this if you don't want to use JWT for all of your apis*/)
               .AddJwtBearer(cfg =>
               {
                   cfg.RequireHttpsMetadata = false;
                   cfg.SaveToken = true;
                   cfg.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidIssuer = jwtAppSettingOptions[nameof(JwtConfig.JwtIssuer)],
                       ValidAudiences = _audience,
                       IssuerSigningKey = secret
                   };
               });

            //定義角色授權驗證方案 (Policy)
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminAuthorize", policy => policy.RequireRole("SysAdmin", "Admin"));
                options.AddPolicy("SuperAuthorize", policy => policy.RequireRole("SysAdmin"));
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //使用驗證
            app.UseAuthentication();

            //跨網域存取設定
            app.UseCors(builder => builder.WithOrigins("*")
                                          .WithMethods("POST", "GET")// AllowAnyMethod()
                                          .AllowAnyHeader()
                                          .AllowCredentials()
                                          );

            app.UseMvc();
        }
    }
}
