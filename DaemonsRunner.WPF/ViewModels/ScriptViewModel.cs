using CommunityToolkit.Mvvm.ComponentModel;
using DaemonsRunner.Domain.Models;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptViewModel : ObservableObject
{
	public required ScriptId ScriptId { get; init; }

	[ObservableProperty]
	private bool _isReadyToStart;

	[ObservableProperty]
	private string _title = null!;

	[ObservableProperty]
	private string _command = null!;

	public ExecutableFileViewModel? ExecutableFileViewModel { get; init; }
}
