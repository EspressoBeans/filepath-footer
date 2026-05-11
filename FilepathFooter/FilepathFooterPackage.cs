using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace FilepathFooter
{
    /// <summary>
    /// The VS package entry point.  The extension's real work is done by the
    /// MEF-exported <see cref="Editor.FilepathFooterMarginFactory"/>, so this
    /// package only needs to register itself with Visual Studio.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class FilepathFooterPackage : AsyncPackage
    {
        public const string PackageGuidString = "d4f8e6b2-3a1c-4d5e-8f7a-9b0c1d2e3f4a";

        protected override Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            return Task.CompletedTask;
        }
    }
}
