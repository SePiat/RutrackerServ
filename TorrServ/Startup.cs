using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Hangfire;
using TorrServData.Models;
using Core;
using MovDownloader;
using CommService;
using AFINNService;
using LemmatizationService;
using TServRepositories.UoW;
using TorrServData.Data;
using TorrServ.Controllers;

namespace TorrServ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();*/
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("TorrServ")));
            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()
                .AddDefaultUI()   /*?*/
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //add hangfire
            services.AddHangfire(config =>
            config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));



            services.AddTransient<IGenericRepository<TorrentMovie>, TorrentMoveRepository>();
            services.AddTransient<IGenericRepository<SourceOfMovies>, SourceOfMoviesRepository>();
            services.AddTransient<IGenericRepository<Comments>, CommentsRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IMoviesGetterService<TorrentMovie>, MoviesGetterService>();
            services.AddTransient<IMovCommIndexServ, CommentIndexGetter>();
            services.AddTransient<ICommetsGetter<Comments>, CommetsGetter>();
            services.AddTransient<ISaveCommentsToDb, SaveCommentsToDb>();
            services.AddTransient<ISaveMovies, SaveMovies>();
            services.AddTransient<IAFINNService, AFINN>();
            services.AddScoped<ILemmatization, Lemmatization>();
            services.AddTransient<ICommentHanlder, CommentHanlder>();


            services.AddMvc();

            //доступ к сервису откуда угодно
            /*services.AddCors(options =>
            {
                options.AddPolicy("CORS_Policy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .Build());
            });*/





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISaveMovies saveMovies)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
                endpoints.MapRazorPages();
            });            
            
            app.UseCors("CORS_Policy");


            //Hangfire
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });

            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire");

            RecurringJob.AddOrUpdate(
                () => saveMovies.SaveMov(),
                Cron.Weekly());

        }
    }
}
