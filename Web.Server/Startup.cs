﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Havit.NewProjectTemplate.Contracts;
using Havit.NewProjectTemplate.DependencyInjection;
using Havit.NewProjectTemplate.Facades.Crm;
using Havit.NewProjectTemplate.Facades.Infrastructure.Security.Authentication;
using Havit.NewProjectTemplate.Facades.Infrastructure.Security.Identity;
using Havit.NewProjectTemplate.Facades.System;
using Havit.NewProjectTemplate.Model.Security;
using Havit.NewProjectTemplate.Web.Server.Infrastructure.ApplicationInsights;
using Havit.NewProjectTemplate.Web.Server.Infrastructure.ConfigurationExtensions;
using Havit.NewProjectTemplate.Web.Server.Infrastructure.Interceptors;
using Havit.NewProjectTemplate.Web.Server.Infrastructure.Security;
using Havit.NewProjectTemplate.Web.Server.Tools;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;

namespace Havit.NewProjectTemplate.Web.Server
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.ConfigureForWebServer(configuration);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddDatabaseDeveloperPageExceptionFilter();

			services.AddOptions();

			services.AddCustomizedMailing(configuration);

			// SmtpExceptionMonitoring to errors@havit.cz
			services.AddExceptionMonitoring(configuration);

			// Application Insights
			services.AddApplicationInsightsTelemetry(configuration);
			services.AddSingleton<ITelemetryInitializer, GrpcRequestStatusTelemetryInitializer>();
			services.AddSingleton<ITelemetryInitializer, EnrichmentTelemetryInitializer>();
			services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

			AddAuth(services);

			// server-side UI
			services.AddControllersWithViews();
			services.AddRazorPages();

			// gRPC
			services.AddSingleton<ServerExceptionsGrpcServerInterceptor>();
			services.AddSingleton(BinderConfiguration.Create(marshallerFactories: new[] { ProtoBufMarshallerFactory.Create(RuntimeTypeModel.Default.RegisterApplicationContracts()) }, binder: new ProtoBufServiceBinder()));
			services.AddCodeFirstGrpc(config =>
			{
				config.Interceptors.Add<ServerExceptionsGrpcServerInterceptor>();
				config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
			});
		}

		private static void AddAuth(IServiceCollection services)
		{
			// Authentication, Authorization, Identity
			services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<Role>()
				.AddUserStore<UserStore>()
				.AddRoleStore<RoleStore>();
			services.AddIdentityServer()
				.AddAspNetIdentity<User>()
				.AddClients()
				.AddSigningCredentials()
				.AddIdentityResources()
				.AddApiResources()
				.AddProfileService<IdentityServerProfileService>();
			services.PostConfigure<ApiAuthorizationOptions>(options =>
			{
				options.Clients["Havit.NewProjectTemplate.Web.Client"].AlwaysIncludeUserClaimsInIdToken = true;
				options.IdentityResources["openid"].UserClaims.Add("name");
				options.ApiResources.Single().UserClaims.Add("name");
				options.IdentityResources["openid"].UserClaims.Add("role");
				options.ApiResources.Single().UserClaims.Add("role");
			});
			services.AddAuthentication()
				.AddIdentityServerJwt();

			// server-side support for User.IsInRole(), see https://leastprivilege.com/2016/08/21/why-does-my-authorize-attribute-not-work/
			// https://docs.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-identity-server?view=aspnetcore-5.0&tabs=visual-studio#api-authorization-options
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");
			services.AddScoped<IApplicationAuthenticationService, ApplicationAuthenticationService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseMigrationsEndPoint();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				// TODO app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseExceptionMonitoring();

			app.UseRouting();

			app.UseAuthentication();
			app.UseIdentityServer();
			app.UseAuthorization();

			app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");

				// TODO Mass-registration
				endpoints.MapGrpcService<ContactFacade>();

				endpoints.MapGrpcService<DataSeedFacade>();
				endpoints.MapGrpcService<MaintenanceFacade>();
			});

			app.UpgradeDatabaseSchemaAndData();
		}
	}
}
