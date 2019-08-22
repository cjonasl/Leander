using System.Web.Optimization;
using System.Web.Mvc;



namespace CAST.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #if DEBUG
                BundleTable.EnableOptimizations = false;
            #else   
                BundleTable.EnableOptimizations = true;
            #endif

            #region scripts

            // layout
            bundles.Add(new ScriptBundle("~/scripts/layout-js").Include(
                "~/Scripts/respond.js",
                "~/Scripts/jquery-3.4.1.js",
                "~/Scripts/jquery.idletimer.js",
                "~/Scripts/jquery.idletimeout.js",
                "~/Scripts/buttons.js",
                "~/Scripts/jquery.bgiframe.js",
                "~/Scripts/jquery-ui-1.12.1.js",
                "~/Scripts/authenticatedInfo.js",
                "~/Scripts/visibility.js",
                "~/Scripts/jquery.form.min.js",
                "~/Scripts/configuration.js",
                "~/Scripts/dialogs.js",
                "~/Scripts/functions.js",
                "~/Scripts/datepicker.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/EditToolbar-js").Include("~/Scripts/EditToolbar.js"));

            bundles.Add(new ScriptBundle("~/scripts/EditToolbarResponsiveHeightCaption-js").Include(
                "~/Scripts/EditToolbar.js",
                "~/Scripts/ResponsiveHeightCaption.js"));

            bundles.Add(new ScriptBundle("~/scripts/JqueryShorten-js").Include("~/Scripts/jquery.shorten.js"));

            /*
                "~/Scripts/ResponsiveHeightCaption.js",
                "~/Scripts/EditToolbar.js",
                "~/Scripts/jquery.shorten.js
             * */

            //UI scripts - added by Mark Watts

            //bundles.Add(new ScriptBundle("~/scripts/ui-js").Include(
            //    "~/Scripts/lity.min.js",
            //    "~/Scripts/equalheight.js"));


            //bundles.Add(new ScriptBundle("~/scripts/cookieconsent-js").Include(
            //  "~/Scripts/cookieconsent.min.js"));

          
            // addresses script
            bundles.Add(new ScriptBundle("~/scripts/addresses-js").Include(
                "~/Scripts/addresses.js"));

            // file-upload
            bundles.Add(new ScriptBundle("~/scripts/file-upload-js").Include(
                "~/Scripts/fileUpload.js"));


            // alternative products
            bundles.Add(new ScriptBundle("~/scripts/alternative-products-js").Include(
                    "~/Scripts/alternativeProductsList.js"));
            
            // diagnostic page
            bundles.Add(new ScriptBundle("~/scripts/diagnostic-js").Include(
                "~/Scripts/diagnostic.js"));
           
            // job details page
            bundles.Add(new ScriptBundle("~/scripts/job-details-js").Include(
                "~/Scripts/EditToolbar.js",
                "~/Scripts/job-details.js",
                "~/Scripts/Selector.js"));

            #endregion

            bundles.Add(new ScriptBundle("~/scripts/NotesView-js").Include(
            "~/Scripts/jqGrid/jquery.jqGrid.min.js"));

            #region styles
            bundles.Add(new StyleBundle("~/css/main-css").Include(
                "~/Content/css/styles.css", new CssRewriteUrlTransform()).Include
                ("~/Content/css/unique.css", "~/Content/css/lity.min.css"));

            bundles.Add(new StyleBundle("~/css/datepicker-css").Include(
               "~/Content/css/datepicker.css"));

            bundles.Add(new StyleBundle("~/css/tabs-css").Include(
                "~/Content/css/tabs.css"));
            #endregion
             
        }
    }
}
