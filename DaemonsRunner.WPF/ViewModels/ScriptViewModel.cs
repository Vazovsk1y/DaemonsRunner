using CommunityToolkit.Mvvm.ComponentModel;
using DaemonsRunner.Core.Enums;
using DaemonsRunner.Core.Models;
using System.IO;

namespace DaemonsRunner.WPF.ViewModels;

internal partial class ScriptViewModel : ObservableObject
{
	[ObservableProperty]
	private bool _isSelected;
	public required ScriptId ScriptId { get; init; }

	public required string Title { get; init; }

	public required string Command { get; init; }

	public required RuntimeType RuntimeType { get; init; }

	public DirectoryInfo? WorkingDirectory { get; init; }
}
