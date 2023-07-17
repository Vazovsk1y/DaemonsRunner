using DaemonsRunner.BuisnessLayer.Responses.Enums;
using DaemonsRunner.BuisnessLayer.Services.Interfaces;
using DaemonsRunner.Commands;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Infrastructure.Extensions;
using DaemonsRunner.Infrastructure.Managers.Interfaces;
using DaemonsRunner.Infrastructure.Messages;
using DaemonsRunner.ViewModels.Base;
using DaemonsRunner.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Windows.Input;

namespace DaemonsRunner.ViewModels
{
    internal class WorkSpaceViewModel : BaseViewModel, IDisposable
    {
        #region --Fields--

        private readonly IDataBus _dataBus;
        private readonly IDisposable _subscription;
        private readonly IServiceManager _serviceManager;
        private readonly FilesPanelViewModel _filesPanelViewModel;
        private readonly IScriptConfigureService _configureSctiptsService;
        private readonly ObservableCollection<IScriptExecutorViewModel> _scriptsExecutorsViewModels = new();

        private ObservableCollection<PHPScript> _configuredScripts;
        private bool? _isStartButtonEnable = null;
        private bool? _isStopButtonEnable = null;

        #endregion

        #region --Properties--

        public bool? IsStartButtonEnable
        {
            get
            {
                bool defaultCondition = ConfiguredScripts is ICollection<PHPScript> { Count: > 0 } &&
                      ScriptsExecutorsViewModels is ICollection<IScriptExecutorViewModel> { Count: 0 };

                return _isStartButtonEnable is bool condition
                    ? condition
                    : defaultCondition;
            }
            set
            {
                if (Set(ref _isStartButtonEnable, value))
                {
                    OnPropertyChanged(nameof(StartScriptsCommand));
                }
            }
        }

        public bool? IsStopButtonEnable
        {
            get => _isStopButtonEnable is bool condition
                    ? condition
                    : ScriptsExecutorsViewModels is ICollection<IScriptExecutorViewModel> { Count: > 0 };
            set
            {
                if (Set(ref _isStopButtonEnable, value))
                {
                    OnPropertyChanged(nameof(StopScriptsCommand));
                }
            }
        }

        public ObservableCollection<IScriptExecutorViewModel> ScriptsExecutorsViewModels => _scriptsExecutorsViewModels;

        public ObservableCollection<PHPScript> ConfiguredScripts
        {
            get => _configuredScripts;
            set => Set(ref _configuredScripts, value);
        }

        #endregion

        #region --Constructors--

        public WorkSpaceViewModel() 
        {
            var testScripts = Enumerable.Range(0, 10).Select(e => new PHPScript(_filesPanelViewModel.Files.FirstOrDefault())).ToList();
            _configuredScripts = new ObservableCollection<PHPScript>(testScripts);
        }

        public WorkSpaceViewModel(
            FilesPanelViewModel filesPanelViewModel,
            IScriptConfigureService configureSctiptsService,
            IServiceManager serviceManager,
            IDataBus dataBus)
        {
            _filesPanelViewModel = filesPanelViewModel;
            _configureSctiptsService = configureSctiptsService;
            _serviceManager = serviceManager;
            _dataBus = dataBus;
            _subscription = _dataBus.RegisterHandler<ScriptExitedMessage>(OnScriptExited);
        }

        #endregion

        #region --Commands--

        public ICommand ConfigureScriptsCommand => new RelayCommand(
            OnConfigureScriptsExecute,
            (arg) => _filesPanelViewModel.Files.Count > 0);

        public ICommand ClearConfiguredScripts => new RelayCommand(
            (arg) => ConfiguredScripts.Clear(),
            (arg) => ConfiguredScripts is ICollection<PHPScript> { Count: > 0 });

        public ICommand StartScriptsCommand => new RelayCommand(
            OnStartScriptsExecute,
            (arg) => (bool)IsStartButtonEnable!);

        public ICommand StopScriptsCommand => new RelayCommand(
            OnStopScriptsExecute,
            (arg) => (bool)IsStopButtonEnable!);

        #region -Commands Handlers-

        private async void OnConfigureScriptsExecute(object obj)
        {
            var response = await _configureSctiptsService.ConfigureScriptsAsync(_filesPanelViewModel.Files).ConfigureAwait(false);
            if (response.OperationStatus is StatusCode.Success)
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    ConfiguredScripts = new ObservableCollection<PHPScript>(response.Data!);
                });
            }
            _dataBus.Send(response.Description);
        }

        private async void OnStartScriptsExecute(object obj)
        {
            IsStartButtonEnable = false;
            var (responses, executorsViewModels) = await _serviceManager.GetStartingResultAsync(ConfiguredScripts).ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"{executorsViewModels.ToList().Count} scripts were successfully started.");
            }
            else
            {
                _dataBus.SendAll(failedResponses.Select(r => r.Description));
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                ScriptsExecutorsViewModels.AddRange(executorsViewModels);
            });
            IsStopButtonEnable = null;
        }

        private async void OnStopScriptsExecute(object obj)
        {
            IsStopButtonEnable = false;
            var (responses, successfullyStoppedViewModels) = await _serviceManager.GetStoppingResultAsync(ScriptsExecutorsViewModels).ConfigureAwait(false);
            var failedResponses = responses.Where(r => r.OperationStatus is StatusCode.Fail).ToList();

            if (failedResponses.Count is 0)
            {
                _dataBus.Send($"All scripts were succsessfully stopped.");
            }
            else
            {
                _dataBus.SendAll(failedResponses.Select(r => r.Description));
            }

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                ScriptsExecutorsViewModels.RemoveAll(successfullyStoppedViewModels);
            });
            IsStartButtonEnable = null;
        }

        #endregion

        #endregion

        #region --Methods--

        public void Dispose()
        {
            _subscription.Dispose();
            if (ScriptsExecutorsViewModels.Count > 0)
            {
                foreach (var scriptViewModel in ScriptsExecutorsViewModels)
                {
                    scriptViewModel.Dispose();
                }
                ScriptsExecutorsViewModels.Clear();
            }
        }

        #region --Databus messages handlers--

        private async void OnScriptExited(ScriptExitedMessage message)
        {
            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                ScriptsExecutorsViewModels.Remove(message.Sender);
            });
            IsStopButtonEnable = null;
            IsStartButtonEnable = null;
        }

        #endregion

        #endregion
    }
}
