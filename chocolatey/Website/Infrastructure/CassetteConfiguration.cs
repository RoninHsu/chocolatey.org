using System.IO;
using System.Text.RegularExpressions;
using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace NuGetGallery
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // TODO: Configure your bundles here...
            // Please read http://getcassette.net/documentation/configuration

            // This default configuration treats each file as a separate 'bundle'.
            // In production the content will be minified, but the files are not combined.
            // So you probably want to tweak these defaults!
            //bundles.AddPerIndividualFile<StylesheetBundle>("Content");
            //bundles.AddPerIndividualFile<ScriptBundle>("Scripts");

            // To combine files, try something like this instead:
            //   bundles.Add<StylesheetBundle>("Content");
            // In production mode, all of ~/Content will be combined into a single bundle.

            // If you want a bundle per folder, try this:
            //   bundles.AddPerSubDirectory<ScriptBundle>("Scripts");
            // Each immediate sub-directory of ~/Scripts will be combined into its own bundle.
            // This is useful when there are lots of scripts for different areas of the website.

            bundles.AddPerIndividualFile<StylesheetBundle>("Content", new FileSearch
            {
                Pattern = "*.css",
                SearchOption = SearchOption.AllDirectories,
                Exclude = new Regex("style\\.css$")
            });
#if DEBUG
            bundles.AddPerSubDirectory<ScriptBundle>("Scripts", new FileSearch
            {
                Pattern = "*.js",
                SearchOption = SearchOption.AllDirectories,
                Exclude = new Regex("-vsdoc\\.js$")
            });
            //bundles.AddPerSubDirectory<ScriptBundle>("js", new FileSearch
            //                                     {
            //                                         Pattern = "*.js;*.coffee",
            //                                         SearchOption = SearchOption.AllDirectories,
            //                                         Exclude = new Regex("-vsdoc\\.js$")
            //                                     }); 

#else

            bundles.AddPerSubDirectory<ScriptBundle>("Scripts", new FileSearch
            {
                Pattern = "*.js",
                SearchOption = SearchOption.AllDirectories,
                Exclude = new Regex("-vsdoc\\.js$|\\.debug\\.js$")
            });

#endif
            //this.Log().Debug(() => "Cassette is now configured...");


        }
    }
}