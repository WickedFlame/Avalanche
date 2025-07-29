using Avalanche.WriteModel.Commands.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avalanche.WriteModel.CommandHandlers
{
    public class SettingsCommandHandler : CommandHandler<DropDatabaseCommand>
    {
        public override void Handle(DropDatabaseCommand @event)
        {
            throw new NotImplementedException();
        }
    }
}
