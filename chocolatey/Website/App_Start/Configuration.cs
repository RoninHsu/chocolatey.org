﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;

namespace NuGetGallery
{
    public class Configuration : IConfiguration
    {
        private static readonly Dictionary<string, Lazy<object>> _configThunks = new Dictionary<string, Lazy<object>>();
        private readonly Lazy<string> _httpSiteRootThunk;
        private readonly Lazy<string> _httpsSiteRootThunk;

        public Configuration()
        {
            _httpSiteRootThunk = new Lazy<string>(GetHttpSiteRoot);
            _httpsSiteRootThunk = new Lazy<string>(GetHttpsSiteRoot);
        }

        public static string ReadAppSettings(string key)
        {
            return ReadAppSettings(key, value => value);
        }

        public static T ReadAppSettings<T>(
            string key,
            Func<string, T> valueThunk)
        {
            if (!_configThunks.ContainsKey(key))
                _configThunks.Add(key, new Lazy<object>(() =>
                {
                    var value = ConfigurationManager.AppSettings[string.Format("Gallery:{0}", key)];
                    if (string.IsNullOrWhiteSpace(value))
                        value = null;
                    return valueThunk(value);
                }));

            return (T)_configThunks[key].Value;
        }

        public string AzureStorageAccessKey
        {
            get
            {
                return ReadAppSettings("AzureStorageAccessKey");
            }
        }

        public string AzureStorageAccountName
        {
            get
            {
                return ReadAppSettings("AzureStorageAccountName");
            }
        }

        public string AzureStorageBlobUrl
        {
            get
            {
                return ReadAppSettings("AzureStorageBlobUrl");
            }
        }

        public string FileStorageDirectory
        {
            get
            {
                return ReadAppSettings(
                    "FileStorageDirectory",
                    value => value ?? HttpContext.Current.Server.MapPath("~/App_Data/Files"));
            }
        }

        public PackageStoreType PackageStoreType
        {
            get
            {
                return ReadAppSettings(
                    "PackageStoreType",
                    value => (PackageStoreType)Enum.Parse(typeof(PackageStoreType), value ?? PackageStoreType.NotSpecified.ToString()));
            }
        }

        public string AzureCdnHost
        {
            get
            {
                return ReadAppSettings("AzureCdnHost");
            }
        }
		
        public string S3Bucket
        {
            get
            {
                return ReadAppSettings<string>(
                   "S3Bucket",
                   (value) => value ?? string.Empty);
            }
        }

        public string S3AccessKey
        {
            get
            {
                return ReadAppSettings<string>(
                   "S3AccessKey",
                   (value) => value ?? string.Empty);
            }
        }

        public string S3SecretKey
        {
            get
            {
                return ReadAppSettings<string>(
                  "S3SecretKey",
                  (value) => value ?? string.Empty);
            }
        }

        public bool SmtpEnableSsl
        {
            get
            {
                return ReadAppSettings<bool>(
                 "SmtpEnableSsl",
                 (value) => bool.Parse(value ?? bool.TrueString));
            }
        }

        protected virtual string GetConfiguredSiteRoot()
        {
            return ReadAppSettings("SiteRoot");
        }

        protected virtual HttpRequestBase GetCurrentRequest()
        {
            return new HttpRequestWrapper(HttpContext.Current.Request);
        }

        public string GetSiteRoot(bool useHttps)
        {
            return useHttps ? _httpsSiteRootThunk.Value : _httpSiteRootThunk.Value;
        }
        
        private string GetHttpSiteRoot()
        {
            var request = GetCurrentRequest();
            string siteRoot;
            
            if (request.IsLocal)
                siteRoot = request.Url.GetLeftPart(UriPartial.Authority) + '/';
            else
                siteRoot = GetConfiguredSiteRoot();

            if (!siteRoot.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !siteRoot.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The configured site root must start with either http:// or https://.");

            if (siteRoot.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                siteRoot = "http://" + siteRoot.Substring(8);

            return siteRoot;
        }

        private string GetHttpsSiteRoot()
        {
            var siteRoot = _httpSiteRootThunk.Value;

            if (!siteRoot.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The configured HTTP site root must start with http://.");

            return "https://" + siteRoot.Substring(7);
        }
    }
}