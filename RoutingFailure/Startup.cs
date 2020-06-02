using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RoutingFailure
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
#if DEBUG
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
#else
            services.AddControllersWithViews();
#endif

            services.AddRazorPages();

            ConfigureAuthentication(services);

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
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
                    name: "SearchResults",
                    pattern: "Search/{term}/{page}",
                    defaults: new { controller = "Home", action = "Search", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "AutocompleteResults",
                    pattern: "Autocomplete/{q}/{MaxResults}",
                    defaults: new { controller = "Home", action = "Autocomplete", MaxResults = 10 }
                );

                endpoints.MapControllerRoute(
                    name: "ReviewsPage",
                    pattern: "Reviews/{page}",
                    defaults: new { controller = "Review", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "Review",
                    pattern: "Review/{id}/{action=Details}",
                    defaults: new { controller = "Reviews", action = "Details" }
                    );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonRoleCountryYear",
                    pattern: "Person/{person}-{role}/Country/{country}/Year/{year}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonRoleCountry",
                    pattern: "Person/{person}-{role}/Country/{country}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonCountryYear",
                    pattern: "Person/{person}/Country/{country}/Year/{year}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonCountry",
                    pattern: "Person/{person}/Country/{country}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonRoleYear",
                    pattern: "Person/{person}-{role}/Year/{year}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonRole",
                    pattern: "Person/{person}-{role}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPersonYear",
                    pattern: "Person/{person}/Year/{year}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPerson",
                    pattern: "Person/{person}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesCountry",
                    pattern: "Country/{country}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesYear",
                    pattern: "Movies/Year/{year}/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapControllerRoute(
                    name: "MoviesPage",
                    pattern: "Movies/{page}",
                    defaults: new { controller = "Movies", action = "Index", page = 1 }
                );

                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }

        /// <summary>
        /// Set up users and roles for Identity service
        /// </summary>
        private static void ConfigureAuthentication(IServiceCollection services)
        {
            //services.AddIdentity<IdentityUser, IdentityRole>(options =>
            //{
            //    options.User = new UserOptions() { RequireUniqueEmail = true };
            //    options.Password = new PasswordOptions()
            //    {
            //        RequiredLength = 10,
            //        RequiredUniqueChars = 4,
            //        RequireDigit = false,
            //        RequireLowercase = false,
            //        RequireNonAlphanumeric = false,
            //        RequireUppercase = false
            //    };
            //})
            //.AddRoles<IdentityRole>()
            //.AddEntityFrameworkStores<IdentityContext>()
            //.AddRoleManager<RoleManager<IdentityRole>>()
            //.AddDefaultTokenProviders()
            //.AddDefaultUI()
            ;
        }
    }
}
