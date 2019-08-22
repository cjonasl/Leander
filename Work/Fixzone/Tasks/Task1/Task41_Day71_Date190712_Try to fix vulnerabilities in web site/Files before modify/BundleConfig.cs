﻿using System.Web.Optimization;
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
                "~/Scripts/jquery-1.8.2.js",
                "~/Scripts/jquery.idletimer.js",
                "~/Scripts/jquery.idletimeout.js",
                "~/Scripts/buttons.js",
                "~/Scripts/jquery.bgiframe.js",
                "~/Scripts/jquery-ui-1.9.0.js",
                "~/Scripts/authenticatedInfo.js",
                "~/Scripts/visibility.js",
                "~/Scripts/jquery.form.min.js"
                ,"~/Scripts/configuration.js"
                ,"~/Scripts/dialogs.js"
                , "~/Scripts/functions.js"
                ,"~/Scripts/datepicker.js"
                
                ));


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
                "~/Scripts/Selector.js",
                "~/Scripts/Edit.js"));

            #endregion

            bundles.Add(new ScriptBundle("~/scripts/NotesView-js").Include(
            "~/Scripts/jqGrid/jquery.jqGrid.min.js"));

            #region styles
            //bundles.Add(new StyleBundle("~/css/main-css").Include(
            //    "~/Content/css/bootstrap.css", new CssRewriteUrlTransform()).Include(
            //       "~/Content/css/BootstrapOverride.css",
            //         "~/Content/css/base.css",
            //    "~/Content/css/custom.css",
            //    "~/Content/css/animations.css",
            //    "~/Content/css/PagedList.css",
            //    "~/Content/ui.jqgrid.css",
            //    "~/Content/css/layout.css"));

//
            
                bundles.Add(new StyleBundle("~/css/main-css").Include(
                    "~/Content/css/styles.css", new CssRewriteUrlTransform()).Include
                    ("~/Content/css/unique.css","~/Content/css/lity.min.css"));

            
               
            
            bundles.Add(new StyleBundle("~/css/datepicker-css").Include(
               "~/Content/css/datepicker.css"));


           
           
            #endregion
             
        }
    }
}
