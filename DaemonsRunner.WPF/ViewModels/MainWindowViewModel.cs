using CommunityToolkit.Mvvm.ComponentModel;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class MainWindowViewModel : ObservableObject
{
	#region --Fields--


	#endregion

	#region --Properties--

	public ScriptsPanelViewModel ScriptsPanelViewModel { get; }

	public NotificationsPanelViewModel NotificationsPanelViewModel { get; }

	[ObservableProperty]
	private string _windowTitle = App.Name;

	#endregion

	#region --Constructors--

	public MainWindowViewModel()
	{

	}

	public MainWindowViewModel(
		ScriptsPanelViewModel workSpaceViewModel,
		NotificationsPanelViewModel notificationsPanelViewModel)
	{
		ScriptsPanelViewModel = workSpaceViewModel;
		NotificationsPanelViewModel = notificationsPanelViewModel;
		ScriptsPanelViewModel.IsActive = true;
	}

	#endregion

	#region --Commands--



	#endregion

	#region --Methods--


	#endregion
}
