﻿using System.Collections.Generic;
using Sources.Frameworks.DoozyWrappers.SignalBuses.Infrastructure.ViewCommands.Interfaces;
using Sources.Frameworks.DoozyWrappers.SignalBuses.Infrastructure.ViewCommands.Interfaces.Handlers;
using Sources.Frameworks.UiFramework.Views.Domain;

namespace Sources.Frameworks.DoozyWrappers.SignalBuses.Infrastructure.ViewCommands.Implementation.Handlers
{
    public class MainMenuUiViewCommandHandler : IUiViewCommandHandler
    {
        private readonly Dictionary<FormCommandId, IViewCommand> _commands = 
            new Dictionary<FormCommandId, IViewCommand>();
        
        public MainMenuUiViewCommandHandler(
            PauseCommand pauseCommand,
            UnPauseCommand unPauseCommand,
            SaveVolumeCommand saveVolumeCommand,
            ClearSavesCommand clearSavesCommand)
        {
            _commands[pauseCommand.Id] = pauseCommand;
            _commands[unPauseCommand.Id] = unPauseCommand;
            _commands[saveVolumeCommand.Id] = saveVolumeCommand;
            _commands[clearSavesCommand.Id] = clearSavesCommand;
        }
        
        public void Handle(FormCommandId formCommandId)
        {
            if(_commands.ContainsKey(formCommandId) == false)
                throw new KeyNotFoundException(nameof(formCommandId));
            
            _commands[formCommandId].Handle();
        }

    }
}