using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Joker.Factories.Schedulers;
using Joker.Platforms.Factories.Schedulers;
using Joker.PubSubUI.Shared.Navigation;
using Joker.WinUI3.Sample.Factories.Schedulers;
using Microsoft.UI.Threading;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Ninject;
using Joker.WPF.Sample.Modularity;
using Joker.WPF.Sample.Navigation;

namespace Joker.WinUI3.Sample
{
  /// <summary>
  /// Provides application-specific behavior to supplement the default Application class.
  /// </summary>
  public partial class App : Application
  {
    private Window m_window;
    public static readonly IKernel Kernel = new StandardKernel();

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
      this.InitializeComponent();

      this.Suspending += OnSuspending;
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
      Kernel.Load<AppNinjectModule>();
      Kernel.Bind<IDialogManager>().To<DialogManager>().InSingletonScope();
      Kernel.Bind<ISchedulersFactory, IPlatformSchedulersFactory>().To<PlatformSchedulersFactory>().InSingletonScope();

      m_window = new MainWindow();
      m_window.Activate();
      m_window.Closed += OnWindowClosed;
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
      Kernel.Dispose();
    }

    /// <summary>
    /// Invoked when application execution is being suspended.  Application state is saved
    /// without knowing whether the application will be terminated or resumed with the contents
    /// of memory still intact.
    /// </summary>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      // Save application state and stop any background activity
    }
  }
}
