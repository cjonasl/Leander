namespace ClientConnect.Process
{
    /// <summary>
    /// Enumerates pre-defined processes
    /// </summary>
    public enum PredefinedProcess
    {
        /// <summary>
        /// Home page process
        /// </summary>
        Home = 1,

        /// <summary>
        /// Search for products
        /// </summary>
        ProductSearch = 2,

        /// <summary>
        /// Product details
        /// </summary>
        ProductDetails = 3,

        /// <summary>
        /// Standard repair process
        /// </summary>
        StandardRepair = 4,

        /// <summary>
        /// Search for jobs
        /// </summary>
        JobSearch = 5,

        /// <summary>
        /// Job details
        /// </summary>
        JobDetails = 6,

        /// <summary>
        /// Repair process job page
        /// </summary>
        NotBookedJobList = 7,

        /// <summary>
        /// Repair process confirm page
        /// </summary>
        StandardRepairConfirm = 8,

        /// <summary>
        /// Repair process report page
        /// </summary>
        StandardRepairReport = 9,

        /// <summary>
        /// Sign in
        /// </summary>
        SignIn = 10,

        /// <summary>
        /// Administration page
        /// </summary>
        Administration = 11,

        /// <summary>
        /// Jobs by status
        /// </summary>
        JobsByStatus = 12,

        /// <summary>
        /// First time new user entering 
        /// </summary>
        UserEmptyPassword = 14,

        /// <summary>
        /// Change password
        /// </summary>
        UserForgottenPassword = 15,

        /// <summary>
        /// User details
        /// </summary>
        UserDetails = 16,

        /// <summary>
        /// Add user
        /// </summary>
        AddUser = 17,

        /// <summary>
        /// Expired password
        /// </summary>
        ExpiredPassword = 18,

        /// <summary>
        /// Show product details report
        /// </summary>
        ShowProductDetailsReport = 19,

        /// <summary>
        /// Additional links
        /// </summary>
        AdditionalLinks = 20,

        /// <summary>
        /// Register the job
        /// </summary>
        RegisteringJob = 22,


        // book job from client portal
        BookRepair = 24,
        JobList = 25,
        CustSearch = 28
           , CustjBook = 29,
        AdditJobB = 30,
        CallcenterSignIn=31

    }
}