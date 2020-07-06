using JWTAuthentication.Data;
using JWTAuthentication.Helpers;
using JWTAuthentication.Models;
using JWTAuthentication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace JWTAuthentication
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
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            var smtpSection = Configuration.GetSection("SMTP");
            services.Configure<SMTP>(smtpSection);
            var smtp = smtpSection.Get<SMTP>();

            SmtpClient smtpClient = new SmtpClient(smtp.Host);
            smtpClient.Port = smtp.PortSSL;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(smtp.Username, smtp.Password);
            

            services.AddSingleton(appSettings);
            services.AddSingleton(smtp);
            services.AddSingleton(smtpClient);

            services.AddSingleton<ITokenManager, TokenManager>();
            services.AddSingleton<IRefreshTokenManager<RefreshTokenClaims>, RefreshTokenManager<RefreshTokenClaims>>();
            services.AddTransient<IMailSender, MailSender>();
            services.AddDbContext<RelacjeBazyDanychContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("RelacjeBazyDanychContext")));

            services.AddIdentity<User, IdentityRole>(x =>
                {
                    x.Password = new PasswordOptions
                    {
                        RequireUppercase = false,
                        RequireDigit = false,
                        RequiredLength = 3,
                        RequireLowercase = false,
                        RequireNonAlphanumeric = false,
                        RequiredUniqueChars = 0
                    };

                    x.SignIn = new SignInOptions
                    {
                        RequireConfirmedEmail = true
                    };
                }
                ).AddEntityFrameworkStores<RelacjeBazyDanychContext>();


            var key = Encoding.ASCII.GetBytes(appSettings.JWT_Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            })
            .AddGoogle(options =>
                {
                    options.ClientId = appSettings.Google_Authentication_Id;
                    options.ClientSecret = appSettings.Google_Authentication_Secret;
                }); ;

            services.AddAuthorization();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}