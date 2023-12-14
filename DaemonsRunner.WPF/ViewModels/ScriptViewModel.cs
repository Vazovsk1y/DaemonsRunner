using CommunityToolkit.Mvvm.ComponentModel;
using DaemonsRunner.Core.Models;
using System.IO;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptViewModel : ObservableObject
{
	public required ScriptId ScriptId { get; init; }

	[ObservableProperty]
	private bool _IsSelected;

	[ObservableProperty]
	private string _title = null!;

	[ObservableProperty]
	private string _command = null!;

	public bool IsWorkingDirectoryExists => WorkingDirectory is { Exists: true };

	public DirectoryInfo? WorkingDirectory { get; init; }
}
