// Global using directives

global using Autofac;
global using Autofac.Extras.DynamicProxy;
global using Castle.DynamicProxy;
global using EntityGuardian.BackgroundServices;
global using EntityGuardian.Entities;
global using EntityGuardian.Entities.Dtos;
global using EntityGuardian.Enums;
global using EntityGuardian.Interfaces;
global using EntityGuardian.Middlewares;
global using EntityGuardian.Options;
global using EntityGuardian.Storages;
global using EntityGuardian.Storages.SqlServer;
global using EntityGuardian.Utilities;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.StaticFiles;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.FileProviders;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using SmartOrderBy.Dtos;
global using SmartWhere.Attributes;
global using SmartWhere.Enums;
global using SmartWhere.Interfaces;
global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Transactions;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using SmartOrderBy;
global using SmartWhere;
#if NETSTANDARD2_1
global using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif