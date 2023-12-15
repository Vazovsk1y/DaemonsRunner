using CommunityToolkit.Mvvm.ComponentModel;
using DaemonsRunner.Core.Enums;
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

	public RuntimeType RuntimeType { get; init; }

	public DirectoryInfo? WorkingDirectory { get; init; }
}
