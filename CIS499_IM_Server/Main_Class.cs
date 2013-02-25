// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main_Class.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the Main_Class type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System.ServiceProcess;

    /// <summary>
    /// The main_ class.
    /// </summary>
    public partial class MainClass : ServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainClass"/> class.
        /// </summary>
        public MainClass()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The on start.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        protected override void OnStart(string[] args)
        {
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        protected override void OnStop()
        {
        }
    }
}
