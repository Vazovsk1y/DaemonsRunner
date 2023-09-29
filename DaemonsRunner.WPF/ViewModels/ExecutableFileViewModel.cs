using CommunityToolkit.Mvvm.ComponentModel;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ExecutableFileViewModel : ObservableObject
{
	[ObservableProperty]
	private string _path = null!;

	[ObservableProperty]
	private string _name = null!;

	[ObservableProperty]
	private string _extension = null!;
}
